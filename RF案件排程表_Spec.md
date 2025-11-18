# 📙 RF案件排程系統 — 系統規格書 (Spec v2.0)

---

## 📖 文件說明

**文件目的：** 詳細定義系統每個功能的畫面元素、操作流程、欄位規範、驗證規則及防呆處理。

**版本歷程：**
- v1.0 (2024-01-15)：初版，僅包含Login模組
- v2.0 (2024-01-20)：擴充完整功能模組

---

## 1. 登入與帳號管理模組

### 1.1 Login 畫面 (SCR-LOGIN-001)

**畫面目的：** 提供使用者身份驗證入口

**操作流程：**
```
1. 使用者輸入帳號與密碼
2. 按下「登入」或按Enter鍵
3. 系統驗證
   ├─ 成功 → 跳轉至主畫面
   └─ 失敗 → 顯示錯誤訊息，清空密碼欄位
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
| 帳號停用 | 403 | "帳號已停用，請聯繫主管" |
| 連續失敗5次 | 429 | "登入次數過多，請10分鐘後再試" |

**安全規則：**
- 錯誤訊息不區分「帳號不存在」與「密碼錯誤」
- 密碼輸入後不可複製
- 閒置15分鐘自動登出

---

### 1.2 忘記密碼畫面 (SCR-FORGOT-001)

**進入方式：** Login畫面 → 點擊「忘記密碼？」

**說明文字內容：**
```
請選擇以下其中一種方式來重設密碼：
• 使用帳號：輸入您的登入帳號
• 使用Email：輸入您註冊時使用的Email

如果資料正確，我們會將重設密碼連結寄到您的信箱。
```

**操作流程：**
```
1. 選擇重設方式（帳號 or Email）
2. 輸入對應資料
3. 按下「送出」
4. 系統顯示提示訊息（不論成功或失敗）
   → "如果您提供的資料正確，重設密碼連結已寄出，請檢查您的信箱（包含垃圾郵件匣）"
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
IF 帳號/Email存在 THEN
    生成Token（GUID + Hash）
    儲存至PasswordReset表（ExpireAt = 30分鐘後）
    發送Email（Template: MAIL-RESET-001）
ELSE
    不執行任何動作（避免洩漏帳號存在與否）
END IF

回傳前端：200 OK + 統一訊息
```

---

### 1.3 重設密碼畫面 (SCR-RESET-001)

**進入方式：** Email連結 `https://api.example.com/reset?token={Token}`

**密碼規則文字 (lblPasswordRule)：**
```
密碼需符合以下規則：
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
| Token已過期 | 410 | "重設連結已過期，請重新申請" | 提供返回登入連結 |
| Token已使用 | 410 | "此連結已使用過" | 提示已完成重設 |
| 新密碼與舊密碼相同 | 400 | "新密碼不可與舊密碼相同" | 保持在當前頁面 |

**成功流程：**
```
1. 驗證通過
2. 更新User.PasswordHash
3. 設定PasswordReset.UsedAt = 當前時間
4. 新增AuditLog（Action=PasswordReset）
5. 跳轉至成功頁面（顯示3秒後自動跳轉登入）
```

---

### 1.4 使用者管理畫面 (SCR-USER-001)

**權限要求：** Manager Only

**畫面布局：**
```
┌─────────────────────────────────────────┐
│ [新增使用者]  [搜尋: ________] [🔍]      │
├─────────────────────────────────────────┤
│ Grid: 使用者清單                         │
│ ┌───┬────┬────┬───┬────┬────┬─────┐     │
│ │選│帳號│姓名│角色│Email│狀態│操作  │     │
│ ├───┼────┼────┼───┼────┼────┼─────┤     │
│ │☐│eng01│王小明│工程│...│啟用│[編輯]│    │
│ │☐│eng02│李小華│工程│...│停用│[編輯]│    │
│ └───┴────┴────┴───┴────┴────┴─────┘     │
│                                         │
│  [批次停用]              [批次啟用]       │
└─────────────────────────────────────────┘
```

**操作按鈕規格：**
| 按鈕 | 觸發動作 | 前置條件 | 後置動作 |
|------|---------|---------|---------|
| 新增使用者 | 開啟 SCR-USER-002 | - | 重整Grid |
| 編輯 | 開啟 SCR-USER-002 | - | 重整該列 |
| 重設密碼 | 確認對話框 → 發送重設信 | IsActive=true | 顯示成功訊息 |
| 批次停用 | 確認對話框 → API批次更新 | 至少勾選1筆 | 重整Grid |

---

### 1.5 新增/編輯使用者對話框 (SCR-USER-002)

**開啟模式：** Modal Dialog

**新增模式特殊規則：**
- 系統自動產生隨機初始密碼（12碼）
- 自動發送Email通知新使用者（Template: MAIL-NEWUSER-001）
- Email包含帳號與提示「首次登入需重設密碼」

**編輯模式限制：**
- 帳號不可修改
- 停用帳號前需確認：「此使用者負責 {N} 個未完成測項，停用後需重新分配，確定要停用嗎?」

**驗證規則：**
| 欄位 | 規則 | 錯誤訊息 |
|------|------|---------|
| Account | 英數底線、5-20字元、不可重複 | "帳號已存在" / "帳號格式不正確" |
| Email | Email格式、不可重複 | "Email格式錯誤" / "此Email已被使用" |
| WeeklyHours | 1-80之間 | "每週工時需在1-80之間" |

---

## 2. 案件建立精靈 (Wizard)

### 2.1 精靈主畫面 (SCR-WIZARD-001)

**權限要求：** Manager Only

**步驟導航規則：**
- Step1 完成前無法進入 Step2
- 可隨時返回前一步驟修改
- Step4 完成後顯示「完成」按鈕
- 點擊「完成」前會顯示完整預覽摘要

**離開確認：**
- 未完成時點擊關閉 → 提示「資料尚未儲存,確定要離開嗎？」
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
| 日期區間超過1年 | 警告："日期區間超過365天，確定嗎？" | 下一步 |

**下一步動作：**
- 驗證通過 → 將資料暫存至Wizard Context
- 跳轉至 Step 2

---

### 2.3 Step 2 - 選擇法規 (SCR-WIZARD-003)

**左側清單 (可選法規):**
- 顯示所有系統內建的法規名稱
- 支援多選（CheckBox）
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
1. 從下拉選單選擇法規（預設第一個）
2. 點擊「新增測項」→ 開啟對話框 (SCR-WIZARD-005)
3. 在Grid中檢視/編輯/刪除測項
4. 切換法規時，顯示該法規的測項清單

**刪除確認：**
- "確定要刪除測項「{TestItemName}」嗎？"
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
| 預估工時>100 | 警告："工時超過100小時，確認嗎？" |

**確定動作：**
- 驗證通過 → 回傳測項資料至 SCR-WIZARD-004
- 在Grid中新增/更新該列

---

### 2.6 Step 4 - 分配工程師 (SCR-WIZARD-006)

**操作流程：**
1. 選擇法規 → 選擇測項
2. 點擊「新增工程師」→ 開啟對話框 (SCR-WIZARD-007)
3. 檢視Loading警示（>80%顯示紅色）
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

**下一步動作（完成按鈕）：**
1. 驗證所有測項皆已分配工程師
2. 顯示確認摘要對話框 (SCR-WIZARD-008)
3. 確認後寫入DB：
   - Project (Status=Active)
   - Regulation
   - TestItem (Status=NotStarted)
   - TestItemEngineer
4. 寫入AuditLog (Action=Create)
5. 跳轉至案件總覽畫面

---

### 2.7 Step 4-1 - 新增/編輯工程師分配對話框 (SCR-WIZARD-007)

**開啟模式：** Modal Dialog

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 未選擇工程師 | "請選擇工程師" |
| 分配工時≤0 | "分配工時需大於0" |
| 該工程師已在清單中 | "此工程師已被分配至此測項" |
| NewLoading>100% | 警告："此分配將使工程師超載，確定嗎？" |

---

### 2.8 Step 4-2 - 完成確認摘要對話框 (SCR-WIZARD-008)

**開啟模式：** Modal Dialog

**超載工程師顯示：**
- 若無超載：顯示「無」（綠色）
- 若有超載：顯示「王小明(120%), 李小華(95%)」（紅色）

**確定建立動作：**
1. 顯示進度條：「正在建立案件...」
2. 呼叫API：POST /api/projects/create-with-wizard
3. 後端Transaction處理
4. 成功：顯示「案件建立成功！」→ 3秒後跳轉
5. 失敗：顯示錯誤訊息，保持在Wizard畫面

---

## 3. 工時回報模組

### 3.1 我的測項清單畫面 (SCR-WORKLOG-001)

**權限要求：** Engineer Only

**篩選選項：**
- 全部
- 未開始
- 進行中
- 已完成
- 延遲中

**狀態顏色標示：**
- 未開始：灰色
- 進行中：藍色
- 已完成：綠色
- 延遲中：紅色

**操作按鈕：**
| 按鈕 | 功能 | 跳轉畫面 |
|------|------|---------|
| 回報工時 | 開啟工時回報對話框 | SCR-WORKLOG-002 |
| 查看記錄 | 顯示該測項的所有WorkLog | SCR-WORKLOG-003 |

---

### 3.2 回報工時對話框 (SCR-WORKLOG-002)

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
| 該日期已回報過 | "此日期已有工時記錄，請至記錄列表修改" |
| 狀態=延遲但未選延遲原因 | "請至少選擇1個延遲原因" |
| ActualHours累計>AssignedHours×1.5 | 警告："工時已超過分配工時50%，確定嗎？" |

**送出動作：**
1. 驗證通過
2. 呼叫API：POST /api/worklogs
3. 後端自動更新TestItem.Status：
   - 若Status=Delayed → TestItem.Status=Delayed
   - 若Status=Completed且所有工程師都完成 → TestItem.Status=Completed
   - 否則 → TestItem.Status=InProgress
4. 寫入AuditLog
5. 關閉對話框，重整SCR-WORKLOG-001清單

---

### 3.3 工時記錄查詢畫面 (SCR-WORKLOG-003)

**開啟模式：** Modal Dialog 

**編輯規則：**
- 7天內的記錄可編輯
- 超過7天：顯示「此記錄已超過7天，無法修改」

**刪除規則：**
- 7天內可刪除
- 需確認：「確定要刪除此工時記錄嗎？」
- 刪除後重新計算TestItem狀態

**匯出Excel功能：**
- 匯出當前篩選結果
- 檔名格式：`WorkLog_{TestItemName}_{DateTime}.xlsx`

---

## 4. 案件管理模組 (Manager)

### 4.1 案件總覽畫面 (SCR-PROJECT-001)

**權限要求：** Manager Only

**狀態篩選選項：**
- 全部
- 草稿 (Draft)
- 進行中 (Active)
- 已完成 (Completed)
- 延遲 (Delayed)
- 暫停 (OnHold)

**優先順序顏色：**
- High：紅色粗體
- Medium：橘色
- Low：綠色

**操作按鈕：**
| 按鈕 | 功能 | 跳轉畫面 |
|------|------|---------|
| 新增案件 | 開啟建案精靈 | SCR-WIZARD-001 |
| 詳細 | 查看案件詳情 | SCR-PROJECT-002 |
| 編輯 | 修改案件基本資料 | SCR-PROJECT-003 |
| 刪除 | Soft Delete | 確認對話框 |

**刪除確認：**
```
確定要刪除案件「{ProjectName}」嗎？

此操作將同時刪除：
• {RegulationCount} 個法規
• {TestItemCount} 個測項
• 所有工程師分配記錄

⚠ 工時記錄將保留以供查詢

[取消] [確定刪除]
```

---

### 4.2 案件詳細畫面 (SCR-PROJECT-002)

**Tab 1 - 法規與測項：**
- TreeView結構：
  ```
  ├─ FCC (2025/11/01 ~ 2025/11/30)
  │  ├─ Conducted (未開始)
  │  └─ Radiated (進行中)
  ├─ CE (2025/12/01 ~ 2025/12/31)
  │  └─ DFC (未開始)
  ```
**Tab 2 - 工時統計：**
- Chart: 預估工時 vs 實際工時 (Bar Chart)
- Grid: 各測項工時明細

**Tab 3 - 延遲分析：**
- PieChart: 延遲原因分布
- Grid: 延遲測項清單

---

### 4.3 編輯案件對話框 (SCR-PROJECT-003)

**開啟模式：** Modal Dialog

**畫面元素：** (同 SCR-WIZARD-002，但不含Wizard步驟導航)

**額外欄位：**
| 元素類型 | 標籤 | 控制項 | 說明 |
|---------|------|--------|------|
| ComboBox | 狀態 | cboStatus | Manager可手動設為OnHold |
| TextBox (多行) | 修改理由 | txtReason | 修改時必填，寫入AuditLog |

**驗證規則：** (同建案規則)

---

### 4.4 測項管理畫面 (SCR-TESTITEM-001)

**進入方式：** SCR-PROJECT-002 → TreeView右鍵 → 編輯測項

**開啟模式：** Modal Dialog

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
   - 建立空的版本，後續需手動分配工程師
4. 確定後：
   - 建立TestItemRevision記錄
   - TestItem.Status自動改為InProgress
   - 寫入AuditLog

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 版本編號已存在 | "此版本編號已存在" |
| 預估工時≤0 | "預估工時需大於0" |

---

## 5. Loading 報表模組 (Manager)

### 5.1 工程師Loading總覽畫面 (SCR-LOADING-001)

**權限要求：** Manager Only

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

---

### 5.2 工程師Loading明細畫面 (SCR-LOADING-002)

**開啟模式：** 獨立視窗

**每日工時趨勢圖：**
- X軸：日期
- Y軸：工時
- 顯示期間內每日實際工時
- 標示平均線

---

## 6. 延遲原因管理模組 (Manager)

### 6.1 延遲原因管理畫面 (SCR-DELAY-001)

**權限要求：** Manager Only

**操作規則：**
- 使用次數>0的原因：
  - 不可刪除
  - 只能停用（停用後不顯示在工時回報畫面）
- 使用次數=0的原因：
  - 可刪除（實體刪除）

---

### 6.2 新增/編輯延遲原因對話框 (SCR-DELAY-002)

**開啟模式：** Modal Dialog

**驗證規則：**
| 驗證項目 | 錯誤訊息 |
|---------|---------|
| 原因文字空白 | "請輸入原因文字" |
| 原因文字重複 | "此原因已存在" |

---

## 7. 稽核日誌模組 (Manager)

### 7.1 稽核日誌查詢畫面 (SCR-AUDIT-001)

**權限要求：** Manager Only

**篩選條件：**

**資料表選項：**
- 全部
- Project
- Regulation
- TestItem
- TestItemEngineer
- WorkLog
- User
- DelayReason

**操作類型選項：**
- 全部
- Create (新增)
- Update (更新)
- Delete (刪除)
- PasswordReset (密碼重設)

**操作人：**
- 下拉選單顯示所有使用者

**[詳細]按鈕：**
- 跳轉至 SCR-AUDIT-002 (詳細比對)

---

### 7.2 稽核日誌詳細畫面 (SCR-AUDIT-002)

**開啟模式：** Modal Dialog (大視窗)

---

## 8. 報表模組 (Manager)

### 8.1 案件進度報表畫面 (SCR-REPORT-001)

**權限要求：** Manager Only


### 8.2 工時統計報表畫面 (SCR-REPORT-002)

**權限要求：** Manager Only


### 8.3 延遲分析報表畫面 (SCR-REPORT-003)

**權限要求：** Manager Only

---


## 9. 防呆處理總表

### 9.1 日期驗證

| 情境 | 驗證規則 | 錯誤訊息 |
|------|---------|---------|
| Project.StartDate | 不可早於今天 | "開始日期不可早於今天" |
| Project.EndDate | 需晚於StartDate | "結束日期需晚於開始日期" |
| Regulation日期 | 需在Project日期±30天內 | "法規日期需在案件日期前後30天內" |
| WorkLog.WorkDate | 不可晚於今天 | "不可回報未來日期" |

### 9.2 數值驗證

| 欄位 | 範圍 | 錯誤訊息 |
|------|------|---------|
| EstimatedHours | 0.5 - 999.9 | "預估工時需在0.5-999.9之間" |
| ActualHours | 0.5 - 12 | "單日工時需在0.5-12之間" |
| AssignedHours | 0.5 - 999.9 | "分配工時需在0.5-999.9之間" |
| WeeklyAvailableHours | 1 - 80 | "每週工時需在1-80之間" |

### 9.3 重複性驗證

| 欄位 | 範圍 | 錯誤訊息 |
|------|------|---------|
| User.Account | 全系統唯一 | "此帳號已存在" |
| User.Email | 全系統唯一 | "此Email已被使用" |
| Project.ProjectName | 全系統唯一 | "此案件名稱已存在" |
| TestItem.TestItemName | 同Regulation下唯一 | "此測項名稱已存在於該法規" |
| WorkLog | 同工程師+同測項+同日期唯一 | "此日期已有工時記錄" |

### 9.4 刪除防呆

| 對象 | 規則 | 提示訊息 |
|------|------|---------|
| Project | 有未完成TestItem | "此案件尚有未完成測項，確定刪除？" |
| User | 有負責測項 | "此使用者負責{N}個未完成測項，停用後需重新分配" |
| DelayReason | 已被使用 | "此原因已被使用，無法刪除，僅能停用" |
| TestItem | 有工時記錄 | "此測項已有工時記錄，確定刪除？" |

### 9.5 併發控制

| 情境 | 處理方式 | 提示訊息 |
|------|---------|---------|
| 同時編輯Project | 使用RowVersion檢查 | "此案件已被他人修改，請重新載入" |
| 同時編輯TestItem | 使用RowVersion檢查 | "此測項已被他人修改，請重新載入" |
| 同時回報WorkLog | 檢查日期唯一性 | "此日期已有工時記錄" |

---

## 10. 操作流程圖

### 10.1 建案完整流程

```
開始
  ↓
[Manager登入]
  ↓
[點擊「新增案件」]
  ↓
┌─────────────────┐
│ Wizard Step 1   │
│ 填寫案件基本資料 │
└─────────────────┘
  ↓
驗證通過？
  ├─ No → 顯示錯誤訊息，停留在Step 1
  └─ Yes
      ↓
┌─────────────────┐
│ Wizard Step 2   │
│ 選擇法規與日期   │
└─────────────────┘
  ↓
至少選1個法規？
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
┌─────────────────┐
│ Wizard Step 3   │
│ 新增測試項目     │
└─────────────────┘
  ↓
每個法規都有測項？
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
┌─────────────────┐
│ Wizard Step 4   │
│ 分配工程師       │
└─────────────────┘
  ↓
每個測項都有主要工程師？
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
[顯示確認摘要]
  ↓
確認建立？
  ├─ No → 返回Wizard修改
  └─ Yes
      ↓
[呼叫API：POST /api/projects/create-with-wizard]
  ↓
[Transaction處理]
  ├─ 建立Project (Status=Active)
  ├─ 建立Regulation
  ├─ 建立TestItem (Status=NotStarted)
  ├─ 建立TestItemEngineer
  └─ 寫入AuditLog
  ↓
成功？
  ├─ No → 顯示錯誤，回到Wizard
  └─ Yes
      ↓
[顯示「案件建立成功」]
  ↓
[3秒後跳轉至案件總覽]
  ↓
結束
```

---

### 10.2 工時回報流程

```
開始
  ↓
[Engineer登入]
  ↓
[進入「我的測項」畫面]
  ↓
[選擇測項，點擊「回報工時」]
  ↓
┌─────────────────────┐
│ 回報工時對話框       │
│ - 選擇版本           │
│ - 選擇日期           │
│ - 輸入工時           │
│ - 選擇狀態           │
│ - (延遲時)選延遲原因 │
│ - 輸入備註           │
└─────────────────────┘
  ↓
前端驗證通過？
  ├─ No → 顯示錯誤訊息，停留在對話框
  └─ Yes
      ↓
[呼叫API：POST /api/worklogs]
  ↓
後端驗證
  ├─ 日期是否已回報？
  ├─ 工時是否合理？
  └─ 延遲狀態是否有原因？
  ↓
驗證通過？
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
[更新Project.Status]
  ├─ IF 任一TestItem=Delayed
  │   → Project.Status=Delayed
  ├─ ELSE IF 所有TestItem=Completed
  │   → Project.Status=Completed
  └─ ELSE → Project.Status=Active
  ↓
[寫入AuditLog]
  ↓
[回傳成功訊息]
  ↓
[關閉對話框，重整測項清單]
  ↓
結束
```

---

### 10.3 忘記密碼流程

```
開始
  ↓
[使用者在登入畫面點擊「忘記密碼？」]
  ↓
┌─────────────────────┐
│ 忘記密碼畫面         │
│ - 選擇使用帳號/Email │
│ - 輸入對應資料       │
└─────────────────────┘
  ↓
[按下「送出」]
  ↓
[呼叫API：POST /api/auth/forgot-password]
  ↓
後端處理
  ├─ 依帳號/Email查詢User
  ├─ IF 找到 THEN
  │   ├─ 產生Token (GUID + Hash)
  │   ├─ 儲存至PasswordReset表
  │   │   - ExpireAt = 現在+30分鐘
  │   │   - UsedAt = NULL
  │   └─ 發送Email (含重設連結)
  └─ ELSE
      └─ 不執行任何動作 (安全考量)
  ↓
[回傳前端：統一成功訊息]
  ↓
[顯示「如果資料正確，重設連結已寄出」]
  ↓
使用者收到Email
  ↓
[點擊Email中的連結]
  ↓
[開啟瀏覽器，載入重設密碼頁面]
  ↓
[呼叫API：GET /api/auth/validate-token?token=xxx]
  ↓
Token有效？
  ├─ No (不存在/已過期/已使用)
  │   → 顯示錯誤頁面：「連結已失效，請重新申請」
  └─ Yes
      ↓
┌─────────────────────┐
│ 重設密碼畫面         │
│ - 輸入新密碼         │
│ - 確認新密碼         │
└─────────────────────┘
  ↓
前端驗證
  ├─ 密碼長度8-20？
  ├─ 包含英文與數字？
  └─ 兩次密碼一致？
  ↓
驗證通過？
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
[按下「確認送出」]
  ↓
[呼叫API：POST /api/auth/reset-password]
  ↓
後端處理
  ├─ 再次驗證Token
  ├─ Hash新密碼
  ├─ 更新User.PasswordHash
  ├─ 設定PasswordReset.UsedAt=現在
  └─ 寫入AuditLog
  ↓
成功？
  ├─ No → 顯示錯誤訊息
  └─ Yes
      ↓
[顯示「密碼重設成功，3秒後跳轉登入頁」]
  ↓
[自動跳轉至登入畫面]
  ↓
結束
```

---

## 11. API端點規範 (API Endpoints Specification)

### 11.1 認證模組 (Auth)

| 端點 | 方法 | 說明 | Request Body | Response |
|------|------|------|--------------|----------|
| /api/auth/login | POST | 使用者登入 | {account, password} | {token, user} |
| /api/auth/forgot-password | POST | 申請重設密碼 | {account?, email?} | {success: true} |
| /api/auth/validate-token | GET | 驗證重設Token | Query: token | {valid: true/false} |
| /api/auth/reset-password | POST | 執行密碼重設 | {token, newPassword} | {success: true} |

---

### 11.2 使用者管理 (Users)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/users | GET | 取得使用者清單 | Manager |
| /api/users/{id} | GET | 取得使用者詳細 | Manager |
| /api/users | POST | 新增使用者 | Manager |
| /api/users/{id} | PUT | 更新使用者 | Manager |
| /api/users/{id}/deactivate | POST | 停用使用者 | Manager |
| /api/users/{id}/activate | POST | 啟用使用者 | Manager |

---

### 11.3 案件管理 (Projects)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/projects | GET | 取得案件清單 | Manager |
| /api/projects/{id} | GET | 取得案件詳細 | Manager |
| /api/projects/create-with-wizard | POST | Wizard建案 | Manager |
| /api/projects/{id} | PUT | 更新案件 | Manager |
| /api/projects/{id} | DELETE | 刪除案件(Soft) | Manager |
| /api/projects/{id}/regulations | GET | 取得案件的法規清單 | Manager |
| /api/projects/{id}/testitems | GET | 取得案件的測項清單 | Manager |

---

### 11.4 測試項目 (TestItems)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/testitems/{id} | GET | 取得測項詳細 | All |
| /api/testitems | POST | 新增測項 | Manager |
| /api/testitems/{id} | PUT | 更新測項 | Manager |
| /api/testitems/{id} | DELETE | 刪除測項(Soft) | Manager |
| /api/testitems/{id}/engineers | GET | 取得測項工程師 | All |
| /api/testitems/{id}/engineers | POST | 分配工程師 | Manager |
| /api/testitems/{id}/revisions | GET | 取得補測版本清單 | All |
| /api/testitems/{id}/revisions | POST | 建立補測版本 | Manager |

---

### 11.5 工時管理 (WorkLogs)

| 端點 | 方法 | 說明 | 權限 |
|------|------|------|------|
| /api/worklogs/my-tasks | GET | 取得我的測項清單 | Engineer |
| /api/worklogs | POST | 新增工時記錄 | Engineer |
| /api/worklogs/{id} | PUT | 修改工時記錄 | Engineer(7天內) |
| /api/worklogs/{id} | DELETE | 刪除工時記錄 | Engineer(7天內) |
| /api/worklogs/testitem/{id} | GET | 取得測項的工時記錄 | All |
| /api/worklogs/{id}/override | PUT | Manager覆寫工時 | Manager |

---

## 12. 錯誤碼對照表

### 12.1 認證相關 (AUTH)

| 錯誤碼 | HTTP狀態 | 說明 | 前端處理 |
|--------|---------|------|---------|
| AUTH001 | 401 | 帳號或密碼錯誤 | 顯示訊息，清空密碼欄 |
| AUTH002 | 403 | 帳號已停用 | 顯示訊息，提示聯繫主管 |
| AUTH003 | 429 | 登入次數過多 | 顯示訊息，禁用登入按鈕 |
| AUTH004 | 401 | Token已過期 | 自動跳轉登入頁 |
| AUTH005 | 404 | 重設Token無效 | 顯示錯誤頁，提供重新申請連結 |
| AUTH006 | 410 | 重設Token已使用 | 顯示訊息，跳轉登入頁 |

---

### 12.2 驗證相關 (VAL)

| 錯誤碼 | HTTP狀態 | 說明 | 前端處理 |
|--------|---------|------|---------|
| VAL001 | 400 | 必填欄位未填寫 | 標記錯誤欄位 |
| VAL002 | 400 | 資料格式錯誤 | 顯示格式說明 |
| VAL003 | 400 | 數值超出範圍 | 顯示允許範圍 |
| VAL004 | 409 | 資料重複 | 提示修改 |
| VAL005 | 400 | 日期邏輯錯誤 | 顯示正確規則 |

---

### 12.3 業務邏輯 (BIZ)

| 錯誤碼 | HTTP狀態 | 說明 | 前端處理 |
|--------|---------|------|---------|
| BIZ001 | 409 | 測項已有工時記錄，無法刪除 | 顯示警告，提供查看工時連結 |
| BIZ002 | 409 | 工程師已超載 | 顯示警告，允許繼續或取消 |
| BIZ003 | 409 | 該日期已有工時記錄 | 提示修改現有記錄 |
| BIZ004 | 403 | 工時記錄超過7天無法修改 | 顯示訊息，提示聯繫主管 |
| BIZ005 | 409 | 延遲原因已被使用，無法刪除 | 提示只能停用 |
| BIZ006 | 409 | 資料已被他人修改 | 提供重新載入按鈕 |

---

### 12.4 系統錯誤 (SYS)

| 錯誤碼 | HTTP狀態 | 說明 | 前端處理 |
|--------|---------|------|---------|
| SYS001 | 500 | 資料庫連線失敗 | 顯示錯誤，建議稍後再試 |
| SYS002 | 500 | Transaction失敗 | 顯示錯誤，資料已回滾 |
| SYS003 | 503 | 服務暫時無法使用 | 顯示維護訊息 |
| SYS004 | 500 | Email發送失敗 | 記錄日誌，顯示警告 |

---

## 13. 畫面關聯圖

```
登入畫面 (SCR-LOGIN-001)
  ├─[忘記密碼] → 忘記密碼畫面 (SCR-FORGOT-001)
  │                └─[Email連結] → 重設密碼畫面 (SCR-RESET-001)
  │
  ├─[Engineer登入成功] → 我的測項清單 (SCR-WORKLOG-001)
  │                       ├─[回報工時] → 回報工時對話框 (SCR-WORKLOG-002)
  │                       └─[查看記錄] → 工時記錄查詢 (SCR-WORKLOG-003)
  │
  └─[Manager登入成功] → 主選單
                          ├─[案件管理]
                          │   ├─ 案件總覽 (SCR-PROJECT-001)
                          │   │   ├─[新增] → 建案精靈 (SCR-WIZARD-001~006)
                          │   │   ├─[詳細] → 案件詳細 (SCR-PROJECT-002)
                          │   │   └─[編輯] → 編輯案件 (SCR-PROJECT-003)
                          │   │
                          │   └─ 測項管理 (SCR-TESTITEM-001)
                          │       └─[建立補測] → 補測版本對話框 (SCR-REVISION-001)
                          │
                          ├─[工程師管理]
                          │   └─ 使用者管理 (SCR-USER-001)
                          │       └─[新增/編輯] → 使用者對話框 (SCR-USER-002)
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
