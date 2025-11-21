# 📘 RF案件排程系統 — 系統分析文件 (SA v3.0)

---

## 📖 文件說明

**版本歷程:**
- v1.0 (2025-11-14):初版分析文件
- v2.0 (2025-11-17):調整職責邊界,移除技術實作細節
- v2.1 (2025-11-19):補充混合登入機制(Local + Windows 驗證)、JWT 安全性說明、IAM 權限模型(Permission / PermissionGroup / UserPermission)
- v2.2 (2025-11-20):補充 TestItem 狀態計算邏輯、狀態逆向操作規則、完整 ERD 圖
- v2.3 (2025-11-20):WorkLog、TestItemRevision Soft Delete 機制說明、Regulation 狀態計算邏輯、DelayReason 停用機制說明、PermissionGroupMapping 欄位補充
- **v3.0 (2025-11-21):重大更新**
  - 補充 TestItemRevision 完整定義(RevisionType、欄位說明、UI 流程)
  - 新增工程師分配相關權限(TESTITEM_ASSIGN_ENGINEER、TESTITEM_ASSIGN_SUPPORT、TESTITEM_REMOVE_ENGINEER)
  - 新增 Regulation 維護權限與 UI 介面(REGULATION_ADD、REGULATION_DISABLE、REGULATION_REMOVE)
  - 新增補測版本管理權限(TESTITEM_REVISION_CREATE、TESTITEM_REVISION_ROLLBACK)
  - 統一權限命名(AUDIT_VIEW、SYSTEM_SETTING)
  - 補充 8 個缺少的 UI 介面定義
  - 強化 Email 合併邏輯(首次由主管新增)
  - TestItemEngineer RoleType 詳細定義(Main1/Main2/Main3)
  - 主管案件總覽改為 GridControl 列表模式
  - 補充完整 UI Flow 與操作流程

---

## 1. 專案背景與問題陳述

### 1.1 現況描述

RF測試實驗室目前面臨的挑戰:

**案件特性:**
- 單一案件需測試多種法規(FCC、NCC、CE、IC、TELEC等)
- 每個法規包含多種測試項目(Conducted、Radiated、Blocking等)
- 測試需在不同場地進行(Lab A、Lab B、Lab C等)
- 涉及多位工程師輪班作業(白班、小夜班、大夜班)

**當前管理方式:**
- 使用Excel試算表記錄案件資訊
- 透過Email與口頭溝通協調工作
- 工時記錄在個人筆記或Excel中
- 進度追蹤依賴人工整理

---

### 1.2 核心問題分析

#### 問題1:資訊分散與查詢困難
- **現象:** 案件資料散落在多個Excel檔案與Email中
- **影響:** 難以快速找到特定案件、法規或測項的進度

#### 問題2:工時追蹤不透明
- **現象:** 工程師各自記錄工時,無統一格式
- **影響:** 
  - 主管無法即時掌握實際工時
  - 補測版本(v1/v2/v3)工時混雜,無法比較分析
  - 預估與實際工時差異難以分析

#### 問題3:工程師負載無法視覺化
- **現象:** 主管無法即時看到每位工程師的負載狀況
- **影響:**
  - 任務分配不均,導致部分工程師超載
  - 無法提前預警資源瓶頸
  - 影響案件交期

#### 問題4:延遲原因難以追蹤
- **現象:** 僅知道案件「延遲」,但不清楚根本原因
- **影響:**
  - 無法針對性改善流程
  - 無法釐清責任歸屬(設備、客戶、人力、場地)
  - 重複發生相同問題

#### 問題5:缺乏權限控管與稽核
- **現象:** 
  - 任何人都可修改Excel,無記錄誰改了什麼
  - 資料被誤刪或覆蓋時無法追查
- **影響:** 資料可信度降低,發生爭議時無憑據

#### 問題6:多人協作版本衝突
- **現象:** 兩人同時編輯同一Excel,後存檔者覆蓋前者
- **影響:** 資料遺失,需重新輸入

---

### 1.3 使用者身份合併機制(Email-Based Identity Merge)

系統採用 **Email 作為跨登入來源的唯一身份識別(Primary Identity Key)**,用於整合 Local 帳密登入與 Windows AD 驗證登入,使同一位使用者以不同方式登入時,仍能被識別為同一個系統帳號。

#### (1)合併原則
當使用者以任一方式登入(Local 或 AD)時,系統執行:

1. 從登入來源取得 Email(Local 由使用者輸入;AD 由 AD Server 提供)
2. 查詢:`SELECT * FROM Users WHERE Email = {LoginEmail}`
3. 若找到相同 Email → 視為同一使用者,不新增 User 紀錄
4. 若找不到 → 建立新使用者(依登入來源決定 AuthType)

#### (2)Email 為唯一身份識別
- Email 欄位必須為 **唯一值(UNIQUE)**
- Local / AD 兩種登入方式都需提供 Email
- 不允許兩筆使用者用相同 Email

#### (3)使用者分類

| 登入方式 | Email 來源 | 使用欄位 |
|----------|------------|----------|
| Local 帳密登入 | 使用者輸入 | Account + Email |
| AD 驗證登入 | AD 提供 | ADAccount + Email |

無論使用者以哪種方式登入,Email 相同即視為同一使用者。

#### (4)Email 首次建立規則 **【v3.0 新增】**

**重要變更:使用者不可自行註冊,Email 必須由主管預先建立**

```
使用者建立流程:
1. 主管透過「使用者管理」介面新增使用者
2. 填寫必要資訊:
   - Account(Local) 或 ADAccount(AD)
   - Email(必填,唯一)
   - DisplayName
   - Role
3. 系統檢查:
   IF Email 已存在 THEN
       顯示錯誤:「此 Email 已被使用」
       拒絕建立
   END IF
4. 建立成功後:
   - Local 使用者:發送初始密碼至 Email
   - AD 使用者:發送歡迎信,告知可使用 Windows 驗證登入
```

**登入時 Email 驗證:**
```
Local 登入:
  - 使用者輸入 Account + Password
  - 後端驗證後,取得該 User 的 Email
  - Email 必須已存在於 User 表中

AD 登入:
  - 系統自動取得 Windows 登入的 ADAccount
  - 查詢 User 表:WHERE ADAccount = {登入帳號}
  - 若不存在:顯示「您的 AD 帳號尚未註冊,請聯絡主管」
  - 若存在但 IsActive = false:顯示「帳號已停用」
```

**Email 唯一性強制規則:**
- User.Email 欄位設定 UNIQUE 約束
- 新增/修改使用者時,後端檢查 Email 唯一性
- Email 比對採用 **不區分大小寫(Case-Insensitive)**
- 儲存前統一轉為小寫
- 錯誤訊息:「此 Email 已被使用,請使用其他 Email」

---

## 2. 專案目標與成功指標

### 2.1 專案願景

> 建立一套集中化、可追蹤、可分析的RF案件排程與工時管理系統,讓工程師專注測試工作,讓主管即時掌握專案進度與資源負載。

---

### 2.2 專案目標

#### 目標1:集中化資料管理
- **描述:** 所有案件、法規、測項、工時資料集中儲存於單一系統
- **衡量指標:** 
  - 90%以上的資料查詢可在系統內完成
  - 不再需要Excel進行案件管理

#### 目標2:標準化流程
- **描述:** 透過Wizard流程標準化建案步驟

#### 目標3:即時工時追蹤
- **描述:** 工程師每日回報工時,系統即時累計與統計
- **衡量指標:** 
  - 工時回報即時性達100%(當日回報)

#### 目標4:視覺化資源負載
- **描述:** 主管可即時查看每位工程師的負載狀況

#### 目標5:延遲根因分析
- **描述:** 系統記錄延遲原因,支援統計分析
- **衡量指標:** 
  - 100%延遲案件有明確原因記錄
  - 每季產出延遲原因分析報告

#### 目標6:完整稽核機制
- **描述:** 所有資料異動皆可追蹤

---

## 3. 利害關係人分析

### 3.1 主要利害關係人

#### 3.1.1 工程師 (Engineer)
- **人數:** 約20人
- **需求:**
  - 快速查看自己負責的測項
  - 簡便的工時回報介面
  - 了解自己的工作進度
- **痛點:**
  - 現有Excel難以快速找到自己的任務
  - 工時記錄繁瑣易遺漏
  - 不清楚自己的Loading狀況
- **期望:**
  - 工時回報時間<2分鐘/次
  - 可隨時查看歷史工時記錄
  - 介面直覺易用

---

#### 3.1.2 實驗室主管 (Manager)
- **人數:** 3-5人
- **需求:**
  - 快速建立新案件
  - 即時掌握所有案件進度
  - 視覺化工程師負載
  - 分析延遲原因
  - 產出各類管理報表
- **痛點:**
  - Excel建案繁瑣易錯
  - 無法即時看到資源瓶頸
  - 手動彙整報表耗時
  - 延遲原因難以追蹤
- **期望:**
  - Wizard引導式建案
  - Dashboard一目了然
  - 一鍵產出報表
  - 完整稽核追蹤

---

## 4. 系統範圍定義

### 4.1 系統邊界

#### 4.1.1 納入範圍 (In Scope)

**核心功能:**
1. ✅ 案件管理(建立、查詢、修改、刪除)
2. ✅ 法規管理(多法規支援、新增、停用、移除) **【v3.0 新增 UI】**
3. ✅ 測試項目管理(測項定義、工程師分配、權限細分) **【v3.0 新增】**
4. ✅ 工時回報(工程師端)
5. ✅ 工時查詢與統計(主管端)
6. ✅ Loading計算與視覺化
7. ✅ 延遲原因管理與分析
8. ✅ 補測版本管理(v1/v2/v3...,含 RevisionType、回滾功能) **【v3.0 完整定義】**
9. ✅ 稽核日誌(資料異動追蹤)
10. ✅ 權限控管(工程師/主管)
11. ✅ 忘記密碼功能(Email重設)
12. ✅ 報表產出(進度、工時、延遲分析)

**資料管理:**
- ✅ 使用者管理(首次由主管建立,Email 唯一性) **【v3.0 強化】**
- ✅ 角色管理(Engineer/Manager)
- ✅ 延遲原因字典維護
- ✅ Soft Delete機制

---

### 4.2 系統與外部系統互動

```
┌───────────────────────────────────────
│   RF案件排程系統 (本系統)             │
│                                     │
│  ┌──────  ┌──────  ┌──────       │
│  │案件  │  │工時  │  │Loading│       │
│  │管理  │  │管理  │  │分析  │        │
│  └──────  └──────  └──────       │ 
│                                     │
└─────────────────────────────────────┘
         ↓ (Email)
┌─────────────────
│  Email Server   │ (發送密碼重設信、新帳號通知)
└─────────────────
```

**說明:**
- 系統僅與Email Server互動(發送系統通知信)
- 資料匯出格式:Excel(供人工匯入其他系統)

---

## 5. 功能需求總覽

### 5.1 功能模組架構

```
RF案件排程系統
│
├─ 1. 認證與授權模組
│   ├─ 1.1 使用者登入(Local 帳號)
│   ├─ 1.2 Windows 驗證登入(AD 帳號)
│   ├─ 1.3 忘記密碼(Email重設)
│   ├─ 1.4 角色與權限控管(RBAC + IAM)
│   │   ├─ 角色:Engineer / Manager / Admin
│   │   ├─ 權限:以 PermissionCode 控制
│   │   └─ 權限群組:預設權限集合,可個別加減
│   └─ 1.5 登入安全機制(失敗次數限制、Token 過期)
│
├─ 2. 案件管理模組
│   ├─ 2.1 Wizard建案流程
│   ├─ 2.2 案件查詢與瀏覽(列表模式 GridControl) **【v3.0 更新】**
│   ├─ 2.3 案件修改(含理由記錄)
│   ├─ 2.4 案件刪除(Soft Delete)
│   └─ 2.5 案件狀態自動計算
│
├─ 3. 法規管理模組 **【v3.0 新增】**
│   ├─ 3.1 法規列表管理(SCR-REGULATION-001)
│   ├─ 3.2 新增法規(SCR-REGULATION-002)
│   ├─ 3.3 停用/移除法規(SCR-REGULATION-003)
│   └─ 3.4 法規狀態自動計算
│
├─ 4. 測試項目管理模組
│   ├─ 4.1 測項列表(SCR-TESTITEM-LIST-001) **【v3.0 新增】**
│   ├─ 4.2 測項新增與修改
│   ├─ 4.3 工程師分配(Main1/Main2/Main3 + Support) **【v3.0 細化】**
│   │   ├─ SCR-ENGINEER-ASSIGN-001(工程師指派介面)
│   │   ├─ 權限:TESTITEM_ASSIGN_ENGINEER
│   │   ├─ 權限:TESTITEM_ASSIGN_SUPPORT
│   │   └─ 權限:TESTITEM_REMOVE_ENGINEER
│   ├─ 4.4 補測版本管理(Revision) **【v3.0 完整定義】**
│   │   ├─ SCR-REVISION-LIST-001(補測版本列表)
│   │   ├─ RevisionType(Command/Retest/Fix/Others)
│   │   ├─ 權限:TESTITEM_REVISION_CREATE
│   │   └─ 權限:TESTITEM_REVISION_ROLLBACK(回滾功能)
│   ├─ 4.5 測項狀態追蹤與逆向操作
│   └─ 4.6 測項刪除(Soft Delete)
│
├─ 5. 工時管理模組
│   ├─ 5.1 工時回報(工程師端)
│   ├─ 5.2 工時查詢(工程師可查自己)
│   ├─ 5.3 工時修改(7天內可改)
│   ├─ 5.4 工時統計(主管端)
│   └─ 5.5 工時覆寫(主管含理由)
│
├─ 6. Loading分析模組
│   ├─ 6.1 工程師Loading總覽
│   ├─ 6.2 工程師Loading明細
│   └─ 6.3 Loading報表匯出
│
├─ 7. 延遲管理模組
│   ├─ 7.1 延遲原因字典維護
│   ├─ 7.2 延遲案件追蹤
│   └─ 7.3 延遲原因分析報表
│
├─ 8. 使用者管理模組
│   ├─ 8.1 使用者新增(主管建立,Email 唯一性檢查) **【v3.0 強化】**
│   ├─ 8.2 使用者修改
│   ├─ 8.3 使用者停用/啟用
│   └─ 8.4 使用者查詢
│
├─ 9. 稽核日誌模組
│   ├─ 9.1 自動記錄資料異動
│   ├─ 9.2 稽核日誌查詢
│   └─ 9.3 稽核日誌詳細檢視
│
├─ 10. 報表模組
│   ├─ 10.1 案件進度報表
│   ├─ 10.2 工時統計報表
│   ├─ 10.3 延遲分析報表
│   └─ 10.4 報表匯出(Excel)
│
└─ 11. 系統設定模組
    ├─ 11.1 一般參數設定
    ├─ 11.2 Email設定
    └─ 11.3 資料保留政策
```

---

### 5.2 使用案例圖 (Use Case Diagram)

```
┌───────────────────────────────────────────
│           RF案件排程系統                   │
│                                           │
│  ┌──────────────────────────────────────  │
│  │  認證與授權                           │ │
│  │  • 登入(Local / AD)                   │ │
│  │  • 忘記密碼                           │ │
│  └──────────────────────────────────────  │
│                                           │
│  ┌──────────────────────────────────────  │
│  │  工程師功能                           │ │
│  │  • 查看我的測項                       │ │
│  │  • 回報工時                           │ │
│  │  • 查詢工時記錄                       │ │
│  │  • 修改工時(7天內)                    │ │
│  │  • 取消測項完成狀態                   │ │
│  └──────────────────────────────────────  │
│             ↑                             │
│         Engineer                          │
│                                           │
│  ┌──────────────────────────────────────  │
│  │  主管功能                            │  │
│  │  • Wizard建案                        │  │
│  │  • 管理案件(列表模式)                 │ **【v3.0】**
│  │  • 管理法規(新增/停用/移除)           │ **【v3.0】**
│  │  • 管理測項與狀態覆寫                 │  │
│  │  • 分配工程師(Main1/2/3+Support)     │ **【v3.0】**
│  │  • 建立補測版本(含 RevisionType)     │ **【v3.0】**
│  │  • 補測版本回滾                      │ **【v3.0】**
│  │  • 查看Loading報表                   │  │
│  │  • 查看延遲分析                      │  │
│  │  • 管理使用者與權限(首次建立Email)    │ **【v3.0】**
│  │  • 管理延遲原因                      │  │
│  │  • 查詢稽核日誌                      │  │
│  │  • 產出各類報表                      │  │
│  │  • 覆寫工時(含理由)                  │  │
│  └──────────────────────────────────────  │
│             ↑                             │
│          Manager                          │
│                                           │
└───────────────────────────────────────────┘
```

---

## 6. 功能需求詳述

### 6.1 認證與授權需求

#### FR-AUTH-001:使用者登入(Local)
- **優先級:** 必須 (Must Have)
- **描述:** 使用者以帳號與密碼登入系統,登入成功後需依 Email 合併為最終身份識別
- **驗收標準:**
  - 驗證 Account 與 PasswordHash 是否正確
  - 取得使用者 Email,並以 Email 為唯一身份識別
  - 若 Email 已存在於其他登入來源(如 AD),則視為同一使用者
  - 停用帳號無法登入
  - 連續失敗 5 次需鎖定帳號 10 分鐘

#### FR-AUTH-101:Windows AD 登入(Email 合併版)
- **優先級:** 應該 (Should Have)
- **描述:** 支援 Windows AD 驗證登入,並以 Email 為唯一身份識別
- **驗收標準:**
  - 系統可取得 ADAccount、DisplayName、Email
  - 若 Email 存在於 User 表,視為同一使用者
  - 若 Email 不存在,建立新的 AD 使用者
  - AD 未提供 Email → 登入失敗
  - 停用帳號無法登入

#### FR-AUTH-102:JWT Token 驗證與 Payload 規範
- **優先級:** 必須 (Must Have)  
- **描述:** 系統使用 JWT 作為身份驗證憑證
- **驗收標準:**
  - JWT 結構:Header + Payload + Signature
  - Payload 只放授權相關必要資訊
  - 使用 HMAC-SHA256 簽章
  - 每次 API 呼叫需驗證 Token
  - Token 過期時間可由 SystemSetting 設定

#### FR-AUTH-002:密碼安全儲存
- **優先級:** 必須 (Must Have)
- **描述:** 密碼需以不可逆方式儲存
- **驗收標準:**
  - 資料庫儲存Hash值
  - 系統無任何功能可查看原密碼
  - Hash演算法使用業界標準(如bcrypt)

#### FR-AUTH-003:忘記密碼功能
- **優先級:** 必須 (Must Have)
- **描述:** 使用者可透過Email重設密碼
- **驗收標準:**
  - 可用帳號或Email申請重設
  - 系統發送重設連結至註冊Email
  - 連結30分鐘內有效
  - 連結僅能使用一次

#### FR-AUTH-004:角色權限控管
- **優先級:** 必須 (Must Have)
- **描述:** 系統需區分Engineer與Manager角色
- **驗收標準:**
  - Engineer僅能存取自己的功能
  - Manager可存取所有功能
  - 無權限時顯示明確錯誤訊息

---

### 6.2 案件管理需求

#### FR-PROJ-001:Wizard建案流程
- **優先級:** 必須 (Must Have)
- **描述:** 提供4步驟引導式建案流程
- **驗收標準:**
  - Step 1完成才能進Step 2
  - 可隨時返回前一步驟修改
  - Step 4完成顯示確認摘要
  - 確認後一次性寫入所有資料

#### FR-PROJ-002:案件基本資料管理
- **優先級:** 必須 (Must Have)
- **描述:** 可新增、查詢、修改、刪除案件
- **驗收標準:**
  - 案件名稱不可重複
  - 日期邏輯正確(結束≥開始)
  - 修改需填寫理由
  - 刪除採Soft Delete

#### FR-PROJ-003:案件狀態自動計算
- **優先級:** 必須 (Must Have)
- **描述:** 案件狀態依 Regulation 狀態自動更新(三層推算:TestItem → Regulation → Project)
- **驗收標準:**
  - 任一 Regulation = Delayed → Project = Delayed
  - 所有 Regulation = Completed → Project = Completed
  - 任一 Regulation = InProgress → Project = Active
  - 所有 Regulation = NotStarted → Project = Draft
  - 主管可手動設為 OnHold
  - 狀態變更需寫入 AuditLog

#### FR-PROJ-004:Regulation 狀態自動計算
- **優先級:** 必須 (Must Have)
- **描述:** Regulation 狀態依 TestItem 狀態自動計算
- **驗收標準:**
  - 任一 TestItem = Delayed → Regulation = Delayed
  - 所有 TestItem = Completed → Regulation = Completed
  - 任一 TestItem = InProgress → Regulation = InProgress
  - 所有 TestItem = NotStarted → Regulation = NotStarted
  - 主管可手動設為 OnHold
  - 狀態變更需寫入 AuditLog

#### FR-PROJ-005:三層狀態推算機制
- **優先級:** 必須 (Must Have)
- **描述:** 系統支援 TestItem → Regulation → Project 三層狀態推算
- **驗收標準:**
  - TestItem 狀態變更時自動觸發 Regulation 重算
  - Regulation 狀態變更時自動觸發 Project 重算
  - 每層狀態計算遵循優先順序規則
  - 手動設定的狀態(ManualStatusOverride)不被自動邏輯覆蓋
  - 所有狀態變更記錄至 AuditLog

#### FR-PROJ-006:案件總覽列表模式 **【v3.0 新增】**
- **優先級:** 必須 (Must Have)
- **描述:** 主管案件總覽改為 GridControl 列表模式,提供更好的排序、篩選功能
- **驗收標準:**
  - 使用 GridControl 呈現案件列表
  - 支援欄位排序(案件名稱、優先順序、狀態、日期)
  - 支援多條件篩選(狀態、優先順序、客戶名稱)
  - 支援快速搜尋功能
  - 每列提供操作按鈕(詳細、編輯、刪除)
  - 顯示案件進度百分比(完成測項數/總測項數)

---

### 6.3 法規管理需求 **【v3.0 新增完整模組】**

#### FR-REG-001:法規列表管理
- **優先級:** 必須 (Must Have)
- **描述:** 提供法規列表查詢與管理介面
- **驗收標準:**
  - 顯示所有法規及其狀態
  - 可依案件篩選法規
  - 顯示法規開始/結束日期
  - 顯示法規下的測項數量
  - 權限:REGULATION_VIEW

#### FR-REG-002:新增法規
- **優先級:** 必須 (Must Have)
- **描述:** 主管可在已存在的案件中新增法規
- **驗收標準:**
  - 選擇法規類型(FCC/NCC/CE/IC/TELEC等)
  - 填寫法規名稱(如 Part 24、PLMN)
  - 設定法規測試日期
  - 日期需在案件日期±30天內(可警告但允許繼續)
  - 新增後法規狀態為 NotStarted
  - 權限:REGULATION_ADD

#### FR-REG-003:停用法規
- **優先級:** 必須 (Must Have)
- **描述:** 主管可停用不需要的法規
- **驗收標準:**
  - 停用前檢查是否有測項存在
  - 若有測項需顯示警告並確認
  - 停用後該法規不參與案件狀態計算
  - 停用記錄寫入 AuditLog
  - 權限:REGULATION_DISABLE

#### FR-REG-004:移除法規
- **優先級:** 必須 (Must Have)
- **描述:** 主管可移除法規(Soft Delete)
- **驗收標準:**
  - 移除前檢查是否有測項或工時記錄
  - 若有記錄顯示影響範圍並確認
  - 採用 Soft Delete(IsDeleted=true)
  - 移除後該法規及其測項不顯示在列表中
  - 歷史資料仍可在稽核日誌中查詢
  - 權限:REGULATION_REMOVE

#### FR-REG-005:法規狀態自動計算邏輯
- **優先級:** 必須 (Must Have)
- **描述:** Regulation 狀態依測項狀態自動計算
- **計算邏輯:**
```
IF 主管手動設定 OnHold → Regulation.Status = OnHold(最高優先級)
ELSE IF 任一 TestItem.Status = Delayed → Regulation.Status = Delayed
ELSE IF 所有 TestItem.Status = Completed → Regulation.Status = Completed
ELSE IF 任一 TestItem.Status = InProgress → Regulation.Status = InProgress
ELSE → Regulation.Status = NotStarted(初始狀態)
```
- **驗收標準:**
  - 測項狀態變更時自動重算法規狀態
  - 主管手動設定的 OnHold 需標記(ManualStatusOverride=true)
  - 手動狀態不被自動邏輯覆蓋
  - 狀態變更寫入 AuditLog

---

### 6.4 測試項目管理需求 **【v3.0 擴充】**

#### FR-TEST-001:測項列表管理 **【v3.0 新增】**
- **優先級:** 必須 (Must Have)
- **描述:** 提供測項列表查詢介面
- **驗收標準:**
  - 可依法規篩選測項
  - 顯示測項名稱、類型、場地、狀態
  - 顯示負責工程師(Main1/Main2/Main3)
  - 顯示預估工時 vs 實際工時
  - 提供快速搜尋功能
  - 權限:TESTITEM_VIEW

#### FR-TEST-002:測項新增與修改
- **優先級:** 必須 (Must Have)
- **描述:** 可在法規下新增測項
- **驗收標準:**
  - 預估工時需>0
  - 需指定測試類型與場地
  - 權限:TESTITEM_CREATE、TESTITEM_UPDATE

#### FR-TEST-003:工程師分配(細化版) **【v3.0 重大更新】**
- **優先級:** 必須 (Must Have)
- **描述:** 可將測項分配給工程師,支援多主責與支援工程師
- **RoleType 定義:**
  - **Main1(主要負責人):** 必須指派,該測項的第一負責人,具有完整決策權
  - **Main2(次要負責人):** 選填,當 Main1 不在時可代理決策
  - **Main3(第三負責人):** 選填,多人分工時使用
  - **Support(支援人員):** 選填,協助測試但不負責決策
- **驗收標準:**
  - 至少需1位 Main1 工程師
  - Main2、Main3、Support 為選填
  - 同一測項不可重複分配同一工程師
  - 分配工時總和與預估工時差異>20%需警告
  - 顯示工程師當前Loading
  - 每個角色都需分配對應工時
  - 權限:
    - TESTITEM_ASSIGN_ENGINEER(分配主責工程師)
    - TESTITEM_ASSIGN_SUPPORT(分配支援工程師)
    - TESTITEM_REMOVE_ENGINEER(移除工程師)

#### FR-TEST-004:補測版本管理(完整定義) **【v3.0 完整定義】**
- **優先級:** 必須 (Must Have)
- **描述:** 可建立測項的補測版本,記錄完整補測資訊
- **TestItemRevision 欄位定義:**
  - **RevisionId (PK):** 補測版本唯一識別碼
  - **TestItemId (FK):** 對應的測項
  - **RevisionNumber (INT):** 版本號(1, 2, 3...,系統自動遞增)
  - **RevisionType (NVARCHAR(20)):** 補測類型,可選值:
    - `Command`: 客戶指令(預設值)
    - `Retest`: 測試不通過需重測
    - `Fix`: 修正問題後重測
    - `Others`: 其他原因
  - **EstimatedHours (DECIMAL(10,2)):** 主管預估此次補測所需工時
  - **Reason (NVARCHAR(200)):** 補測原因(Command 內容摘要)
  - **Description (NVARCHAR(500)):** 詳細說明(可選)
  - **CreatedByUserId / CreatedDate:** 建立者與時間
  - **ModifiedByUserId / ModifiedDate:** 修改者與時間
  - **IsDeleted / DeletedByUserId / DeletedDate:** Soft Delete 欄位
- **驗收標準:**
  - 版本編號不可重複(v2/v3/v4...)
  - 可選擇複製原工程師分配
  - 建立補測後測項狀態改為InProgress
  - 需填寫補測原因與選擇 RevisionType
  - 支援 Soft Delete(刪除後仍保留關聯的 WorkLog 查詢)
  - 權限:TESTITEM_REVISION_CREATE

#### FR-TEST-005:補測版本回滾功能 **【v3.0 新增】**
- **優先級:** 應該 (Should Have)
- **描述:** 主管可將測項回滾至先前的補測版本
- **驗收標準:**
  - 僅能回滾至已完成的版本
  - 回滾時需填寫原因
  - 回滾後:
    - 建立新的 Revision 記錄
    - 複製目標版本的工程師分配
    - 測項狀態設為 InProgress
  - 回滾操作寫入 AuditLog
  - 權限:TESTITEM_REVISION_ROLLBACK

#### FR-TEST-006:測項狀態計算邏輯(更新)
- **優先級:** 必須 (Must Have)
- **描述:** TestItem 狀態依優先順序自動計算,並支援手動覆寫
- **驗收標準:**
  - **狀態計算優先順序:**
    1. IF 主管手動設定 OnHold → **OnHold**(最高優先級)
    2. ELSE IF 剛建立 TestItemRevision → **InProgress**(補測事件覆蓋舊狀態)
    3. ELSE IF WorkLog 中存在 Delayed 狀態 → **Delayed**
    4. ELSE IF 任一工程師按「Complete TestItem」→ **Completed**
    5. ELSE IF WorkLog 中存在 InProgress 狀態 → **InProgress**
    6. ELSE → **NotStarted**(初始狀態)
  - 系統需記錄狀態變更歷程至 AuditLog
  - 主管手動設定的 OnHold 狀態需額外標記,避免被自動邏輯覆蓋

#### FR-TEST-007:測項狀態逆向操作(更新)
- **優先級:** 必須 (Must Have)
- **描述:** TestItem 狀態允許逆向修改,非單向不可逆
- **工程師權限:**
  - 可取消自己誤按的 Completed 狀態
  - 可將 Completed 改回 InProgress
  - 需提供「取消完成」按鈕
  - 取消操作需寫入 AuditLog
- **主管權限:**
  - 可覆寫 TestItem 的任何狀態
  - 可修改:Completed → InProgress / Delayed / OnHold
  - 可修改:Delayed → InProgress / Completed
  - 可修改:OnHold → 任何狀態
  - 修改需填寫理由,記錄至 AuditLog
- **自動狀態亦可覆寫:**
  - 建立 TestItemRevision 自動產生的 InProgress 可由主管手動改為其他狀態
- 所有狀態變更皆需記錄:Who / When / From / To / Why

---

### 6.5 工時管理需求

#### FR-WORK-001:工時回報
- **優先級:** 必須 (Must Have)
- **描述:** 工程師可回報每日工時
- **驗收標準:**
  - 工作日期不可晚於今天
  - 單日工時0.5-12小時
  - 同測項同日期不可重複回報
  - 狀態=延遲時必須選延遲原因
  - 需選擇對應的補測版本(RevisionId) **【v3.0 強化】**

#### FR-WORK-002:工時修改限制
- **優先級:** 必須 (Must Have)
- **描述:** 工程師可修改7天內的工時
- **驗收標準:**
  - 7天內可修改
  - 超過7天顯示錯誤訊息
  - 修改後寫入AuditLog

#### FR-WORK-003:工時狀態同步
- **優先級:** 必須 (Must Have)
- **描述:** 工時回報後自動更新測項狀態
- **驗收標準:**
  - 首次回報→測項InProgress
  - 任一工程師回報延遲→測項Delayed
  - 工程師按完成→觸發測項狀態重算

#### FR-WORK-004:工時查詢
- **優先級:** 必須 (Must Have)
- **描述:** 工程師可查詢自己的工時記錄
- **驗收標準:**
  - 可依測項篩選
  - 可依日期區間篩選
  - 可依版本篩選
  - 顯示統計資訊(總工時、剩餘工時)

#### FR-WORK-005:主管覆寫工時
- **優先級:** 應該 (Should Have)
- **描述:** 主管可修改任何工時記錄
- **驗收標準:**
  - 需填寫修改理由
  - 理由寫入AuditLog
  - 原值與新值皆記錄

#### FR-WORK-006:工時刪除(Soft Delete) **【v3.0 新增】**
- **優先級:** 應該 (Should Have)
- **描述:** 主管可刪除工時記錄(採 Soft Delete)
- **驗收標準:**
  - 僅 Manager 具有 WORKLOG_DELETE 權限
  - 一般工程師無法刪除工時,僅能修改
  - 刪除操作需填寫刪除理由
  - 設定 IsDeleted = true,記錄 DeletedByUserId 與 DeletedDate
  - 已刪除的 WorkLog 不出現在一般查詢中
  - 已刪除記錄仍保留於稽核追蹤
  - Manager 可透過特殊查詢介面查看已刪除的工時記錄

---

### 6.6 Loading分析需求

#### FR-LOAD-001:Loading計算
- **優先級:** 必須 (Must Have)
- **描述:** 系統自動計算工程師Loading
- **驗收標準:**
  - Assigned Loading = Σ(分配工時) / 可用工時 × 100%
  - Actual Loading = Σ(實際工時) / 可用工時 × 100%
  - 僅計算Active狀態案件

#### FR-LOAD-002:Loading視覺化
- **優先級:** 必須 (Must Have)
- **描述:** 以顏色標示Loading狀態
- **驗收標準:**
  - ≤60%:綠色
  - 61-80%:黃色
  - 81-100%:橘色
  - 100%以上:紅色(超載)

#### FR-LOAD-003:Loading明細
- **優先級:** 必須 (Must Have)
- **描述:** 可查看工程師Loading明細
- **驗收標準:**
  - 顯示負責的所有測項
  - 顯示各測項分配/實際/剩餘工時
  - 提供工時趨勢圖表

#### FR-LOAD-004:超載預警
- **優先級:** 應該 (Should Have)
- **描述:** 分配工作時若導致超載需警告
- **驗收標準:**
  - 分配時即時計算新Loading
  - >100%顯示警告對話框
  - 允許繼續或取消

---

### 6.7 延遲管理需求

#### FR-DELAY-001:延遲原因字典
- **優先級:** 必須 (Must Have)
- **描述:** 主管可維護延遲原因清單
- **驗收標準:**
  - 可新增延遲原因(含類型:設備/客戶/工程師/場地)
  - 可修改延遲原因文字
  - 可停用/啟用
  - 已使用的不可刪除(僅能停用)

#### FR-DELAY-002:延遲原因記錄
- **優先級:** 必須 (Must Have)
- **描述:** 工時狀態=延遲時需選原因
- **驗收標準:**
  - 可多選延遲原因
  - 至少需選1個
  - 未選擇無法送出

#### FR-DELAY-003:延遲分析報表
- **優先級:** 應該 (Should Have)
- **描述:** 提供延遲原因統計分析
- **驗收標準:**
  - 顯示延遲原因分布(圓餅圖)
  - 顯示延遲測項清單
  - 可依期間/案件篩選
  - 可匯出Excel

---

### 6.8 使用者管理需求 **【v3.0 強化】**

#### FR-USER-001:使用者新增(強化 Email 唯一性)
- **優先級:** 必須 (Must Have)
- **描述:** 管理者可新增使用者,Email 需作為唯一身份識別,**首次建立必須由主管執行**
- **驗收標準:**
  - Account 唯一(僅適用 Local 帳號)
  - Email 必須唯一,兩筆使用者不得使用相同 Email
  - 若 Email 已存在(不論 Local 或 AD)→ 禁止新增
  - 系統自動產生初始密碼(僅 Local)
  - 自動發送 Email 給新使用者
  - **使用者不可自行註冊,必須由主管透過「使用者管理」介面建立**
  - 新增時系統檢查 Email 唯一性(不區分大小寫)
  - Email 儲存前統一轉為小寫

#### FR-USER-002:使用者停用
- **優先級:** 必須 (Must Have)
- **描述:** 主管可停用使用者
- **驗收標準:**
  - 停用前檢查是否負責未完成測項
  - 若有負責測項顯示警告
  - 停用後無法登入
  - 歷史資料保留

#### FR-USER-003:使用者資料修改
- **優先級:** 必須 (Must Have)
- **描述:** 主管可修改使用者資料
- **驗收標準:**
  - 可修改顯示名稱、Email、角色
  - 帳號不可修改
  - 每週可用工時可調整
  - **Email 修改時需檢查唯一性**

#### FR-USER-101:權限與群組管理介面(IAM)
- **優先級:** 應該 (Should Have)  
- **描述:** 具備管理權限的使用者可透過介面管理使用者的角色、權限群組與個別權限
- **驗收標準:**
  - 可將使用者指派至一個或多個權限群組
  - 可為特定使用者額外授予或撤銷個別 Permission
  - 可設定臨時權限的到期日,過期後自動失效
  - 可查詢某一位使用者目前的「有效權限」來源
  - 所有權限變更皆須寫入 AuditLog

---

### 6.9 稽核日誌需求

#### FR-AUDIT-001:自動記錄異動
- **優先級:** 必須 (Must Have)
- **描述:** 系統自動記錄重要資料異動
- **驗收標準:**
  - 記錄資料表名稱
  - 記錄記錄ID
  - 記錄操作類型(Create/Update/Delete)
  - 記錄操作人
  - 記錄操作時間
  - 記錄變更前後值(JSON格式)

#### FR-AUDIT-002:稽核日誌查詢
- **優先級:** 必須 (Must Have)
- **描述:** 主管可查詢稽核日誌
- **驗收標準:**
  - 可依資料表篩選
  - 可依操作類型篩選
  - 可依操作人篩選
  - 可依時間區間篩選
  - 可匯出Excel

#### FR-AUDIT-003:變更比對檢視
- **優先級:** 應該 (Should Have)
- **描述:** 可檢視資料變更前後差異
- **驗收標準:**
  - 雙欄顯示Before/After
  - 變更欄位以顏色標示
  - 支援JSON格式解析

---

### 6.10 報表需求

#### FR-REPORT-001:案件進度報表
- **優先級:** 必須 (Must Have)
- **描述:** 顯示案件整體進度
- **驗收標準:**
  - 顯示案件基本資訊
  - 顯示法規完成度(長條圖)
  - 顯示測項進度明細
  - 顯示工時統計(預估vs實際)
  - 支援Excel匯出

#### FR-REPORT-002:工時統計報表
- **優先級:** 必須 (Must Have)
- **描述:** 提供多維度工時統計
- **驗收標準:**
  - 可依案件統計
  - 可依工程師統計
  - 可依期間統計
  - 提供圖表視覺化
  - 支援Excel匯出

#### FR-REPORT-003:延遲分析報表
- **優先級:** 應該 (Should Have)
- **描述:** 分析延遲原因與趨勢
- **驗收標準:**
  - 延遲原因分布圓餅圖
  - 延遲測項清單
  - 平均延遲天數統計
  - 可依期間篩選

---

## 7. 非功能需求

### 7.1 效能需求 (Performance)

#### NFR-PERF-001:回應時間
- **需求:** 一般查詢操作回應時間<2秒
- **測試條件:** 50筆資料以內
- **優先級:** 必須 (Must Have)

#### NFR-PERF-002:工時查詢效能
- **需求:** 單一案件工時查詢<3秒
- **測試條件:** 500筆WorkLog記錄
- **優先級:** 必須 (Must Have)

#### NFR-PERF-003:Loading計算效能
- **需求:** Loading計算<2秒
- **測試條件:** 15位工程師、30個Active案件
- **優先級:** 必須 (Must Have)

#### NFR-PERF-004:報表產出效能
- **需求:** 報表產出<5秒
- **測試條件:** 單一案件、100個測項
- **優先級:** 應該 (Should Have)

---

### 7.2 安全需求 (Security)

#### NFR-SEC-001:密碼安全
- **需求:** 密碼採Hash不可逆儲存
- **標準:** 使用bcrypt或類似演算法
- **優先級:** 必須 (Must Have)

#### NFR-SEC-002:身份驗證
- **需求:** 所有API需驗證身份
- **標準:** 使用JWT Token機制
- **優先級:** 必須 (Must Have)

#### NFR-SEC-003:權限控管
- **需求:** 依角色控管功能存取
- **標準:** Engineer不能存取Manager功能
- **優先級:** 必須 (Must Have)

#### NFR-SEC-004:資料隔離
- **需求:** 工程師只能看自己的資料
- **標準:** 查詢需加入UserId過濾
- **優先級:** 必須 (Must Have)

#### NFR-SEC-005:密碼複雜度
- **需求:** 密碼需符合複雜度規則
- **標準:** 8-20字元、含英文數字
- **優先級:** 必須 (Must Have)

#### NFR-SEC-006:登入失敗鎖定
- **需求:** 連續失敗5次後鎖定
- **標準:** IP或帳號層級鎖定
- **優先級:** 應該 (Should Have)

#### NFR-SEC-007:JWT 結構與簽章安全
- **需求:** JWT Token 必須使用對稱簽章演算法 HMAC-SHA256(HS256)
- **說明:**
  - Header.alg = "HS256",Header.typ = "JWT"
  - Signature = HMAC-SHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), SecretKey)
  - SecretKey 僅存放於伺服器端設定檔
- **優先級:** 必須 (Must Have)

#### NFR-SEC-008:JWT Payload 資料最小化
- **需求:** JWT Payload 只保留授權必須的資訊
- **說明:**
  - Payload 建議欄位:UserId、顯示名稱、角色、權限清單、過期時間
  - 不得在 Payload 放入:密碼、工時詳細資料、Email 內文等敏感資訊
- **優先級:** 必須 (Must Have)

---

### 7.3 可用性需求 (Usability)

#### NFR-USAB-001:學習曲線
- **需求:** 新使用者30分鐘內可完成基本操作
- **衡量:** 使用者測試達成率>80%
- **優先級:** 應該 (Should Have)

#### NFR-USAB-002:錯誤訊息
- **需求:** 錯誤訊息清楚明確
- **標準:** 告知原因與解決方式
- **優先級:** 必須 (Must Have)

#### NFR-USAB-003:操作直覺性
- **需求:** 不需查閱手冊可完成90%操作
- **衡量:** 使用者測試回饋
- **優先級:** 應該 (Should Have)

#### NFR-USAB-004:回饋即時性
- **需求:** 操作後立即顯示結果
- **標準:** 載入中顯示進度指示
- **優先級:** 必須 (Must Have)

---

### 7.4 可靠性需求 (Reliability)

#### NFR-RELI-001:可用性
- **需求:** 系統可用性>99%
- **測試條件:** 每月統計
- **優先級:** 應該 (Should Have)

#### NFR-RELI-002:資料完整性
- **需求:** 資料異動需Transaction保護
- **標準:** 關鍵操作全成功或全失敗
- **優先級:** 必須 (Must Have)

#### NFR-RELI-003:資料備份
- **需求:** 每日自動備份
- **標準:** 保留30天備份
- **優先級:** 必須 (Must Have)

#### NFR-RELI-004:錯誤處理
- **需求:** 系統錯誤不導致資料遺失
- **標準:** 異常回滾、記錄日誌
- **優先級:** 必須 (Must Have)

---

### 7.5 可維護性需求 (Maintainability)

#### NFR-MAIN-001:日誌記錄
- **需求:** 記錄系統運作日誌
- **標準:** 包含錯誤、警告、資訊層級
- **優先級:** 必須 (Must Have)

#### NFR-MAIN-002:版本控制
- **需求:** 程式碼使用版本控管
- **標準:** Git或類似工具
- **優先級:** 必須 (Must Have)

---

### 7.6 擴充性需求 (Scalability)

#### NFR-SCAL-001:使用者擴充
- **需求:** 支援至少50位同時上線使用者
- **測試條件:** 負載測試驗證
- **優先級:** 應該 (Should Have)

#### NFR-SCAL-002:資料量擴充
- **需求:** 支援至少5年歷史資料
- **估算:** 約50,000筆WorkLog記錄
- **優先級:** 應該 (Should Have)

#### NFR-SCAL-003:查詢效能維持
- **需求:** 資料量增長不影響查詢效能
- **標準:** 使用索引與分頁機制
- **優先級:** 必須 (Must Have)

---

### 7.7 相容性需求 (Compatibility)

#### NFR-COMP-001:瀏覽器支援
- **需求:** 支援主流瀏覽器
- **標準:** Chrome、Edge、Firefox 最新版本
- **優先級:** 必須 (Must Have)

#### NFR-COMP-002:螢幕解析度
- **需求:** 支援常見解析度
- **標準:** 1920x1080、1366x768
- **優先級:** 必須 (Must Have)

#### NFR-COMP-003:資料匯出格式
- **需求:** 匯出Excel可被Office開啟
- **標準:** .xlsx格式
- **優先級:** 必須 (Must Have)

---

## 8. 資料模型

### 8.1 核心實體關係 (Entity Relationship)

#### 8.1.1 完整 ERD 圖 **【v2.2 補充完整圖】**

```
┌──────────────────────┐
│   Users              │
├──────────────────────┤
│ UserId (PK)          │
│ Account              │◄────────────┐
│ ADAccount            │             │
│ Email (UNIQUE) ★     │             │
│ PasswordHash         │             │
│ DisplayName          │             │
│ Role                 │             │
│ AuthType             │             │
│ IsActive             │             │
│ WeeklyWorkHours      │             │
│ LastLoginDate        │             │
│ FailedLoginAttempts  │             │
│ LockoutEndTime       │             │
│ CreatedDate          │             │
│ ModifiedDate         │             │
└──────────────────────┘             │
         │ 1                         │
         │                           │
         │                           │
         │ N                         │
┌──────────────────────┐             │
│   Projects           │             │
├──────────────────────┤             │
│ ProjectId (PK)       │             │
│ ProjectName (UNIQUE) │             │
│ CustomerName         │             │
│ Priority             │             │
│ StartDate            │             │
│ EndDate              │             │
│ Status               │             │
│ ManualStatusOverride │             │
│ Description          │             │
│ IsDeleted            │             │
│ CreatedByUserId (FK) ├─────────────┘
│ CreatedDate          │
│ ModifiedByUserId (FK)│
│ ModifiedDate         │
│ DeletedByUserId (FK) │
│ DeletedDate          │
└──────────────────────┘
         │ 1
         │
         │
         │ N
┌──────────────────────┐
│   Regulations        │
├──────────────────────┤
│ RegulationId (PK)    │
│ ProjectId (FK)       ├───────┐
│ RegulationType       │       │
│ RegulationName       │       │
│ StartDate            │       │
│ EndDate              │       │
│ Status               │       │
│ ManualStatusOverride │       │
│ IsDeleted            │       │
│ CreatedByUserId (FK) │       │
│ CreatedDate          │       │
│ ModifiedByUserId (FK)│       │
│ ModifiedDate         │       │
│ DeletedByUserId (FK) │       │
│ DeletedDate          │       │
└──────────────────────┘       │
         │ 1                   │
         │                     │
         │                     │
         │ N                   │
┌──────────────────────┐       │
│   TestItems          │       │
├──────────────────────┤       │
│ TestItemId (PK)      │       │
│ RegulationId (FK)    ├───────┘
│ TestItemName         │
│ TestType             │
│ Location             │
│ EstimatedHours       │
│ Status               │
│ ManualStatusOverride │
│ IsDeleted            │
│ CreatedByUserId (FK) │
│ CreatedDate          │
│ ModifiedByUserId (FK)│
│ ModifiedDate         │
│ DeletedByUserId (FK) │
│ DeletedDate          │
└──────────────────────┘
         │ 1                    
         │                      
         ├────────────────────┐
         │ N                  │ N
┌──────────────────────┐  ┌──────────────────────┐
│ TestItemEngineer     │  │ TestItemRevision     │
├──────────────────────┤  ├──────────────────────┤
│ TestItemEngineerId(PK│  │ RevisionId (PK)      │
│ TestItemId (FK)      │  │ TestItemId (FK)      ├───┐
│ UserId (FK)          │  │ RevisionNumber       │   │
│ RoleType ★           │  │ RevisionType ★       │   │
│ AssignedHours        │  │ EstimatedHours       │   │
│ CreatedByUserId (FK) │  │ Reason               │   │
│ CreatedDate          │  │ Description          │   │
│ ModifiedByUserId (FK)│  │ IsDeleted ★          │   │
│ ModifiedDate         │  │ CreatedByUserId (FK) │   │
└──────────────────────┘  │ CreatedDate          │   │
                          │ ModifiedByUserId (FK)│   │
                          │ ModifiedDate         │   │
                          │ DeletedByUserId (FK) │   │
                          │ DeletedDate          │   │
                          └──────────────────────┘   │
                                   │ 1               │
                                   │                 │
                                   │                 │
                                   │ N               │
┌──────────────────────┐           │                 │
│   WorkLogs           │           │                 │
├──────────────────────┤           │                 │
│ WorkLogId (PK)       │           │                 │
│ TestItemId (FK)      ├───────────┘                 │
│ UserId (FK)          │                             │
│ RevisionId (FK) ★    ├─────────────────────────────┘
│ WorkDate             │
│ Hours                │
│ Status               │
│ Note                 │
│ IsDeleted ★          │
│ CreatedDate          │
│ ModifiedByUserId (FK)│
│ ModifiedDate         │
│ DeletedByUserId (FK) │
│ DeletedDate          │
└──────────────────────┘
         │ N
         │
         │
         │ N
┌──────────────────────┐
│ WorkLogDelayReason   │
├──────────────────────┤
│ WorkLogDelayReasonId │
│ WorkLogId (FK)       │
│ DelayReasonId (FK)   ├───┐
│ CreatedDate          │   │
└──────────────────────┘   │
                           │
                           │ N
┌──────────────────────┐   │
│   DelayReasons       │   │
├──────────────────────┤   │
│ DelayReasonId (PK)   │◄──┘
│ ReasonText           │
│ ReasonType           │
│ IsActive ★           │
│ CreatedDate          │
│ ModifiedDate         │
└──────────────────────┘

┌──────────────────────┐
│   AuditLogs          │
├──────────────────────┤
│ AuditLogId (PK)      │
│ TableName            │
│ RecordId             │
│ Operation            │
│ UserId (FK)          │
│ ChangedDate          │
│ OldValues (JSON)     │
│ NewValues (JSON)     │
└──────────────────────┘

┌──────────────────────┐
│ PasswordResetTokens  │
├──────────────────────┤
│ TokenId (PK)         │
│ UserId (FK)          │
│ Token (UNIQUE)       │
│ ExpiryDate           │
│ IsUsed               │
│ CreatedDate          │
└──────────────────────┘

┌──────────────────────┐
│   SystemSettings     │
├──────────────────────┤
│ SettingId (PK)       │
│ SettingKey (UNIQUE)  │
│ SettingValue         │
│ Description          │
│ ModifiedByUserId (FK)│
│ ModifiedDate         │
└──────────────────────┘

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
IAM 權限模型 (v2.1 補充)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

┌──────────────────────┐
│   Permissions        │
├──────────────────────┤
│ PermissionId (PK)    │
│ PermissionCode       │◄────────────┐
│ PermissionName       │             │
│ Description          │             │
│ Category             │             │
│ IsActive             │             │
└──────────────────────┘             │
         │ N                         │
         │                           │
         │                           │
         │ N                         │ N
┌──────────────────────────────┐    │
│ PermissionGroupMapping       │    │
├──────────────────────────────┤    │
│ MappingId (PK)               │    │
│ PermissionGroupId (FK) ──────┼────┤
│ PermissionId (FK) ───────────┼────┘
│ CreatedDate                  │
└──────────────────────────────┘
         │ N
         │
         │
         │ 1
┌──────────────────────┐
│ PermissionGroups     │
├──────────────────────┤
│ PermissionGroupId(PK)│
│ GroupName            │
│ Description          │
│ IsActive             │
│ CreatedDate          │
│ ModifiedDate         │
└──────────────────────┘
         │ N
         │
         │
         │ N
┌──────────────────────────────┐
│ UserPermissions              │
├──────────────────────────────┤
│ UserPermissionId (PK)        │
│ UserId (FK)                  │
│ PermissionId (FK) (Nullable) │
│ PermissionGroupId (FK) (N... │
│ GrantType                    │
│ ExpiryDate (Nullable)        │
│ GrantedByUserId (FK)         │
│ GrantedDate                  │
│ RevokedDate (Nullable)       │
└──────────────────────────────┘

圖例:
★ = v2.2 以後補充或更新的欄位
```

**重點說明:**
1. **三層狀態推算:** TestItem → Regulation → Project
2. **Soft Delete 機制:** 多張表支援 IsDeleted + DeletedByUserId + DeletedDate
3. **補測版本管理:** TestItemRevision 完整定義 **【v3.0】**
4. **工時記錄:** WorkLog 關聯到 RevisionId
5. **延遲原因:** 多對多關係(WorkLog ←→ DelayReason)
6. **IAM 權限:** Permission / PermissionGroup / UserPermission

---

### 8.2 核心實體定義

#### 8.2.1 Users (使用者)

**用途:** 儲存系統使用者資訊

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| UserId | INT | ✓ | 主鍵,自動遞增 |
| Account | NVARCHAR(50) | △ | 本地登入帳號(Local 使用者必填) |
| ADAccount | NVARCHAR(100) | △ | AD 帳號(AD 使用者必填) |
| Email | NVARCHAR(100) | ✓ | Email(唯一識別,UNIQUE) **【v2.1】** |
| PasswordHash | NVARCHAR(255) | △ | 密碼雜湊值(僅 Local 使用者) |
| DisplayName | NVARCHAR(50) | ✓ | 顯示名稱 |
| Role | NVARCHAR(20) | ✓ | 角色(Engineer/Manager/Admin) |
| AuthType | NVARCHAR(10) | ✓ | 認證類型(Local/AD) **【v2.1】** |
| IsActive | BIT | ✓ | 是否啟用 |
| WeeklyWorkHours | DECIMAL(5,2) | ✓ | 每週可用工時 |
| LastLoginDate | DATETIME | ✗ | 最後登入時間 |
| FailedLoginAttempts | INT | ✓ | 登入失敗次數 |
| LockoutEndTime | DATETIME | ✗ | 鎖定結束時間 |
| CreatedDate | DATETIME | ✓ | 建立時間 |
| ModifiedDate | DATETIME | ✓ | 修改時間 |

**索引:**
- PK: UserId
- UNIQUE: Email **【v2.1 強制】**
- UNIQUE: Account (WHERE Account IS NOT NULL)
- UNIQUE: ADAccount (WHERE ADAccount IS NOT NULL)
- INDEX: Role

**約束:**
- Email 不可為 NULL
- Email 不區分大小寫(儲存前轉小寫)
- AuthType = 'Local' → Account 與 PasswordHash 必填
- AuthType = 'AD' → ADAccount 必填
- WeeklyWorkHours 預設值 = 40.0

---

#### 8.2.2 Projects (案件)

**用途:** 儲存測試案件基本資訊

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| ProjectId | INT | ✓ | 主鍵,自動遞增 |
| ProjectName | NVARCHAR(100) | ✓ | 案件名稱(唯一) |
| CustomerName | NVARCHAR(100) | ✓ | 客戶名稱 |
| Priority | NVARCHAR(20) | ✓ | 優先順序(High/Medium/Low) |
| StartDate | DATE | ✓ | 預計開始日期 |
| EndDate | DATE | ✓ | 預計結束日期 |
| Status | NVARCHAR(20) | ✓ | 狀態(Draft/Active/Completed/OnHold/Delayed) |
| ManualStatusOverride | BIT | ✓ | 是否手動覆寫狀態 **【v2.2】** |
| Description | NVARCHAR(500) | ✗ | 案件說明 |
| IsDeleted | BIT | ✓ | 是否刪除(Soft Delete) |
| CreatedByUserId | INT | ✓ | 建立者(FK → Users) |
| CreatedDate | DATETIME | ✓ | 建立時間 |
| ModifiedByUserId | INT | ✗ | 修改者(FK → Users) |
| ModifiedDate | DATETIME | ✗ | 修改時間 |
| DeletedByUserId | INT | ✗ | 刪除者(FK → Users) |
| DeletedDate | DATETIME | ✗ | 刪除時間 |

**索引:**
- PK: ProjectId
- UNIQUE: ProjectName (WHERE IsDeleted = 0)
- INDEX: Status, Priority
- INDEX: IsDeleted

**約束:**
- EndDate >= StartDate
- Status 初始值 = 'Draft'
- ManualStatusOverride 預設值 = 0

---

#### 8.2.3 Regulations (法規)

**用途:** 儲存案件需測試的法規資訊

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| RegulationId | INT | ✓ | 主鍵,自動遞增 |
| ProjectId | INT | ✓ | 案件ID(FK → Projects) |
| RegulationType | NVARCHAR(20) | ✓ | 法規類型(FCC/NCC/CE/IC/TELEC等) |
| RegulationName | NVARCHAR(50) | ✓ | 法規名稱(如 Part 24) |
| StartDate | DATE | ✓ | 法規測試開始日期 |
| EndDate | DATE | ✓ | 法規測試結束日期 |
| Status | NVARCHAR(20) | ✓ | 狀態(NotStarted/InProgress/Completed/Delayed/OnHold) **【v2.2】** |
| ManualStatusOverride | BIT | ✓ | 是否手動覆寫狀態 **【v2.2】** |
| IsDeleted | BIT | ✓ | 是否刪除(Soft Delete) |
| CreatedByUserId | INT | ✓ | 建立者(FK → Users) |
| CreatedDate | DATETIME | ✓ | 建立時間 |
| ModifiedByUserId | INT | ✗ | 修改者(FK → Users) |
| ModifiedDate | DATETIME | ✗ | 修改時間 |
| DeletedByUserId | INT | ✗ | 刪除者(FK → Users) |
| DeletedDate | DATETIME | ✗ | 刪除時間 |

**索引:**
- PK: RegulationId
- INDEX: ProjectId
- INDEX: Status

**約束:**
- EndDate >= StartDate
- Status 初始值 = 'NotStarted'

---

#### 8.2.4 TestItems (測試項目)

**用途:** 儲存具體測試項目

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| TestItemId | INT | ✓ | 主鍵,自動遞增 |
| RegulationId | INT | ✓ | 法規ID(FK → Regulations) |
| TestItemName | NVARCHAR(100) | ✓ | 測項名稱 |
| TestType | NVARCHAR(50) | ✓ | 測試類型(Conducted/Radiated/Blocking等) |
| Location | NVARCHAR(50) | ✓ | 測試場地(Lab A/Lab B/Lab C等) |
| EstimatedHours | DECIMAL(10,2) | ✓ | 預估總工時 |
| Status | NVARCHAR(20) | ✓ | 狀態(NotStarted/InProgress/Completed/Delayed/OnHold) **【v2.2】** |
| ManualStatusOverride | BIT | ✓ | 是否手動覆寫狀態 **【v2.2】** |
| IsDeleted | BIT | ✓ | 是否刪除(Soft Delete) |
| CreatedByUserId | INT | ✓ | 建立者(FK → Users) |
| CreatedDate | DATETIME | ✓ | 建立時間 |
| ModifiedByUserId | INT | ✗ | 修改者(FK → Users) |
| ModifiedDate | DATETIME | ✗ | 修改時間 |
| DeletedByUserId | INT | ✗ | 刪除者(FK → Users) |
| DeletedDate | DATETIME | ✗ | 刪除時間 |

**索引:**
- PK: TestItemId
- INDEX: RegulationId
- INDEX: Status

**約束:**
- EstimatedHours > 0
- Status 初始值 = 'NotStarted'

---

#### 8.2.5 TestItemEngineer (測項工程師分配) **【v3.0 更新】**

**用途:** 記錄測項分配給哪些工程師,支援多主責與支援人員

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| TestItemEngineerId | INT | ✓ | 主鍵,自動遞增 |
| TestItemId | INT | ✓ | 測項ID(FK → TestItems) |
| UserId | INT | ✓ | 工程師ID(FK → Users) |
| RoleType | NVARCHAR(20) | ✓ | 角色類型 **【v3.0 新增】** |
| AssignedHours | DECIMAL(10,2) | ✓ | 分配工時 |
| CreatedByUserId | INT | ✓ | 分配者(FK → Users) |
| CreatedDate | DATETIME | ✓ | 分配時間 |
| ModifiedByUserId | INT | ✗ | 修改者(FK → Users) |
| ModifiedDate | DATETIME | ✗ | 修改時間 |

**RoleType 定義 【v3.0】:**
- **Main1:** 主要負責人(必須指派)
- **Main2:** 次要負責人(選填)
- **Main3:** 第三負責人(選填)
- **Support:** 支援人員(選填)

**索引:**
- PK: TestItemEngineerId
- UNIQUE: (TestItemId, UserId) - 同一測項不可重複分配同一工程師
- INDEX: TestItemId
- INDEX: UserId

**約束:**
- AssignedHours > 0
- 每個 TestItem 至少需1位 RoleType = 'Main1' 的工程師

---

#### 8.2.6 TestItemRevision (補測版本) **【v3.0 完整定義】**

**用途:** 記錄測項的補測版本資訊

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| RevisionId | INT | ✓ | 主鍵,自動遞增 |
| TestItemId | INT | ✓ | 測項ID(FK → TestItems) |
| RevisionNumber | INT | ✓ | 版本號(1, 2, 3...) |
| RevisionType | NVARCHAR(20) | ✓ | 補測類型 **【v3.0 新增】** |
| EstimatedHours | DECIMAL(10,2) | ✓ | 此次補測預估工時 |
| Reason | NVARCHAR(200) | ✓ | 補測原因摘要 |
| Description | NVARCHAR(500) | ✗ | 詳細說明 |
| IsDeleted | BIT | ✓ | 是否刪除(Soft Delete) **【v2.3】** |
| CreatedByUserId | INT | ✓ | 建立者(FK → Users) |
| CreatedDate | DATETIME | ✓ | 建立時間 |
| ModifiedByUserId | INT | ✗ | 修改者(FK → Users) |
| ModifiedDate | DATETIME | ✗ | 修改時間 |
| DeletedByUserId | INT | ✗ | 刪除者(FK → Users) **【v2.3】** |
| DeletedDate | DATETIME | ✗ | 刪除時間 **【v2.3】** |

**RevisionType 可選值 【v3.0】:**
- **Command:** 客戶指令變更(預設值)
- **Retest:** 測試不通過需重測
- **Fix:** 修正問題後重測
- **Others:** 其他原因

**索引:**
- PK: RevisionId
- UNIQUE: (TestItemId, RevisionNumber) WHERE IsDeleted = 0
- INDEX: TestItemId

**約束:**
- RevisionNumber >= 1
- EstimatedHours > 0
- RevisionType IN ('Command', 'Retest', 'Fix', 'Others')
- 預設值: RevisionType = 'Command'

---

#### 8.2.7 WorkLogs (工時記錄) **【v2.3 更新】**

**用途:** 記錄工程師每日工時

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| WorkLogId | INT | ✓ | 主鍵,自動遞增 |
| TestItemId | INT | ✓ | 測項ID(FK → TestItems) |
| UserId | INT | ✓ | 工程師ID(FK → Users) |
| RevisionId | INT | ✗ | 補測版本ID(FK → TestItemRevision) **【v2.2】** |
| WorkDate | DATE | ✓ | 工作日期 |
| Hours | DECIMAL(5,2) | ✓ | 工時(小時) |
| Status | NVARCHAR(20) | ✓ | 狀態(InProgress/Delayed) |
| Note | NVARCHAR(200) | ✗ | 備註 |
| IsDeleted | BIT | ✓ | 是否刪除(Soft Delete) **【v2.3】** |
| CreatedDate | DATETIME | ✓ | 建立時間 |
| ModifiedByUserId | INT | ✗ | 修改者(FK → Users) |
| ModifiedDate | DATETIME | ✗ | 修改時間 |
| DeletedByUserId | INT | ✗ | 刪除者(FK → Users) **【v2.3】** |
| DeletedDate | DATETIME | ✗ | 刪除時間 **【v2.3】** |

**索引:**
- PK: WorkLogId
- UNIQUE: (TestItemId, UserId, WorkDate, RevisionId) WHERE IsDeleted = 0
- INDEX: TestItemId
- INDEX: UserId
- INDEX: WorkDate
- INDEX: RevisionId
- INDEX: IsDeleted **【v2.3】**

**約束:**
- Hours BETWEEN 0.5 AND 12
- WorkDate <= GETDATE()
- Status IN ('InProgress', 'Delayed')

---

#### 8.2.8 DelayReasons (延遲原因字典) **【v2.3 更新】**

**用途:** 儲存延遲原因選項

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| DelayReasonId | INT | ✓ | 主鍵,自動遞增 |
| ReasonText | NVARCHAR(100) | ✓ | 延遲原因文字 |
| ReasonType | NVARCHAR(20) | ✓ | 原因類型(Equipment/Customer/Engineer/Site) |
| IsActive | BIT | ✓ | 是否啟用 **【v2.3 重要】** |
| CreatedDate | DATETIME | ✓ | 建立時間 |
| ModifiedDate | DATETIME | ✗ | 修改時間 |

**停用機制說明 【v2.3】:**
- **不提供刪除功能**(無 IsDeleted 欄位)
- 改用 **IsActive** 欄位控制是否顯示於選單
- IsActive = 0 → 停用,不出現在新增工時的延遲原因選單
- 已使用該原因的歷史 WorkLog 仍保留關聯
- 查詢歷史資料時仍可顯示停用的原因文字

**索引:**
- PK: DelayReasonId
- INDEX: IsActive
- INDEX: ReasonType

**約束:**
- ReasonType IN ('Equipment', 'Customer', 'Engineer', 'Site')
- IsActive 預設值 = 1(啟用)

---

#### 8.2.9 WorkLogDelayReason (工時延遲原因關聯)

**用途:** 多對多關係,記錄工時對應的延遲原因

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| WorkLogDelayReasonId | INT | ✓ | 主鍵,自動遞增 |
| WorkLogId | INT | ✓ | 工時ID(FK → WorkLogs) |
| DelayReasonId | INT | ✓ | 延遲原因ID(FK → DelayReasons) |
| CreatedDate | DATETIME | ✓ | 建立時間 |

**索引:**
- PK: WorkLogDelayReasonId
- UNIQUE: (WorkLogId, DelayReasonId)
- INDEX: WorkLogId
- INDEX: DelayReasonId

---

#### 8.2.10 AuditLogs (稽核日誌)

**用途:** 記錄所有重要資料異動

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| AuditLogId | INT | ✓ | 主鍵,自動遞增 |
| TableName | NVARCHAR(50) | ✓ | 資料表名稱 |
| RecordId | INT | ✓ | 記錄ID |
| Operation | NVARCHAR(20) | ✓ | 操作類型(Create/Update/Delete/StatusChange) |
| UserId | INT | ✓ | 操作人(FK → Users) |
| ChangedDate | DATETIME | ✓ | 異動時間 |
| OldValues | NVARCHAR(MAX) | ✗ | 變更前值(JSON格式) |
| NewValues | NVARCHAR(MAX) | ✗ | 變更後值(JSON格式) |

**索引:**
- PK: AuditLogId
- INDEX: TableName
- INDEX: RecordId
- INDEX: UserId
- INDEX: ChangedDate

**約束:**
- Operation IN ('Create', 'Update', 'Delete', 'StatusChange')

---

#### 8.2.11 PasswordResetTokens (密碼重設 Token)

**用途:** 儲存密碼重設請求的 Token

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| TokenId | INT | ✓ | 主鍵,自動遞增 |
| UserId | INT | ✓ | 使用者ID(FK → Users) |
| Token | NVARCHAR(255) | ✓ | 重設 Token(GUID) |
| ExpiryDate | DATETIME | ✓ | 過期時間(建立後30分鐘) |
| IsUsed | BIT | ✓ | 是否已使用 |
| CreatedDate | DATETIME | ✓ | 建立時間 |

**索引:**
- PK: TokenId
- UNIQUE: Token
- INDEX: UserId
- INDEX: ExpiryDate

**約束:**
- IsUsed 預設值 = 0

---

#### 8.2.12 SystemSettings (系統設定)

**用途:** 儲存系統參數設定

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| SettingId | INT | ✓ | 主鍵,自動遞增 |
| SettingKey | NVARCHAR(50) | ✓ | 設定鍵值(唯一) |
| SettingValue | NVARCHAR(500) | ✓ | 設定值 |
| Description | NVARCHAR(200) | ✗ | 說明 |
| ModifiedByUserId | INT | ✗ | 修改者(FK → Users) |
| ModifiedDate | DATETIME | ✗ | 修改時間 |

**索引:**
- PK: SettingId
- UNIQUE: SettingKey

**預設設定項目:**
```
SettingKey                  | SettingValue | Description
----------------------------|--------------|------------------
JWT_SECRET                  | (隨機字串)   | JWT 簽章密鑰
JWT_EXPIRY_MINUTES          | 480          | Token 過期時間(分鐘)
PASSWORD_RESET_EXPIRY_MIN   | 30           | 密碼重設連結有效時間
LOGIN_FAILURE_LOCKOUT_COUNT | 5            | 登入失敗鎖定次數
LOGIN_LOCKOUT_MINUTES       | 10           | 鎖定時間(分鐘)
WORKLOG_EDIT_DAYS_LIMIT     | 7            | 工時可修改天數
EMAIL_SMTP_SERVER           | smtp.xxx.com | Email SMTP 伺服器
EMAIL_SMTP_PORT             | 587          | SMTP 埠號
EMAIL_FROM_ADDRESS          | noreply@...  | 寄件者 Email
```

---

### 8.3 IAM 權限模型 **【v2.1 補充】**

#### 8.3.1 Permissions (權限定義)

**用途:** 定義系統中所有可用權限

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| PermissionId | INT | ✓ | 主鍵,自動遞增 |
| PermissionCode | NVARCHAR(50) | ✓ | 權限代碼(唯一) |
| PermissionName | NVARCHAR(100) | ✓ | 權限名稱 |
| Description | NVARCHAR(200) | ✗ | 權限說明 |
| Category | NVARCHAR(50) | ✓ | 權限分類 |
| IsActive | BIT | ✓ | 是否啟用 |

**索引:**
- PK: PermissionId
- UNIQUE: PermissionCode
- INDEX: Category

**權限清單 【v3.0 更新】:**

| PermissionCode | PermissionName | Category | 說明 |
|---------------|----------------|----------|------|
| PROJECT_VIEW | 檢視案件 | Project | 檢視案件列表與詳細資訊 |
| PROJECT_CREATE | 建立案件 | Project | 使用 Wizard 建立新案件 |
| PROJECT_UPDATE | 修改案件 | Project | 修改案件基本資料 |
| PROJECT_DELETE | 刪除案件 | Project | 刪除案件(Soft Delete) |
| PROJECT_STATUS_OVERRIDE | 覆寫案件狀態 | Project | 手動設定案件狀態 |
| REGULATION_VIEW | 檢視法規 | Regulation | 檢視法規列表 **【v3.0】** |
| REGULATION_ADD | 新增法規 | Regulation | 在案件中新增法規 **【v3.0】** |
| REGULATION_DISABLE | 停用法規 | Regulation | 停用不需要的法規 **【v3.0】** |
| REGULATION_REMOVE | 移除法規 | Regulation | 移除法規(Soft Delete) **【v3.0】** |
| TESTITEM_VIEW | 檢視測項 | TestItem | 檢視測項列表 **【v3.0】** |
| TESTITEM_CREATE | 建立測項 | TestItem | 建立新測項 |
| TESTITEM_UPDATE | 修改測項 | TestItem | 修改測項資訊 |
| TESTITEM_DELETE | 刪除測項 | TestItem | 刪除測項(Soft Delete) |
| TESTITEM_STATUS_OVERRIDE | 覆寫測項狀態 | TestItem | 手動設定測項狀態 |
| TESTITEM_ASSIGN_ENGINEER | 分配主責工程師 | TestItem | 分配 Main1/2/3 工程師 **【v3.0】** |
| TESTITEM_ASSIGN_SUPPORT | 分配支援工程師 | TestItem | 分配 Support 工程師 **【v3.0】** |
| TESTITEM_REMOVE_ENGINEER | 移除工程師 | TestItem | 移除測項的工程師分配 **【v3.0】** |
| TESTITEM_REVISION_CREATE | 建立補測版本 | TestItem | 建立測項的補測版本 **【v3.0】** |
| TESTITEM_REVISION_ROLLBACK | 補測版本回滾 | TestItem | 回滾至先前版本 **【v3.0】** |
| WORKLOG_VIEW_ALL | 檢視所有工時 | WorkLog | 檢視所有人的工時記錄 |
| WORKLOG_VIEW_OWN | 檢視自己工時 | WorkLog | 僅檢視自己的工時 |
| WORKLOG_CREATE | 回報工時 | WorkLog | 回報每日工時 |
| WORKLOG_UPDATE_OWN | 修改自己工時 | WorkLog | 修改自己的工時(7天內) |
| WORKLOG_OVERRIDE | 覆寫工時 | WorkLog | 主管覆寫任何工時 |
| WORKLOG_DELETE | 刪除工時 | WorkLog | 刪除工時記錄 **【v3.0】** |
| LOADING_VIEW | 檢視 Loading | Loading | 檢視工程師 Loading 報表 |
| DELAY_REASON_MANAGE | 管理延遲原因 | DelayReason | 維護延遲原因字典 |
| USER_VIEW | 檢視使用者 | User | 檢視使用者列表 |
| USER_CREATE | 建立使用者 | User | 新增使用者 |
| USER_UPDATE | 修改使用者 | User | 修改使用者資訊 |
| USER_DISABLE | 停用使用者 | User | 停用/啟用使用者 |
| AUDIT_VIEW | 檢視稽核日誌 | Audit | 查詢稽核日誌 **【v3.0 統一命名】** |
| REPORT_GENERATE | 產生報表 | Report | 產生各類報表 |
| SYSTEM_SETTING | 系統設定 | System | 修改系統參數 **【v3.0 統一命名】** |

---

#### 8.3.2 PermissionGroups (權限群組)

**用途:** 將權限打包成群組,方便批次授予

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| PermissionGroupId | INT | ✓ | 主鍵,自動遞增 |
| GroupName | NVARCHAR(50) | ✓ | 群組名稱(唯一) |
| Description | NVARCHAR(200) | ✗ | 群組說明 |
| IsActive | BIT | ✓ | 是否啟用 |
| CreatedDate | DATETIME | ✓ | 建立時間 |
| ModifiedDate | DATETIME | ✗ | 修改時間 |

**索引:**
- PK: PermissionGroupId
- UNIQUE: GroupName

**預設群組 【v3.0 更新】:**

**Engineer 權限群組:**
- TESTITEM_VIEW
- WORKLOG_VIEW_OWN
- WORKLOG_CREATE
- WORKLOG_UPDATE_OWN

**Manager 權限群組:**
- 所有 PROJECT_* 權限
- 所有 REGULATION_* 權限 **【v3.0】**
- 所有 TESTITEM_* 權限(包含新增的分配/回滾) **【v3.0】**
- 所有 WORKLOG_* 權限
- LOADING_VIEW
- DELAY_REASON_MANAGE
- USER_VIEW
- USER_CREATE
- USER_UPDATE
- USER_DISABLE
- AUDIT_VIEW
- REPORT_GENERATE
- SYSTEM_SETTING

---

#### 8.3.3 PermissionGroupMapping (權限群組對應) **【v2.3 更新】**

**用途:** 定義權限群組包含哪些權限

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| MappingId | INT | ✓ | 主鍵,自動遞增 **【v2.3】** |
| PermissionGroupId | INT | ✓ | 權限群組ID(FK → PermissionGroups) |
| PermissionId | INT | ✓ | 權限ID(FK → Permissions) |
| CreatedDate | DATETIME | ✓ | 建立時間 |

**索引:**
- PK: MappingId **【v2.3 修正】**
- UNIQUE: (PermissionGroupId, PermissionId)
- INDEX: PermissionGroupId
- INDEX: PermissionId

---

#### 8.3.4 UserPermissions (使用者權限)

**用途:** 記錄使用者的權限來源(群組或個別授予)

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| UserPermissionId | INT | ✓ | 主鍵,自動遞增 |
| UserId | INT | ✓ | 使用者ID(FK → Users) |
| PermissionId | INT | ✗ | 權限ID(FK → Permissions),個別權限時填寫 |
| PermissionGroupId | INT | ✗ | 權限群組ID(FK → PermissionGroups),群組權限時填寫 |
| GrantType | NVARCHAR(20) | ✓ | 授予類型(Group/Individual/Temporary) |
| ExpiryDate | DATETIME | ✗ | 到期日(僅 Temporary 需填) |
| GrantedByUserId | INT | ✓ | 授予者(FK → Users) |
| GrantedDate | DATETIME | ✓ | 授予時間 |
| RevokedDate | DATETIME | ✗ | 撤銷時間 |

**索引:**
- PK: UserPermissionId
- INDEX: UserId
- INDEX: PermissionId
- INDEX: PermissionGroupId
- INDEX: ExpiryDate

**約束:**
- GrantType IN ('Group', 'Individual', 'Temporary')
- PermissionId 與 PermissionGroupId 至少一個不為 NULL
- GrantType = 'Temporary' → ExpiryDate 必填

**權限計算邏輯:**
```
使用者有效權限 = 
  (所有 PermissionGroup 中的 Permission)
  UNION
  (Individual GrantType 的 Permission)
  UNION
  (Temporary GrantType 且未過期的 Permission)
  
  WHERE RevokedDate IS NULL
```

---

## 9. 介面規格定義 **【v3.0 補充完整】**

### 9.1 使用者端介面 (UI Screens)

#### 9.1.1 登入相關

**SCR-AUTH-001: 登入畫面**
- **描述:** 使用者登入系統
- **輸入:**
  - 帳號(Account)
  - 密碼(Password)
  - 登入方式選擇(Local / Windows AD)
- **按鈕:**
  - [登入]
  - [忘記密碼]
- **驗證:**
  - 欄位必填檢查
  - 連續失敗5次鎖定
- **導向:**
  - 成功 → 依角色導向首頁
  - 失敗 → 顯示錯誤訊息

**SCR-AUTH-002: 忘記密碼**
- **描述:** 申請密碼重設
- **輸入:**
  - Email 或 Account
- **流程:**
  1. 輸入 Email/Account
  2. 系統發送重設連結至註冊信箱
  3. 顯示「已發送重設信,請至信箱查收」
- **按鈕:**
  - [送出]
  - [返回登入]

**SCR-AUTH-003: 重設密碼**
- **描述:** 透過 Token 重設密碼
- **輸入:**
  - 新密碼
  - 確認密碼
- **驗證:**
  - Token 有效性(30分鐘)
  - 密碼複雜度規則
  - 兩次輸入一致
- **按鈕:**
  - [確認重設]

---

#### 9.1.2 案件管理 **【v3.0 更新】**

**SCR-PROJECT-WIZARD-001: Wizard 建案流程**

**Step 1: 案件基本資料**
- **輸入:**
  - 案件名稱*
  - 客戶名稱*
  - 優先順序*(下拉選單: High/Medium/Low)
  - 預計開始日期*
  - 預計結束日期*
  - 案件說明
- **驗證:**
  - 結束日期 >= 開始日期
  - 案件名稱不重複
- **按鈕:**
  - [下一步]
  - [取消]

**Step 2: 法規選擇**
- **功能:**
  - 可多選法規(CheckBox)
  - 法規選項: FCC、NCC、CE、IC、TELEC、其他
- **每個法規填寫:**
  - 法規名稱(如 Part 24)
  - 測試開始日期*
  - 測試結束日期*
- **驗證:**
  - 至少選擇1個法規
  - 法規日期需在案件日期範圍內(可警告但允許繼續)
- **按鈕:**
  - [上一步]
  - [下一步]
  - [取消]

**Step 3: 測試項目設定**
- **功能:**
  - 為每個法規新增測試項目
  - 可複製其他法規的測項範本
- **每個測項填寫:**
  - 測項名稱*
  - 測試類型*(下拉: Conducted/Radiated/Blocking/...)
  - 測試場地*(下拉: Lab A/Lab B/Lab C)
  - 預估工時*(小時)
- **驗證:**
  - 每個法規至少1個測項
  - 預估工時 > 0
- **按鈕:**
  - [新增測項]
  - [複製範本]
  - [上一步]
  - [下一步]
  - [取消]

**Step 4: 工程師分配 【v3.0 更新】**
- **功能:**
  - 為每個測項分配工程師
  - 顯示工程師當前 Loading
- **分配介面:**
  - 測項列表(Grid)
  - 每個測項可分配:
    - Main1(主要負責人)*
    - Main2(次要負責人)
    - Main3(第三負責人)
    - Support(支援人員,可多選)
  - 為每個角色分配工時
- **顯示資訊:**
  - 工程師當前 Loading(顏色標示)
  - 分配後預估 Loading
- **驗證:**
  - 每個測項至少1位 Main1
  - 分配工時總和與預估工時差異 > 20% 顯示警告
  - 工程師 Loading > 100% 顯示警告(可繼續)
- **按鈕:**
  - [上一步]
  - [完成建案]
  - [取消]

**Step 5: 確認摘要**
- **顯示:**
  - 案件資訊摘要
  - 法規清單
  - 測項數量統計
  - 總預估工時
  - 工程師分配總覽
- **按鈕:**
  - [確認建立]
  - [返回修改]
  - [取消]

---

**SCR-PROJECT-LIST-001: 案件總覽(列表模式) 【v3.0 新增】**
- **描述:** 主管檢視所有案件列表
- **顯示欄位:**
  - 案件名稱
  - 客戶名稱
  - 優先順序(顏色標示)
  - 狀態(圖示 + 文字)
  - 進度百分比(完成測項數/總測項數)
  - 開始日期
  - 結束日期
- **功能:**
  - 欄位排序(點擊欄位標題)
  - 多條件篩選:
    - 狀態(多選)
    - 優先順序(多選)
    - 客戶名稱(輸入)
    - 日期區間
  - 快速搜尋(案件名稱)
  - 每列操作按鈕:
    - [詳細]
    - [編輯]
    - [刪除]
- **按鈕:**
  - [新增案件](導向 Wizard)
  - [匯出 Excel]
- **權限:** MANAGER

**SCR-PROJECT-DETAIL-001: 案件詳細資訊**
- **描述:** 檢視案件完整資訊
- **顯示區塊:**
  1. **基本資訊**
     - 案件名稱、客戶、優先順序、狀態、日期
  2. **法規列表**
     - 法規類型、名稱、狀態、測項數量
     - 點擊法規展開測項清單
  3. **進度統計**
     - 整體進度百分比
     - 預估工時 vs 實際工時
     - 延遲測項數量
  4. **工時趨勢圖**
     - 每日工時累計折線圖
- **按鈕:**
  - [編輯案件]
  - [新增法規] **【v3.0】**
  - [產生報表]
  - [返回列表]
- **權限:** MANAGER

---

#### 9.1.3 法規管理 **【v3.0 新增完整模組】**

**SCR-REGULATION-001: 法規列表管理**
- **描述:** 檢視與管理案件中的法規
- **顯示欄位:**
  - 法規類型(FCC/NCC/CE/IC/TELEC)
  - 法規名稱
  - 狀態(顏色標示)
  - 測試日期
  - 測項數量
  - 完成度
- **篩選:**
  - 依案件篩選
  - 依狀態篩選
- **每列操作:**
  - [檢視測項]
  - [編輯]
  - [停用]
  - [移除]
- **按鈕:**
  - [新增法規]
  - [返回案件]
- **權限:** REGULATION_VIEW

**SCR-REGULATION-002: 新增法規**
- **描述:** 在已存在的案件中新增法規
- **輸入:**
  - 法規類型*(下拉選單)
  - 法規名稱*
  - 測試開始日期*
  - 測試結束日期*
  - 說明
- **驗證:**
  - 日期需在案件日期±30天內(可警告但允許繼續)
  - 結束日期 >= 開始日期
- **按鈕:**
  - [確認新增]
  - [取消]
- **權限:** REGULATION_ADD

**SCR-REGULATION-003: 停用/移除法規**
- **描述:** 停用或移除不需要的法規
- **停用流程:**
  1. 點擊[停用]
  2. 檢查是否有測項存在
  3. 若有測項顯示警告:「此法規有 X 個測項,停用後將不參與狀態計算」
  4. 確認後設定 IsActive = false
- **移除流程:**
  1. 點擊[移除]
  2. 檢查是否有測項或工時記錄
  3. 若有記錄顯示影響範圍
  4. 確認後執行 Soft Delete
  5. 記錄至 AuditLog
- **按鈕:**
  - [確認]
  - [取消]
- **權限:** REGULATION_DISABLE、REGULATION_REMOVE

---

#### 9.1.4 測試項目管理 **【v3.0 擴充】**

**SCR-TESTITEM-LIST-001: 測項列表 【v3.0 新增】**
- **描述:** 檢視測項列表
- **顯示欄位:**
  - 測項名稱
  - 測試類型
  - 測試場地
  - 狀態(顏色標示)
  - 負責工程師(Main1/Main2/Main3)
  - 預估工時
  - 實際工時
  - 剩餘工時
  - 當前版本(v1/v2/v3)
- **篩選:**
  - 依法規篩選
  - 依狀態篩選
  - 依工程師篩選
- **快速搜尋:**
  - 測項名稱
- **每列操作:**
  - [詳細]
  - [編輯]
  - [分配工程師]
  - [建立補測]
  - [刪除]
- **按鈕:**
  - [新增測項]
  - [返回法規]
- **權限:** TESTITEM_VIEW

**SCR-TESTITEM-DETAIL-001: 測項詳細資訊**
- **描述:** 檢視測項完整資訊
- **顯示區塊:**
  1. **基本資訊**
     - 測項名稱、類型、場地、狀態
  2. **工程師分配**
     - Main1、Main2、Main3、Support
     - 各角色分配工時
  3. **工時統計**
     - 預估工時
     - 實際工時(依版本拆分)
     - 剩餘工時
  4. **補測版本歷史**
     - 版本列表(v1/v2/v3)
     - 各版本工時統計
  5. **工時記錄明細**
     - 日期、工程師、工時、狀態、版本
- **按鈕:**
  - [編輯]
  - [分配工程師]
  - [建立補測]
  - [覆寫狀態]
  - [返回列表]
- **權限:** TESTITEM_VIEW

**SCR-ENGINEER-ASSIGN-001: 工程師分配介面 【v3.0 新增】**
- **描述:** 分配測項的負責工程師
- **顯示區塊:**
  1. **測項資訊**
     - 測項名稱、預估工時
  2. **工程師選擇**
     - Main1(主要負責人)*:下拉選單
       - 分配工時:___ 小時*
     - Main2(次要負責人):下拉選單
       - 分配工時:___ 小時
     - Main3(第三負責人):下拉選單
       - 分配工時:___ 小時
     - Support(支援人員):多選 CheckBox
       - 各支援人員分配工時
  3. **工程師 Loading 顯示**
     - 當前 Loading
     - 分配後預估 Loading(顏色警示)
- **驗證:**
  - 至少需1位 Main1
  - 分配工時總和與預估工時差異 > 20% 警告
  - Loading > 100% 警告(可繼續)
  - 不可重複分配同一工程師
- **按鈕:**
  - [確認分配]
  - [取消]
- **權限:** TESTITEM_ASSIGN_ENGINEER、TESTITEM_ASSIGN_SUPPORT

**SCR-ENGINEER-REMOVE-001: 移除工程師 【v3.0 新增】**
- **描述:** 移除測項的工程師分配
- **流程:**
  1. 選擇要移除的工程師
  2. 檢查是否有該工程師的工時記錄
  3. 若有工時顯示警告
  4. 確認後移除分配
- **按鈕:**
  - [確認移除]
  - [取消]
- **權限:** TESTITEM_REMOVE_ENGINEER

---

#### 9.1.5 補測版本管理 **【v3.0 完整定義】**

**SCR-REVISION-LIST-001: 補測版本列表**
- **描述:** 檢視測項的補測版本歷史
- **顯示欄位:**
  - 版本號(v1/v2/v3...)
  - 補測類型(Command/Retest/Fix/Others)
  - 補測原因
  - 預估工時
  - 實際工時
  - 建立日期
  - 建立者
- **每列操作:**
  - [檢視明細]
  - [回滾至此版本]
  - [刪除]
- **按鈕:**
  - [建立新版本]
  - [返回測項]
- **權限:** TESTITEM_VIEW

**SCR-REVISION-CREATE-001: 建立補測版本**
- **描述:** 為測項建立新的補測版本
- **輸入:**
  - 補測類型*(下拉選單)
    - Command(客戶指令)
    - Retest(測試不通過)
    - Fix(修正後重測)
    - Others(其他)
  - 補測原因*(文字輸入,限200字)
  - 詳細說明(文字區域,限500字)
  - 預估工時*(小時)
  - 是否複製原工程師分配(CheckBox)
- **顯示資訊:**
  - 當前版本號
  - 新版本號(自動遞增)
  - 原工程師分配(若選擇複製)
- **驗證:**
  - 預估工時 > 0
  - 補測原因必填
- **流程:**
  1. 填寫補測資訊
  2. 選擇是否複製工程師分配
  3. 確認後:
     - 建立 TestItemRevision 記錄
     - 若選擇複製,建立對應的 TestItemEngineer 記錄
     - 測項狀態自動改為 InProgress
     - 記錄至 AuditLog
- **按鈕:**
  - [確認建立]
  - [取消]
- **權限:** TESTITEM_REVISION_CREATE

**SCR-REVISION-ROLLBACK-001: 補測版本回滾 【v3.0 新增】**
- **描述:** 回滾測項至先前的補測版本
- **輸入:**
  - 選擇目標版本*(下拉選單,僅顯示已完成的版本)
  - 回滾原因*(文字輸入,限200字)
- **顯示資訊:**
  - 目標版本詳細資訊
  - 工程師分配(將被複製)
  - 預估工時
- **驗證:**
  - 僅能回滾至已完成的版本
  - 回滾原因必填
- **流程:**
  1. 選擇目標版本
  2. 填寫回滾原因
  3. 確認後:
     - 建立新的 Revision 記錄(版本號遞增)
     - RevisionType = 目標版本的 RevisionType
     - Reason = "回滾至 v{N}:{原因}"
     - 複製目標版本的工程師分配
     - 測項狀態改為 InProgress
     - 記錄至 AuditLog
- **按鈕:**
  - [確認回滾]
  - [取消]
- **權限:** TESTITEM_REVISION_ROLLBACK

---

#### 9.1.6 工時管理

**SCR-WORKLOG-REPORT-001: 工時回報(工程師端)**
- **描述:** 工程師回報每日工時
- **輸入:**
  - 測項選擇*(下拉選單,顯示自己負責的測項)
  - 補測版本*(下拉選單,顯示該測項的版本)
  - 工作日期*(日期選擇器,預設今天)
  - 工時*(小時,0.5-12)
  - 狀態*(單選: InProgress/Delayed)
  - 延遲原因(若狀態=Delayed,多選 CheckBox,至少選1個)
  - 備註(文字輸入)
- **驗證:**
  - 工作日期不可晚於今天
  - 工時範圍 0.5-12
  - 同測項同日期同版本不可重複回報
  - 狀態=Delayed 必須選延遲原因
- **顯示:**
  - 本週已回報工時統計
  - 測項剩餘工時警示
- **按鈕:**
  - [送出]
  - [取消]
- **權限:** WORKLOG_CREATE

**SCR-WORKLOG-LIST-001: 我的工時記錄(工程師端)**
- **描述:** 工程師查詢自己的工時記錄
- **顯示欄位:**
  - 測項名稱
  - 補測版本
  - 工作日期
  - 工時
  - 狀態
  - 延遲原因
  - 備註
- **篩選:**
  - 依測項篩選
  - 依日期區間篩選
  - 依版本篩選
  - 依狀態篩選
- **統計資訊:**
  - 本週總工時
  - 本月總工時
  - 各測項累計工時
- **每列操作:**
  - [修改](僅7天內)
  - [檢視]
- **按鈕:**
  - [回報工時]
  - [匯出 Excel]
- **權限:** WORKLOG_VIEW_OWN

**SCR-WORKLOG-EDIT-001: 修改工時**
- **描述:** 工程師修改自己的工時(7天內)
- **顯示:**
  - 原工時資訊(唯讀)
- **可修改欄位:**
  - 工時
  - 狀態
  - 延遲原因
  - 備註
- **驗證:**
  - 僅能修改7天內的記錄
  - 工時範圍 0.5-12
  - 狀態=Delayed 必須選延遲原因
- **按鈕:**
  - [確認修改]
  - [取消]
- **權限:** WORKLOG_UPDATE_OWN

**SCR-WORKLOG-MANAGER-001: 工時總覽(主管端)**
- **描述:** 主管檢視所有工時記錄
- **顯示欄位:**
  - 案件名稱
  - 測項名稱
  - 工程師
  - 補測版本
  - 工作日期
  - 工時
  - 狀態
  - 延遲原因
- **篩選:**
  - 依案件篩選
  - 依測項篩選
  - 依工程師篩選
  - 依日期區間篩選
  - 依版本篩選
  - 依狀態篩選
- **統計資訊:**
  - 總工時統計
  - 各工程師工時分布
  - 延遲工時佔比
- **每列操作:**
  - [檢視]
  - [覆寫]
  - [刪除]
- **按鈕:**
  - [匯出 Excel]
  - [產生報表]
- **權限:** WORKLOG_VIEW_ALL

**SCR-WORKLOG-OVERRIDE-001: 覆寫工時(主管)**
- **描述:** 主管覆寫工時記錄
- **顯示:**
  - 原工時資訊(唯讀)
- **可修改欄位:**
  - 工時
  - 狀態
  - 延遲原因
  - 備註
  - 覆寫理由*(必填,限200字)
- **驗證:**
  - 覆寫理由必填
  - 工時範圍 0.5-12
- **流程:**
  1. 修改欄位
  2. 填寫覆寫理由
  3. 確認後:
     - 更新 WorkLog 記錄
     - 記錄至 AuditLog(含覆寫理由)
- **按鈕:**
  - [確認覆寫]
  - [取消]
- **權限:** WORKLOG_OVERRIDE

**SCR-WORKLOG-DELETE-001: 刪除工時(主管) 【v3.0 新增】**
- **描述:** 主管刪除工時記錄(Soft Delete)
- **顯示:**
  - 工時詳細資訊
- **輸入:**
  - 刪除理由*(必填,限200字)
- **驗證:**
  - 刪除理由必填
- **流程:**
  1. 填寫刪除理由
  2. 確認後:
     - 設定 IsDeleted = true
     - 記錄 DeletedByUserId 與 DeletedDate
     - 記錄至 AuditLog(含刪除理由)
- **按鈕:**
  - [確認刪除]
  - [取消]
- **權限:** WORKLOG_DELETE

---

#### 9.1.7 Loading 分析

**SCR-LOADING-OVERVIEW-001: Loading 總覽**
- **描述:** 主管檢視所有工程師 Loading
- **顯示方式:**
  - 卡片式排列
  - 每張卡片顯示:
    - 工程師姓名
    - Assigned Loading(分配負載)
    - Actual Loading(實際負載)
    - Loading 百分比(進度條 + 顏色)
    - 負責測項數量
- **顏色標示:**
  - ≤60%: 綠色
  - 61-80%: 黃色
  - 81-100%: 橘色
  - >100%: 紅色(超載警示)
- **排序:**
  - 依 Loading 由高到低
  - 依姓名排序
- **按鈕:**
  - [檢視明細](點擊卡片)
  - [匯出 Excel]
- **權限:** LOADING_VIEW

**SCR-LOADING-DETAIL-001: 工程師 Loading 明細**
- **描述:** 檢視單一工程師的 Loading 詳細資訊
- **顯示區塊:**
  1. **Loading 統計**
     - Assigned Loading
     - Actual Loading
     - 本週可用工時
     - 本週已用工時
  2. **負責測項列表**
     - 測項名稱
     - 案件名稱
     - 角色(Main1/Main2/Main3/Support)
     - 分配工時
     - 實際工時
     - 剩餘工時
     - 狀態
  3. **工時趨勢圖**
     - 近30天每日工時折線圖
     - 分配工時 vs 實際工時比較
- **按鈕:**
  - [返回總覽]
  - [匯出 Excel]
- **權限:** LOADING_VIEW

---

#### 9.1.8 延遲管理

**SCR-DELAY-REASON-001: 延遲原因管理**
- **描述:** 主管維護延遲原因字典
- **顯示欄位:**
  - 原因文字
  - 原因類型(Equipment/Customer/Engineer/Site)
  - 是否啟用
  - 使用次數(統計)
  - 最後使用日期
- **每列操作:**
  - [編輯]
  - [停用/啟用]
- **按鈕:**
  - [新增原因]
- **權限:** DELAY_REASON_MANAGE

**SCR-DELAY-REASON-ADD-001: 新增延遲原因**
- **描述:** 新增延遲原因
- **輸入:**
  - 原因文字*(限100字)
  - 原因類型*(下拉選單: Equipment/Customer/Engineer/Site)
- **驗證:**
  - 原因文字不可重複
- **按鈕:**
  - [確認新增]
  - [取消]
- **權限:** DELAY_REASON_MANAGE

**SCR-DELAY-REASON-EDIT-001: 編輯延遲原因**
- **描述:** 修改延遲原因
- **可修改欄位:**
  - 原因文字
  - 原因類型
- **驗證:**
  - 已使用的原因不可刪除,僅能停用
- **按鈕:**
  - [確認修改]
  - [停用]
  - [取消]
- **權限:** DELAY_REASON_MANAGE

**SCR-DELAY-ANALYSIS-001: 延遲分析報表 【v3.0 新增】**
- **描述:** 分析延遲原因與趨勢
- **顯示區塊:**
  1. **延遲原因分布**
     - 圓餅圖:各原因類型佔比
     - 長條圖:各原因使用次數
  2. **延遲測項清單**
     - 測項名稱
     - 案件名稱
     - 延遲原因
     - 延遲天數
     - 負責工程師
  3. **統計資訊**
     - 總延遲測項數
     - 平均延遲天數
     - 最常見延遲原因 Top 5
- **篩選:**
  - 依期間篩選
  - 依案件篩選
  - 依原因類型篩選
- **按鈕:**
  - [匯出 Excel]
  - [匯出圖表]
- **權限:** LOADING_VIEW

---

#### 9.1.9 使用者管理 **【v3.0 更新】**

**SCR-USER-LIST-001: 使用者列表**
- **描述:** 主管檢視與管理使用者
- **顯示欄位:**
  - 帳號
  - 顯示名稱
  - Email
  - 角色
  - 認證類型(Local/AD)
  - 是否啟用
  - 最後登入時間
  - 每週可用工時
- **篩選:**
  - 依角色篩選
  - 依啟用狀態篩選
  - 依認證類型篩選
- **每列操作:**
  - [編輯]
  - [停用/啟用]
  - [重設密碼]
  - [權限設定]
- **按鈕:**
  - [新增使用者]
- **權限:** USER_VIEW

**SCR-USER-ADD-001: 新增使用者 【v3.0 強化】**
- **描述:** 主管新增使用者(首次建立 Email)
- **輸入:**
  - 認證類型*(單選: Local/AD)
  - 【若 Local】
    - 帳號*(限50字,唯一)
    - Email*(限100字,唯一,必填)
    - 顯示名稱*(限50字)
  - 【若 AD】
    - AD 帳號*(限100字,唯一)
    - Email*(限100字,唯一,必填)
    - 顯示名稱*(限50字)
  - 角色*(下拉選單: Engineer/Manager/Admin)
  - 每週可用工時*(預設40)
  - 權限群組(多選 CheckBox)
- **驗證:**
  - Email 必填且唯一(不區分大小寫)
  - 若 Email 已存在於系統 → 顯示錯誤:「此 Email 已被使用」
  - 帳號/AD 帳號需唯一
- **流程:**
  1. 填寫使用者資訊
  2. 系統檢查 Email 唯一性
  3. 確認後:
     - 建立 User 記錄
     - 【若 Local】系統產生初始密碼
     - 發送 Email 通知(含初始密碼或歡迎信)
     - 指派預設權限群組
     - 記錄至 AuditLog
- **按鈕:**
  - [確認新增]
  - [取消]
- **權限:** USER_CREATE

**SCR-USER-EDIT-001: 編輯使用者 【v3.0 更新】**
- **描述:** 修改使用者資訊
- **顯示:**
  - 帳號/AD 帳號(唯讀)
  - 認證類型(唯讀)
- **可修改欄位:**
  - 顯示名稱
  - Email*(需檢查唯一性)
  - 角色
  - 每週可用工時
  - 權限群組
- **驗證:**
  - Email 修改時檢查唯一性
  - 若 Email 已被其他使用者使用 → 顯示錯誤
- **按鈕:**
  - [確認修改]
  - [取消]
- **權限:** USER_UPDATE

**SCR-USER-DISABLE-001: 停用/啟用使用者**
- **描述:** 停用或啟用使用者帳號
- **停用流程:**
  1. 點擊[停用]
  2. 檢查是否負責未完成測項
  3. 若有測項顯示警告:「此使用者負責 X 個未完成測項」
  4. 確認後設定 IsActive = false
  5. 記錄至 AuditLog
- **啟用流程:**
  1. 點擊[啟用]
  2. 設定 IsActive = true
  3. 記錄至 AuditLog
- **按鈕:**
  - [確認]
  - [取消]
- **權限:** USER_DISABLE

**SCR-USER-PERMISSION-001: 使用者權限設定 【v3.0 新增】**
- **描述:** 設定使用者的權限
- **顯示區塊:**
  1. **權限群組**
     - 已指派的群組(CheckBox,可勾選/取消)
     - 可新增其他群組
  2. **個別權限**
     - 依分類顯示所有權限
     - 可個別授予/撤銷權限
     - 顯示權限來源(群組/個別授予/臨時)
  3. **臨時權限**
     - 可設定權限到期日
     - 顯示已設定的臨時權限與到期日
  4. **有效權限摘要**
     - 列出使用者當前所有有效權限
     - 標示權限來源
- **按鈕:**
  - [確認變更]
  - [取消]
- **權限:** USER_UPDATE

---

#### 9.1.10 稽核日誌 **【v3.0 更新】**

**SCR-AUDIT-LIST-001: 稽核日誌查詢**
- **描述:** 主管查詢稽核日誌
- **顯示欄位:**
  - 異動時間
  - 資料表名稱
  - 記錄ID
  - 操作類型
  - 操作人
  - 變更摘要
- **篩選:**
  - 依資料表篩選
  - 依操作類型篩選
  - 依操作人篩選
  - 依時間區間篩選
- **每列操作:**
  - [檢視明細]
- **按鈕:**
  - [匯出 Excel]
- **權限:** AUDIT_VIEW

**SCR-AUDIT-DETAIL-001: 稽核日誌明細**
- **描述:** 檢視變更前後差異
- **顯示方式:**
  - 雙欄對比(Before / After)
  - 變更欄位以顏色標示
  - JSON 格式自動解析為表格
- **顯示資訊:**
  - 操作人
  - 操作時間
  - 操作類型
  - 資料表名稱
  - 記錄ID
  - 變更前值
  - 變更後值
- **按鈕:**
  - [返回列表]
- **權限:** AUDIT_VIEW

---

#### 9.1.11 報表模組 **【v3.0 新增】**

**SCR-REPORT-PROJECT-001: 案件進度報表**
- **描述:** 產生案件進度報表
- **輸入:**
  - 選擇案件*(下拉選單)
- **顯示內容:**
  1. **案件基本資訊**
     - 案件名稱、客戶、優先順序、日期
  2. **法規完成度**
     - 長條圖:各法規完成百分比
  3. **測項進度明細**
     - 測項列表(狀態、工程師、工時)
  4. **工時統計**
     - 預估工時 vs 實際工時
     - 工時差異分析
  5. **延遲分析**
     - 延遲測項數量
     - 延遲原因統計
- **按鈕:**
  - [匯出 Excel]
  - [匯出 PDF]
  - [列印]
- **權限:** REPORT_GENERATE

**SCR-REPORT-WORKLOG-001: 工時統計報表**
- **描述:** 產生工時統計報表
- **輸入:**
  - 統計維度*(單選: 案件/工程師/期間)
  - 日期區間*
  - 【若選案件】選擇案件
  - 【若選工程師】選擇工程師
- **顯示內容:**
  1. **工時統計**
     - 總工時
     - 平均每日工時
     - 工時分布圖表
  2. **明細列表**
     - 依選擇維度顯示明細
  3. **趨勢分析**
     - 工時趨勢折線圖
- **按鈕:**
  - [匯出 Excel]
  - [匯出圖表]
- **權限:** REPORT_GENERATE

---

#### 9.1.12 系統設定 **【v3.0 新增】**

**SCR-SYSTEM-SETTING-001: 系統參數設定**
- **描述:** 主管修改系統參數
- **設定項目:**
  1. **JWT 設定**
     - Token 過期時間(分鐘)
  2. **登入安全設定**
     - 登入失敗鎖定次數
     - 鎖定時間(分鐘)
  3. **工時設定**
     - 工時可修改天數限制
  4. **密碼重設設定**
     - 重設連結有效時間(分鐘)
  5. **Email 設定**
     - SMTP 伺服器
     - SMTP 埠號
     - 寄件者 Email
     - 是否啟用 SSL
- **驗證:**
  - 數值範圍檢查
  - Email 格式檢查
  - SMTP 連線測試
- **按鈕:**
  - [儲存變更]
  - [測試 Email]
  - [重設為預設值]
- **權限:** SYSTEM_SETTING

---

## 10. 業務規則與邏輯 **【v3.0 更新】**

### 10.1 狀態計算規則

#### 10.1.1 TestItem 狀態計算(含補測邏輯)

**優先順序(由高到低):**

1. **OnHold(暫停)**
   - 條件: ManualStatusOverride = true AND Status = 'OnHold'
   - 說明: 主管手動設定為暫停,最高優先級
   - 不被自動邏輯覆蓋

2. **InProgress(進行中)**
   - 條件: 剛建立 TestItemRevision 記錄
   - 說明: 補測事件發生時自動觸發
   - 可被主管手動覆寫

3. **Delayed(延遲)**
   - 條件: EXISTS(SELECT * FROM WorkLogs WHERE TestItemId = X AND Status = 'Delayed')
   - 說明: 任一工程師回報延遲即觸發

4. **Completed(完成)**
   - 條件: 工程師按「Complete TestItem」按鈕
   - 說明: 可被工程師或主管取消

5. **InProgress(進行中)**
   - 條件: EXISTS(SELECT * FROM WorkLogs WHERE TestItemId = X AND Status = 'InProgress')
   - 說明: 至少有一筆工時記錄

6. **NotStarted(未開始)**
   - 條件: 以上皆不符合
   - 說明: 初始狀態

**狀態可逆性 【v3.0 強調】:**
- 工程師可取消自己誤按的 Completed
- 主管可覆寫任何狀態
- 所有狀態變更需記錄至 AuditLog

---

#### 10.1.2 Regulation 狀態計算

**優先順序(由高到低):**

1. **OnHold(暫停)**
   - 條件: ManualStatusOverride = true AND Status = 'OnHold'
   - 說明: 主管手動設定

2. **Delayed(延遲)**
   - 條件: 任一 TestItem.Status = 'Delayed'
   - 計算: EXISTS(SELECT * FROM TestItems WHERE RegulationId = X AND Status = 'Delayed' AND IsDeleted = 0)

3. **Completed(完成)**
   - 條件: 所有 TestItem.Status = 'Completed'
   - 計算: NOT EXISTS(SELECT * FROM TestItems WHERE RegulationId = X AND Status != 'Completed' AND IsDeleted = 0)

4. **InProgress(進行中)**
   - 條件: 任一 TestItem.Status = 'InProgress'
   - 計算: EXISTS(SELECT * FROM TestItems WHERE RegulationId = X AND Status = 'InProgress' AND IsDeleted = 0)

5. **NotStarted(未開始)**
   - 條件: 所有 TestItem.Status = 'NotStarted'
   - 說明: 初始狀態

**狀態變更時機:**
- TestItem 狀態變更時自動觸發重算
- 主管手動覆寫時立即生效
- 變更需記錄至 AuditLog

---

#### 10.1.3 Project 狀態計算

**優先順序(由高到低):**

1. **OnHold(暫停)**
   - 條件: ManualStatusOverride = true AND Status = 'OnHold'
   - 說明: 主管手動設定

2. **Delayed(延遲)**
   - 條件: 任一 Regulation.Status = 'Delayed'
   - 計算: EXISTS(SELECT * FROM Regulations WHERE ProjectId = X AND Status = 'Delayed' AND IsDeleted = 0)

3. **Completed(完成)**
   - 條件: 所有 Regulation.Status = 'Completed'
   - 計算: NOT EXISTS(SELECT * FROM Regulations WHERE ProjectId = X AND Status != 'Completed' AND IsDeleted = 0)

4. **Active(進行中)**
   - 條件: 任一 Regulation.Status = 'InProgress'
   - 計算: EXISTS(SELECT * FROM Regulations WHERE ProjectId = X AND Status = 'InProgress' AND IsDeleted = 0)

5. **Draft(草稿)**
   - 條件: 所有 Regulation.Status = 'NotStarted'
   - 說明: 初始狀態

**狀態變更時機:**
- Regulation 狀態變更時自動觸發重算
- 主管手動覆寫時立即生效
- 變更需記錄至 AuditLog

---

### 10.2 Loading 計算規則

#### 10.2.1 Assigned Loading(分配負載)

**計算公式:**
```
Assigned Loading = (Σ 分配工時) / (每週可用工時 × 目標週數) × 100%

其中:
- 分配工時: SUM(TestItemEngin
|
#### 10.2.1 Assigned Loading(分配負載)

**計算公式:**
```
Assigned Loading = (Σ 分配工時) / (每週可用工時 × 目標週數) × 100%

其中:
- 分配工時: SUM(TestItemEngineer.AssignedHours WHERE UserId = X AND TestItem 未完成)
- 每週可用工時: Users.WeeklyWorkHours
- 目標週數: 依案件結束日期計算
```

**計算範圍:**
- 僅計算 Active 狀態案件
- 排除 OnHold、Completed、Deleted 的測項
- 即時計算,不儲存

**顏色標示:**
- ≤60%: 綠色(輕度負載)
- 61-80%: 黃色(中度負載)
- 81-100%: 橘色(高度負載)
- >100%: 紅色(超載警示)

---

#### 10.2.2 Actual Loading(實際負載)

**計算公式:**
```
Actual Loading = (Σ 實際工時) / (每週可用工時 × 已過週數) × 100%

其中:
- 實際工時: SUM(WorkLogs.Hours WHERE UserId = X AND WorkDate >= ProjectStartDate)
- 每週可用工時: Users.WeeklyWorkHours
- 已過週數: (今天 - 案件開始日期) / 7
```

**計算範圍:**
- 計算近期(如最近4週)實際工時
- 用於評估工程師實際工作量
- 與 Assigned Loading 比較可看出預估準確度

---

### 10.3 工時回報規則

#### 10.3.1 工時回報限制

1. **日期限制**
   - 工作日期不可晚於今天
   - 可補報過去的工時(無天數限制)
   - 未來日期拒絕送出

2. **工時範圍**
   - 最小: 0.5 小時
   - 最大: 12 小時
   - 超出範圍顯示錯誤訊息

3. **重複回報檢查**
   - 檢查條件: (TestItemId, UserId, WorkDate, RevisionId)
   - 若重複顯示錯誤:「此測項在此日期已有工時記錄」
   - 引導使用者使用「修改」功能

4. **延遲原因必填**
   - 若 Status = 'Delayed' → 至少選1個 DelayReason
   - 若未選擇拒絕送出
   - 可多選延遲原因

5. **補測版本必選 【v3.0】**
   - 必須選擇對應的 RevisionId
   - 若測項有多個版本,下拉選單顯示所有版本
   - 預設選擇最新版本

---

#### 10.3.2 工時修改限制

**工程師權限:**
- 僅能修改自己的工時
- 僅能修改7天內的記錄
- 超過7天顯示錯誤:「此記錄超過7天,無法修改,請聯絡主管」

**主管權限:**
- 可覆寫任何工時記錄(無天數限制)
- 需填寫覆寫理由(必填,限200字)
- 覆寫操作記錄至 AuditLog

**修改規則:**
- 修改時保留原值於 AuditLog.OldValues
- ModifiedByUserId 記錄修改者
- ModifiedDate 記錄修改時間

---

### 10.4 補測版本管理規則 **【v3.0 新增】**

#### 10.4.1 版本編號規則

**自動遞增:**
- 版本號從 1 開始(v1, v2, v3...)
- 系統自動計算下一個版本號
- 計算公式: `MAX(RevisionNumber) + 1 WHERE TestItemId = X`

**唯一性:**
- (TestItemId, RevisionNumber) 必須唯一
- 刪除的版本(IsDeleted=true)不計入

---

#### 10.4.2 補測類型定義 【v3.0】

| RevisionType | 中文說明 | 使用情境 |
|--------------|---------|---------|
| Command | 客戶指令 | 客戶要求變更測試內容(預設值) |
| Retest | 測試不通過 | 首次測試未通過,需重新測試 |
| Fix | 修正後重測 | 發現問題並修正後重新測試 |
| Others | 其他原因 | 不屬於以上類型的補測 |

**選擇指引:**
- 客戶要求變更 → Command
- 測試結果 Fail → Retest
- 發現 Bug 修正後 → Fix
- 其他情況 → Others

---

#### 10.4.3 補測觸發流程

**建立補測版本時:**
1. 系統自動計算版本號(v2, v3...)
2. 主管填寫補測資訊(類型、原因、預估工時)
3. 可選擇是否複製原工程師分配
4. 確認後:
   - 建立 TestItemRevision 記錄
   - 若選擇複製,建立對應的 TestItemEngineer 記錄
   - 測項狀態自動改為 InProgress
   - 記錄至 AuditLog

**工時記錄關聯:**
- 工程師回報工時時必須選擇版本
- WorkLog.RevisionId 記錄屬於哪個版本
- 可依版本統計工時

---

#### 10.4.4 補測版本回滾規則 【v3.0】

**回滾條件:**
- 僅能回滾至已完成的版本
- 不可回滾至 NotStarted 或 Deleted 的版本

**回滾流程:**
1. 選擇目標版本(如 v2)
2. 填寫回滾原因
3. 確認後:
   - 建立新版本(如 v4)
   - RevisionType 繼承目標版本
   - Reason = "回滾至 v{N}:{原因}"
   - 複製目標版本的工程師分配
   - 測項狀態改為 InProgress
4. 記錄至 AuditLog

**回滾後效果:**
- 不刪除原版本記錄
- 舊版本工時記錄保留
- 新版本獨立記錄工時

---

### 10.5 Soft Delete 機制 **【v2.3 補充】**

#### 10.5.1 支援 Soft Delete 的資料表

**完整支援:**
- Projects
- Regulations
- TestItems
- TestItemRevision **【v2.3】**
- WorkLogs **【v2.3】**

**操作規則:**
1. 刪除時設定:
   - IsDeleted = true
   - DeletedByUserId = 當前使用者ID
   - DeletedDate = 當前時間

2. 查詢時自動過濾:
   - 一般查詢: `WHERE IsDeleted = 0`
   - 關聯查詢: `JOIN ... WHERE IsDeleted = 0`

3. 保留歷史資料:
   - 已刪除記錄仍保留於資料庫
   - 稽核日誌可查詢刪除記錄
   - 主管可透過特殊介面查看已刪除記錄

---

#### 10.5.2 WorkLog Soft Delete 說明 **【v2.3】**

**刪除權限:**
- 僅 Manager 具有 WORKLOG_DELETE 權限
- 一般工程師無法刪除工時,僅能修改(7天內)

**刪除流程:**
1. 主管選擇要刪除的工時記錄
2. 填寫刪除理由(必填,限200字)
3. 確認後:
   - IsDeleted = true
   - DeletedByUserId = 主管UserId
   - DeletedDate = 當前時間
   - 刪除理由寫入 AuditLog

**刪除後影響:**
- 已刪除的 WorkLog 不出現在:
  - 工時查詢列表
  - 工時統計
  - Loading 計算
- 仍保留於:
  - 稽核日誌
  - 主管的「已刪除工時查詢」介面

**查詢已刪除工時:**
- 主管專用介面: `SCR-WORKLOG-DELETED-001`
- 顯示欄位:
  - 原工時資訊
  - 刪除者
  - 刪除時間
  - 刪除理由(從 AuditLog 取得)
- 不可復原(需聯絡系統管理員)

---

#### 10.5.3 TestItemRevision Soft Delete 說明 **【v2.3】**

**刪除權限:**
- Manager 具有 TESTITEM_REVISION_DELETE 權限(若有定義)
- 或透過 TESTITEM_DELETE 權限刪除

**刪除流程:**
1. 主管選擇要刪除的補測版本
2. 檢查是否有關聯的 WorkLog
3. 若有 WorkLog 顯示警告:「此版本有 X 筆工時記錄」
4. 確認後執行 Soft Delete

**刪除後影響:**
- 已刪除的 Revision 不出現在版本列表
- 關聯的 WorkLog 仍保留(WorkLog.RevisionId 不變)
- 查詢該 WorkLog 時,顯示「版本已刪除」

**查詢規則:**
```sql
-- 一般查詢(排除已刪除)
SELECT * FROM TestItemRevisions WHERE IsDeleted = 0

-- 包含已刪除(僅稽核用)
SELECT * FROM TestItemRevisions WHERE 1=1
```

---

### 10.6 延遲原因管理規則 **【v2.3 補充】**

#### 10.6.1 停用機制(非刪除)

**設計原則:**
- DelayReasons 不提供刪除功能
- 改用 IsActive 欄位控制啟用/停用
- 保留歷史資料完整性

**停用規則:**
1. **新增工時時:**
   - 延遲原因選單僅顯示 IsActive = 1 的原因
   - 查詢: `SELECT * FROM DelayReasons WHERE IsActive = 1 ORDER BY ReasonType, ReasonText`

2. **查詢歷史工時時:**
   - 顯示完整原因文字(不受 IsActive 影響)
   - 查詢: `SELECT DR.ReasonText FROM WorkLogDelayReason WLDR JOIN DelayReasons DR ON WLDR.DelayReasonId = DR.DelayReasonId`

3. **統計報表時:**
   - 包含停用的原因(完整統計)
   - 可標示「(已停用)」於圖例中

**停用流程:**
1. 主管點擊[停用]
2. 系統檢查使用次數
3. 顯示提示:「此原因已被使用 X 次,停用後將不顯示於選單,但歷史記錄仍保留」
4. 確認後設定 IsActive = 0
5. 記錄至 AuditLog

**啟用流程:**
- 點擊[啟用]設定 IsActive = 1
- 立即出現在延遲原因選單

---

#### 10.6.2 延遲原因統計

**統計範圍:**
- 包含所有原因(不論 IsActive)
- 統計欄位: 使用次數、最後使用日期

**統計查詢:**
```sql
SELECT 
  DR.DelayReasonId,
  DR.ReasonText,
  DR.ReasonType,
  DR.IsActive,
  COUNT(WLDR.WorkLogDelayReasonId) AS UsageCount,
  MAX(WL.WorkDate) AS LastUsedDate
FROM DelayReasons DR
LEFT JOIN WorkLogDelayReason WLDR ON DR.DelayReasonId = WLDR.DelayReasonId
LEFT JOIN WorkLogs WL ON WLDR.WorkLogId = WL.WorkLogId AND WL.IsDeleted = 0
GROUP BY DR.DelayReasonId, DR.ReasonText, DR.ReasonType, DR.IsActive
ORDER BY UsageCount DESC
```

**報表顯示:**
- 圓餅圖:各原因類型佔比
- 長條圖:各原因使用次數 Top 10
- 標示停用的原因(不同顏色或加註「(已停用)」)

---

## 11. 系統流程圖 **【v3.0 補充】**

### 11.1 建案完整流程

```
┌─────────────────────────────────────────────┐
│ 主管啟動 Wizard 建案流程                      │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ Step 1: 填寫案件基本資料                      │
│ - 案件名稱、客戶、優先順序、日期               │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ Step 2: 選擇法規與設定日期                    │
│ - 多選法規(FCC/NCC/CE/IC/TELEC)               │
│ - 填寫各法規測試日期                          │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ Step 3: 為每個法規新增測試項目                │
│ - 測項名稱、類型、場地、預估工時               │
│ - 可複製其他法規範本                          │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ Step 4: 分配工程師                            │
│ - 為每個測項分配 Main1/2/3 + Support         │
│ - 顯示工程師 Loading 警示                     │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ Step 5: 確認摘要                              │
│ - 檢視所有資訊                                │
│ - 可返回修改                                  │
└───────────────┬─────────────────────────────┘
                │
                ▼ [確認建立]
┌─────────────────────────────────────────────┐
│ 系統執行 Transaction:                         │
│ 1. INSERT Project                            │
│ 2. INSERT Regulations (N筆)                  │
│ 3. INSERT TestItems (M筆)                    │
│ 4. INSERT TestItemEngineers (P筆)            │
│ 5. INSERT AuditLog (建案記錄)                │
│ 6. COMMIT                                    │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 建案完成,導向案件詳細頁面                      │
└─────────────────────────────────────────────┘
```

---

### 11.2 工時回報與狀態更新流程

```
┌─────────────────────────────────────────────┐
│ 工程師選擇測項與補測版本                       │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 填寫工時資訊                                  │
│ - 工作日期、工時、狀態(InProgress/Delayed)     │
│ - [若Delayed] 選擇延遲原因(多選)               │
└───────────────┬─────────────────────────────┘
                │
                ▼ [送出]
┌─────────────────────────────────────────────┐
│ 系統驗證:                                     │
│ ✓ 日期不可晚於今天                            │
│ ✓ 工時範圍 0.5-12                             │
│ ✓ 不可重複回報(TestItemId+UserId+WorkDate)    │
│ ✓ 若Delayed需選延遲原因                       │
└───────────────┬─────────────────────────────┘
                │ 驗證通過
                ▼
┌─────────────────────────────────────────────┐
│ 系統執行 Transaction:                         │
│ 1. INSERT WorkLog                            │
│ 2. IF Delayed THEN                           │
│      INSERT WorkLogDelayReason (N筆)         │
│    END IF                                    │
│ 3. 更新 TestItem.Status (依規則計算)          │
│ 4. 更新 Regulation.Status (依規則計算)        │
│ 5. 更新 Project.Status (依規則計算)           │
│ 6. INSERT AuditLog (狀態變更記錄)             │
│ 7. COMMIT                                    │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 顯示成功訊息,更新工時統計                      │
└─────────────────────────────────────────────┘
```

---

### 11.3 補測版本建立流程 **【v3.0】**

```
┌─────────────────────────────────────────────┐
│ 主管選擇測項,點擊[建立補測]                   │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 填寫補測資訊                                  │
│ - 補測類型(Command/Retest/Fix/Others)         │
│ - 補測原因                                    │
│ - 預估工時                                    │
│ - 是否複製原工程師分配                         │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 顯示確認摘要                                  │
│ - 當前版本: v{N}                              │
│ - 新版本: v{N+1}                              │
│ - 工程師分配預覽                              │
└───────────────┬─────────────────────────────┘
                │
                ▼ [確認建立]
┌─────────────────────────────────────────────┐
│ 系統執行 Transaction:                         │
│ 1. 計算新版本號 = MAX(RevisionNumber) + 1    │
│ 2. INSERT TestItemRevision                   │
│ 3. IF 選擇複製工程師 THEN                     │
│      INSERT TestItemEngineer (複製原分配)     │
│    END IF                                    │
│ 4. UPDATE TestItem.Status = 'InProgress'     │
│ 5. 觸發 Regulation 與 Project 狀態重算        │
│ 6. INSERT AuditLog (補測建立記錄)             │
│ 7. COMMIT                                    │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 補測版本建立完成                              │
│ - 測項狀態 → InProgress                       │
│ - 工程師可開始回報此版本工時                   │
└─────────────────────────────────────────────┘
```

---

### 11.4 Loading 計算與警示流程

```
┌─────────────────────────────────────────────┐
│ 主管分配工程師給測項                          │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 系統即時計算工程師 Loading                    │
│ Assigned Loading =                           │
│   Σ(分配工時) / (每週可用工時 × 目標週數)     │
└───────────────┬─────────────────────────────┘
                │
                ▼
        ┌───────┴───────┐
        │               │
        ▼               ▼
┌─────────────┐   ┌─────────────┐
│ ≤ 100%      │   │ > 100%      │
│ 正常範圍     │   │ 超載警示     │
└──────┬──────┘   └──────┬──────┘
       │                 │
       │                 ▼
       │          ┌─────────────────────────┐
       │          │ 顯示警告對話框:          │
       │          │ 「分配後 Loading 將達    │
       │          │  {X}%,是否繼續?」       │
       │          └──────┬──────────────────┘
       │                 │
       │          ┌──────┴──────┐
       │          │             │
       │          ▼             ▼
       │    [確認繼續]      [取消]
       │          │             │
       └──────────┴─────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────┐
│ 執行分配,記錄至 TestItemEngineer              │
└─────────────────────────────────────────────┘
```

---

## 12. 安全機制與資料保護 **【v3.0 補充】**

### 12.1 JWT Token 機制 **【v2.1 補充】**

#### 12.1.1 JWT 結構

**Header:**
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload (建議欄位):**
```json
{
  "sub": "UserId",
  "email": "user@example.com",
  "name": "顯示名稱",
  "role": "Manager",
  "permissions": ["PROJECT_CREATE", "TESTITEM_VIEW", ...],
  "iat": 1234567890,
  "exp": 1234596690
}
```

**Signature:**
```
HMACSHA256(
  base64UrlEncode(header) + "." + base64UrlEncode(payload),
  SecretKey
)
```

**重要原則:**
- Payload 不放敏感資訊(密碼、詳細工時、Email 內文)
- SecretKey 僅存於伺服器端,絕不外流
- Token 過期時間可調整(預設480分鐘=8小時)

---

#### 12.1.2 Token 驗證流程

```
┌─────────────────────────────────────────────┐
│ 客戶端發送 API 請求                           │
│ Header: Authorization: Bearer {JWT_TOKEN}    │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 伺服器驗證 Token:                             │
│ 1. 檢查 Token 格式                            │
│ 2. 驗證 Signature(使用 SecretKey)            │
│ 3. 檢查 exp(過期時間)                         │
│ 4. 解析 Payload 取得 UserId 與權限            │
└───────────────┬─────────────────────────────┘
                │
        ┌───────┴───────┐
        │               │
        ▼               ▼
┌─────────────┐   ┌─────────────┐
│ Token 有效   │   │ Token 無效   │
└──────┬──────┘   └──────┬──────┘
       │                 │
       ▼                 ▼
┌─────────────┐   ┌──────────────────┐
│ 執行 API     │   │ 返回 401          │
│ 進行權限檢查 │   │ Unauthorized     │
└─────────────┘   └──────────────────┘
```

---

### 12.2 權限檢查機制

#### 12.2.1 API 層級權限檢查

**檢查流程:**
```
1. 從 JWT Token 解析 UserId
2. 查詢使用者有效權限:
   SELECT PermissionCode 
   FROM (
     -- 群組權限
     SELECT P.PermissionCode 
     FROM UserPermissions UP
     JOIN PermissionGroupMapping PGM ON UP.PermissionGroupId = PGM.PermissionGroupId
     JOIN Permissions P ON PGM.PermissionId = P.PermissionId
     WHERE UP.UserId = {UserId} 
       AND UP.RevokedDate IS NULL
       AND P.IsActive = 1
     
     UNION
     
     -- 個別權限
     SELECT P.PermissionCode
     FROM UserPermissions UP
     JOIN Permissions P ON UP.PermissionId = P.PermissionId
     WHERE UP.UserId = {UserId}
       AND UP.RevokedDate IS NULL
       AND (UP.ExpiryDate IS NULL OR UP.ExpiryDate > GETDATE())
       AND P.IsActive = 1
   ) AS EffectivePermissions

3. 檢查 API 所需權限是否在有效權限清單中
4. 若無權限 → 返回 403 Forbidden
5. 若有權限 → 執行 API 邏輯
```

---

#### 12.2.2 資料層級權限檢查

**工程師存取限制:**
```sql
-- 工程師僅能查詢自己的工時
SELECT * FROM WorkLogs 
WHERE UserId = {CurrentUserId} 
  AND IsDeleted = 0

-- 工程師僅能查詢自己負責的測項
SELECT T.* FROM TestItems T
JOIN TestItemEngineer TE ON T.TestItemId = TE.TestItemId
WHERE TE.UserId = {CurrentUserId}
  AND T.IsDeleted = 0
```

**主管無限制:**
```sql
-- 主管可查詢所有資料
SELECT * FROM WorkLogs WHERE IsDeleted = 0
SELECT * FROM TestItems WHERE IsDeleted = 0
```

---

### 12.3 登入安全機制

#### 12.3.1 失敗鎖定機制

**規則:**
1. 連續失敗5次 → 鎖定帳號10分鐘
2. 記錄至 Users.FailedLoginAttempts
3. 記錄鎖定時間至 Users.LockoutEndTime

**流程:**
```
┌─────────────────────────────────────────────┐
│ 使用者輸入帳號密碼                             │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 檢查 LockoutEndTime                          │
│ IF LockoutEndTime > NOW() THEN               │
│   返回「帳號已鎖定,請於 {時間} 後再試」        │
│ END IF                                       │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│ 驗證帳號密碼                                  │
└───────────────┬─────────────────────────────┘
                │
        ┌───────┴───────┐
        │               │
        ▼               ▼
┌─────────────┐   ┌─────────────────────────┐
│ 驗證成功     │   │ 驗證失敗                 │
└──────┬──────┘   └──────┬──────────────────┘
       │                 │
       ▼                 ▼
┌─────────────┐   ┌─────────────────────────┐
│ 重設失敗次數 │   │ FailedLoginAttempts++    │
│ = 0         │   │ IF >= 5 THEN             │
│ 更新最後登入 │   │   LockoutEndTime =       │
│ 時間         │   │   NOW() + 10分鐘         │
│ 產生 JWT     │   │ END IF                   │
└─────────────┘   └─────────────────────────┘
```

---

#### 12.3.2 密碼安全規則

**密碼複雜度:**
- 長度: 8-20 字元
- 必須包含: 英文字母 + 數字
- 建議包含: 特殊符號

**密碼儲存:**
- 使用 bcrypt 或 PBKDF2 演算法
- 加入 Salt(隨機鹽值)
- 僅儲存 Hash 值,不可逆

**範例(概念):**
```
原密碼: MyPassword123
Salt: RandomSalt456
Hash: bcrypt(MyPassword123 + RandomSalt456) → $2a$10$abcd...xyz
儲存: PasswordHash = $2a$10$abcd...xyz
```

---

### 12.4 資料加密與傳輸安全

#### 12.4.1 傳輸層加密

**HTTPS 強制:**
- 所有 API 通訊必須使用 HTTPS
- TLS 1.2 或以上版本
- 禁止 HTTP 明文傳輸

**API 請求範例:**
```
HTTPS GET /api/projects
Host: rf-scheduling.example.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

#### 12.4.2 敏感資料保護

**需加密儲存的資料:**
- 密碼: bcrypt Hash
- 重設 Token: GUID 或隨機字串

**不需加密但需保護的資料:**
- Email: 儲存明文,但限制存取權限
- 工時記錄: 儲存明文,依權限控管
- 稽核日誌: 儲存明文,僅主管可查

**禁止儲存的資料:**
- 信用卡資訊
- 身分證字號
- 其他個資(非必要)

---

## 13. 錯誤處理與異常管理

### 13.1 錯誤分類與處理原則

#### 13.1.1 錯誤分類

| 錯誤類型 | HTTP 狀態碼 | 處理方式 |
|---------|------------|---------|
| 驗證錯誤 | 400 Bad Request | 顯示具體欄位錯誤訊息 |
| 未授權 | 401 Unauthorized | 導向登入頁面 |
| 權限不足 | 403 Forbidden | 顯示「無權限執行此操作」 |
| 資源不存在 | 404 Not Found | 顯示「找不到指定資源」 |
| 業務邏輯錯誤 | 409 Conflict | 顯示業務規則錯誤訊息 |
| 伺服器錯誤 | 500 Internal Server Error | 顯示通用錯誤,記錄詳細日誌 |

---

#### 13.1.2 錯誤訊息設計原則

**好的錯誤訊息:**
- ✓ 「工作日期不可晚於今天」
- ✓ 「此 Email 已被使用,請使用其他 Email」
- ✓ 「工時範圍需在 0.5-12 小時之間」
- ✓ 「此測項在 2025-11-21 已有工時記錄,請使用修改功能」

**避免的錯誤訊息:**
- ✗ 「錯誤」
- ✗ 「系統異常」
- ✗ 「null reference exception」
- ✗ 「database connection failed」

---

### 13.2 Transaction 與 Rollback 機制

#### 13.2.1 關鍵操作的 Transaction 保護

**需使用 Transaction 的操作:**
1. **建案流程**(插入 Project + Regulations + TestItems + TestItemEngineers)
2. **工時回報**(插入 WorkLog + WorkLogDelayReason + 更新狀態)
3. **補測建立**(插入 TestItemRevision + TestItemEngineers + 更新狀態)
4. **工時覆寫**(更新 WorkLog + 插入 AuditLog)

**Transaction 範例(偽碼):**
```sql
BEGIN TRANSACTION;

TRY
  -- 1. 插入主要資料
  INSERT INTO Projects (...) VALUES (...);
  SET @ProjectId = SCOPE_IDENTITY();
  
  -- 2. 插入關聯資料
  INSERT INTO Regulations (...) VALUES (...);
  INSERT INTO TestItems (...) VALUES (...);
  
  -- 3. 插入稽核日誌
  INSERT INTO AuditLogs (...) VALUES (...);
  
  -- 4. 提交
  COMMIT TRANSACTION;
  
CATCH
  -- 發生錯誤時回滾
  ROLLBACK TRANSACTION;
  
  -- 記錄錯誤日誌
  INSERT INTO ErrorLogs (...) VALUES (...);
  
  -- 拋出例外
  THROW;
END TRY
```

---

#### 13.2.2 資料一致性檢查

**檢查點:**
1. **外鍵約束:** 確保關聯資料存在
2. **唯一性約束:** 確保不重複插入
3. **業務規則驗證:** 執行前先檢查

**檢查範例:**
```sql
-- 檢查 TestItem 是否存在
IF NOT EXISTS(SELECT 1 FROM TestItems WHERE TestItemId = @TestItemId AND IsDeleted = 0)
BEGIN
  RAISERROR('測項不存在', 16, 1);
  RETURN;
END

-- 檢查是否重複回報
IF EXISTS(SELECT 1 FROM WorkLogs 
          WHERE TestItemId = @TestItemId 
            AND UserId = @UserId 
            AND WorkDate = @WorkDate 
            AND RevisionId = @RevisionId
            AND IsDeleted = 0)
BEGIN
  RAISERROR('此測項在此日期已有工時記錄', 16, 1);
  RETURN;
END
```

---

### 13.3 日誌記錄機制

#### 13.3.1 應用程式日誌

**日誌層級:**
- **ERROR:** 系統錯誤,需立即處理
- **WARN:** 警告訊息,可能影響功能
- **INFO:** 一般資訊,記錄重要操作
- **DEBUG:** 除錯資訊,開發環境使用

**記錄內容:**
```
[時間] [層級] [使用者] [操作] [訊息]
2025-11-21 14:30:15 INFO User123 LOGIN 使用者登入成功
2025-11-21 14:35:20 WARN User123 WORKLOG_CREATE Loading 超過 100%
2025-11-21 14:40:30 ERROR User456 PROJECT_CREATE 資料庫連線失敗
```

**儲存方式:**
- 開發環境: 檔案 + Console
- 正式環境: 檔案 + 資料庫(ErrorLogs 表)

---

#### 13.3.2 稽核日誌(AuditLogs)

**記錄範圍:**
- 所有資料異動(Create/Update/Delete)
- 狀態變更
- 權限變更
- 工時覆寫
- 登入/登出

**記錄格式(JSON):**
```json
{
  "TableName": "WorkLogs",
  "RecordId": 12345,
  "Operation": "Update",
  "UserId": 101,
  "ChangedDate": "2025-11-21T14:30:15",
  "OldValues": {
    "Hours": 4.0,
    "Status": "InProgress"
  },
  "NewValues": {
    "Hours": 5.5,
    "Status": "InProgress"
  }
}
```

---

## 14. 測試策略

### 14.1 測試範圍

#### 14.1.1 單元測試(Unit Testing)

**測試對象:**
- 業務邏輯類別
- 資料驗證規則
- 狀態計算邏輯
- Loading 計算公式

**測試工具:**
- xUnit / NUnit (C#)
- JUnit (Java)
- Jest (JavaScript)

**範例測試案例:**
```csharp
[Test]
public void TestItemStatus_WhenWorkLogDelayed_ShouldBeDelayed()
{
    // Arrange
    var testItem = new TestItem { Status = "InProgress" };
    var workLog = new WorkLog { Status = "Delayed" };
    
    // Act
    var result = StatusCalculator.CalculateTestItemStatus(testItem, workLog);
    
    // Assert
    Assert.AreEqual("Delayed", result);
}
```

---

#### 14.1.2 整合測試(Integration Testing)

**測試對象:**
- API 端點
- 資料庫操作
- Transaction 正確性
- 權限檢查

**測試案例範例:**
```
測試案例: 工時回報整合測試
前置條件:
  - 測試資料庫已建立測項 TestItem001
  - 測試使用者 Engineer001 已分配至該測項
  
步驟:
  1. 呼叫 POST /api/worklogs
     Body: { TestItemId: 001, Hours: 4.0, Status: "InProgress" }
  2. 檢查回應: 201 Created
  3. 查詢資料庫: WorkLog 已插入
  4. 查詢資料庫: TestItem.Status = "InProgress"
  5. 查詢資料庫: AuditLog 已記錄
  
期望結果:
  - WorkLog 成功建立
  - 狀態正確更新
  - 稽核日誌正確記錄
```

---

#### 14.1.3 UI 測試(End-to-End Testing)

**測試工具:**
- Selenium
- Cypress
- Playwright

**測試流程範例:**
```
測試案例: Wizard 建案流程
步驟:
  1. 登入系統(Manager 帳號)
  2. 點擊[新增案件]
  3. Step 1 填寫案件資訊
  4. 點擊[下一步]
  5. Step 2 選擇法規
  6. 點擊[下一步]
  7. Step 3 新增測項
  8. 點擊[下一步]
  9. Step 4 分配工程師
  10. 點擊[完成建案]
  11. 驗證案件列表顯示新案件
  
期望結果:
  - 案件成功建立
  - 可在列表中查詢到
```

---

### 14.2 效能測試

#### 14.2.1 負載測試

**測試目標:**
- 50 位使用者同時上線
- 回應時間 < 2 秒

**測試工具:**
- JMeter
- LoadRunner
- k6

**測試情境:**
```
情境 1: 查詢案件列表
  - 50 個並發請求
  - 持續 5 分鐘
  - 期望平均回應時間 < 2 秒

情境 2: 工時回報
  - 20 個並發請求
  - 持續 10 分鐘
  - 期望平均回應時間 < 1.5 秒

情境 3: Loading 計算
  - 10 個並發請求
  - 期望平均回應時間 < 2 秒
```

---

#### 14.2.2 壓力測試

**測試目標:**
- 找出系統極限
- 驗證 Graceful Degradation

**測試方法:**
```
1. 逐步增加並發使用者數: 50 → 100 → 200 → 500
2. 觀察回應時間變化
3. 記錄系統崩潰點
4. 分析瓶頸所在(CPU/Memory/Database)
```

---

## 15. 部署與維運

### 15.1 部署架構

#### 15.1.1 建議部署架構

```
┌─────────────────────────────────────────────┐
│          Load Balancer (Optional)           │
└───────────────┬─────────────────────────────┘
                │
        ┌───────┴────────┐
        │                │
        ▼                ▼
┌──────────────┐  ┌──────────────┐
│ Web Server 1 │  │ Web Server 2 │
│ (IIS / Nginx)│  │ (IIS / Nginx)│
└──────┬───────┘  └──────┬───────┘
       │                 │
       └────────┬────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│         Application Server                  │
│         (API Backend)                       │
└───────────────┬─────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────┐
│         Database Server                     │
│         (SQL Server)                        │
└─────────────────────────────────────────────┘
```

---

#### 15.1.2 環境配置

**開發環境(Development):**
- 單機部署
- 使用 LocalDB 或 SQL Express
- Debug 模式
- 詳細日誌

**測試環境(Staging):**
- 與正式環境相同架構
- 使用測試資料
- 進行整合測試與 UAT

**正式環境(Production):**
- 高可用性配置
- SQL Server Standard 或 Enterprise
- Release 模式
- 關鍵日誌

---

### 15.2 備份與還原策略

#### 15.2.1 資料庫備份

**備份策略:**
```
完整備份: 每天 02:00
差異備份: 每 6 小時
交易記錄備份: 每 1 小時(正式環境)

保留政策:
  - 每日備份: 保留 30 天
  - 每週備份: 保留 3 個月
  - 每月備份: 保留 1 年
```

**備份驗證:**
- 每週進行還原測試
- 驗證備份檔案完整性
- 記錄還原時間(RTO)

---

#### 15.2.2 災難復原計畫

**RTO(Recovery Time Objective):** 4 小時
**RPO(Recovery Point Objective):** 1 小時

**復原步驟:**
```
1. 啟動備援伺服器
2. 還原最新的完整備份
3. 套用差異備份
4. 套用交易記錄備份
5. 驗證資料完整性
6. 切換 DNS 至備援伺服器
7. 通知使用者系統已恢復
```

---

### 15.3 監控與告警

#### 15.3.1 監控項目

**系統監控:**
- CPU 使用率
- Memory 使用率
- Disk 空間
- Network 流量

**應用程式監控:**
- API 回應時間
- 錯誤率
- 登入成功/失敗次數
- 並發使用者數

**資料庫監控:**
- 查詢執行時間
- 死鎖發生次數
- 連線池使用率
- 資料庫大小增長

---

#### 15.3.2 告警設定

**告警條件:**
```
嚴重(Critical):
  - API 回應時間 > 5 秒
  - 錯誤率 > 5%
  - Database 連線失敗
  - Disk 空間 < 10%
  → 立即通知 Email + SMS

警告(Warning):
  - API 回應時間 > 3 秒
  - CPU 使用率 > 80%
  - Memory 使用率 > 85%
  → 發送 Email 通知

資訊(Info):
  - 登入失敗次數異常
  - Loading 超過 100%
  → 記錄日誌
```

---

## 16. 附錄

### 16.1 術語表

| 術語 | 英文 | 說明 |
|------|------|------|
| 案件 | Project | RF測試專案 |
| 法規 | Regulation | 測試需符合的法規(FCC/NCC/CE等) |
| 測項 | Test Item | 具體測試項目(Conducted/Radiated等) |
| 工時 | Work Log | 工程師記錄的工作時數 |
| 補測 | Revision | 測試項目的重新測試版本 |
| Loading | Loading | 工程師的工作負載百分比 |
| 延遲 | Delay | 測試項目未如期完成 |
| 主責 | Main | 主要負責工程師(Main1/2/3) |
| 支援 | Support | 協助測試的工程師 |
| 軟刪除 | Soft Delete | 標記為刪除但保留資料 |
| 稽核 | Audit | 記錄資料異動歷程 |

---

### 16.2 參考文件

**內部文件:**
- 資料庫設計文件(Database Design Document)
- API 規格文件(API Specification)
- 使用者操作手冊(User Manual)
- 系統管理手冊(Administrator Guide)

**外部標準:**
- JWT (RFC 7519)
- RESTful API 設計最佳實踐
- OWASP Top 10 安全指南
- ISO 27001 資訊安全標準

---

### 16.3 修訂歷史

| 版本 | 日期 | 修訂者 | 修訂內容摘要 |
|------|------|-------|-------------|
| v1.0 | 2025-11-14 | SA Team | 初版分析文件 |
| v2.0 | 2025-11-17 | SA Team | 調整職責邊界,移除技術實作細節 |
| v2.1 | 2025-11-19 | SA Team | 補充混合登入、JWT 安全性、IAM 權限模型 |
| v2.2 | 2025-11-20 | SA Team | 補充 TestItem 狀態計算、完整 ERD 圖 |
| v2.3 | 2025-11-20 | SA Team | WorkLog/TestItemRevision Soft Delete、DelayReason 停用機制 |
| **v3.0** | **2025-11-21** | **SA Team** | **重大更新:** <br>- TestItemRevision 完整定義(RevisionType、UI 流程)<br>- 工程師分配相關權限(TESTITEM_ASSIGN_ENGINEER/SUPPORT/REMOVE_ENGINEER)<br>- Regulation 維護權限與 UI(REGULATION_ADD/DISABLE/REMOVE)<br>- 補測版本管理權限(TESTITEM_REVISION_CREATE/ROLLBACK)<br>- 統一權限命名(AUDIT_VIEW、SYSTEM_SETTING)<br>- 補充 8 個缺少的 UI 介面定義<br>- 強化 Email 合併邏輯(首次由主管新增)<br>- TestItemEngineer RoleType 詳細定義<br>- 主管案件總覽改為 GridControl 列表模式<br>- 補充完整業務規則與流程圖 |

---

## 17. 結語

本系統分析文件(SA v3.0)定義了 RF 案件排程系統的完整需求與規格,包含:

✅ **核心功能:** 案件管理、法規管理、測項管理、工時管理、Loading 分析、延遲管理
✅ **權限機制:** 完整的 IAM 權限模型(Permission/PermissionGroup/UserPermission)
✅ **安全機制:** JWT Token、密碼加密、登入鎖定、權限檢查
✅ **補測管理:** 完整的版本管理機制(RevisionType、回滾功能)
✅ **資料保護:** Soft Delete、稽核日誌、備份策略
✅ **UI 規格:** 完整的介面定義與操作流程

**後續步驟:**
1. 技術團隊依據本文件進行系統設計(System Design)
2. 開發團隊實作功能模組
3. 測試團隊依據測試策略執行測試
4. 專案經理追蹤專案進度與品質

**文件維護:**
- 本文件為活文件(Living Document)
- 需求變更時需更新版本號與修訂歷史
- 所有變更需經過變更控制流程(Change Control)

---

**文件完成日期:** 2025-11-21  
**文件版本:** v3.0  
**文件狀態:** 已核准(Approved)  
**下次審查日期:** 2025-12-21