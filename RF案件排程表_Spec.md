# 📙 RF案件排程系統 — 系統規格書 (Spec v2.3 Updated)

---

## 📖 文件說明

**文件目的：** 詳細定義系統每個功能的畫面元素、操作流程、欄位規範、驗證規則及防呆處理。

**版本歷程：**
- v1.0 (2025-11-14)：初版,僅包含Login模組
- v2.0 (2025-11-17)：擴充完整功能模組
- v2.1 (2025-11-19)：新增混合登入機制(Local + Windows AD)、IAM 權限管理介面、JWT Token 安全性規範、臨時權限授予功能
- v2.2 (2025-11-20)：TestItem 狀態計算邏輯明確化、狀態逆向操作支援、Regulation狀態
- v2.3 (2025-11-20)：WorkLog、TestItemRevision Soft Delete 機制說明、Regulation 狀態計算邏輯、DelayReason 停用機制說明、PermissionGroupMapping 欄位補充
- v2.3 Updated (2025-11-21)：
  - 新增 TestItemRevision 完整定義(欄位、UI、流程)
  - 新增工程師分配相關權限(ASSIGN/SUPPORT/REMOVE)
  - 新增 Regulation 管理權限與 UI 介面
  - 新增補測版本管理權限
  - 統一權限命名(AUDIT_VIEW, SYSTEM_SETTING)
  - 補充 8 個缺少的 SCR UI 介面
  - 新增 Email 合併邏輯與首次新增規則
  - 補充 UI Flow 與操作流程
  - 案件總覽改為 GridControl 列表
  - 補充 TestItemEngineer RoleType 詳細定義

---

## 1. 登入與帳號管理模組

### 1.1 Login 畫面 (SCR-LOGIN-001)

**畫面目的：** 提供使用者身份驗證入口，支援 Local 帳號與 Windows AD 兩種登入方式

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label   | 帳號 | -         | -    | -    |
| TextBox | -    | txtAccount | ✔  | 最大長度50 |
| Label   | 密碼 | -         | -    | -    |
| TextBox | -    | txtPassword | ✔ | PasswordChar='*' |
| Button  | 登入 | btnLogin | - | 預設按鈕 |
| Button  | Windows 驗證登入 | btnWindowsAuth | - | 次要按鈕(選用功能) |
| LinkLabel | 忘記密碼? | lnkForgotPassword | - | 底線、藍色 |
| Label   | 【說明】 | lblInfo | - | 灰色小字,見下方 |

**說明文字內容：**
```
• 使用系統帳號密碼登入(適用於共用電腦環境)
• 或點擊「Windows 驗證登入」使用公司 AD 帳號
```

**操作流程 - Local 帳號登入：**
```
1. 使用者輸入帳號與密碼
2. 按下「登入」或按Enter鍵
3. 呼叫 API: POST /api/auth/login
   Request: {account, password, authType: "Local"}
4. 系統驗證
   ├─ 成功 → 回傳 JWT Token,跳轉至主畫面
   └─ 失敗 → 顯示錯誤訊息,清空密碼欄位
```

**操作流程 - Windows AD 登入：**
```
1. 使用者點擊「Windows 驗證登入」
2. 系統透過 Windows Authentication 取得目前登入使用者資訊
3. 呼叫 API: POST /api/auth/login-ad
   Request: {windowsIdentity} (由 IIS 自動提供)
4. 後端處理
   ├─ 檢查 User 表是否存在對應 ADAccount
   ├─ 若不存在 → 自動建立 User (AuthType=AD, 預設角色與權限群組)
   ├─ 若 AD 帳號已停用 → 回傳錯誤
   └─ 成功 → 回傳 JWT Token,跳轉至主畫面
5. 顯示歡迎訊息:"歡迎 {DisplayName}(AD 登入)"
```

**前端驗證規則：**
| 驗證項目 | 錯誤訊息 | 觸發時機 |
|---------|---------|---------|
| 帳號空白 | "請輸入帳號" | btnLogin.Click |
| 密碼空白 | "請輸入密碼" | btnLogin.Click |
| 帳號包含特殊符號 | "帳號僅能包含英數字及底線" | txtAccount.Validating |

**後端驗證規則：**
| 情境 | HTTP狀態碼 | 錯誤訊息 |
|------|-----------|---------|
| 帳密錯誤 | 401 | "帳號或密碼錯誤" |
| 帳號停用 | 403 | "帳號已停用,請洽繫主管" |
| 連續失敗5次 | 429 | "登入次數過多,請10分鐘後再試" |
| AD 帳號無法驗證 | 401 | "Windows 驗證失敗,請確認您的 AD 帳號狀態" |
| AD 帳號已停用 | 403 | "您的 AD 帳號已停用,請洽繫 IT 部門" |

#### 【新增】Email 為唯一識別(Local 與 AD 共用)

本系統將 **Email 作為唯一使用者識別鍵**(Unique Identity)。  
- Local 帳號與 AD 帳號只要 Email 相同,即視為同一位使用者  
- 使用者可以「Local 帳密」或「Windows AD」任一方式登入  
- 登入前後端比對 Email 一律採 **不區分大小寫 (case-insensitive)**  
- 註冊、新增使用者、修改 Email 時,後端會 **自動轉為小寫後存入資料庫**  
- 重複 Email(大小寫不同也算重複)時,後端一律回傳錯誤:「此 Email 已被使用」

此機制可確保:
- 同一個工程師不會因 Local / AD 登入而產生兩筆資料  
- Loading、工時、權限、稽核日誌都能統一追蹤同一位使用者 

#### 【新增】Email 合併邏輯與首次新增規則

**Email 合併邏輯：**
- 當 User 表中已存在某 Email(不區分大小寫),則該使用者可同時使用 Local 帳密或 AD 登入
- 系統會自動合併該使用者的所有資料(工時、Loading、權限等)
- Email 為唯一識別鍵,不允許重複(含大小寫變化)

**首次新增規則：**
- **一般使用者不可自行註冊或新增 Email**
- 所有新使用者必須由 Manager 或 Admin 透過「使用者管理」介面新增
- 新增時系統會驗證 Email 唯一性(不區分大小寫)
- AD 首次登入時:
  - 若系統設定「自動建立 AD 使用者」= true,則自動建立 User 記錄
  - 若設定為 false,則顯示錯誤並要求聯繫主管
  - 自動建立時會使用 AD 提供的 Email,若 Email 已存在則登入失敗

**JWT Token 規範：**

**Token 結構：**
```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "12345",                     // UserId
    "name": "王小明",                    // DisplayName
    "role": "Engineer",                 // 主要角色
    "authType": "Local",                // Local 或 AD
    "perms": [                          // 關鍵權限代碼
      "PROJECT_VIEW",
      "WORKLOG_CREATE",
      "WORKLOG_VIEW_OWN"
    ],
    "exp": 1700000000,                  // 過期時間戳
    "iat": 1699971200                   // 簽發時間戳
  },
  "signature": "..."                    // HMAC-SHA256 簽章
}
```

**Token 安全規則：**
- **簽章演算法：** HMAC-SHA256 (HS256)
- **有效期限：** 8 小時(可透過 SystemSetting 調整)
- **SecretKey 管理：**
  - 儲存於伺服器端設定檔(appsettings.json 或環境變數)
  - 不得硬編碼在程式碼中
  - 不得傳送至前端
- **Payload 限制：**
  - ✅ **允許：** UserId、DisplayName、角色、權限代碼摘要
  - ❌ **禁止：** 密碼 Hash、Email 內容、完整工時記錄、重設連結
- **驗證規則：**
  - 每次 API 呼叫需驗證 Token 是否存在、格式正確、簽章有效、未過期
  - Token 過期自動跳轉登入頁

**安全規則：**
- 錯誤訊息不區分「帳號不存在」與「密碼錯誤」
- 密碼輸入後不可複製
- 閒置15分鐘自動登出
- Token 儲存於記憶體,不使用 localStorage(防 XSS)

---

### 1.2 忘記密碼畫面 (SCR-FORGOT-001)

**進入方式：** Login畫面 → 點擊「忘記密碼?」

**注意事項：** 此功能僅適用於 Local 帳號

**畫面元素：**
| 元素類型 | 標籤/內容 | 控制項名稱 | 必填 | 說明 |
|---------|----------|-----------|------|------|
| Label | 【說明文字】 | - | - | 見下方說明文字 |
| RadioButton | 使用帳號 | rdoAccount | - | 預設選中 |
| TextBox | - | txtAccount | ✔* | 選中rdoAccount時必填 |
| RadioButton | 使用Email | rdoEmail | - | - |
| TextBox | - | txtEmail | ✔* | 選中rdoEmail時必填 |
| Button | 送出 | btnSubmit | - | - |
| Button | 返回登入 | btnBack | - | 次要按鈕 |

**說明文字內容：**
```
請選擇以下其中一種方式來重設密碼:
• 使用帳號:輸入您的登入帳號
• 使用Email:輸入您註冊時使用的Email

⚠ 此功能僅適用於系統帳號,如果資料正確,我們會將重設密碼連結寄到您的信箱。
```

**操作流程：**
```
1. 選擇重設方式(帳號 or Email)
2. 輸入對應資料
3. 按下「送出」
4. 系統顯示提示訊息(不論成功或失敗)
   → "如果您提供的資料正確,重設密碼連結已寄出,請檢查您的信箱(包含垃圾郵件匣)"
5. 使用者點擊Email中的連結 → 跳轉至 SCR-RESET-001
```

**前端驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 選擇帳號但未填寫 | "請輸入帳號" |
| 選擇Email但未填寫 | "請輸入Email" |
| Email格式錯誤 | "Email格式不正確" |

**後端處理邏輯：**
```
IF 帳號/Email存在 且 AuthType=Local THEN
    生成Token(GUID + Hash)
    儲存至PasswordReset表(ExpireAt = 30分鐘後)
    發送Email(Template: MAIL-RESET-001)
ELSE IF 帳號存在 且 AuthType=AD THEN
    不執行任何動作(避免洩漏 AD 帳號資訊)
ELSE
    不執行任何動作(避免洩漏帳號存在與否)
END IF

回傳前端:200 OK + 統一訊息
```

---

### 1.3 重設密碼畫面 (SCR-RESET-001)

**進入方式：** Email連結 `https://api.example.com/reset?token={Token}`

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 新密碼 | - | - | - |
| TextBox | - | txtNewPassword | ✔ | PasswordChar='*' |
| Label | - | lblPasswordRule | - | 灰色小字,見下方 |
| Label | 確認新密碼 | - | - | - |
| TextBox | - | txtConfirmPassword | ✔ | PasswordChar='*' |
| Button | 確認送出 | btnSubmit | - | - |

**密碼規則文字 (lblPasswordRule)：**
```
密碼需符合以下規則:
• 長度8-20字元
• 至少包含1個英文字母
• 至少包含1個數字
```

**前端驗證規則：**
| 驗證項目 | 錯誤訊息 | 觸發時機 |
|---------|---------|---------|
| 新密碼空白 | "請輸入新密碼" | txtNewPassword.Validating |
| 長度不足 | "密碼長度需8-20字元" | txtNewPassword.Validating |
| 未包含英文 | "密碼需包含至少1個英文字母" | txtNewPassword.Validating |
| 未包含數字 | "密碼需包含至少1個數字" | txtNewPassword.Validating |
| 確認密碼空白 | "請再次輸入密碼" | txtConfirmPassword.Validating |
| 兩次密碼不一致 | "兩次輸入的密碼不相同" | btnSubmit.Click |

**後端驗證規則：**
| 情境 | HTTP狀態碼 | 錯誤訊息 | 處理方式 |
|------|-----------|---------|---------|
| Token不存在 | 404 | "重設連結無效" | 顯示錯誤頁 |
| Token已過期 | 410 | "重設連結已過期,請重新申請" | 提供返回登入連結 |
| Token已使用 | 410 | "此連結已使用過" | 提示已完成重設 |
| 新密碼與舊密碼相同 | 400 | "新密碼不可與舊密碼相同" | 保持在當前頁面 |

**成功流程：**
```
1. 驗證通過
2. 更新User.PasswordHash
3. 設定PasswordReset.UsedAt = 當前時間
4. 新增AuditLog(Action=PasswordReset)
5. 跳轉至成功頁面(顯示3秒後自動跳轉登入)
```

---

### 1.4 使用者管理畫面 (SCR-USER-001)

**權限要求：** Manager 或 Admin,需具備 `USER_VIEW` 權限

**畫面布局：**
```
┌─────────────────────────────────────────┐
│ [新增使用者]  [搜尋: ________] [🔍]      │
├─────────────────────────────────────────┤
│ Grid: 使用者清單                         │
│ ┌───┬────┬────┬───┬────┬───┬────┬─────┐ │
│ │選│帳號│姓名│角色│驗證│Email│狀態│操作 │ │
│ ├───┼────┼────┼───┼────┼───┼────┼─────┤ │
│ │☐│eng01│王小明│工程│本地│...│啟用│[…] │ │
│ │☐│eng02│李小華│工程│AD│...│啟用│[…]   │ │
│ └───┴────┴────┴───┴────┴───┴────┴─────┘ │
│                                         │
│  [批次停用]              [批次啟用]      │
└─────────────────────────────────────────┘
```

**Grid欄位規格：**
| 欄位名稱 | 資料來源 | 寬度 | 對齊 | 排序 | 說明 |
|---------|---------|------|------|------|------|
| 選取 | - | 40px | 中 | ✖ | CheckBox |
| 帳號 | Account | 120px | 左 | ✓ | 預設排序 |
| 姓名 | DisplayName | 100px | 左 | ✓ | - |
| 角色 | RoleName | 80px | 中 | ✓ | Engineer/Manager/Admin |
| 驗證類型 | AuthType | 60px | 中 | ✓ | 本地/AD |
| Email | Email | 200px | 左 | ✓ | - |
| 狀態 | IsActive | 60px | 中 | ✓ | 啟用(綠)/停用(灰) |
| 可用工時/週 | WeeklyAvailableHours | 100px | 右 | ✓ | 顯示單位 |
| 操作 | - | 180px | 中 | ✖ | [編輯][權限][重設密碼] |

**操作按鈕規格：**
| 按鈕 | 觸發動作 | 前置條件 | 後置動作 | 權限要求 |
|------|---------|---------|---------|---------|
| 新增使用者 | 開啟 SCR-USER-002 | - | 重整Grid | USER_CREATE |
| 編輯 | 開啟 SCR-USER-002 | - | 重整該列 | USER_UPDATE |
| 權限 | 開啟 SCR-USER-PERM-001 | - | 重整該列 | USER_MANAGE_PERMISSION |
| 重設密碼 | 確認對話框 → 發送重設信 | IsActive=true, AuthType=Local | 顯示成功訊息 | USER_RESET_PASSWORD |
| 批次停用 | 確認對話框 → API批次更新 | 至少勾選1筆 | 重整Grid | USER_UPDATE |

**重設密碼按鈕顯示規則：**
- AuthType=Local → 顯示「重設密碼」按鈕
- AuthType=AD → 顯示「N/A(AD帳號)」灰色文字

---

### 1.5 新增/編輯使用者對話框 (SCR-USER-002)

**開啟模式：** Modal Dialog

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 帳號 | - | - | - |
| TextBox | - | txtAccount | ✔ | 新增時可編輯,編輯時唯讀 |
| Label | 驗證類型 | - | - | - |
| ComboBox | - | cboAuthType | ✔ | Local / AD(新增時可選,編輯時唯讀) |
| Label | AD 帳號 | - | - | 選擇 AD 時顯示 |
| TextBox | - | txtADAccount | ✔* | 格式:DOMAIN\Account 或 sAMAccountName |
| Label | 顯示名稱 | - | - | - |
| TextBox | - | txtDisplayName | ✔ | - |
| Label | Email | - | - | - |
| TextBox | - | txtEmail | ✔ | - |
| Label | 角色 | - | - | - |
| ComboBox | - | cboRole | ✔ | Engineer/Manager/Admin |
| Label | 每週可用工時 | - | - | - |
| NumericUpDown | - | nudWeeklyHours | ✔ | 預設37.5 |
| CheckBox | 啟用帳號 | chkIsActive | - | 預設勾選 |
| Button | 確定 | btnOK | - | - |
| Button | 取消 | btnCancel | - | - |

**驗證類型切換邏輯：**
```
cboAuthType.SelectedIndexChanged:
    IF cboAuthType.SelectedValue = "AD" THEN
        txtADAccount.Visible = True
        txtADAccount.Required = True
        lblADAccount.Visible = True
        lblPasswordInfo.Text = "(AD 帳號將使用公司密碼,無需設定初始密碼)"
    ELSE
        txtADAccount.Visible = False
        txtADAccount.Required = False
        lblADAccount.Visible = False
        lblPasswordInfo.Text = "(系統將自動產生初始密碼並發送 Email 通知)"
    END IF
```

**新增模式特殊規則：**
- **Local 帳號：**
  - 系統自動產生隨機初始密碼(12碼)
  - 自動發送Email通知新使用者(Template: MAIL-NEWUSER-001)
  - Email包含帳號與提示「首次登入需重設密碼」
- **AD 帳號：**
  - 不產生初始密碼(使用 AD 密碼)
  - 自動依角色分配預設權限群組(Engineer → Engineer Group,Manager → Manager Group)
  - 發送 Email 通知使用者可透過「Windows 驗證登入」存取系統
  - 首次 AD 登入時,系統自動建立對應 User 記錄

**編輯模式限制：**
- 帳號不可修改
- 驗證類型不可修改
- 停用帳號前需確認:「此使用者負責 {N} 個未完成測項,停用後需重新分配,確定要停用嗎?」

**驗證規則：**
| 欄位 | 規則 | 錯誤訊息 |
|------|------|---------|
| Account | 英數底線、5-20字元、不可重複 | "帳號已存在" / "帳號格式不正確" |
| ADAccount | 格式需符合 DOMAIN\Account 或 sAMAccountName | "AD 帳號格式不正確" |
| ADAccount | 不可重複(系統內唯一) | "此 AD 帳號已被註冊" |
| Email | Email格式、不可重複 | "Email格式錯誤" / "此Email已被使用" |
| WeeklyHours | 1-80之間 | "每週工時需在1-80之間" |

#### 【新增】Email 大小寫規則(Case-Insensitive)

- 前端與後端比對 Email 一律採 **不區分大小寫比對**  
- 新增 / 編輯使用者時,後端會自動將 Email **轉為小寫再存入資料庫**  
- 任何兩個 Email(例如 `Jimmy@Lab.com` 與 `jimmy@lab.com`)會被視為完全相同  
- 當使用者以 Local 或 AD 登入時,後端會將登入取得的 Email 轉為小寫進行查詢 

---

### 1.6 使用者權限管理畫面 (SCR-USER-PERM-001) 【新增】

**權限要求：** Admin 或具備 `USER_MANAGE_PERMISSION` 權限

**開啟模式：** Modal Dialog (大視窗 800x600)

**畫面目的：** 管理特定使用者的權限群組與個別權限授予/撤銷

**畫面布局：**
```
┌───────────────────────────────────────────────┐
│ 使用者權限管理 - 王小明 (eng01)                 │
├───────────────────────────────────────────────┤
│                                               │
│ Tab 1: 權限群組                                │
│ ┌─────────────────────────────────────────┐   │
│ │ 【目前所屬群組】                         │   │
│ │ ☑ Engineer (工程師預設權限)             │   │
│ │ ☐ Manager (主管權限)                    │   │
│ │ ☐ Admin (系統管理者)                    │   │
│ │ ☐ Auditor (稽核人員)                    │   │
│ │                                         │   │
│ │ [套用變更]                               │   │
│ └─────────────────────────────────────────┘   │
│                                               │
│ Tab 2: 個別權限                               │
│ ┌─────────────────────────────────────────┐   │
│ │ 【額外授予的權限】                       │   │
│ │ Grid: 個別權限清單                       │   │
│ │ ┌────┬──────┬──────┬───────┬───┬──┐     │   │
│ │ │權限│授權人│授權日期│到期日│狀態│操作│    │   │
│ │ ├────┼──────┼──────┼───────┼───┼──┤     │   │
│ │ │查看│主管A│2025/11│2025/12│有效│[撤]│   │   │
│ │ │所有│      │/01   │/31   │    │銷 │    │   │
│ │ │工時│      │      │       │    │   │   │   │
│ │ └────┴──────┴──────┴───────┴───┴──┘     │   │
│ │                                         │   │
│ │ [新增個別權限]                           │   │
│ └─────────────────────────────────────────┘   │
│                                               │
│ Tab 3: 有效權限總覽                           │
│ ┌─────────────────────────────────────────┐   │
│ │ 【此使用者目前可執行的操作】              │   │
│ │ TreeView:                               │   │
│ │ ├─ 案件管理                              │   │
│ │ │  ├─ 查看案件 (來源:Engineer群組)       │   │
│ │ │  └─ 建立案件 (無權限)                  │   │
│ │ ├─ 測試項目                             │   │
│ │ │  ├─ 查看測項 (來源:Engineer群組)       │   │
│ │ │  └─ 編輯測項 (無權限)                  │   │
│ │ ├─ 工時管理                             │   │
│ │ │  ├─ 查看自己工時 (來源:Engineer群組)    │  │
│ │ │  ├─ 回報工時 (來源:Engineer群組)       │   │
│ │ │  └─ 查看所有工時 (來源:個別授權)        │   │
│ │ └─ ...                                  │   │
│ └─────────────────────────────────────────┘   │
│                                               │
│                        [關閉]                 │
└───────────────────────────────────────────────┘
```

**Tab 1 - 權限群組操作：**
| 操作 | 說明 | API 端點 |
|------|------|---------|
| 勾選群組 | 將使用者加入該權限群組 | POST /api/users/{id}/groups |
| 取消勾選 | 將使用者移出該權限群組 | DELETE /api/users/{id}/groups/{groupId} |
| 套用變更 | 批次更新群組關係 | PUT /api/users/{id}/groups |

**Tab 2 - 個別權限 Grid 欄位：**
| 欄位名稱 | 資料來源 | 寬度 | 說明 |
|---------|---------|------|------|
| 權限名稱 | PermissionName | 150px | 如「查看所有工時」 |
| 授權人 | GrantedByUser.DisplayName | 100px | - |
| 授權日期 | GrantedDate | 100px | - |
| 到期日 | ExpireDate | 100px | NULL 顯示「永久」 |
| 狀態 | IsActive + ExpireDate | 60px | 有效(綠)/已過期(灰) |
| 操作 | - | 80px | [撤銷]按鈕 |

**新增個別權限操作流程：**
```
1. 點擊「新增個別權限」
2. 開啟對話框 (SCR-USER-PERM-002)
3. 選擇要授予的 Permission
4. 選擇到期日(可選「永久」)
5. 確認後呼叫 API: POST /api/users/{id}/permissions
6. 重整 Grid
7. 寫入 AuditLog
```

**Tab 3 - 有效權限總覽：**
- 以 TreeView 結構顯示使用者目前所有有效權限
- 每個權限標註來源：
  - `(來源:Engineer群組)`
  - `(來源:個別授權)`
  - `(來源:Manager群組)`
- 無權限的項目以灰色斜體顯示「(無權限)」

**權限計算邏輯：**
```
使用者有效權限 = 
  (所有所屬群組的權限 UNION 所有個別授予的有效權限)
  WHERE IsActive = true 
    AND (ExpireDate IS NULL OR ExpireDate >= 當前日期)
```

---

### 1.7 新增個別權限對話框 (SCR-USER-PERM-002) 【新增】

**開啟模式：** Modal Dialog

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 選擇權限 | - | - | - |
| ComboBox | - | cboPermission | ✔ | 下拉顯示所有可用 Permission |
| Label | 權限說明 | - | - | - |
| TextBox (多行) | - | txtDescription | - | 唯讀,自動顯示選中權限的說明 |
| Label | 到期日 | - | - | - |
| RadioButton | 永久有效 | rdoPermanent | - | 預設選中 |
| RadioButton | 指定到期日 | rdoExpire | - | - |
| DateTimePicker | - | dtpExpireDate | ✔* | 選中 rdoExpire 時必填 |
| Label | 授權理由 | - | - | - |
| TextBox (多行) | - | txtReason | ✔ | 寫入 AuditLog |
| Button | 確定 | btnOK | - | - |
| Button | 取消 | btnCancel | - | - |

**權限下拉選單分類顯示：**
```
案件管理
  ├─ 查看案件 (PROJECT_VIEW)
  ├─ 建立案件 (PROJECT_CREATE)
  ├─ 編輯案件 (PROJECT_UPDATE)
  └─ 刪除案件 (PROJECT_DELETE)
測試項目
  ├─ 查看測項 (TESTITEM_VIEW)
  ├─ 編輯測項 (TESTITEM_UPDATE)
  └─ 刪除測項 (TESTITEM_DELETE)
工時管理
  ├─ 查看自己工時 (WORKLOG_VIEW_OWN)
  ├─ 查看所有工時 (WORKLOG_VIEW_ALL)
  ├─ 回報工時 (WORKLOG_CREATE)
  ├─ 修改工時(7天內) (WORKLOG_UPDATE_OWN)
  └─ 覆寫工時 (WORKLOG_OVERRIDE)
...
```

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 未選擇權限 | "請選擇要授予的權限" |
| 該權限已存在 | "此使用者已擁有此權限" |
| 到期日早於今天 | "到期日不可早於今天" |
| 授權理由空白 | "請填寫授權理由" |

---

### 1.8 權限群組管理畫面 (SCR-PERMGROUP-001) 【新增】

**權限要求：** Admin 或具備 `PERMISSION_MANAGE` 權限

**畫面目的：** 管理系統內的權限群組定義與其包含的權限

**畫面布局：**
```
┌─────────────────────────────────────────┐
│ [新增權限群組]  [搜尋: ________] [🔍]    │
├─────────────────────────────────────────┤
│ Grid: 權限群組清單                       │
│ ┌────┬──────┬──────────┬────┬─────┐     │
│ │選取│群組名稱│說明      │狀態│操作  │     │
│ ├────┼──────┼──────────┼────┼─────┤     │
│ │☐│Engineer│工程師預設│啟用│[編輯]│      │
│ │☐│Manager │主管權限  │啟用│[編輯]│      │
│ │☐│Admin   │系統管理員│啟用│[編輯]│      │
│ │☐│Auditor │稽核人員  │啟用│[編輯]│      │
│ └────┴──────┴──────────┴────┴─────┘     │
│                                         │
│  [批次停用]              [批次啟用]       │
└─────────────────────────────────────────┘
```

**Grid 欄位規格：**
| 欄位名稱 | 資料來源 | 寬度 | 排序 | 說明 |
|---------|---------|------|------|------|
| 選取 | - | 40px | ✖ | CheckBox |
| 群組名稱 | GroupName | 120px | ✓ | - |
| 說明 | Description | 300px | ✓ | - |
| 權限數量 | - | 80px | ✓ | 該群組包含的權限總數 |
| 使用者數量 | - | 80px | ✓ | 屬於該群組的使用者數 |
| 狀態 | IsActive | 60px | ✓ | 啟用(綠)/停用(灰) |
| 操作 | - | 150px | ✖ | [編輯][權限設定] |

**操作按鈕：**
| 按鈕 | 功能 | 跳轉畫面 |
|------|------|---------|
| 新增權限群組 | 開啟對話框 | SCR-PERMGROUP-002 |
| 編輯 | 修改群組名稱與說明 | SCR-PERMGROUP-002 |
| 權限設定 | 設定群組包含的權限 | SCR-PERMGROUP-003 |

---

### 1.9 編輯權限群組對話框 (SCR-PERMGROUP-002) 【新增】

**開啟模式：** Modal Dialog

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 群組名稱 | - | - | - |
| TextBox | - | txtGroupName | ✔ | 最大長度50 |
| Label | 說明 | - | - | - |
| TextBox (多行) | - | txtDescription | - | 最大長度200 |
| CheckBox | 啟用群組 | chkIsActive | - | 預設勾選,**停用後不可指派給新用戶** |
| Button | 確定 | btnOK | - | - |
| Button | 取消 | btnCancel | - | - |

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 群組名稱空白 | "請輸入群組名稱" |
| 群組名稱重複 | "此群組名稱已存在" |

**權限群組停用機制：**
- PermissionGroup **不支援刪除**,僅能透過 `IsActive = false` 停用
- 停用後該群組不可再指派給新用戶
- 已指派該群組的用戶不受影響(保留既有權限)
- 系統預設群組(Engineer/Manager/Admin)不可停用
- 停用操作寫入 AuditLog

---

### 1.10 權限群組權限設定畫面 (SCR-PERMGROUP-003) 【新增】

**開啟模式：** Modal Dialog (大視窗 700x600)

**畫面目的：** 設定某個權限群組包含哪些 Permission

**畫面布局：**
```
┌───────────────────────────────────────────┐
│ 權限群組權限設定 - Engineer                │
├───────────────────────────────────────────┤
│                                           │
│ 【選擇此群組應包含的權限】                   │
│                                           │
│ TreeView: (階層式 CheckBox)                │
│ ☑ 案件管理                                │
│   ☑ 查看案件 (PROJECT_VIEW)               │
│   ☐ 建立案件 (PROJECT_CREATE)             │
│   ☐ 編輯案件 (PROJECT_UPDATE)             │
│   ☐ 刪除案件 (PROJECT_DELETE)             │
│ ☑ 測試項目                                │
│   ☑ 查看測項 (TESTITEM_VIEW)              │
│   ☐ 編輯測項 (TESTITEM_UPDATE)            │
│   ☐ 刪除測項 (TESTITEM_DELETE)            │
│ ☑ 工時管理                                │
│   ☑ 查看自己工時 (WORKLOG_VIEW_OWN)       │
│   ☐ 查看所有工時 (WORKLOG_VIEW_ALL)       │
│   ☑ 回報工時 (WORKLOG_CREATE)             │
│   ☑ 修改工時(7天內) (WORKLOG_UPDATE_OWN)  │
│   ☐ 覆寫工時 (WORKLOG_OVERRIDE)           │
│ ☐ 使用者管理                              │
│   ☐ 查看使用者 (USER_VIEW)                │
│   ☐ 建立使用者 (USER_CREATE)              │
│   ☐ 編輯使用者 (USER_UPDATE)              │
│   ☐ 管理權限 (USER_MANAGE_PERMISSION)     │
│ ...                                       │
│                                           │
│            [全選] [全不選] [套用預設]      │
│                                           │
│                        [確定] [取消]       │
└───────────────────────────────────────────┘
```

**操作按鈕：**
| 按鈕 | 功能 |
|------|------|
| 全選 | 勾選所有權限 |
| 全不選 | 取消所有勾選 |
| 套用預設 | 依群組類型套用預設權限集合 |
| 確定 | 儲存變更至 PermissionGroupMapping |

**預設權限集合定義：**

**Engineer 預設權限：**
- PROJECT_VIEW
- TESTITEM_VIEW
- TESTITEM_STATUS_CANCEL
- WORKLOG_VIEW_OWN
- WORKLOG_CREATE
- WORKLOG_UPDATE_OWN (7天內)
- LOADING_VIEW_OWN

**Manager 預設權限：**
- (繼承 Engineer 所有權限)
- PROJECT_CREATE, PROJECT_UPDATE, PROJECT_DELETE
- TESTITEM_CREATE, TESTITEM_UPDATE, TESTITEM_DELETE
- TESTITEM_ASSIGN_ENGINEER, TESTITEM_ASSIGN_SUPPORT, TESTITEM_REMOVE_ENGINEER
- TESTITEM_STATUS_OVERRIDE
- TESTITEM_REVISION_CREATE
- REGULATION_ADD, REGULATION_DISABLE, REGULATION_REMOVE
- WORKLOG_VIEW_ALL
- WORKLOG_OVERRIDE
- WORKLOG_DELETE
- LOADING_VIEW_ALL
- DELAY_MANAGE
- USER_VIEW, USER_CREATE, USER_UPDATE
- AUDIT_VIEW
- REPORT_VIEW_ALL

**Admin 預設權限：**
- (繼承 Manager 所有權限)
- USER_MANAGE_PERMISSION
- PERMISSION_MANAGE
- SYSTEM_SETTING

**確定動作：**
1. 收集所有勾選的 PermissionId
2. 呼叫 API: PUT /api/permissiongroups/{groupId}/permissions
3. Request Body:
   ```json
   {
     "mappingId": 123,
     "groupId": 1,
     "permissionId": 5,
     "createdByUserId": 10,
     "createdDate": "2025-11-20T10:30:00Z"
   }
   ```
**說明：**
- 新增 `CreatedByUserId` 欄位記錄操作者
- 新增 `CreatedDate` 欄位記錄建立時間
- 變更權限對應時需寫入 AuditLog
- 刪除舊對應時不實際刪除,而是建立完整的新對應集合
4. 後端刪除舊的 PermissionGroupMapping,新增新的對應
5. 寫入 AuditLog
6. 關閉對話框

---

## 2. 案件建立精靈 (Wizard)

### 2.1 精靈主畫面 (SCR-WIZARD-001)

**權限要求：** Manager 或具備 `PROJECT_CREATE` 權限

**步驟導航規則：**
- Step1 完成前無法進入 Step2
- 可隨時返回前一步驟修改
- Step4 完成後顯示「完成」按鈕
- 點擊「完成」前會顯示完整預覽摘要

**離開確認：**
- 未完成時點擊關閉 → 提示「資料尚未儲存,確定要離開嗎?」
- Step4完成後點擊「完成」→ 寫入DB → 跳轉至案件總覽

---

### 2.2 Step 1 - 建立案件基本資料 (SCR-WIZARD-002)

**驗證規則：**
| 驗證項目 | 錯誤訊息 | 觸發時機 |
|---------|---------|---------|
| 案件名稱空白 | "請輸入案件名稱" | 下一步 |
| 案件名稱重複 | "此案件名稱已存在" | txtProjectName.Validating |
| 開始日期早於今天 | "開始日期不可早於今天" | dtpStartDate.ValueChanged |
| 結束日期早於開始日期 | "結束日期需晚於開始日期" | 下一步 |
| 日期區間超過1年 | 警告:"日期區間超過365天,確定嗎?" | 下一步 |

**下一步動作：**
- 驗證通過 → 將資料暫存至Wizard Context
- 跳轉至 Step 2

---

### 2.3 Step 2 - 選擇法規 (SCR-WIZARD-003)

**左側清單 (可選法規):**
- 顯示所有系統內建的法規名稱
- 支援多選(CheckBox)
- 支援搜尋過濾

**中間按鈕：**
| 按鈕 | 功能 | 啟用條件 |
|------|------|---------|
| >> | 將選中的法規加入右側 | 左側至少勾選1項 |
| << | 將選中的法規移除 | 右側至少選中1項 |

**右側清單 (已選法規):**
- 顯示已加入的法規及其開始/結束日期
- 點選某法規 → 下方顯示日期編輯區

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 未選擇任何法規 | "至少需選擇1個法規" |
| 法規日期超出案件日期±30天 | "法規日期需在案件日期前後30天內" |
| 法規結束日期早於開始日期 | "結束日期需晚於開始日期" |

**下一步動作：**
- 將已選法規及日期暫存至Wizard Context
- 跳轉至 Step 3

---

### 2.4 Step 3 - 新增測試項目 (SCR-WIZARD-004)

**操作流程：**
1. 從下拉選單選擇法規(預設第一個)
2. 點擊「新增測項」→ 開啟對話框 (SCR-WIZARD-005)
3. 在Grid中檢視/編輯/刪除測項
4. 切換法規時,顯示該法規的測項清單

**刪除確認：**
- "確定要刪除測項「{TestItemName}」嗎?"
- 刪除後重新計算預估工時小計

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 某法規下無任何測項 | "法規「{RegulationName}」至少需1個測項" |
| 預估工時小計=0 | "總預估工時不可為0" |

**下一步動作：**
- 驗證所有法規皆有測項
- 將測項資料暫存至Wizard Context
- 跳轉至 Step 4

---

### 2.5 Step 3-1 - 新增/編輯測項對話框 (SCR-WIZARD-005)

**開啟模式：** Modal Dialog

**測試類型選項：**
- Conducted 
- Radiated
- Blocking 
- Adaptivity
- DFS
- Other (其他)

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 測項名稱空白 | "請輸入測項名稱" |
| 測項名稱在該法規下重複 | "此測項名稱已存在於該法規" |
| 預估工時≤0 | "預估工時需大於0" |
| 預估工時>100 | 警告:"工時超過100小時,確認嗎?" |

**確定動作：**
- 驗證通過 → 回傳測項資料至 SCR-WIZARD-004
- 在Grid中新增/更新該列

---

### 2.6 Step 4 - 分配工程師 (SCR-WIZARD-006)

**操作流程：**
1. 選擇法規 → 選擇測項
2. 點擊「新增工程師」→ 開啟對話框 (SCR-WIZARD-007)
3. 檢視Loading警示(>80%顯示紅色)
4. 調整分配工時
5. 切換至其他測項繼續分配

**驗證規則：**
| 驗證項目 | 錯誤訊息 | 類型 |
|---------|---------|------|
| 未分配任何工程師 | "此測項至少需分配1位工程師" | 錯誤 |
| 無主要工程師 | "此測項至少需1位主要工程師" | 錯誤 |
| 分配工時小計與預估工時差異>20% | "分配工時與預估工時差異過大" | 警告 |
| 某工程師Loading>100% | "工程師{Name}已超載({Loading}%)" | 警告 |

**Loading計算公式：**
```
Loading(%) = Σ(所有Active案件該工程師的AssignedHours) / WeeklyAvailableHours × 100
```

**下一步動作(完成按鈕)：**
1. 驗證所有測項皆已分配工程師
2. 顯示確認摘要對話框 (SCR-WIZARD-008)
3. 確認後寫入DB：
   - Project (Status=Active)
   - Regulation (Status=NotStarted)
   - TestItem (Status=NotStarted)
   - TestItemEngineer
4. 寫入AuditLog (Action=Create)
5. 跳轉至案件總覽畫面

---

### 2.7 Step 4-1 - 新增/編輯工程師分配對話框 (SCR-WIZARD-007)

**開啟模式：** Modal Dialog

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 選擇工程師 | - | - | - |
| ComboBox | - | cboEngineer | ✔ | 下拉顯示所有工程師 |
| Label | 角色類型 | - | - | - |
| ComboBox | - | cboRoleType | ✔ | Main1/Main2/Main3/Support |
| Label | 分配工時 | - | - | - |
| NumericUpDown | - | nudAssignedHours | ✔ | 0.5-999.9 |
| Label | 當前Loading | - | - | - |
| Label | - | lblCurrentLoading | - | 顯示該工程師目前Loading% |
| Label | 新增後Loading | - | - | - |
| Label | - | lblNewLoading | - | 顯示新增此分配後的Loading% |
| Button | 確定 | btnOK | - | - |
| Button | 取消 | btnCancel | - | - |

**RoleType 說明：**
| RoleType | 說明 | 權重 | 備註 |
|----------|------|------|------|
| Main1 | 主要負責工程師 | 100% | 測項主要負責人,負責協調與最終確認 |
| Main2 | 次要負責工程師 | 80% | 協助主要工程師,可獨立執行測試 |
| Main3 | 第三負責工程師 | 60% | 輔助角色,學習與支援 |
| Support | 支援工程師 | 40% | 提供技術支援,不直接負責測試執行 |

**RoleType 使用規則：**
- 每個測項至少需要 1 位 Main1
- Main1 可以有多位(例如複雜測項需要多位主要負責人)
- Main2/Main3 為選配,視測項複雜度決定
- Support 角色不計入主要工程師數量
- RoleType 會影響工時回報權限與 Loading 計算權重

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 未選擇工程師 | "請選擇工程師" |
| 分配工時≤0 | "分配工時需大於0" |
| 該工程師已在清單中 | "此工程師已被分配至此測項" |
| NewLoading>100% | 警告:"此分配將使工程師超載,確定嗎?" |

---

### 2.8 Step 4-2 - 完成確認摘要對話框 (SCR-WIZARD-008)

**開啟模式：** Modal Dialog

**超載工程師顯示：**
- 若無超載:顯示「無」(綠色)
- 若有超載:顯示「王小明(120%), 李小華(95%)」(紅色)

**確定建立動作：**
1. 顯示進度條:「正在建立案件...」
2. 呼叫API:POST /api/projects/create-with-wizard
3. 後端Transaction處理
4. 成功:顯示「案件建立成功!」→ 3秒後跳轉
5. 失敗:顯示錯誤訊息,保持在Wizard畫面

---

## 3. 工時回報模組

### 3.1 我的測項清單畫面 (SCR-WORKLOG-001)

**權限要求：** Engineer 或具備 `WORKLOG_VIEW_OWN` 權限

**篩選選項：**
- 全部
- 未開始
- 進行中
- 已完成
- 延遲中

**狀態顏色標示：**
- 未開始:灰色
- 進行中:藍色
- 已完成:綠色
- 延遲中:紅色

**操作按鈕：**
| 按鈕 | 功能 | 跳轉畫面 | 權限要求 |
|------|------|---------|---------|
| 回報工時 | 開啟工時回報對話框 | SCR-WORKLOG-002 | WORKLOG_CREATE |
| 查看記錄 | 顯示該測項的所有WorkLog | SCR-WORKLOG-003 | WORKLOG_VIEW_OWN |
| 取消完成 | 取消測項完成狀態 | SCR-WORKLOG-004 | TESTITEM_STATUS_CANCEL |

---

### 3.2 回報工時對話框 (SCR-WORKLOG-002)

**權限要求：** 具備 `WORKLOG_CREATE` 權限

**開啟模式：** Modal Dialog

**延遲原因CheckBox動態控制：**
```
rdoDelayed.CheckedChanged:
    IF rdoDelayed.Checked THEN
        chkListDelayReason.Visible = True
        chkListDelayReason.Required = True
    ELSE
        chkListDelayReason.Visible = False
        chkListDelayReason.Required = False
        清空所有勾選
    END IF
```

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 工作日期晚於今天 | "不可回報未來日期" |
| 實際工時≤0 | "實際工時需大於0" |
| 實際工時>12 | "單日工時不可超過12小時" |
| 該日期已回報過 | "此日期已有工時記錄,請至記錄列表修改" |
| 狀態=延遲但未選延遲原因 | "請至少選擇1個延遲原因" |
| ActualHours累計>AssignedHours×1.5 | 警告:"工時已超過分配工時50%,確定嗎?" |

**送出動作：**
1. 驗證通過
2. 呼叫API:POST /api/worklogs
3. 後端自動更新TestItem.Status：
   - 若Status=Delayed → TestItem.Status=Delayed
   - 若Status=Completed且所有工程師都完成 → TestItem.Status=Completed
   - 否則 → TestItem.Status=InProgress
4. 寫入AuditLog
5. 關閉對話框,重整SCR-WORKLOG-001清單

#### 【新增】需選擇 TestItemRevision

若某 TestItem 存在多個補測版本(TestItemRevision),  
工時回報時使用者必須選擇「對應的補測版本(RevisionId)」。

**畫面新增欄位：**

| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| ComboBox | 測項版本 | cboRevision | ✔ | 顯示所有有效的 TestItemRevision |

**驗證規則：**

| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 未選擇 RevisionId | "請選擇本次工時所屬的補測版本" |

**後端行為：**

- 新增 WorkLog 時必須包含 `RevisionId`
- 若 TestItem 只有 1 個版本,後端自動帶入該版本
- Revision 刪除時不可刪除已被 WorkLog 使用者(需改為停用)

---

### 3.3 工時記錄查詢畫面 (SCR-WORKLOG-003)

**權限要求：** 具備 `WORKLOG_VIEW_OWN` 權限(查看自己)或 `WORKLOG_VIEW_ALL`(查看所有)

**開啟模式：** Modal Dialog 

**編輯規則：**
- 7天內的記錄可編輯(需 `WORKLOG_UPDATE_OWN` 權限)
- 超過7天:顯示「此記錄已超過7天,無法修改」
- Manager 可覆寫任何工時(需 `WORKLOG_OVERRIDE` 權限)

**刪除規則：**
- 7天內可刪除
- 需確認:「確定要刪除此工時記錄嗎?」
- 刪除後重新計算TestItem狀態

**匯出Excel功能：**
- 匯出當前篩選結果
- 檔名格式:`WorkLog_{TestItemName}_{DateTime}.xlsx`

**WorkLog 刪除機制：**
- 採用 Soft Delete(`IsDeleted = true`)
- 僅 Manager 具有 `WORKLOG_DELETE` 權限
- 刪除需填寫理由,寫入 AuditLog
- 一般工程師無法刪除工時,僅能修改(7天內)

---

### 3.4 測項完成狀態取消功能 (SCR-WORKLOG-004) 【新增】

**權限要求：** Engineer 或具備 `TESTITEM_STATUS_CANCEL` 權限

**開啟方式：** SCR-WORKLOG-001 → 選擇已完成測項 → [取消完成]按鈕

**功能目的：** 允許工程師取消自己誤按的測項完成狀態

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 測項名稱 | - | - | 顯示要取消的測項 |
| Label | 目前狀態 | - | - | Completed |
| Label | 警告訊息 | - | - | 紅色文字,見下方 |
| TextBox (多行) | 取消原因 | txtReason | ✔ | 寫入 AuditLog |
| Button | 確定取消 | btnConfirm | - | - |
| Button | 返回 | btnCancel | - | - |

**警告訊息內容：**
```
⚠ 取消完成狀態後,此測項將改為「進行中」
⚠ 此操作將寫入稽核日誌
⚠ 請確認您確實需要取消完成狀態
```

**操作流程：**
```
1. 工程師選擇狀態為 Completed 的測項
2. 點擊[取消完成]按鈕
3. 系統檢查權限:TESTITEM_STATUS_CANCEL
4. 開啟取消確認對話框
5. 填寫取消原因
6. 確認後呼叫 API: POST /api/testitems/{id}/cancel-completion
7. 後端處理：
   ├─ 檢查該測項是否為 Completed
   ├─ 檢查操作者是否為該測項的負責工程師
   ├─ 更新 TestItem.Status = InProgress
   ├─ 寫入 AuditLog
   │   - Action = StatusChange
   │   - OldValue = Completed
   │   - NewValue = InProgress
   │   - Reason = 取消原因
   └─ 重新計算 Regulation.Status 與 Project.Status
8. 刷新測項清單
```

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 測項非 Completed 狀態 | "此測項目前不是已完成狀態" |
| 非負責工程師 | "您不是此測項的負責工程師,無法取消完成狀態" |
| 取消原因空白 | "請填寫取消原因" |
| 取消原因過短 | "取消原因至少需10個字元" |

**後端驗證規則：**
| 情境 | HTTP狀態碼 | 錯誤訊息 |
|------|-----------|---------|
| 測項已有補測版本 | 409 | "此測項已建立補測版本,無法取消完成" |
| 測項被主管設為 OnHold | 409 | "此測項已被主管設為暫停,請洽繫主管" |

---

## 4. 案件管理模組 (Manager)

### 4.1 案件總覽畫面 (SCR-PROJECT-001)

**權限要求：** Manager 或具備 `PROJECT_VIEW` 權限

**畫面類型：** GridControl 列表模式

**畫面布局：**
```
┌─────────────────────────────────────────┐
│ [新增案件]  [搜尋: ________] [🔍]      │
│ [狀態篩選: ▼全部] [優先順序: ▼全部]    │
├─────────────────────────────────────────┤
│ Grid: 案件清單                          │
│ ┌───┬────────┬──────┬────┬────┬─────┐  │
│ │選│案件名稱│狀態  │優先│進度│操作  │  │
│ ├───┼────────┼──────┼────┼────┼─────┤  │
│ │☐│5G專案  │進行中│高  │65% │[詳細]│  │
│ │☐│Wi-Fi6E │延遲  │中  │40% │[詳細]│  │
│ └───┴────────┴──────┴────┴────┴─────┘  │
│                                         │
│  [批次停用]              [批次啟用]      │
└─────────────────────────────────────────┘
```

**Grid 欄位規格：**
| 欄位名稱 | 資料來源 | 寬度 | 對齊 | 排序 | 說明 |
|---------|---------|------|------|------|------|
| 選取 | - | 40px | 中 | ✖ | CheckBox |
| 案件名稱 | ProjectName | 200px | 左 | ✓ | 預設排序 |
| 狀態 | Status | 100px | 中 | ✓ | Draft/Active/Completed/Delayed/OnHold |
| 優先順序 | Priority | 80px | 中 | ✓ | High(紅)/Medium(橘)/Low(綠) |
| 進度 | - | 100px | 中 | ✓ | 進度條+百分比 |
| 開始日期 | StartDate | 100px | 中 | ✓ | yyyy/MM/dd |
| 結束日期 | EndDate | 100px | 中 | ✓ | yyyy/MM/dd |
| 負責主管 | ManagerName | 100px | 左 | ✓ | - |
| 操作 | - | 150px | 中 | ✖ | [詳細][編輯][刪除] |

**狀態篩選選項：**
- 全部
- 草稿 (Draft)
- 進行中 (Active)
- 已完成 (Completed)
- 延遲 (Delayed)
- 暫停 (OnHold)

**優先順序顏色：**
- High:紅色粗體
- Medium:橘色
- Low:綠色

**操作按鈕：**
| 按鈕 | 功能 | 跳轉畫面 | 權限要求 |
|------|------|---------|---------|
| 新增案件 | 開啟建案精靈 | SCR-WIZARD-001 | PROJECT_CREATE |
| 詳細 | 查看案件詳情 | SCR-PROJECT-002 | PROJECT_VIEW |
| 編輯 | 修改案件基本資料 | SCR-PROJECT-003 | PROJECT_UPDATE |
| 刪除 | Soft Delete | 確認對話框 | PROJECT_DELETE |

**刪除確認：**
```
確定要刪除案件「{ProjectName}」嗎?

此操作將同時刪除：
• {RegulationCount} 個法規
• {TestItemCount} 個測項
• 所有工程師分配記錄

⚠ 工時記錄將保留以供查詢

[取消] [確定刪除]
```

#### 【新增】Regulation 狀態 (Status) 定義與自動計算規則

每個 Regulation 會自動根據其底下所有 TestItem 的狀態計算出 Regulation.Status。  
狀態等級如下：

- **NotStarted**:所有 TestItem 均為 NotStarted  
- **InProgress**:至少有 1 個 TestItem 為 InProgress  
- **Delayed**:至少有 1 個 TestItem 為 Delayed  
- **Completed**:所有 TestItem 均為 Completed  
- **OnHold**:主管手動設定

**計算順序(由高優先度到低)：**
```
IF 主管手動設定 OnHold → Regulation.Status = OnHold(最高優先級,手動狀態應被保留)
ELSE IF 任一 TestItem.Status = Delayed → Regulation.Status = Delayed
ELSE IF 所有 TestItem.Status = Completed → Regulation.Status = Completed
ELSE IF 任一 TestItem.Status = InProgress → Regulation.Status = InProgress
ELSE → Regulation.Status = NotStarted(初始狀態)
```

**Regulation 狀態會在下列事件中自動更新：**
- 測項建立/刪除  
- 測項狀態變更(工時回報後自動變更)  
- 補測版本完成  
- 工時覆寫  

**狀態手動覆寫：**
- Manager 可手動將 Regulation.Status 設為 OnHold
- 設定 `ManualStatusOverride = true` 標記
- 手動設定 OnHold 後,不會被自動邏輯覆蓋
- 取消手動狀態時,系統重新計算
- 所有狀態變更寫入 AuditLog

---

### 4.2 案件詳細畫面 (SCR-PROJECT-002)

**權限要求：** 具備 `PROJECT_VIEW` 權限

**Tab 1 - 法規與測項：**
- TreeView結構：
  ```
  ├─ FCC (2025/11/01 ~ 2025/11/30)
  │  ├─ Conducted (未開始)
  │  └─ Radiated (進行中)
  ├─ CE (2025/12/01 ~ 2025/12/31)
  │  └─ DFC (未開始)
  ```

**右鍵選單：**
- [編輯測項] → SCR-TESTITEM-001
- [修改狀態] → SCR-TESTITEM-STATUS-001
- [指派工程師] → SCR-ENGINEER-ASSIGN-001
- [建立補測版本] → SCR-TESTITEM-REVISION-001
- [查看補測版本] → SCR-REVISION-LIST-001

**Tab 2 - 工時統計：**
- Chart: 預估工時 vs 實際工時 (Bar Chart)
- Grid: 各測項工時明細
- 權限要求:`WORKLOG_VIEW_ALL` 或 `REPORT_VIEW_ALL`

**Tab 3 - 延遲分析：**
- PieChart: 延遲原因分布
- Grid: 延遲測項清單
- 權限要求:`DELAY_VIEW` 或 `REPORT_VIEW_ALL`

---

### 4.3 編輯案件對話框 (SCR-PROJECT-003)

**權限要求：** 具備 `PROJECT_UPDATE` 權限

**開啟模式：** Modal Dialog

**畫面元素：** (同 SCR-WIZARD-002,但不含Wizard步驟導航)

**額外欄位：**
| 元素類型 | 標籤 | 控制項 | 說明 |
|---------|------|--------|------|
| ComboBox | 狀態 | cboStatus | Manager可手動設為OnHold |
| TextBox (多行) | 修改理由 | txtReason | 修改時必填,寫入AuditLog |

**驗證規則：** (同建案規則)

---

### 4.4 測項管理畫面 (SCR-TESTITEM-001)

**權限要求：** 具備 `TESTITEM_UPDATE` 權限

**進入方式：** SCR-PROJECT-002 → TreeView右鍵 → 編輯測項

**開啟模式：** Modal Dialog

**建立補測版本功能：**
當建立 TestItemRevision 後,系統會：
1. 自動將 TestItem.Status 設為 InProgress
2. 清除 TestItem.ManualStatusOverride(如果之前被設為手動狀態)
3. 寫入 AuditLog：
   ```json
   {
     "Action": "RevisionCreated",
     "OldValue": {"Status": "Completed", "ManualStatusOverride": true},
     "NewValue": {"Status": "InProgress", "ManualStatusOverride": false},
     "Reason": "建立補測版本 v2"
   }
   ```

**⚠ 注意：** 即使測項被設為手動狀態,建立補測版本仍會自動改為 InProgress,但主管可在建立後再次手動修改。

**建立原因選項：**
- 測試不通過需重測
- 客戶要求新版本
- 樣品更換
- 韌體更新
- 場地變更
- 其他

**操作流程：**
1. 填寫版本資訊
2. 若勾選「複製原工程師分配」：
   - 自動建立相同的TestItemEngineer記錄
   - AssignedHours = 原值
3. 若未勾選：
   - 建立空的版本,後續需手動分配工程師
4. 確定後：
   - 建立TestItemRevision記錄
   - TestItem.Status自動改為InProgress
   - 寫入AuditLog

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 版本編號已存在 | "此版本編號已存在" |
| 預估工時≤0 | "預估工時需大於0" |
| 建立原因空白 | "請選擇或填寫建立原因" |

---

### 4.5 測項狀態手動覆寫功能 (SCR-TESTITEM-STATUS-001) 【新增】

**權限要求：** Manager 或具備 `TESTITEM_STATUS_OVERRIDE` 權限

**進入方式：** SCR-PROJECT-002 → TreeView右鍵 → [修改狀態]

**開啟模式：** Modal Dialog

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 測項名稱 | - | - | 顯示測項資訊 |
| Label | 目前狀態 | - | - | 顯示當前狀態 |
| ComboBox | 新狀態 | cboNewStatus | ✔ | NotStarted/InProgress/Completed/Delayed/OnHold |
| CheckBox | 設為手動狀態 | chkManualOverride | - | 勾選後狀態不會被自動邏輯覆寫 |
| TextBox (多行) | 修改理由 | txtReason | ✔ | 寫入 AuditLog |
| Button | 確定 | btnOK | - | - |
| Button | 取消 | btnCancel | - | - |

**狀態選項說明：**
| 狀態值 | 顯示文字 | 說明 |
|--------|---------|------|
| NotStarted | 未開始 | 測項尚未有任何工時回報 |
| InProgress | 進行中 | 測項正在進行測試 |
| Completed | 已完成 | 測項測試完成 |
| Delayed | 延遲中 | 測項因故延遲 |
| OnHold | 暫停 | 主管暫停此測項 |

**設為手動狀態說明：**
```
☑ 勾選此選項後,此測項的狀態將不會被以下事件自動變更：
  • 工程師回報工時
  • 建立補測版本
  • 其他自動狀態計算邏輯

⚠ 手動狀態僅能由主管再次手動修改
⚠ 如需恢復自動狀態計算,請將此選項取消勾選
```

**操作流程：**
```
1. 主管在測項上右鍵選擇[修改狀態]
2. 系統檢查權限:TESTITEM_STATUS_OVERRIDE
3. 開啟狀態修改對話框
4. 選擇新狀態
5. 決定是否勾選「設為手動狀態」
6. 填寫修改理由
7. 確認後呼叫 API: PUT /api/testitems/{id}/status
8. 後端處理：
   ├─ 檢查權限
   ├─ 更新 TestItem.Status = NewStatus
   ├─ 更新 TestItem.ManualStatusOverride = chkManualOverride
   ├─ 寫入 AuditLog
   │   - Action = StatusOverride
   │   - OldValue = JSON{Status, ManualStatusOverride}
   │   - NewValue = JSON{NewStatus, NewManualOverride}
   │   - Reason = 修改理由
   └─ 重新計算 Regulation.Status 與 Project.Status
9. 刷新測項狀態顯示
```

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 新狀態與目前相同 | "新狀態與目前狀態相同" |
| 修改理由空白 | "請填寫修改理由" |
| 修改理由過短 | "修改理由至少需10個字元" |

---

### 4.6 【新增】法規管理畫面 (SCR-REGULATION-001)

**權限要求：** Manager 或具備 `REGULATION_ADD` 權限

**進入方式：** SCR-PROJECT-002 → [法規管理] Tab 或按鈕

**開啟模式：** 獨立視窗或嵌入式面板

**畫面布局：**
```
┌─────────────────────────────────────────┐
│ 案件「5G專案」- 法規管理                 │
├─────────────────────────────────────────┤
│ [新增法規]                               │
├─────────────────────────────────────────┤
│ Grid: 法規清單                          │
│ ┌───┬────────┬──────┬──────┬────┬─────┐│
│ │選│法規名稱│狀態  │開始  │結束│操作  ││
│ ├───┼────────┼──────┼──────┼────┼─────┤│
│ │☐│FCC     │進行中│11/01 │11/30│[編輯]││
│ │☐│CE      │未開始│12/01 │12/31│[編輯]││
│ └───┴────────┴──────┴──────┴────┴─────┘│
│                                         │
│ [停用所選]  [移除所選]                   │
└─────────────────────────────────────────┘
```

**Grid 欄位規格：**
| 欄位名稱 | 資料來源 | 寬度 | 說明 |
|---------|---------|------|------|
| 選取 | - | 40px | CheckBox |
| 法規名稱 | RegulationName | 150px | - |
| 狀態 | Status | 100px | NotStarted/InProgress/Completed/Delayed/OnHold |
| 開始日期 | StartDate | 100px | yyyy/MM/dd |
| 結束日期 | EndDate | 100px | yyyy/MM/dd |
| 測項數量 | - | 80px | 該法規下的測項總數 |
| 操作 | - | 180px | [編輯][停用][移除] |

**操作按鈕：**
| 按鈕 | 功能 | API 端點 | 權限要求 |
|------|------|---------|---------|
| 新增法規 | 開啟對話框 | POST /api/regulations | REGULATION_ADD |
| 編輯 | 修改法規資訊 | PUT /api/regulations/{id} | REGULATION_ADD |
| 停用 | 設為 IsActive=false | POST /api/regulations/{id}/disable | REGULATION_DISABLE |
| 移除 | Soft Delete | DELETE /api/regulations/{id} | REGULATION_REMOVE |

**停用與移除差異：**
- **停用**:法規設為 IsActive=false,保留所有測項,但不再顯示在新案件選單
- **移除**:Soft Delete,法規與測項都標記為已刪除,但資料保留

---

### 4.7 【新增】新增/編輯法規對話框 (SCR-REGULATION-002)

**開啟模式：** Modal Dialog

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 法規名稱 | - | - | - |
| TextBox | - | txtRegulationName | ✔ | 最大長度100 |
| Label | 開始日期 | - | - | - |
| DateTimePicker | - | dtpStartDate | ✔ | - |
| Label | 結束日期 | - | - | - |
| DateTimePicker | - | dtpEndDate | ✔ | - |
| Label | 說明 | - | - | - |
| TextBox (多行) | - | txtDescription | - | 最大長度500 |
| CheckBox | 啟用 | chkIsActive | - | 預設勾選 |
| Button | 確定 | btnOK | - | - |
| Button | 取消 | btnCancel | - | - |

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 法規名稱空白 | "請輸入法規名稱" |
| 結束日期早於開始日期 | "結束日期需晚於開始日期" |
| 日期超出案件日期範圍 | "法規日期需在案件日期範圍內" |

---

### 4.8 【新增】工程師指派畫面 (SCR-ENGINEER-ASSIGN-001)

**權限要求：** Manager 或具備 `TESTITEM_ASSIGN_ENGINEER` 權限

**進入方式：** SCR-PROJECT-002 → TreeView右鍵 → [指派工程師]

**開啟模式：** Modal Dialog

**畫面布局：**
```
┌─────────────────────────────────────────┐
│ 工程師指派 - Conducted Test              │
├─────────────────────────────────────────┤
│ [新增工程師]                             │
├─────────────────────────────────────────┤
│ Grid: 已指派工程師清單                   │
│ ┌────────┬────────┬──────┬──────┬─────┐│
│ │工程師  │角色類型│分配  │Loading│操作  ││
│ ├────────┼────────┼──────┼──────┼─────┤│
│ │王小明  │Main1   │8.0hr │75%   │[移除]││
│ │李小華  │Support │4.0hr │60%   │[移除]││
│ └────────┴────────┴──────┴──────┴─────┘│
│                                         │
│ 預估工時: 12.0hr  已分配: 12.0hr         │
│                                         │
│                        [確定] [取消]     │
└─────────────────────────────────────────┘
```

**Grid 欄位規格：**
| 欄位名稱 | 資料來源 | 寬度 | 說明 |
|---------|---------|------|------|
| 工程師 | EngineerName | 120px | - |
| 角色類型 | RoleType | 100px | Main1/Main2/Main3/Support |
| 分配工時 | AssignedHours | 100px | 可編輯 |
| Loading | - | 80px | 該工程師目前Loading% |
| 操作 | - | 80px | [移除]按鈕 |

**操作按鈕：**
| 按鈕 | 功能 | 權限要求 |
|------|------|---------|
| 新增工程師 | 開啟 SCR-WIZARD-007 | TESTITEM_ASSIGN_ENGINEER |
| 移除 | 移除工程師指派 | TESTITEM_REMOVE_ENGINEER |

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 至少需1位Main1 | "測項至少需要1位主要負責工程師(Main1)" |
| 分配工時與預估工時差異>20% | 警告:"分配工時與預估工時差異過大" |

---

### 4.9 【新增】補測版本列表畫面 (SCR-REVISION-LIST-001)

**權限要求：** 具備 `TESTITEM_VIEW` 權限

**進入方式：** SCR-PROJECT-002 → TreeView右鍵 → [查看補測版本]

**開啟模式：** Modal Dialog

**畫面布局：**
```
┌─────────────────────────────────────────┐
│ 補測版本列表 - Conducted Test            │
├─────────────────────────────────────────┤
│ Grid: 版本清單                          │
│ ┌────┬──────┬────────┬──────┬────┬────┐│
│ │版本│類型  │預估工時│建立者│日期│操作││
│ ├────┼──────┼────────┼──────┼────┼────┤│
│ │v1  │原始  │8.0hr   │主管A │11/1│-   ││
│ │v2  │Retest│6.0hr   │主管A │11/5│[回]││
│ │v3  │Fix   │4.0hr   │主管B │11/8│[回]││
│ └────┴──────┴────────┴──────┴────┴────┘│
│                                         │
│                              [關閉]     │
└─────────────────────────────────────────┘
```

**Grid 欄位規格：**
| 欄位名稱 | 資料來源 | 寬度 | 說明 |
|---------|---------|------|------|
| 版本編號 | RevisionNumber | 80px | v1, v2, v3... |
| 類型 | RevisionType | 100px | Command/Retest/Fix/Others |
| 預估工時 | EstimatedHours | 100px | - |
| 建立者 | CreatedByUser | 100px | - |
| 建立日期 | CreatedDate | 100px | yyyy/MM/dd |
| 原因 | Reason | 200px | 補測原因說明 |
| 操作 | - | 80px | [回滾]按鈕(僅最新版本) |

**回滾功能：**
- 僅最新版本可回滾
- 需 `TESTITEM_REVISION_ROLLBACK` 權限
- 點擊後跳轉至 SCR-REVISION-ROLLBACK-001

---

### 4.10 【新增】補測版本回滾對話框 (SCR-REVISION-ROLLBACK-001)

**權限要求：** Manager 或具備 `TESTITEM_REVISION_ROLLBACK` 權限

**開啟模式：** Modal Dialog

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 測項名稱 | - | - | 顯示測項資訊 |
| Label | 當前版本 | - | - | 顯示要回滾的版本 |
| Label | 警告訊息 | - | - | 紅色文字,見下方 |
| TextBox (多行) | 回滾理由 | txtReason | ✔ | 寫入 AuditLog |
| Button | 確定回滾 | btnConfirm | - | - |
| Button | 取消 | btnCancel | - | - |

**警告訊息內容：**
```
⚠ 回滾版本後,該版本的所有工時記錄將被標記為無效
⚠ 測項將回到前一版本狀態
⚠ 此操作將寫入稽核日誌
⚠ 請確認您確實需要回滾版本
```

**操作流程：**
```
1. 選擇最新版本點擊[回滾]
2. 系統檢查權限:TESTITEM_REVISION_ROLLBACK
3. 開啟回滾確認對話框
4. 填寫回滾理由
5. 確認後呼叫 API: POST /api/testitems/revisions/{id}/rollback
6. 後端處理：
   ├─ 標記該版本的所有 WorkLog 為無效
   ├─ 將 TestItem 回到前一版本狀態
   ├─ 寫入 AuditLog
   └─ 重新計算 TestItem.Status
7. 刷新版本列表
```

---

### 4.11 【新增】測項列表畫面 (SCR-TESTITEM-LIST-001)

**權限要求：** Manager 或具備 `TESTITEM_VIEW` 權限

**進入方式：** 主選單 → [測項管理]

**畫面類型：** GridControl 列表模式

**畫面布局：**
```
┌─────────────────────────────────────────┐
│ [搜尋: ________] [案件: ▼全部] [🔍]    │
│ [狀態篩選: ▼全部] [負責人: ▼全部]      │
├─────────────────────────────────────────┤
│ Grid: 測項清單                          │
│ ┌───┬────────┬──────┬──────┬────┬─────┐│
│ │選│測項名稱│案件  │法規  │狀態│負責人││
│ ├───┼────────┼──────┼──────┼────┼─────┤│
│ │☐│Conducted│5G專案│FCC   │進行│王小明││
│ │☐│Radiated │5G專案│FCC   │延遲│李小華││
│ └───┴────────┴──────┴──────┴────┴─────┘│
│                                         │
│ [批次指派]  [匯出Excel]                  │
└─────────────────────────────────────────┘
```

**Grid 欄位規格：**
| 欄位名稱 | 資料來源 | 寬度 | 排序 | 說明 |
|---------|---------|------|------|------|
| 選取 | - | 40px | ✖ | CheckBox |
| 測項名稱 | TestItemName | 150px | ✓ | - |
| 案件名稱 | ProjectName | 150px | ✓ | - |
| 法規名稱 | RegulationName | 100px | ✓ | - |
| 狀態 | Status | 100px | ✓ | NotStarted/InProgress/Completed/Delayed/OnHold |
| 主要負責人 | MainEngineer | 100px | ✓ | RoleType=Main1 的工程師 |
| 預估工時 | EstimatedHours | 80px | ✓ | - |
| 實際工時 | ActualHours | 80px | ✓ | 累計工時 |
| 操作 | - | 150px | ✖ | [詳細][編輯][指派] |

---

## 5. Loading 報表模組 (Manager)

### 5.1 工程師Loading總覽畫面 (SCR-LOADING-001)

**權限要求：** Manager 或具備 `LOADING_VIEW_ALL` 權限

**檢視期間選項：**
- 本週
- 本月
- 下週
- 下月
- 自訂期間

**Loading顏色規則：**
```
IF Loading ≤ 60% THEN
    BackColor = LightGreen
ELSE IF Loading ≤ 80% THEN
    BackColor = Yellow
ELSE IF Loading ≤ 100% THEN
    BackColor = Orange
ELSE
    BackColor = Red
END IF
```

**[詳細]按鈕：**
- 跳轉至 SCR-LOADING-002 (該工程師的明細)
- 權限要求:`LOADING_VIEW_ALL`

---

### 5.2 工程師Loading明細畫面 (SCR-LOADING-002)

**權限要求：** 具備 `LOADING_VIEW_ALL` 權限

**開啟模式：** 獨立視窗

**每日工時趨勢圖：**
- X軸:日期
- Y軸:工時
- 顯示期間內每日實際工時
- 標示平均線

---

## 6. 延遲原因管理模組 (Manager)

### 6.1 延遲原因管理畫面 (SCR-DELAY-001)

**權限要求：** Manager 或具備 `DELAY_MANAGE` 權限

**操作規則：**
- **DelayReason 採用 IsActive 機制(不使用 Soft Delete)**
- 使用次數 > 0 的原因：
  - **不可刪除**(實體刪除會破壞 WorkLog 關聯)
  - 僅能透過 `IsActive = false` 停用
  - 停用後不顯示在工時回報畫面的下拉選單
  - 歷史資料仍可查詢與顯示
- 使用次數 = 0 的原因：
  - 可實體刪除
- 停用/啟用操作需寫入 AuditLog

**停用延遲原因流程：**
```
1. Manager 選擇欲停用的延遲原因
2. 系統檢查該原因是否被使用(COUNT WorkLogDelayReason)
3. IF 使用次數 > 0 THEN
     設定 IsActive = false
     寫入 AuditLog
     顯示「已停用,歷史記錄不受影響」
   ELSE
     詢問「此原因尚未被使用,是否直接刪除?」
     允許實體刪除或停用
4. 前端刷新清單
```

**查詢規則：**
- 工時回報下拉選單:僅顯示 `IsActive = true` 的原因
- 延遲原因管理畫面:顯示所有原因(含已停用)
- 工時記錄查詢:顯示完整原因文字(即使已停用)
- 延遲分析報表:包含所有原因(含已停用)

---

### 6.2 新增/編輯延遲原因對話框 (SCR-DELAY-002)

**權限要求：** 具備 `DELAY_MANAGE` 權限

**開啟模式：** Modal Dialog

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 原因文字空白 | "請輸入原因文字" |
| 原因文字重複 | "此原因已存在" |

---

## 7. 稽核日誌模組 (Manager)

### 7.1 稽核日誌查詢畫面 (SCR-AUDIT-001)

**權限要求：** Manager 或具備 `AUDIT_VIEW` 權限

**篩選條件：**

**資料表選項：**
- 全部
- Project
- Regulation
- TestItem
- TestItemEngineer
- TestItemRevision
- WorkLog
- User
- DelayReason
- Permission
- PermissionGroup
- UserPermission

**操作類型選項：**
- 全部
- Create (新增)
- Update (更新)
- Delete (刪除)
- PasswordReset (密碼重設)
- PermissionGrant (權限授予)
- PermissionRevoke (權限撤銷)
- StatusChange (狀態變更)
- RevisionCreated (補測版本建立)

**操作人：**
- 下拉選單顯示所有使用者

**[詳細]按鈕：**
- 跳轉至 SCR-AUDIT-002 (詳細比對)

---

### 7.2 稽核日誌詳細畫面 (SCR-AUDIT-002)

**權限要求：** 具備 `AUDIT_VIEW` 權限

**開啟模式：** Modal Dialog (大視窗)

**Before/After 比對顯示：**
```
┌────────────────────────────────────────────┐
│ 稽核日誌詳細資訊                            │
├────────────────────────────────────────────┤
│ 操作人:王小明 (eng01)                      │
│ 操作時間:2025-11-19 14:30:25               │
│ 操作類型:Update (更新)                     │
│ 資料表:TestItem                            │
│ 記錄ID:12345                               │
│ 修改理由:客戶要求調整測試項目內容            │
├────────────────────────────────────────────┤
│ 【變更內容】                                │
│                                            │
│ 欄位名稱      │ 變更前           │ 變更後    │
│ ──────────────┼──────────────────┼──────────   │
│ TestItemName │ Conducted Test  │ Conduct…  │
│ EstimatedHrs │ 8.0             │ 12.0      │
│ Status       │ NotStarted      │ InProgr…  │
│                                            │
│ 【完整JSON】                                │
│ Before:                                    │
│ {                                          │
│   "TestItemName": "Conducted Test",        │
│   "EstimatedHours": 8.0,                   │
│   "Status": "NotStarted"                   │
│ }                                          │
│                                            │
│ After:                                     │
│ {                                          │
│   "TestItemName": "Conducted Test V2",     │
│   "EstimatedHours": 12.0,                  │
│   "Status": "InProgress"                   │
│ }                                          │
│                                            │
│                              [關閉]         │
└────────────────────────────────────────────┘
```

---

## 8. 報表模組 (Manager)

### 8.1 案件進度報表畫面 (SCR-REPORT-001)

**權限要求：** Manager 或具備 `REPORT_VIEW_ALL` 權限

**報表內容：**
- 案件基本資訊
- 法規完成度(長條圖)
- 測項進度明細
- 工時統計(預估 vs 實際)
- 支援 Excel 匯出

---

### 8.2 工時統計報表畫面 (SCR-REPORT-002)

**權限要求：** 具備 `REPORT_VIEW_ALL` 權限

**統計維度：**
- 依案件統計
- 依工程師統計
- 依期間統計
- 提供圖表視覺化
- 支援 Excel 匯出

---

### 8.3 延遲分析報表畫面 (SCR-REPORT-003)

**權限要求：** 具備 `REPORT_VIEW_ALL` 權限

**報表內容：**
- 延遲原因分布圓餅圖
- 延遲測項清單
- 平均延遲天數統計
- 可依期間篩選
- 支援 Excel 匯出

---

## 9. 系統設定模組

### 9.1 系統參數設定畫面 (SCR-SETTING-001)

**權限要求：** Admin 或具備 `SYSTEM_SETTING` 權限

**設定項目：**

**一般參數：**
| 設定項目 | 說明 | 預設值 |
|---------|------|--------|
| 預設每週工時 | 新增使用者時的預設值 | 37.5 |
| WorkLog可修改天數 | 工程師可修改工時的天數限制 | 7 |
| 登入失敗鎖定次數 | 連續失敗幾次後鎖定 | 5 |
| 登入鎖定時間(分鐘) | 鎖定多久後可重試 | 10 |
| JWT Token 有效期限(小時) | Token 過期時間 | 8 |

**Email 設定：**
| 設定項目 | 說明 |
|---------|------|
| SMTP 伺服器 | Email 伺服器位址 |
| SMTP 連接埠 | 通常 25 或 587 |
| 寄件者 Email | 系統發信來源 |
| 寄件者名稱 | 顯示名稱 |
| 是否需要驗證 | SMTP 驗證 |
| 帳號/密碼 | SMTP 認證資訊 |

**資料保留政策：**
| 設定項目 | 說明 | 預設值 |
|---------|------|--------|
| AuditLog 保留天數 | 超過天數自動清理 | 365 |
| 已刪除資料保留天數 | Soft Delete 資料保留 | 90 |

**Windows AD 設定(選用)：**
| 設定項目 | 說明 |
|---------|------|
| 啟用 AD 登入 | 是否顯示「Windows 驗證登入」按鈕 |
| AD 網域 | 預設網域名稱 |
| 自動建立 AD 使用者 | 首次 AD 登入時自動建立 User |
| AD 使用者預設角色 | 自動建立時的預設角色 |
| AD 使用者預設群組 | 自動建立時的預設權限群組 |

---

## 10. 防呆處理總表

### 10.1 日期驗證

| 情境 | 驗證規則 | 錯誤訊息 |
|------|---------|---------|
| Project.StartDate | 不可早於今天 | "開始日期不可早於今天" |
| Project.EndDate | 需晚於StartDate | "結束日期需晚於開始日期" |
| Regulation日期 | 需在Project日期±30天內 | "法規日期需在案件日期前後30天內" |
| WorkLog.WorkDate | 不可晚於今天 | "不可回報未來日期" |
| UserPermission.ExpireDate | 不可早於今天 | "到期日不可早於今天" |

### 10.2 數值驗證

| 欄位 | 範圍 | 錯誤訊息 |
|------|------|---------|
| EstimatedHours | 0.5 - 999.9 | "預估工時需在0.5-999.9之間" |
| ActualHours | 0.5 - 12 | "單日工時需在0.5-12之間" |
| AssignedHours | 0.5 - 999.9 | "分配工時需在0.5-999.9之間" |
| WeeklyAvailableHours | 1 - 80 | "每週工時需在1-80之間" |

### 10.3 重複性驗證

| 欄位 | 範圍 | 錯誤訊息 |
|------|------|---------|
| User.Account | 全系統唯一 | "此帳號已存在" |
| User.Email | 全系統唯一(不區分大小寫) | "此Email已被使用" |
| User.ADAccount | 全系統唯一 | "此 AD 帳號已被註冊" |
| Project.ProjectName | 全系統唯一 | "此案件名稱已存在" |
| TestItem.TestItemName | 同Regulation下唯一 | "此測項名稱已存在於該法規" |
| WorkLog | 同工程師+同測項+同日期唯一 | "此日期已有工時記錄" |
| PermissionGroup.GroupName | 全系統唯一 | "此群組名稱已存在" |

### 10.4 刪除防呆

| 對象 | 規則 | 提示訊息 |
|------|------|---------|
| Project | 有未完成TestItem | "此案件尚有未完成測項,確定刪除?" |
| User | 有負責測項 | "此使用者負責{N}個未完成測項,停用後需重新分配" |
| DelayReason | 已被使用 | "此原因已被使用,無法刪除,僅能停用" |
| TestItem | 有工時記錄 | "此測項已有工時記錄,確定刪除?" |
| PermissionGroup | 有使用者屬於此群組 | "此群組尚有 {N} 位使用者,確定刪除?" |
| Permission | 已被群組或使用者使用 | "此權限已被使用,無法刪除,僅能停用" |

### 10.5 併發控制

| 情境 | 處理方式 | 提示訊息 |
|------|---------|---------|
| 同時編輯Project | 使用RowVersion檢查 | "此案件已被他人修改,請重新載入" |
| 同時編輯TestItem | 使用RowVersion檢查 | "此測項已被他人修改,請重新載入" |
| 同時回報WorkLog | 檢查日期唯一性 | "此日期已有工時記錄" |
| 同時修改權限 | 使用Transaction | "權限設定已被他人修改,請重新載入" |

### 10.6 權限驗證防呆

| 情境 | 驗證規則 | 錯誤訊息 |
|------|---------|---------|
| 無權限存取功能 | 檢查 JWT Token 內的權限清單 | "您沒有權限執行此操作" |
| Token 過期 | 驗證 exp 欄位 | "登入已過期,請重新登入" |
| Token 被竄改 | 驗證簽章 | "驗證失敗,請重新登入" |
| 權限已過期 | 檢查 UserPermission.ExpireDate | "您的臨時權限已過期" |
| 使用者已停用 | 檢查 User.IsActive | "帳號已停用,請洽繫主管" |

### 10.7 測項狀態變更防呆 【新增】

| 情境 | 驗證規則 | 錯誤訊息 |
|------|---------|---------|
| 工程師取消非自己的測項完成 | 檢查 TestItemEngineer | "您不是此測項的負責工程師" |
| 工程師取消已有補測版本的測項 | 檢查 TestItemRevision | "此測項已建立補測版本,無法取消完成" |
| 主管覆寫狀態但未填理由 | 檢查 Reason 欄位 | "請填寫修改理由" |
| 手動狀態被自動邏輯覆寫 | 檢查 ManualStatusOverride | 應被阻擋,寫入錯誤日誌 |
| 建立補測時測項非 Completed | 顯示警告 | "此測項尚未完成,確定要建立補測?" |

---

## 11. 操作流程圖

### 11.1 建案完整流程

```
開始
  ↓
[Manager登入]
  ↓
[點擊「新增案件」]
  ↓
[檢查權限:PROJECT_CREATE]
  ├─ 無權限 → 顯示「您沒有權限建立案件」→ 結束
  └─ 有權限 ↓
┌──────────────────┐
│ Wizard Step 1   │
│ 填寫案件基本資料 │
└──────────────────┘
  ↓
驗證通過?
  ├─ No → 顯示錯誤訊息,停留在Step 1
  └─ Yes
      ↓
┌──────────────────┐
│ Wizard Step 2   │
│ 選擇法規與日期   │
└──────────────────┘
  ↓
至少選1個法規?
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
┌──────────────────┐
│ Wizard Step 3   │
│ 新增測試項目     │
└──────────────────┘
  ↓
每個法規都有測項?
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
┌──────────────────┐
│ Wizard Step 4   │
│ 分配工程師       │
└──────────────────┘
  ↓
每個測項都有主要工程師?
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
[顯示確認摘要]
  ↓
確認建立?
  ├─ No → 返回Wizard修改
  └─ Yes
      ↓
[呼叫API:POST /api/projects/create-with-wizard]
  ↓
[Transaction處理]
  ├─ 建立Project (Status=Active)
  ├─ 建立Regulation (Status=NotStarted)
  ├─ 建立TestItem (Status=NotStarted)
  ├─ 建立TestItemEngineer
  └─ 寫入AuditLog
  ↓
成功?
  ├─ No → 顯示錯誤,回到Wizard
  └─ Yes
      ↓
[顯示「案件建立成功!」]
  ↓
[3秒後跳轉至案件總覽]
  ↓
結束
```

---

### 11.2 工時回報流程

```
開始
  ↓
[Engineer登入]
  ↓
[進入「我的測項」畫面]
  ↓
[檢查權限:WORKLOG_VIEW_OWN]
  ├─ 無權限 → 顯示「您沒有權限查看測項」→ 結束
  └─ 有權限 ↓
[選擇測項,點擊「回報工時」]
  ↓
[檢查權限:WORKLOG_CREATE]
  ├─ 無權限 → 顯示「您沒有權限回報工時」→ 結束
  └─ 有權限 ↓
┌──────────────────────┐
│ 回報工時對話框       │
│ - 選擇版本           │
│ - 選擇日期           │
│ - 輸入工時           │
│ - 選擇狀態           │
│ - (延遲時)選延遲原因  │
│ - 輸入備註           │
└──────────────────────┘
  ↓
前端驗證通過?
  ├─ No → 顯示錯誤訊息,停留在對話框
  └─ Yes
      ↓
[呼叫API:POST /api/worklogs]
  ↓
[後端驗證 JWT Token]
  ├─ Token 無效/過期 → 401 → 跳轉登入頁
  └─ Token 有效 ↓
[檢查權限:WORKLOG_CREATE]
  ├─ 無權限 → 403 → 顯示錯誤
  └─ 有權限 ↓
後端驗證
  ├─ 日期是否已回報?
  ├─ 工時是否合理?
  └─ 延遲狀態是否有原因?
  ↓
驗證通過?
  ├─ No → 回傳錯誤訊息
  └─ Yes
      ↓
[寫入WorkLog記錄]
  ↓
[寫入WorkLogDelayReason (若有)]
  ↓
[更新TestItem.Status]
  ├─ IF 任一WorkLog.Status=Delayed
  │   → TestItem.Status=Delayed
  ├─ ELSE IF 所有工程師最後WorkLog=Completed
  │   → TestItem.Status=Completed
  └─ ELSE → TestItem.Status=InProgress
  ↓
[更新Regulation.Status]
  ├─ IF 任一TestItem=Delayed
  │   → Regulation.Status=Delayed
  │   → Project.Status=Delayed
  ├─ ELSE IF 所有TestItem=Completed
  │   → Regulation.Status=Completed
  │   → Project.Status=Completed
  └─ ELSE → Project.Status=Active
  ↓
[寫入AuditLog]
  ↓
[回傳成功訊息]
  ↓
[關閉對話框,重整測項清單]
  ↓
結束
```

---

### 11.3 忘記密碼流程

```
開始
  ↓
[使用者在登入畫面點擊「忘記密碼?」]
  ↓
┌──────────────────────┐
│ 忘記密碼畫面         │
│ - 選擇使用帳號/Email │
│ - 輸入對應資料       │
└──────────────────────┘
  ↓
[按下「送出」]
  ↓
[呼叫API:POST /api/auth/forgot-password]
  ↓
後端處理
  ├─ 依帳號/Email查詢User
  ├─ IF 找到 且 AuthType=Local THEN
  │   ├─ 產生Token (GUID + Hash)
  │   ├─ 儲存至PasswordReset表
  │   │   - ExpireAt = 現在+30分鐘
  │   │   - UsedAt = NULL
  │   └─ 發送Email (含重設連結)
  ├─ ELSE IF 找到 且 AuthType=AD THEN
  │   └─ 不執行任何動作 (AD 帳號不支援系統重設)
  └─ ELSE
      └─ 不執行任何動作 (安全考量)
  ↓
[回傳前端:統一成功訊息]
  ↓
[顯示「如果資料正確,重設連結已寄出」]
  ↓
使用者收到Email?
  ├─ No (AD帳號或帳號不存在) → 無Email → 結束
  └─ Yes ↓
[點擊Email中的連結]
  ↓
[開啟瀏覽器,載入重設密碼頁面]
  ↓
[呼叫API:GET /api/auth/validate-token?token=xxx]
  ↓
Token有效?
  ├─ No (不存在/已過期/已使用)
  │   → 顯示錯誤頁面:「連結已失效,請重新申請」
  └─ Yes
      ↓
┌──────────────────────┐
│ 重設密碼畫面         │
│ - 輸入新密碼         │
│ - 確認新密碼         │
└──────────────────────┘
  ↓
前端驗證
  ├─ 密碼長度8-20?
  ├─ 包含英文與數字?
  └─ 兩次密碼一致?
  ↓
驗證通過?
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
[按下「確認送出」]
  ↓
[呼叫API:POST /api/auth/reset-password]
  ↓
後端處理
  ├─ 再次驗證Token
  ├─ Hash新密碼
  ├─ 更新User.PasswordHash
  ├─ 設定PasswordReset.UsedAt=現在
  └─ 寫入AuditLog
  ↓
成功?
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
[顯示「密碼重設成功,3秒後跳轉登入頁」]
  ↓
[自動跳轉至登入畫面]
  ↓
結束
```

---

### 11.4 權限授予流程 【新增】

```
開始
  ↓
[Admin 登入]
  ↓
[進入「使用者管理」畫面]
  ↓
[點擊某使用者的「權限」按鈕]
  ↓
[檢查權限:USER_MANAGE_PERMISSION]
  ├─ 無權限 → 顯示「您沒有權限管理使用者權限」→ 結束
  └─ 有權限 ↓
[開啟權限管理畫面 SCR-USER-PERM-001]
  ↓
[選擇 Tab 2: 個別權限]
  ↓
[點擊「新增個別權限」]
  ↓
┌──────────────────────┐
│ 新增個別權限對話框   │
│ - 選擇權限           │
│ - 選擇到期日         │
│ - 輸入授權理由       │
└──────────────────────┘
  ↓
驗證通過?
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
[呼叫API:POST /api/users/{userId}/permissions]
  ↓
[後端驗證 JWT Token & 權限]
  ├─ 無 USER_MANAGE_PERMISSION → 403
  └─ 有權限 ↓
[檢查該權限是否已存在]
  ├─ 已存在 → 400 → 顯示「此使用者已擁有此權限」
  └─ 不存在 ↓
[寫入 UserPermission]
  ├─ UserId
  ├─ PermissionId
  ├─ GrantedByUserId (當前操作者)
  ├─ GrantedDate
  ├─ ExpireDate (可為 NULL)
  └─ IsActive = true
  ↓
[寫入 AuditLog]
  ├─ Action = PermissionGrant
  ├─ Reason = 授權理由
  └─ NewValue = JSON(UserPermission)
  ↓
[回傳成功]
  ↓
[關閉對話框,重整權限清單]
  ↓
[Tab 3 自動重新計算有效權限總覽]
  ↓
結束
```

---

### 11.5 混合登入流程 【新增】

```
開始
  ↓
[使用者進入登入畫面]
  ↓
選擇登入方式?
  ├─ Local 帳號 ──────────────────────────
  │                                 │
  │  [輸入帳號與密碼]                │
  │   ↓                             │
  │  [按下「登入」]                   │
  │   ↓                             │
  │  [呼叫 API: POST /api/auth/login]
  │   ↓                             │
  │  後端驗證                        │
  │   ├─ 帳號不存在 → 401            │
  │   ├─ 密碼錯誤 → 401              │
  │   ├─ 帳號停用 → 403              │
  │   ├─ 失敗5次 → 429              │
  │   └─ 驗證通過 ↓                 │
  │                                │
  │  [產生 JWT Token]               │
  │   ├─ sub: UserId               │
  │   ├─ name: DisplayName         │
  │   ├─ role: Role                │
  │   ├─ authType: "Local"         │
  │   ├─ perms: [...]              │
  │   └─ exp: 8小時後               │
  │   ↓                            │
  │  [記錄 LastLoginDate/IP]       │
  │   ↓                            │
  │  [回傳 Token 至前端]            │
  │   ↓                            │
  │  [前端儲存 Token 於記憶體]       │
  │   ↓                            │
  │  [跳轉至主畫面]                 │
  │   ↓                            │
  └──→ 結束                        │
                                  │
  └─ Windows AD ──────────────────┘
                                  
     [點擊「Windows 驗證登入」]
      ↓
     [瀏覽器發送請求至 IIS]
      ↓
     [IIS 啟用 Windows Authentication]
      ↓
     [自動取得 Windows 登入資訊]
      ↓
     [呼叫 API: POST /api/auth/login-ad]
      ↓
     後端處理
      ├─ 解析 WindowsIdentity
      ├─ 取得 ADAccount (DOMAIN\Account)
      ├─ 查詢 User 表
      │  ├─ 若不存在 →
      │  │   [檢查系統設定:自動建立AD使用者]
      │  │    ├─ 是 →
      │  │    │   [建立 User]
      │  │    │    ├─ AuthType = AD
      │  │    │    ├─ ADAccount = DOMAIN\Account
      │  │    │    ├─ Role = 預設角色
      │  │    │    ├─ 加入預設權限群組
      │  │    │    └─ PasswordHash = NULL
      │  │    └─ 否 → 401 (AD帳號未註冊)
      │  └─ 若存在 ↓
      ├─ 檢查 User.IsActive
      │   ├─ false → 403 (帳號已停用)
      │   └─ true ↓
      └─ 驗證 AD 帳號狀態 (透過 AD API)
          ├─ AD 帳號已停用 → 403
          └─ AD 帳號有效 ↓
     
     [產生 JWT Token]
      ├─ sub: UserId
      ├─ name: DisplayName
      ├─ role: Role
      ├─ authType: "AD"
      ├─ perms: [...]
      └─ exp: 8小時後
      ↓
     [記錄 LastLoginDate/IP]
      ↓
     [回傳 Token 至前端]
      ↓
     [前端儲存 Token 於記憶體]
      ↓
     [跳轉至主畫面]
      ↓
     結束
```

---

### 11.6 測項狀態計算流程 【新增】

```
觸發事件
  ↓
[系統檢查 TestItem.ManualStatusOverride]
  ├─ true → 保持目前狀態,不執行自動計算 → 結束
  └─ false ↓

┌────────────────────────────────────────┐
│ 狀態計算優先順序(由高到低)              │
└────────────────────────────────────────┘
  ↓
[1. 檢查是否為補測事件]
  └─ IF 剛建立 TestItemRevision
      → TestItem.Status = InProgress
      → ManualStatusOverride = false
      → 寫入 AuditLog
      → 結束
  ↓
[2. 檢查 WorkLog 是否有 Delayed]
  └─ IF 任一 WorkLog.Status = Delayed
      → TestItem.Status = Delayed
      → 寫入 AuditLog
      → 結束
  ↓
[3. 檢查是否所有工程師都完成]
  └─ IF 所有負責工程師最後一筆 WorkLog.Status = Completed
      → TestItem.Status = Completed
      → 寫入 AuditLog
      → 結束
  ↓
[4. 檢查是否有 WorkLog]
  └─ IF 存在任何 WorkLog 記錄
      → TestItem.Status = InProgress
      → 寫入 AuditLog
      → 結束
  ↓
[5. 預設狀態]
  → TestItem.Status = NotStarted
  → 結束

┌────────────────────────────────────────┐
│ 連鎖更新                                 │
└────────────────────────────────────────┘
  ↓
[重新計算 Regulation.Status]
  ├─ IF Regulation.ManualStatusOverride = true → 保持目前狀態,不執行自動計算
  ├─ ELSE IF 任一 TestItem = Delayed → Regulation.Status = Delayed
  ├─ ELSE IF 所有 TestItem = Completed → Regulation.Status = Completed
  ├─ ELSE IF 任一 TestItem = InProgress → Regulation.Status = InProgress
  └─ ELSE → Regulation.Status = NotStarted
  ↓
  寫入 AuditLog(記錄 Regulation 狀態變更原因)
  ↓
[重新計算 Project.Status]
  ├─ IF 任一 Regulation = Delayed → Project.Status = Delayed
  ├─ ELSE IF 所有 Regulation = Completed → Project.Status = Completed
  ├─ ELSE IF 任一 Regulation = InProgress → Project.Status = Active
  └─ ELSE → Project.Status = Draft
  ↓
結束
```

---

## 12. API端點規範 (API Endpoints Specification)

### 12.1 認證模組 (Auth)

| 端點 | 方法 | 說明 | Request Body | Response | 權限 |
|------|------|------|--------------|----------|------|
| /api/auth/login | POST | Local帳號登入 | {account, password} | {token, user} | Public |
| /api/auth/login-ad | POST | Windows AD登入 | - (自動取得) | {token, user} | Public |
| /api/auth/forgot-password | POST | 申請重設密碼 | {account?, email?} | {success: true} | Public |
| /api/auth/validate-token | GET | 驗證重設Token | Query: token | {valid: true/false} | Public |
| /api/auth/reset-password | POST | 執行密碼重設 | {token, newPassword} | {success: true} | Public |
| /api/auth/refresh-token | POST | 刷新Token | {currentToken} | {newToken} | Authenticated |
| /api/auth/logout | POST | 登出(使Token失效) | - | {success: true} | Authenticated |

**JWT Token Response 格式：**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "userId": 123,
    "account": "eng01",
    "displayName": "王小明",
    "role": "Engineer",
    "authType": "Local",
    "email": "eng01@example.com",
    "expiresAt": "2025-11-19T22:30:00Z"
  }
}
```

---

### 12.2 使用者管理 (Users)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/users | GET | 取得使用者清單 | USER_VIEW |
| /api/users/{id} | GET | 取得使用者詳細 | USER_VIEW |
| /api/users | POST | 新增使用者 | USER_CREATE |
| /api/users/{id} | PUT | 更新使用者 | USER_UPDATE |
| /api/users/{id}/deactivate | POST | 停用使用者 | USER_UPDATE |
| /api/users/{id}/activate | POST | 啟用使用者 | USER_UPDATE |
| /api/users/{id}/reset-password | POST | 重設使用者密碼 | USER_RESET_PASSWORD |

---

### 12.3 權限管理 (Permissions) 【新增】

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/permissions | GET | 取得所有權限清單 | PERMISSION_VIEW |
| /api/permissions/{id} | GET | 取得權限詳細 | PERMISSION_VIEW |
| /api/permissions | POST | 新增權限 | PERMISSION_MANAGE |
| /api/permissions/{id} | PUT | 更新權限 | PERMISSION_MANAGE |
| /api/permissions/{id}/deactivate | POST | 停用權限 | PERMISSION_MANAGE |

---

### 12.4 權限群組管理 (Permission Groups) 【新增】

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/permissiongroups | GET | 取得權限群組清單 | PERMISSION_VIEW |
| /api/permissiongroups/{id} | GET | 取得群組詳細 | PERMISSION_VIEW |
| /api/permissiongroups | POST | 新增權限群組 | PERMISSION_MANAGE |
| /api/permissiongroups/{id} | PUT | 更新權限群組 | PERMISSION_MANAGE |
| /api/permissiongroups/{id}/permissions | GET | 取得群組的權限 | PERMISSION_VIEW |
| /api/permissiongroups/{id}/permissions | PUT | 設定群組權限 | PERMISSION_MANAGE |

**PUT /api/permissiongroups/{id}/permissions Request:**
```json
{
  "permissionIds": [1, 2, 5, 8, 12, 15]
}
```

---

### 12.5 使用者權限管理 (User Permissions) 【新增】

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/users/{id}/groups | GET | 取得使用者所屬群組 | USER_MANAGE_PERMISSION |
| /api/users/{id}/groups | POST | 將使用者加入群組 | USER_MANAGE_PERMISSION |
| /api/users/{id}/groups/{groupId} | DELETE | 將使用者移出群組 | USER_MANAGE_PERMISSION |
| /api/users/{id}/permissions | GET | 取得使用者個別權限 | USER_MANAGE_PERMISSION |
| /api/users/{id}/permissions | POST | 授予個別權限 | USER_MANAGE_PERMISSION |
| /api/users/{id}/permissions/{permissionId} | DELETE | 撤銷個別權限 | USER_MANAGE_PERMISSION |
| /api/users/{id}/effective-permissions | GET | 取得有效權限總覽 | USER_MANAGE_PERMISSION |

**POST /api/users/{id}/permissions Request:**
```json
{
  "permissionId": 123,
  "expireDate": "2025-12-31T23:59:59Z",
  "reason": "臨時需要查看所有工時以進行專案分析"
}
```

---

### 12.6 案件管理 (Projects)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/projects | GET | 取得案件清單 | PROJECT_VIEW |
| /api/projects/{id} | GET | 取得案件詳細 | PROJECT_VIEW |
| /api/projects/create-with-wizard | POST | Wizard建案 | PROJECT_CREATE |
| /api/projects/{id} | PUT | 更新案件 | PROJECT_UPDATE |
| /api/projects/{id} | DELETE | 刪除案件(Soft) | PROJECT_DELETE |
| /api/projects/{id}/regulations | GET | 取得案件的法規清單 | PROJECT_VIEW |
| /api/projects/{id}/testitems | GET | 取得案件的測項清單 | PROJECT_VIEW |

---

### 12.7 法規管理 (Regulations) 【新增】

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/regulations | GET | 取得法規清單 | PROJECT_VIEW |
| /api/regulations/{id} | GET | 取得法規詳細 | PROJECT_VIEW |
| /api/regulations | POST | 新增法規 | REGULATION_ADD |
| /api/regulations/{id} | PUT | 更新法規 | REGULATION_ADD |
| /api/regulations/{id}/disable | POST | 停用法規 | REGULATION_DISABLE |
| /api/regulations/{id} | DELETE | 移除法規(Soft) | REGULATION_REMOVE |

---

### 12.8 測試項目 (TestItems)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/testitems/{id} | GET | 取得測項詳細 | TESTITEM_VIEW |
| /api/testitems | POST | 新增測項 | TESTITEM_CREATE |
| /api/testitems/{id} | PUT | 更新測項 | TESTITEM_UPDATE |
| /api/testitems/{id} | DELETE | 刪除測項(Soft) | TESTITEM_DELETE |
| /api/testitems/{id}/engineers | GET | 取得測項工程師 | TESTITEM_VIEW |
| /api/testitems/{id}/engineers | POST | 分配工程師 | TESTITEM_ASSIGN_ENGINEER |
| /api/testitems/{id}/engineers/{engineerId} | DELETE | 移除工程師 | TESTITEM_REMOVE_ENGINEER |
| /api/testitems/{id}/revisions | GET | 取得補測版本清單 | TESTITEM_VIEW |
| /api/testitems/{id}/revisions | POST | 建立補測版本 | TESTITEM_REVISION_CREATE |
| /api/testitems/{id}/cancel-completion | POST | 工程師取消測項完成狀態 | TESTITEM_STATUS_CANCEL |
| /api/testitems/{id}/status | PUT | 主管手動覆寫測項狀態 | TESTITEM_STATUS_OVERRIDE |
| /api/testitems/revisions/{id} | DELETE | 刪除補測版本(Soft) | TESTITEM_UPDATE |
| /api/testitems/revisions/{id}/rollback | POST | 回滾補測版本 | TESTITEM_REVISION_ROLLBACK |

---

### 12.9 工時管理 (WorkLogs)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/worklogs/my-tasks | GET | 取得我的測項清單 | WORKLOG_VIEW_OWN |
| /api/worklogs | POST | 新增工時記錄 | WORKLOG_CREATE |
| /api/worklogs/{id} | PUT | 修改工時記錄 | WORKLOG_UPDATE_OWN |
| /api/worklogs/{id} | DELETE | 刪除工時記錄(Soft) | WORKLOG_DELETE |
| /api/worklogs/testitem/{id} | GET | 取得測項的工時記錄 | WORKLOG_VIEW_OWN 或 WORKLOG_VIEW_ALL |
| /api/worklogs/{id}/override | PUT | Manager覆寫工時 | WORKLOG_OVERRIDE |

**權限檢查邏輯：**
- `WORKLOG_VIEW_OWN`:只能查看自己的工時
- `WORKLOG_VIEW_ALL`:可查看所有工程師的工時
- `WORKLOG_UPDATE_OWN`:只能修改自己7天內的工時
- `WORKLOG_OVERRIDE`:可修改任何工時(含理由)
- `WORKLOG_DELETE`:可刪除工時記錄(Soft Delete,僅限Manager)

---

### 12.10 Loading 分析 (Loading)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/loading/overview | GET | 取得所有工程師Loading總覽 | LOADING_VIEW_ALL |
| /api/loading/engineer/{id} | GET | 取得特定工程師Loading明細 | LOADING_VIEW_ALL |
| /api/loading/my-loading | GET | 取得我的Loading | LOADING_VIEW_OWN |

---

### 12.11 延遲原因管理 (Delay Reasons)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/delayreasons | GET | 取得延遲原因清單 | DELAY_VIEW |
| /api/delayreasons/{id} | GET | 取得延遲原因詳細 | DELAY_VIEW |
| /api/delayreasons | POST | 新增延遲原因 | DELAY_MANAGE |
| /api/delayreasons/{id} | PUT | 更新延遲原因 | DELAY_MANAGE |
| /api/delayreasons/{id}/deactivate | POST | 停用延遲原因(IsActive=false) | DELAY_MANAGE |
| /api/delayreasons/{id}/activate | POST | 啟用延遲原因(IsActive=true) | DELAY_MANAGE |
| /api/delayreasons/{id} | DELETE | 刪除延遲原因(僅限未使用者) | DELAY_MANAGE |

**DELETE 端點補充說明：**
- 實體刪除僅適用於 `使用次數 = 0` 的延遲原因
- 若使用次數 > 0,後端回傳 409 Conflict
- 錯誤訊息:「此原因已被使用,無法刪除,請改用停用功能」

---

### 12.12 稽核日誌 (Audit Logs)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/auditlogs | GET | 查詢稽核日誌 | AUDIT_VIEW |
| /api/auditlogs/{id} | GET | 取得稽核日誌詳細 | AUDIT_VIEW |
| /api/auditlogs/export | GET | 匯出稽核日誌 | AUDIT_VIEW |

**GET /api/auditlogs Query Parameters:**
```
?tableName=User
&operationType=Update
&operatorId=123
&dateFrom=2025-11-01
&dateTo=2025-11-30
&pageSize=50
&pageNumber=1
```

---

### 12.13 報表 (Reports)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/reports/project-progress/{id} | GET | 案件進度報表 | REPORT_VIEW_ALL |
| /api/reports/worklog-statistics | GET | 工時統計報表 | REPORT_VIEW_ALL |
| /api/reports/delay-analysis | GET | 延遲分析報表 | REPORT_VIEW_ALL |
| /api/reports/export/excel | POST | 匯出Excel報表 | REPORT_VIEW_ALL |

---

### 12.14 系統設定 (System Settings)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/settings | GET | 取得系統設定 | SYSTEM_SETTING |
| /api/settings | PUT | 更新系統設定 | SYSTEM_SETTING |
| /api/settings/email/test | POST | 測試Email設定 | SYSTEM_SETTING |

---

## 13. 錯誤碼對照表

### 13.1 認證相關 (AUTH)

| 錯誤碼 | HTTP狀態 | 說明 | 前端處理 |
|--------|---------|------|---------|
| AUTH001 | 401 | 帳號或密碼錯誤 | 顯示訊息,清空密碼欄 |
| AUTH002 | 403 | 帳號已停用 | 顯示訊息,提示洽繫主管 |
| AUTH003 | 429 | 登入次數過多 | 顯示訊息,禁用登入按鈕 |
| AUTH004 | 401 | Token已過期 | 自動跳轉登入頁 |
| AUTH005 | 404 | 重設Token無效 | 顯示錯誤頁,提供重新申請連結 |
| AUTH006 | 410 | 重設Token已使用 | 顯示訊息,跳轉登入頁 |
| AUTH007 | 401 | Windows驗證失敗 | 顯示訊息,建議使用Local帳號或洽繫IT |
| AUTH008 | 403 | AD帳號未註冊 | 顯示訊息,提示洽繫主管註冊 |
| AUTH009 | 400 | Token格式錯誤 | 清除Token,跳轉登入頁 |
| AUTH010 | 401 | Token簽章無效 | 清除Token,跳轉登入頁 |

---

### 13.2 權限相關 (PERM) 【新增】

| 錯誤碼 | HTTP狀態 | 說明 | 前端處理 |
|--------|---------|------|---------|
| PERM001 | 403 | 無權限執行此操作 | 顯示訊息,隱藏或禁用該功能 |
| PERM002 | 400 | 權限已存在 | 顯示訊息 |
| PERM003 | 400 | 權限已過期 | 顯示訊息,提示重新申請 |
| PERM004 | 409 | 權限被使用中,無法刪除 | 提示只能停用 |
| PERM005 | 400 | 無法撤銷群組繼承的權限 | 說明需從群組移除使用者 |

---

### 13.3 驗證相關 (VAL)

| 錯誤碼 | HTTP狀態 | 說明 | 前端處理 |
|--------|---------|------|---------|
| VAL001 | 400 | 必填欄位未填寫 | 標記錯誤欄位 |
| VAL002 | 400 | 資料格式錯誤 | 顯示格式說明 |
| VAL003 | 400 | 數值超出範圍 | 顯示允許範圍 |
| VAL004 | 409 | 資料重複 | 提示修改 |
| VAL005 | 400 | 日期邏輯錯誤 | 顯示正確規則 |

---

### 13.4 業務邏輯 (BIZ)

| 錯誤碼 | HTTP狀態 | 說明 | 前端處理 |
|--------|---------|------|---------|
| BIZ001 | 409 | 測項已有工時記錄,無法刪除 | 顯示警告,提供查看工時連結 |
| BIZ002 | 409 | 工程師已超載 | 顯示警告,允許繼續或取消 |
| BIZ003 | 409 | 該日期已有工時記錄 | 提示修改現有記錄 |
| BIZ004 | 403 | 工時記錄超過7天無法修改 | 顯示訊息,提示洽繫主管 |
| BIZ005 | 409 | 延遲原因已被使用,無法刪除 | 提示只能停用 |
| BIZ006 | 409 | 資料已被他人修改 | 提供重新載入按鈕 |
| BIZ007 | 400 | AD帳號不支援密碼重設 | 提示洽繫IT部門 |
| BIZ008 | 403 | 非負責工程師無法取消完成 | 顯示錯誤,隱藏取消按鈕 |
| BIZ009 | 409 | 測項已有補測版本無法取消完成 | 顯示警告,說明原因 |
| BIZ010 | 409 | 測項被設為手動狀態,自動邏輯被阻擋 | 記錄日誌,通知主管 |
| BIZ011 | 409 | 補測版本已有工時記錄,無法刪除 | 顯示警告,說明影響範圍 |
| BIZ012 | 409 | 延遲原因已被使用,無法刪除 | 提示僅能停用,提供停用按鈕 |
| BIZ013 | 409 | 權限群組已被使用,無法刪除 | 提示僅能停用 |
| BIZ014 | 403 | 系統預設群組不可停用 | 顯示錯誤,隱藏停用按鈕 |

---

### 13.5 系統錯誤 (SYS)

| 錯誤碼 | HTTP狀態 | 說明 | 前端處理 |
|--------|---------|------|---------|
| SYS001 | 500 | 資料庫連線失敗 | 顯示錯誤,建議稍後再試 |
| SYS002 | 500 | Transaction失敗 | 顯示錯誤,資料已回滾 |
| SYS003 | 503 | 服務暫時無法使用 | 顯示維護訊息 |
| SYS004 | 500 | Email發送失敗 | 記錄日誌,顯示警告 |
| SYS005 | 500 | AD連線失敗 | 顯示錯誤,建議使用Local帳號 |

---

## 14. 畫面關聯圖

```
登入畫面 (SCR-LOGIN-001)
  ├─[Local帳號登入] → 驗證成功 → 主選單
  ├─[Windows驗證登入] → AD驗證 → 主選單
  │
  ├─[忘記密碼] → 忘記密碼畫面 (SCR-FORGOT-001)
  │                └─[Email連結] → 重設密碼畫面 (SCR-RESET-001)
  │
  ├─[Engineer登入成功] → 我的測項清單 (SCR-WORKLOG-001)
  │                       ├─[回報工時] → 回報工時對話框 (SCR-WORKLOG-002)
  │                       ├─[查看記錄] → 工時記錄查詢 (SCR-WORKLOG-003)
  │                       └─[取消完成] → 取消完成對話框 (SCR-WORKLOG-004)
  │
  └─[Manager登入成功] → 主選單
                          ├─[案件管理]
                          │   ├─ 案件總覽 (SCR-PROJECT-001)
                          │   │   ├─[新增] → 建案精靈 (SCR-WIZARD-001~006)
                          │   │   ├─[詳細] → 案件詳細 (SCR-PROJECT-002)
                          │   │   │   ├─ Tab1: 法規與測項 (TreeView)
                          │   │   │   │   ├─[右鍵:編輯測項] → SCR-TESTITEM-001
                          │   │   │   │   ├─[右鍵:修改狀態] → SCR-TESTITEM-STATUS-001
                          │   │   │   │   ├─[右鍵:指派工程師] → SCR-ENGINEER-ASSIGN-001
                          │   │   │   │   ├─[右鍵:建立補測] → SCR-TESTITEM-REVISION-001
                          │   │   │   │   └─[右鍵:查看補測版本] → SCR-REVISION-LIST-001
                          │   │   │   │       └─[回滾] → SCR-REVISION-ROLLBACK-001
                          │   │   │   ├─ Tab2: 工時統計
                          │   │   │   └─ Tab3: 延遲分析
                          │   │   └─[編輯] → 編輯案件 (SCR-PROJECT-003)
                          │   │
                          │   ├─ 法規管理 (SCR-REGULATION-001)
                          │   │   └─[新增/編輯] → SCR-REGULATION-002
                          │   │
                          │   └─ 測項列表 (SCR-TESTITEM-LIST-001)
                          │
                          ├─[工程師管理]
                          │   └─ 使用者管理 (SCR-USER-001)
                          │       ├─[新增/編輯] → 使用者對話框 (SCR-USER-002)
                          │       └─[權限] → 權限管理畫面 (SCR-USER-PERM-001)
                          │           └─[新增個別權限] → 對話框 (SCR-USER-PERM-002)
                          │
                          ├─[權限管理]
                          │   └─ 權限群組管理 (SCR-PERMGROUP-001)
                          │       ├─[編輯] → 對話框 (SCR-PERMGROUP-002)
                          │       └─[權限設定] → 權限設定畫面 (SCR-PERMGROUP-003)
                          │
                          ├─[Loading報表]
                          │   └─ Loading總覽 (SCR-LOADING-001)
                          │       └─[詳細] → Loading明細 (SCR-LOADING-002)
                          │
                          ├─[延遲管理]
                          │   └─ 延遲原因管理 (SCR-DELAY-001)
                          │       └─[新增/編輯] → 延遲原因對話框 (SCR-DELAY-002)
                          │
                          ├─[稽核日誌]
                          │   └─ 稽核日誌查詢 (SCR-AUDIT-001)
                          │       └─[詳細] → 稽核日誌詳細 (SCR-AUDIT-002)
                          │
                          ├─[報表中心]
                          │   ├─ 案件進度報表 (SCR-REPORT-001)
                          │   ├─ 工時統計報表 (SCR-REPORT-002)
                          │   └─ 延遲分析報表 (SCR-REPORT-003)
                          │
                          └─[系統設定]
                              └─ 系統參數設定 (SCR-SETTING-001)
```

---

## 15. 權限代碼清單 (Permission Code List) 【更新】

### 15.1 案件管理 (Project Management)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| PROJECT_VIEW | 查看案件 | 可查看案件清單與詳細資訊 |
| PROJECT_CREATE | 建立案件 | 可使用Wizard建立新案件 |
| PROJECT_UPDATE | 編輯案件 | 可修改案件基本資料 |
| PROJECT_DELETE | 刪除案件 | 可刪除案件(Soft Delete) |

---

### 15.2 法規管理 (Regulation Management) 【新增】

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| REGULATION_ADD | 新增法規 | 可新增與編輯法規 |
| REGULATION_DISABLE | 停用法規 | 可停用法規(IsActive=false) |
| REGULATION_REMOVE | 移除法規 | 可移除法規(Soft Delete) |

---

### 15.3 測試項目 (Test Items)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| TESTITEM_VIEW | 查看測項 | 可查看測試項目資訊 |
| TESTITEM_CREATE | 建立測項 | 可新增測試項目 |
| TESTITEM_UPDATE | 編輯測項 | 可修改測項、分配工程師、建立補測版本 |
| TESTITEM_DELETE | 刪除測項 | 可刪除測項(Soft Delete) |
| TESTITEM_STATUS_CANCEL | 取消測項完成 | 可取消自己負責測項的完成狀態 |
| TESTITEM_STATUS_OVERRIDE | 覆寫測項狀態 | 可手動修改任何測項的狀態 |
| TESTITEM_ASSIGN_ENGINEER | 指派主要工程師 | 可指派 Main1/Main2/Main3 工程師 |
| TESTITEM_ASSIGN_SUPPORT | 指派支援工程師 | 可指派 Support 工程師 |
| TESTITEM_REMOVE_ENGINEER | 移除工程師 | 可移除測項的工程師指派 |
| TESTITEM_REVISION_CREATE | 建立補測版本 | 可建立測項補測版本 |
| TESTITEM_REVISION_ROLLBACK | 回滾補測版本 | 可回滾最新補測版本 |

---

### 15.4 工時管理 (Work Logs)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| WORKLOG_VIEW_OWN | 查看自己工時 | 只能查看自己的工時記錄 |
| WORKLOG_VIEW_ALL | 查看所有工時 | 可查看所有工程師的工時記錄 |
| WORKLOG_CREATE | 回報工時 | 可新增工時記錄 |
| WORKLOG_UPDATE_OWN | 修改工時(7天內) | 可修改自己7天內的工時 |
| WORKLOG_OVERRIDE | 覆寫工時 | 可修改任何工時記錄(含理由) |
| WORKLOG_DELETE | 刪除工時記錄 | 可刪除工時記錄(Soft Delete,僅限Manager) |

---

### 15.5 Loading 分析 (Loading Analysis)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| LOADING_VIEW_OWN | 查看自己Loading | 只能查看自己的Loading |
| LOADING_VIEW_ALL | 查看所有Loading | 可查看所有工程師的Loading |

---

### 15.6 延遲管理 (Delay Management)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| DELAY_VIEW | 查看延遲資訊 | 可查看延遲原因與分析 |
| DELAY_MANAGE | 管理延遲原因 | 可新增/修改/刪除延遲原因 |

---

### 15.7 使用者管理 (User Management)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| USER_VIEW | 查看使用者 | 可查看使用者清單與資訊 |
| USER_CREATE | 建立使用者 | 可新增使用者 |
| USER_UPDATE | 編輯使用者 | 可修改使用者資料、停用/啟用 |
| USER_RESET_PASSWORD | 重設密碼 | 可重設使用者密碼 |
| USER_MANAGE_PERMISSION | 管理使用者權限 | 可管理使用者的群組與個別權限 |

---

### 15.8 權限管理 (Permission Management)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| PERMISSION_VIEW | 查看權限 | 可查看權限與權限群組 |
| PERMISSION_MANAGE | 管理權限 | 可管理權限與權限群組定義 |

---

### 15.9 稽核日誌 (Audit Logs)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| AUDIT_VIEW | 查看稽核日誌 | 可查詢與檢視稽核日誌 |

---

### 15.10 報表 (Reports)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| REPORT_VIEW_ALL | 查看所有報表 | 可查看與匯出所有報表 |

---

### 15.11 系統設定 (System Settings)

| PermissionCode | PermissionName | 說明 |
|---------------|----------------|------|
| SYSTEM_SETTING | 系統設定 | 可修改系統參數與設定 |

---

## 16. 預設權限群組配置 【更新】

### 16.1 Engineer (工程師)

**適用對象：** 一般測試工程師

**包含權限：**
- PROJECT_VIEW
- TESTITEM_VIEW
- TESTITEM_STATUS_CANCEL
- WORKLOG_VIEW_OWN
- WORKLOG_CREATE
- WORKLOG_UPDATE_OWN
- LOADING_VIEW_OWN

**說明：** 工程師可查看與自己相關的案件與測項,回報與修改自己的工時,查看自己的Loading狀況。

---

### 16.2 Manager (主管)

**適用對象：** 實驗室主管、專案管理者

**包含權限：**
- (繼承 Engineer 所有權限)
- PROJECT_CREATE
- PROJECT_UPDATE
- PROJECT_DELETE
- REGULATION_ADD
- REGULATION_DISABLE
- REGULATION_REMOVE
- TESTITEM_CREATE
- TESTITEM_UPDATE
- TESTITEM_DELETE
- TESTITEM_STATUS_OVERRIDE
- TESTITEM_ASSIGN_ENGINEER
- TESTITEM_ASSIGN_SUPPORT
- TESTITEM_REMOVE_ENGINEER
- TESTITEM_REVISION_CREATE
- TESTITEM_REVISION_ROLLBACK
- WORKLOG_VIEW_ALL
- WORKLOG_OVERRIDE
- WORKLOG_DELETE
- LOADING_VIEW_ALL
- DELAY_VIEW
- DELAY_MANAGE
- USER_VIEW
- USER_CREATE
- USER_UPDATE
- USER_RESET_PASSWORD
- AUDIT_VIEW
- REPORT_VIEW_ALL

**說明：** 主管擁有完整的案件管理、工時查看、Loading分析、延遲管理、人員管理與報表查看權限。

---

### 16.3 Admin (系統管理者)

**適用對象：** IT部門、系統維護人員

**包含權限：**
- (繼承 Manager 所有權限)
- USER_MANAGE_PERMISSION
- PERMISSION_VIEW
- PERMISSION_MANAGE
- SYSTEM_SETTING

**說明：** 系統管理者擁有最高權限,可管理使用者權限、定義權限群組、修改系統設定。

---

### 16.4 Auditor (稽核人員) 【選用】

**適用對象：** 內部稽核、品質管理人員

**包含權限：**
- PROJECT_VIEW
- TESTITEM_VIEW
- WORKLOG_VIEW_ALL
- LOADING_VIEW_ALL
- AUDIT_VIEW
- REPORT_VIEW_ALL

**說明：** 稽核人員可查看所有資料與報表,但不能修改,適用於需要監督但不參與日常管理的角色。

---

## 17. TestItemRevision 完整定義 【新增】

### 17.1 TestItemRevision 資料表欄位說明

| 欄位名稱 | 資料類型 | 說明 | 必填 | 備註 |
|---------|---------|------|------|------|
| RevisionId | INT | 版本ID | ✔ | Primary Key, Identity |
| TestItemId | INT | 測項ID | ✔ | Foreign Key to TestItem |
| RevisionNumber | INT | 版本編號 | ✔ | v1, v2, v3... |
| RevisionType | NVARCHAR(20) | 補測類型 | ✔ | Command/Retest/Fix/Others |
| EstimatedHours | DECIMAL(10,2) | 預估工時 | ✔ | 主管預估補測工時 |
| Reason | NVARCHAR(200) | 補測原因 | ✔ | Command內容或補測原因 |
| Description | NVARCHAR(500) | 詳細說明 | - | 補充說明 |
| CreatedByUserId | INT | 建立者 | ✔ | 記錄誰建立此版本 |
| CreatedDate | DATETIME | 建立時間 | ✔ | 預設 GETDATE() |
| ModifiedByUserId | INT | 修改者 | - | 最後修改者 |
| ModifiedDate | DATETIME | 修改時間 | - | 最後修改時間 |
| IsDeleted | BIT | 軟刪除標記 | ✔ | 預設 0 |
| DeletedByUserId | INT | 刪除者 | - | 誰刪除此版本 |
| DeletedDate | DATETIME | 刪除時間 | - | 刪除時間 |

**Unique Constraint:**
```sql
CONSTRAINT [UQ_TestItemRevision] UNIQUE ([TestItemId], [RevisionNumber]) 
WHERE [IsDeleted] = 0
```

---

### 17.2 RevisionType 類型說明

| 類型 | 說明 | 使用時機 |
|------|------|---------|
| Command | 客戶要求 | 客戶要求重新測試或測試新版本 |
| Retest | 重新測試 | 測試不通過需重新測試 |
| Fix | 修正後測試 | 產品修正後需再次測試 |
| Others | 其他 | 其他原因,需填寫詳細說明 |

---

### 17.3 TestItemRevision UI 流程

**建立補測版本流程：**
```
1. Manager 在測項詳細頁面點擊[建立補測版本]
2. 系統檢查權限: TESTITEM_REVISION_CREATE
3. 開啟補測版本對話框 (SCR-TESTITEM-REVISION-001)
4. 填寫版本資訊:
   - 系統自動產生 RevisionNumber (當前最大值+1)
   - 選擇 RevisionType
   - 輸入預估工時
   - 填寫補測原因
   - 填寫詳細說明(選填)
   - 選擇是否複製原工程師分配
5. 確認後呼叫 API: POST /api/testitems/{id}/revisions
6. 後端處理:
   ├─ 建立 TestItemRevision 記錄
   ├─ 自動設定 TestItem.Status = InProgress
   ├─ 清除 TestItem.ManualStatusOverride
   ├─ 若勾選複製工程師,建立 TestItemEngineer 記錄
   └─ 寫入 AuditLog
7. 返回測項詳細頁面,顯示新版本
```

---

### 17.4 補測版本建立對話框 (SCR-TESTITEM-REVISION-001) 【新增】

**權限要求：** Manager 或具備 `TESTITEM_REVISION_CREATE` 權限

**開啟模式：** Modal Dialog

**畫面元素：**
| 元素類型 | 標籤 | 控制項名稱 | 必填 | 說明 |
|---------|------|-----------|------|------|
| Label | 測項名稱 | - | - | 顯示測項資訊 |
| Label | 版本編號 | - | - | 系統自動產生,如 v2 |
| Label | 補測類型 | - | - | - |
| ComboBox | - | cboRevisionType | ✔ | Command/Retest/Fix/Others |
| Label | 預估工時 | - | - | - |
| NumericUpDown | - | nudEstimatedHours | ✔ | 0.5-999.9 |
| Label | 補測原因 | - | - | - |
| TextBox | - | txtReason | ✔ | 最大200字 |
| Label | 詳細說明 | - | - | - |
| TextBox (多行) | - | txtDescription | - | 最大500字 |
| CheckBox | 複製原工程師分配 | chkCopyEngineers | - | 勾選後自動複製工程師與工時 |
| Button | 確定 | btnOK | - | - |
| Button | 取消 | btnCancel | - | - |

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 補測類型未選擇 | "請選擇補測類型" |
| 預估工時≤0 | "預估工時需大於0" |
| 補測原因空白 | "請填寫補測原因" |

---

## 18. UI Flow 與操作流程補充 【新增】

### 18.1 工程師指派完整 UI Flow

```
主管開啟測項詳細頁面
  ↓
[右鍵測項 → 指派工程師]
  ↓
檢查權限: TESTITEM_ASSIGN_ENGINEER
  ↓
開啟工程師指派畫面 (SCR-ENGINEER-ASSIGN-001)
  ↓
顯示當前已指派工程師清單
  ├─ 工程師姓名
  ├─ 角色類型 (Main1/Main2/Main3/Support)
  ├─ 分配工時
  └─ Loading %
  ↓
[點擊新增工程師]
  ↓
開啟工程師分配對話框 (SCR-WIZARD-007)
  ├─ 選擇工程師
  ├─ 選擇角色類型
  ├─ 輸入分配工時
  └─ 顯示新增後 Loading
  ↓
確認後新增至清單
  ↓
檢查驗證規則:
  ├─ 至少1位 Main1
  ├─ 分配工時是否合理
  └─ Loading 是否超載
  ↓
[點擊確定]
  ↓
呼叫 API: POST /api/testitems/{id}/engineers
  ↓
後端批次建立 TestItemEngineer 記錄
  ↓
重新計算所有相關工程師的 Loading
  ↓
寫入 AuditLog
  ↓
返回測項詳細頁面
```

---

### 18.2 補測版本完整 UI Flow

```
主管查看已完成測項
  ↓
[右鍵測項 → 建立補測版本]
  ↓
檢查權限: TESTITEM_REVISION_CREATE
  ↓
開啟補測版本對話框 (SCR-TESTITEM-REVISION-001)
  ├─ 顯示測項名稱
  ├─ 自動產生版本編號 (v2)
  ├─ 選擇補測類型
  ├─ 輸入預估工時
  ├─ 填寫補測原因
  ├─ 填寫詳細說明(選填)
  └─ 選擇是否複製原工程師分配
  ↓
[點擊確定]
  ↓
前端驗證
  ↓
呼叫 API: POST /api/testitems/{id}/revisions
  ↓
後端處理:
  ├─ 建立 TestItemRevision 記錄
  ├─ 自動設定 TestItem.Status = InProgress
  ├─ 清除 TestItem.ManualStatusOverride = false
  ├─ IF 勾選複製工程師 THEN
  │   └─ 複製原有 TestItemEngineer 記錄
  ├─ 重新計算 Regulation.Status
  ├─ 重新計算 Project.Status
  └─ 寫入 AuditLog
  ↓
返回測項詳細頁面
  ↓
顯示補測版本清單
  ├─ 原始版本 (v1)
  └─ 新建版本 (v2) ← 當前版本
  ↓
工程師回報工時時選擇版本 v2
```

---

### 18.3 法規管理完整 UI Flow

```
主管開啟案件詳細頁面
  ↓
[點擊法規管理 Tab 或按鈕]
  ↓
檢查權限: REGULATION_ADD
  ↓
開啟法規管理畫面 (SCR-REGULATION-001)
  ↓
顯示當前案件的法規清單
  ├─ 法規名稱
  ├─ 狀態
  ├─ 開始/結束日期
  └─ 測項數量
  ↓
[點擊新增法規]
  ↓
開啟新增法規對話框 (SCR-REGULATION-002)
  ├─ 輸入法規名稱
  ├─ 選擇開始日期
  ├─ 選擇結束日期
  ├─ 填寫說明
  └─ 設定是否啟用
  ↓
[點擊確定]
  ↓
前端驗證
  ↓
呼叫 API: POST /api/regulations
  ↓
後端處理:
  ├─ 建立 Regulation 記錄
  ├─ Status = NotStarted
  └─ 寫入 AuditLog
  ↓
返回法規管理畫面
  ↓
顯示新增的法規
  ↓
[後續可新增測項]
```

---

### 18.4 權限授予完整 UI Flow

```
Admin 開啟使用者管理畫面
  ↓
[點擊某使用者的[權限]按鈕]
  ↓
檢查權限: USER_MANAGE_PERMISSION
  ↓
開啟權限管理畫面 (SCR-USER-PERM-001)
  ↓
顯示 3 個 Tab:
  ├─ Tab1: 權限群組
  │   ├─ 顯示所有可用群組
  │   ├─ 當前所屬群組打勾
  │   └─ 可勾選/取消勾選
  │
  ├─ Tab2: 個別權限
  │   ├─ 顯示額外授予的權限清單
  │   ├─ 顯示授權人、授權日期、到期日
  │   └─ 可新增/撤銷
  │
  └─ Tab3: 有效權限總覽
      └─ TreeView 顯示所有有效權限
          └─ 標註來源(群組/個別)
  ↓
[切換至 Tab2]
  ↓
[點擊新增個別權限]
  ↓
開啟新增個別權限對話框 (SCR-USER-PERM-002)
  ├─ 選擇權限(下拉選單分類顯示)
  ├─ 自動顯示權限說明
  ├─ 選擇到期日(永久/指定日期)
  └─ 填寫授權理由
  ↓
[點擊確定]
  ↓
前端驗證
  ↓
呼叫 API: POST /api/users/{id}/permissions
  ↓
後端處理:
  ├─ 檢查權限是否已存在
  ├─ 建立 UserPermission 記錄
  │   ├─ GrantedByUserId = 當前操作者
  │   ├─ GrantedDate = 現在
  │   └─ ExpireDate = 使用者選擇
  └─ 寫入 AuditLog
  ↓
關閉對話框
  ↓
重整 Tab2 清單
  ↓
自動重新計算 Tab3 有效權限總覽
  ↓
顯示新授予的權限
```

---

## 19. TestItemEngineer RoleType 詳細定義 【新增】

### 19.1 RoleType 完整說明

| RoleType | 中文名稱 | 權重 | 職責說明 | Loading計算 |
|----------|---------|------|---------|-----------|
| Main1 | 主要負責工程師 | 100% | 測項主要負責人,負責協調、執行、最終確認 | 完整計入Loading |
| Main2 | 次要負責工程師 | 80% | 協助主要工程師,可獨立執行部分測試 | 80%計入Loading |
| Main3 | 第三負責工程師 | 60% | 輔助角色,學習與支援性質 | 60%計入Loading |
| Support | 支援工程師 | 40% | 提供技術支援,不直接負責測試執行 | 40%計入Loading |

### 19.2 RoleType 使用規則

**必要規則：**
- 每個測項至少需要 1 位 Main1
- Main1 可以有多位(例如複雜測項需要多位主要負責人)
- 沒有 Main1 的測項無法開始測試

**選配規則：**
- Main2/Main3 為選配,視測項複雜度決定
- Support 角色不計入主要工程師數量
- Support 主要用於提供跨測項技術支援

**權限影響：**
- 只有 Main1 可以標記測項為「已完成」
- Main2/Main3 可以回報工時,但無法完成測項
- Support 可以回報工時,但工時會以較低權重計入

**Loading 計算範例：**
```
測項 A: 預估 10 小時
  ├─ 工程師甲 (Main1): 分配 6 小時 → Loading 計入 6 小時
  ├─ 工程師乙 (Main2): 分配 3 小時 → Loading 計入 2.4 小時 (3 × 80%)
  └─ 工程師丙 (Support): 分配 2 小時 → Loading 計入 0.8 小時 (2 × 40%)
```

### 19.3 RoleType 變更規則

**變更限制：**
- 測項開始後不建議變更 RoleType
- 若需變更,需主管權限 (TESTITEM_UPDATE)
- 變更會影響 Loading 計算,需重新計算所有相關工程師的 Loading
- 變更需寫入 AuditLog

**變更流程：**
```
1. Manager 在工程師指派畫面編輯某工程師
2. 修改 RoleType
3. 系統顯示警告:「變更角色類型將影響 Loading 計算」
4. 確認後呼叫 API: PUT /api/testitems/{id}/engineers/{engineerId}
5. 後端重新計算 Loading
6. 寫入 AuditLog
7. 通知相關工程師
```

---

## 20. 補充說明

### 20.1 Soft Delete 統一規範

本系統採用 Soft Delete 機制的資料表：
- Project
- Regulation
- TestItem
- TestItemRevision
- TestItemEngineer
- WorkLog
- User

**Soft Delete 欄位：**
- `IsDeleted` (BIT): 刪除標記,預設 0
- `DeletedByUserId` (INT): 刪除者 UserId
- `DeletedDate` (DATETIME): 刪除時間

**查詢規則：**
- 一般查詢: WHERE IsDeleted = 0
- 稽核查詢: 可查看所有資料(含已刪除)
- 已刪除資料保留天數: 90 天(可於系統設定調整)

### 20.2 Email 大小寫處理統一規範

**處理原則：**
- 所有 Email 比對一律 **不區分大小寫**
- 儲存時一律 **轉為小寫**
- 顯示時使用 **儲存的小寫值**

**實作位置：**
- 前端: 顯示原始輸入,但送出前不做轉換
- 後端: 接收後立即轉小寫,比對與儲存皆用小寫
- 資料庫: 儲存小寫值

**影響範圍：**
- User.Email
- 登入驗證
- 忘記密碼
- Email 唯一性檢查
- AD 自動建立使用者時的 Email 比對

### 20.3 案件總覽 GridControl 規範

**改為列表模式原因：**
- 提升資料瀏覽效率
- 支援複雜排序與篩選
- 便於批次操作
- 更符合 Manager 使用習慣

**Grid 必要功能：**
- 多欄位排序
- 進階篩選
- 欄位顯示/隱藏設定
- 欄寬調整與記憶
- 匯出 Excel
- 分頁顯示

### 20.4 權限命名統一

為確保 Spec 與 SD 一致,統一權限命名如下：

| Spec 名稱 | SD 名稱 | 統一使用 |
|----------|---------|---------|
| AUDIT_VIEW | AUDITLOG_VIEW | **AUDIT_VIEW** |
| SYSTEM_SETTING | SYSTEM_SETTING_MANAGE | **SYSTEM_SETTING** |

所有文件應使用統一名稱。

---

## 21. 附錄

### 21.1 UI 畫面索引

| 畫面代碼 | 畫面名稱 | 權限要求 |
|---------|---------|---------|
| SCR-LOGIN-001 | 登入畫面 | Public |
| SCR-FORGOT-001 | 忘記密碼畫面 | Public |
| SCR-RESET-001 | 重設密碼畫面 | Public |
| SCR-USER-001 | 使用者管理畫面 | USER_VIEW |
| SCR-USER-002 | 新增/編輯使用者對話框 | USER_CREATE/UPDATE |
| SCR-USER-PERM-001 | 使用者權限管理畫面 | USER_MANAGE_PERMISSION |
| SCR-USER-PERM-002 | 新增個別權限對話框 | USER_MANAGE_PERMISSION |
| SCR-PERMGROUP-001 | 權限群組管理畫面 | PERMISSION_MANAGE |
| SCR-PERMGROUP-002 | 編輯權限群組對話框 | PERMISSION_MANAGE |
| SCR-PERMGROUP-003 | 權限群組權限設定畫面 | PERMISSION_MANAGE |
| SCR-WIZARD-001 | 建案精靈主畫面 | PROJECT_CREATE |
| SCR-WIZARD-002 | Step 1 - 案件基本資料 | PROJECT_CREATE |
| SCR-WIZARD-003 | Step 2 - 選擇法規 | PROJECT_CREATE |
| SCR-WIZARD-004 | Step 3 - 新增測項 | PROJECT_CREATE |
| SCR-WIZARD-005 | Step 3-1 - 測項對話框 | PROJECT_CREATE |
| SCR-WIZARD-006 | Step 4 - 分配工程師 | PROJECT_CREATE |
| SCR-WIZARD-007 | Step 4-1 - 工程師分配對話框 | PROJECT_CREATE |
| SCR-WIZARD-008 | Step 4-2 - 完成確認對話框 | PROJECT_CREATE |
| SCR-PROJECT-001 | 案件總覽畫面 | PROJECT_VIEW |
| SCR-PROJECT-002 | 案件詳細畫面 | PROJECT_VIEW |
| SCR-PROJECT-003 | 編輯案件對話框 | PROJECT_UPDATE |
| SCR-REGULATION-001 | 法規管理畫面 | REGULATION_ADD |
| SCR-REGULATION-002 | 新增/編輯法規對話框 | REGULATION_ADD |
| SCR-TESTITEM-001 | 測項管理畫面 | TESTITEM_UPDATE |
| SCR-TESTITEM-LIST-001 | 測項列表畫面 | TESTITEM_VIEW |
| SCR-TESTITEM-STATUS-001 | 測項狀態手動覆寫 | TESTITEM_STATUS_OVERRIDE |
| SCR-TESTITEM-REVISION-001 | 補測版本建立對話框 | TESTITEM_REVISION_CREATE |
| SCR-ENGINEER-ASSIGN-001 | 工程師指派畫面 | TESTITEM_ASSIGN_ENGINEER |
| SCR-REVISION-LIST-001 | 補測版本列表畫面 | TESTITEM_VIEW |
| SCR-REVISION-ROLLBACK-001 | 補測版本回滾對話框 | TESTITEM_REVISION_ROLLBACK |
| SCR-WORKLOG-001 | 我的測項清單畫面 | WORKLOG_VIEW_OWN |
| SCR-WORKLOG-002 | 回報工時對話框 | WORKLOG_CREATE |
| SCR-WORKLOG-003 | 工時記錄查詢畫面 | WORKLOG_VIEW_OWN/ALL |
| SCR-WORKLOG-004 | 測項完成狀態取消 | TESTITEM_STATUS_CANCEL |
| SCR-LOADING-001 | 工程師Loading總覽畫面 | LOADING_VIEW_ALL |
| SCR-LOADING-002 | 工程師Loading明細畫面 | LOADING_VIEW_ALL |
| SCR-DELAY-001 | 延遲原因管理畫面 | DELAY_MANAGE |
| SCR-DELAY-002 | 新增/編輯延遲原因對話框 | DELAY_MANAGE |
| SCR-AUDIT-001 | 稽核日誌查詢畫面 | AUDIT_VIEW |
| SCR-AUDIT-002 | 稽核日誌詳細畫面 | AUDIT_VIEW |
| SCR-REPORT-001 | 案件進度報表畫面 | REPORT_VIEW_ALL |
| SCR-REPORT-002 | 工時統計報表畫面 | REPORT_VIEW_ALL |
| SCR-REPORT-003 | 延遲分析報表畫面 | REPORT_VIEW_ALL |
| SCR-SETTING-001 | 系統參數設定畫面 | SYSTEM_SETTING |

**總計：42 個 UI 畫面**

---

### 21.2 文件版本記錄

| 版本 | 日期 | 修改者 | 主要變更內容 |
|------|------|--------|------------|
| v1.0 | 2025-11-14 | 系統分析師 | 初版,僅包含Login模組 |
| v2.0 | 2025-11-17 | 系統分析師 | 擴充完整功能模組 |
| v2.1 | 2025-11-19 | 系統分析師 | 新增混合登入、IAM權限管理、JWT Token規範、臨時權限授予 |
| v2.2 | 2025-11-20 | 系統分析師 | TestItem狀態計算邏輯、狀態逆向操作、Regulation狀態 |
| v2.3 | 2025-11-20 | 系統分析師 | WorkLog/TestItemRevision Soft Delete、Regulation狀態計算、DelayReason停用機制、PermissionGroupMapping欄位補充 |
| v2.3 Updated | 2025-11-21 | 系統分析師 | 新增TestItemRevision完整定義、工程師分配權限細分、Regulation管理權限與UI、補測版本管理權限、統一權限命名、補充8個UI介面、Email合併邏輯、UI Flow流程、案件總覽改GridControl、TestItemEngineer RoleType詳細定義 |

---

### 21.3 待確認事項

1. ✅ TestItemRevision 欄位定義 - 已補充完整
2. ✅ 工程師分配權限細分 - 已新增 ASSIGN/SUPPORT/REMOVE
3. ✅ Regulation 管理權限與UI - 已新增 REGULATION_ADD/DISABLE/REMOVE 與 2 個UI
4. ✅ 補測版本管理權限 - 已新增 REVISION_CREATE/ROLLBACK
5. ✅ 權限命名統一 - 已統一為 AUDIT_VIEW, SYSTEM_SETTING
6. ✅ 缺少的 8 個 UI 畫面 - 已全部補充
7. ✅ Email 合併邏輯 - 已補充首次新增規則
8. ✅ UI Flow 與操作流程 - 已補充 4 個完整流程
9. ✅ 案件總覽改為 GridControl - 已更新為列表模式
10. ✅ TestItemEngineer RoleType - 已補充詳細定義

---

### 21.4 參考文件

- SA (System Analysis) - 系統分析文件
- SD (System Design) - 系統設計文件
- DB Schema - 資料庫結構定義
- API Specification - API 介面規格

---

## 結語

本規格書已完整定義 RF案件排程系統的所有功能模組、畫面元素、操作流程、驗證規則與防呆處理。所有功能皆已對應至具體的權限要求與 API 端點,確保系統的安全性與可追溯性。

**重要提醒：**
1. 所有 UI 必須依照本規格實作,不得擅自變更
2. 所有權限檢查必須在前端與後端雙重驗證
3. 所有重要操作必須寫入 AuditLog
4. 所有錯誤訊息必須對使用者友善且具體
5. 所有日期時間一律使用 ISO 8601 格式

**下一步：**
- 更新 SD (System Design) 文件
- 更新 DB Schema
- 更新 API Specification
- 進行 UI/UX 設計
- 開始開發實作

---

**文件結束**