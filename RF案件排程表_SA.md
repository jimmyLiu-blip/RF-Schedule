# 📘 RF案件排程系統 — 系統分析文件 (SA v2.2)

---

## 📖 文件說明

**版本歷程：**
- v1.0 (2025-11-14)：初版分析文件
- v2.0 (2025-11-17)：調整職責邊界，移除技術實作細節
- v2.1 (2025-11-19)：補充混合登入機制（Local + Windows 驗證）、JWT 安全性說明、IAM 權限模型（Permission / PermissionGroup / UserPermission）
- v2.2 (2025-11-20)：補充 TestItem 狀態計算邏輯、狀態逆向操作規則、完整 ERD 圖
- v2.3 (2025-11-20)：WorkLog、TestItemRevision Soft Delete 機制說明、Regulation 狀態計算邏輯、DelayReason 停用機制說明、PermissionGroupMapping 欄位補充

---

## 1. 專案背景與問題陳述

### 1.1 現況描述

RF測試實驗室目前面臨的挑戰：

**案件特性：**
- 單一案件需測試多種法規（FCC、NCC、CE、IC、TELEC等）
- 每個法規包含多種測試項目（Conducted、Radiated、Blocking等）
- 測試需在不同場地進行（Lab A、Lab B、Lab C等）
- 涉及多位工程師輪班作業（白班、小夜班、大夜班）

**當前管理方式：**
- 使用Excel試算表記錄案件資訊
- 透過Email與口頭溝通協調工作
- 工時記錄在個人筆記或Excel中
- 進度追蹤依賴人工整理

---

### 1.2 核心問題分析

#### 問題1：資訊分散與查詢困難
- **現象：** 案件資料散落在多個Excel檔案與Email中
- **影響：** 難以快速找到特定案件、法規或測項的進度

#### 問題2：工時追蹤不透明
- **現象：** 工程師各自記錄工時，無統一格式
- **影響：** 
  - 主管無法即時掌握實際工時
  - 補測版本(v1/v2/v3)工時混雜，無法比較分析
  - 預估與實際工時差異難以分析

#### 問題3：工程師負載無法視覺化
- **現象：** 主管無法即時看到每位工程師的負載狀況
- **影響：**
  - 任務分配不均，導致部分工程師超載
  - 無法提前預警資源瓶頸
  - 影響案件交期

#### 問題4：延遲原因難以追蹤
- **現象：** 僅知道案件「延遲」，但不清楚根本原因
- **影響：**
  - 無法針對性改善流程
  - 無法釐清責任歸屬（設備、客戶、人力、場地）
  - 重複發生相同問題

#### 問題5：缺乏權限控管與稽核
- **現象：** 
  - 任何人都可修改Excel，無記錄誰改了什麼
  - 資料被誤刪或覆蓋時無法追查
- **影響：** 資料可信度降低，發生爭議時無憑據

#### 問題6：多人協作版本衝突
- **現象：** 兩人同時編輯同一Excel，後存檔者覆蓋前者
- **影響：** 資料遺失，需重新輸入

---

### 1.3 使用者身份合併機制（Email-Based Identity Merge）

系統採用 **Email 作為跨登入來源的唯一身份識別（Primary Identity Key）**，用於整合 Local 帳密登入與 Windows AD 驗證登入，使同一位使用者以不同方式登入時，仍能被識別為同一個系統帳號。

#### （1）合併原則
當使用者以任一方式登入（Local 或 AD）時，系統執行：

1. 從登入來源取得 Email（Local 由使用者輸入；AD 由 AD Server 提供）
2. 查詢：`SELECT * FROM Users WHERE Email = {LoginEmail}`
3. 若找到相同 Email → 視為同一使用者，不新增 User 紀錄
4. 若找不到 → 建立新使用者（依登入來源決定 AuthType）

#### （2）Email 為唯一身份識別
- Email 欄位必須為 **唯一值（UNIQUE）**
- Local / AD 兩種登入方式都需提供 Email
- 不允許兩筆使用者用相同 Email

#### （3）使用者分類

| 登入方式 | Email 來源 | 使用欄位 |
|----------|------------|----------|
| Local 帳密登入 | 使用者輸入 | Account + Email |
| AD 驗證登入 | AD 提供 | ADAccount + Email |

無論使用者以哪種方式登入，Email 相同即視為同一使用者。

---

## 2. 專案目標與成功指標

### 2.1 專案願景

> 建立一套集中化、可追蹤、可分析的RF案件排程與工時管理系統，讓工程師專注測試工作，讓主管即時掌握專案進度與資源負載。

---

### 2.2 專案目標

#### 目標1：集中化資料管理
- **描述：** 所有案件、法規、測項、工時資料集中儲存於單一系統
- **衡量指標：** 
  - 90%以上的資料查詢可在系統內完成
  - 不再需要Excel進行案件管理

#### 目標2：標準化流程
- **描述：** 透過Wizard流程標準化建案步驟

#### 目標3：即時工時追蹤
- **描述：** 工程師每日回報工時，系統即時累計與統計
- **衡量指標：** 
  - 工時回報即時性達100%（當日回報）

#### 目標4：視覺化資源負載
- **描述：** 主管可即時查看每位工程師的負載狀況

#### 目標5：延遲根因分析
- **描述：** 系統記錄延遲原因，支援統計分析
- **衡量指標：** 
  - 100%延遲案件有明確原因記錄
  - 每季產出延遲原因分析報告

#### 目標6：完整稽核機制
- **描述：** 所有資料異動皆可追蹤

---

## 3. 利害關係人分析

### 3.1 主要利害關係人

#### 3.1.1 工程師 (Engineer)
- **人數：** 約20人
- **需求：**
  - 快速查看自己負責的測項
  - 簡便的工時回報介面
  - 了解自己的工作進度
- **痛點：**
  - 現有Excel難以快速找到自己的任務
  - 工時記錄繁瑣易遺漏
  - 不清楚自己的Loading狀況
- **期望：**
  - 工時回報時間<2分鐘/次
  - 可隨時查看歷史工時記錄
  - 介面直覺易用

---

#### 3.1.2 實驗室主管 (Manager)
- **人數：** 3-5人
- **需求：**
  - 快速建立新案件
  - 即時掌握所有案件進度
  - 視覺化工程師負載
  - 分析延遲原因
  - 產出各類管理報表
- **痛點：**
  - Excel建案繁瑣易錯
  - 無法即時看到資源瓶頸
  - 手動彙整報表耗時
  - 延遲原因難以追蹤
- **期望：**
  - Wizard引導式建案
  - Dashboard一目了然
  - 一鍵產出報表
  - 完整稽核追蹤

---

## 4. 系統範圍定義

### 4.1 系統邊界

#### 4.1.1 納入範圍 (In Scope)

**核心功能：**
1. ✅ 案件管理（建立、查詢、修改、刪除）
2. ✅ 法規管理（多法規支援）
3. ✅ 測試項目管理（測項定義、工程師分配）
4. ✅ 工時回報（工程師端）
5. ✅ 工時查詢與統計（主管端）
6. ✅ Loading計算與視覺化
7. ✅ 延遲原因管理與分析
8. ✅ 補測版本管理（v1/v2/v3...）
9. ✅ 稽核日誌（資料異動追蹤）
10. ✅ 權限控管（工程師/主管）
11. ✅ 忘記密碼功能（Email重設）
12. ✅ 報表產出（進度、工時、延遲分析）

**資料管理：**
- ✅ 使用者管理
- ✅ 角色管理（Engineer/Manager）
- ✅ 延遲原因字典維護
- ✅ Soft Delete機制

---

### 4.2 系統與外部系統互動

```
┌─────────────────────────────────────┐
│   RF案件排程系統 (本系統)             │
│                                     │
│  ┌──────┐  ┌──────┐  ┌──────┐       │
│  │案件  │  │工時  │  │Loading│       │
│  │管理  │  │管理  │  │分析  │        │
│  └──────┘  └──────┘  └──────┘       │ 
│                                     │
└─────────────────────────────────────┘
         ↓ (Email)
┌─────────────────┐
│  Email Server   │ (發送密碼重設信、新帳號通知)
└─────────────────┘
```

**說明：**
- 系統僅與Email Server互動（發送系統通知信）
- 資料匯出格式：Excel（供人工匯入其他系統）

---

## 5. 功能需求總覽

### 5.1 功能模組架構

```
RF案件排程系統
│
├─ 1. 認證與授權模組
│   ├─ 1.1 使用者登入（Local 帳號）
│   ├─ 1.2 Windows 驗證登入（AD 帳號）
│   ├─ 1.3 忘記密碼（Email重設）
│   ├─ 1.4 角色與權限控管（RBAC + IAM）
│   │   ├─ 角色：Engineer / Manager / Admin
│   │   ├─ 權限：以 PermissionCode 控制
│   │   └─ 權限群組：預設權限集合，可個別加減
│   └─ 1.5 登入安全機制（失敗次數限制、Token 過期）
│
├─ 2. 案件管理模組
│   ├─ 2.1 Wizard建案流程
│   ├─ 2.2 案件查詢與瀏覽
│   ├─ 2.3 案件修改（含理由記錄）
│   ├─ 2.4 案件刪除（Soft Delete）
│   └─ 2.5 案件狀態自動計算
│
├─ 3. 測試項目管理模組
│   ├─ 3.1 測項新增與修改
│   ├─ 3.2 工程師分配（Main/Sub）
│   ├─ 3.3 補測版本管理（Revision）
│   ├─ 3.4 測項狀態追蹤與逆向操作
│   └─ 3.5 測項刪除（Soft Delete）
│
├─ 4. 工時管理模組
│   ├─ 4.1 工時回報（工程師端）
│   ├─ 4.2 工時查詢（工程師可查自己）
│   ├─ 4.3 工時修改（7天內可改）
│   ├─ 4.4 工時統計（主管端）
│   └─ 4.5 工時覆寫（主管含理由）
│
├─ 5. Loading分析模組
│   ├─ 5.1 工程師Loading總覽
│   ├─ 5.2 工程師Loading明細
│   └─ 5.3 Loading報表匯出
│
├─ 6. 延遲管理模組
│   ├─ 6.1 延遲原因字典維護
│   ├─ 6.2 延遲案件追蹤
│   └─ 6.3 延遲原因分析報表
│
├─ 7. 使用者管理模組
│   ├─ 7.1 使用者新增
│   ├─ 7.2 使用者修改
│   ├─ 7.3 使用者停用/啟用
│   └─ 7.4 使用者查詢
│
├─ 8. 稽核日誌模組
│   ├─ 8.1 自動記錄資料異動
│   ├─ 8.2 稽核日誌查詢
│   └─ 8.3 稽核日誌詳細檢視
│
├─ 9. 報表模組
│   ├─ 9.1 案件進度報表
│   ├─ 9.2 工時統計報表
│   ├─ 9.3 延遲分析報表
│   └─ 9.4 報表匯出（Excel）
│
└─ 10. 系統設定模組
    ├─ 10.1 一般參數設定
    ├─ 10.2 Email設定
    └─ 10.3 資料保留政策
```

---

### 5.2 使用案例圖 (Use Case Diagram)

```
┌─────────────────────────────────────────────┐
│           RF案件排程系統                      │
│                                             │
│  ┌──────────────────────────────────────┐  │
│  │  認證與授權                           │  │
│  │  • 登入（Local / AD）                 │  │
│  │  • 忘記密碼                           │  │
│  └──────────────────────────────────────┘  │
│                                             │
│  ┌──────────────────────────────────────┐  │
│  │  工程師功能                           │  │
│  │  • 查看我的測項                       │  │
│  │  • 回報工時                           │  │
│  │  • 查詢工時記錄                       │  │
│  │  • 修改工時(7天內)                    │  │
│  │  • 取消測項完成狀態                   │  │
│  └──────────────────────────────────────┘  │
│             ↑                               │
│         Engineer                            │
│                                             │
│  ┌──────────────────────────────────────┐  │
│  │  主管功能                             │  │
│  │  • Wizard建案                         │  │
│  │  • 管理案件                           │  │
│  │  • 管理測項與狀態覆寫                 │  │
│  │  • 分配工程師                         │  │
│  │  • 建立補測版本                       │  │
│  │  • 查看Loading報表                    │  │
│  │  • 查看延遲分析                       │  │
│  │  • 管理使用者與權限                   │  │
│  │  • 管理延遲原因                       │  │
│  │  • 查詢稽核日誌                       │  │
│  │  • 產出各類報表                       │  │
│  │  • 覆寫工時(含理由)                   │  │
│  └──────────────────────────────────────┘  │
│             ↑                               │
│          Manager                            │
│                                             │
└─────────────────────────────────────────────┘
```

---

## 6. 功能需求詳述

### 6.1 認證與授權需求

#### FR-AUTH-001：使用者登入（Local）
- **優先級：** 必須 (Must Have)
- **描述：** 使用者以帳號與密碼登入系統，登入成功後需依 Email 合併為最終身份識別
- **驗收標準：**
  - 驗證 Account 與 PasswordHash 是否正確
  - 取得使用者 Email，並以 Email 為唯一身份識別
  - 若 Email 已存在於其他登入來源（如 AD），則視為同一使用者
  - 停用帳號無法登入
  - 連續失敗 5 次需鎖定帳號 10 分鐘

#### FR-AUTH-101：Windows AD 登入（Email 合併版）
- **優先級：** 應該 (Should Have)
- **描述：** 支援 Windows AD 驗證登入，並以 Email 為唯一身份識別
- **驗收標準：**
  - 系統可取得 ADAccount、DisplayName、Email
  - 若 Email 存在於 User 表，視為同一使用者
  - 若 Email 不存在，建立新的 AD 使用者
  - AD 未提供 Email → 登入失敗
  - 停用帳號無法登入

#### FR-AUTH-102：JWT Token 驗證與 Payload 規範
- **優先級：** 必須 (Must Have)  
- **描述：** 系統使用 JWT 作為身份驗證憑證
- **驗收標準：**
  - JWT 結構：Header + Payload + Signature
  - Payload 只放授權相關必要資訊
  - 使用 HMAC-SHA256 簽章
  - 每次 API 呼叫需驗證 Token
  - Token 過期時間可由 SystemSetting 設定

#### FR-AUTH-002：密碼安全儲存
- **優先級：** 必須 (Must Have)
- **描述：** 密碼需以不可逆方式儲存
- **驗收標準：**
  - 資料庫儲存Hash值
  - 系統無任何功能可查看原密碼
  - Hash演算法使用業界標準（如bcrypt）

#### FR-AUTH-003：忘記密碼功能
- **優先級：** 必須 (Must Have)
- **描述：** 使用者可透過Email重設密碼
- **驗收標準：**
  - 可用帳號或Email申請重設
  - 系統發送重設連結至註冊Email
  - 連結30分鐘內有效
  - 連結僅能使用一次

#### FR-AUTH-004：角色權限控管
- **優先級：** 必須 (Must Have)
- **描述：** 系統需區分Engineer與Manager角色
- **驗收標準：**
  - Engineer僅能存取自己的功能
  - Manager可存取所有功能
  - 無權限時顯示明確錯誤訊息

---

### 6.2 案件管理需求

#### FR-PROJ-001：Wizard建案流程
- **優先級：** 必須 (Must Have)
- **描述：** 提供4步驟引導式建案流程
- **驗收標準：**
  - Step 1完成才能進Step 2
  - 可隨時返回前一步修改
  - Step 4完成顯示確認摘要
  - 確認後一次性寫入所有資料

#### FR-PROJ-002：案件基本資料管理
- **優先級：** 必須 (Must Have)
- **描述：** 可新增、查詢、修改、刪除案件
- **驗收標準：**
  - 案件名稱不可重複
  - 日期邏輯正確（結束≥開始）
  - 修改需填寫理由
  - 刪除採Soft Delete

#### FR-PROJ-003：案件狀態自動計算
- **優先級：** 必須 (Must Have)
- **描述：** 案件狀態依 Regulation 狀態自動更新（三層推算：TestItem → Regulation → Project）
- **驗收標準：**
  - 任一 Regulation = Delayed → Project = Delayed
  - 所有 Regulation = Completed → Project = Completed
  - 任一 Regulation = InProgress → Project = Active
  - 所有 Regulation = NotStarted → Project = Draft
  - 主管可手動設為 OnHold
  - 狀態變更需寫入 AuditLog

#### FR-PROJ-004：Regulation 狀態自動計算
- **優先級：** 必須 (Must Have)
- **描述：** Regulation 狀態依 TestItem 狀態自動計算
- **驗收標準：**
  - 任一 TestItem = Delayed → Regulation = Delayed
  - 所有 TestItem = Completed → Regulation = Completed
  - 任一 TestItem = InProgress → Regulation = InProgress
  - 所有 TestItem = NotStarted → Regulation = NotStarted
  - 主管可手動設為 OnHold
  - 狀態變更需寫入 AuditLog

#### FR-PROJ-005：三層狀態推算機制
- **優先級：** 必須 (Must Have)
- **描述：** 系統支援 TestItem → Regulation → Project 三層狀態推算
- **驗收標準：**
  - TestItem 狀態變更時自動觸發 Regulation 重算
  - Regulation 狀態變更時自動觸發 Project 重算
  - 每層狀態計算遵循優先順序規則
  - 手動設定的狀態（ManualStatusOverride）不被自動邏輯覆蓋
  - 所有狀態變更記錄至 AuditLog
---

### 6.3 測試項目管理需求

#### FR-TEST-001：測項新增與修改
- **優先級：** 必須 (Must Have)
- **描述：** 可在法規下新增測項
- **驗收標準：**
  - 預估工時需>0
  - 需指定測試類型與場地

#### FR-TEST-002：工程師分配
- **優先級：** 必須 (Must Have)
- **描述：** 可將測項分配給工程師
- **驗收標準：**
  - 至少需1位主要(Main)工程師
  - 可分配多位支援(Sub)工程師
  - 分配工時總和與預估工時差異>20%需警告
  - 顯示工程師當前Loading

#### FR-TEST-003：補測版本管理
- **優先級：** 必須 (Must Have)
- **描述：** 可建立測項的補測版本
- **驗收標準：**
  - 版本編號不可重複（v2/v3/v4...）
  - 可選擇複製原工程師分配
  - 建立補測後測項狀態改為InProgress
  - 需填寫補測原因

#### FR-TEST-004：測項狀態計算邏輯（新增）
- **優先級：** 必須 (Must Have)
- **描述：** TestItem 狀態依優先順序自動計算，並支援手動覆寫
- **驗收標準：**
  - **狀態計算優先順序：**
    1. IF 主管手動設定 OnHold → **OnHold**（最高優先級）
    2. ELSE IF 發生「建立 TestItemRevision」事件 → **InProgress**（補測事件覆蓋舊狀態）
    3. ELSE IF WorkLog 中存在 Delayed 狀態 → **Delayed**
    4. ELSE IF 任一工程師按「Complete TestItem」→ **Completed**
    5. ELSE IF WorkLog 中存在 InProgress 狀態 → **InProgress**
    6. ELSE → **NotStarted**（初始狀態）
  - 系統需記錄狀態變更歷程至 AuditLog
  - 主管手動設定的 OnHold 狀態需額外標記，避免被自動邏輯覆蓋

#### FR-TEST-005：測項狀態逆向操作（新增）
- **優先級：** 必須 (Must Have)
- **描述：** TestItem 狀態允許逆向修改，非單向不可逆
- **驗收標準：**
  - **工程師權限：**
    - 可取消自己誤按的 Completed 狀態
    - 可將 Completed 改回 InProgress
    - 需提供「取消完成」按鈕
    - 取消操作需寫入 AuditLog
  - **主管權限：**
    - 可覆寫 TestItem 的任何狀態
    - 可修改：Completed → InProgress / Delayed / OnHold
    - 可修改：Delayed → InProgress / Completed
    - 可修改：OnHold → 任何狀態
    - 修改需填寫理由，記錄至 AuditLog
  - **自動狀態亦可覆寫：**
    - 建立 TestItemRevision 自動產生的 InProgress 可由主管手動改為其他狀態
  - 所有狀態變更皆需記錄：Who / When / From / To / Why

---

### 6.4 工時管理需求

#### FR-WORK-001：工時回報
- **優先級：** 必須 (Must Have)
- **描述：** 工程師可回報每日工時
- **驗收標準：**
  - 工作日期不可晚於今天
  - 單日工時0.5-12小時
  - 同測項同日期不可重複回報
  - 狀態=延遲時必須選延遲原因

#### FR-WORK-002：工時修改限制
- **優先級：** 必須 (Must Have)
- **描述：** 工程師可修改7天內的工時
- **驗收標準：**
  - 7天內可修改
  - 超過7天顯示錯誤訊息
  - 修改後寫入AuditLog

#### FR-WORK-003：工時狀態同步
- **優先級：** 必須 (Must Have)
- **描述：** 工時回報後自動更新測項狀態
- **驗收標準：**
  - 首次回報→測項InProgress
  - 任一工程師回報延遲→測項Delayed
  - 工程師按完成→觸發測項狀態重算

#### FR-WORK-004：工時查詢
- **優先級：** 必須 (Must Have)
- **描述：** 工程師可查詢自己的工時記錄
- **驗收標準：**
  - 可依測項篩選
  - 可依日期區間篩選
  - 可依版本篩選
  - 顯示統計資訊（總工時、剩餘工時）

#### FR-WORK-005：主管覆寫工時
- **優先級：** 應該 (Should Have)
- **描述：** 主管可修改任何工時記錄
- **驗收標準：**
  - 需填寫修改理由
  - 理由寫入AuditLog
  - 原值與新值皆記錄

---

### 6.5 Loading分析需求

#### FR-LOAD-001：Loading計算
- **優先級：** 必須 (Must Have)
- **描述：** 系統自動計算工程師Loading
- **驗收標準：**
  - Assigned Loading = Σ(分配工時) / 可用工時 × 100%
  - Actual Loading = Σ(實際工時) / 可用工時 × 100%
  - 僅計算Active狀態案件

#### FR-LOAD-002：Loading視覺化
- **優先級：** 必須 (Must Have)
- **描述：** 以顏色標示Loading狀態
- **驗收標準：**
  - ≤60%：綠色
  - 61-80%：黃色
  - 81-100%：橘色
  - 100%以上：紅色（超載）

#### FR-LOAD-003：Loading明細
- **優先級：** 必須 (Must Have)
- **描述：** 可查看工程師Loading明細
- **驗收標準：**
  - 顯示負責的所有測項
  - 顯示各測項分配/實際/剩餘工時
  - 提供工時趨勢圖表

#### FR-LOAD-004：超載預警
- **優先級：** 應該 (Should Have)
- **描述：** 分配工作時若導致超載需警告
- **驗收標準：**
  - 分配時即時計算新Loading
  - >100%顯示警告對話框
  - 允許繼續或取消

---

### 6.6 延遲管理需求

#### FR-DELAY-001：延遲原因字典
- **優先級：** 必須 (Must Have)
- **描述：** 主管可維護延遲原因清單
- **驗收標準：**
  - 可新增延遲原因（含類型：設備/客戶/工程師/場地）
  - 可修改延遲原因文字
  - 可停用/啟用
  - 已使用的不可刪除（僅能停用）

#### FR-DELAY-002：延遲原因記錄
- **優先級：** 必須 (Must Have)
- **描述：** 工時狀態=延遲時需選原因
- **驗收標準：**
  - 可多選延遲原因
  - 至少需選1個
  - 未選擇無法送出

#### FR-DELAY-003：延遲分析報表
- **優先級：** 應該 (Should Have)
- **描述：** 提供延遲原因統計分析
- **驗收標準：**
  - 顯示延遲原因分布（圓餅圖）
  - 顯示延遲測項清單
  - 可依期間/案件篩選
  - 可匯出Excel

---

### 6.7 使用者管理需求

#### FR-USER-001：使用者新增
- **優先級：** 必須 (Must Have)
- **描述：** 管理者可新增使用者，Email 需作為唯一身份識別
- **驗收標準：**
  - Account 唯一（僅適用 Local 帳號）
  - Email 必須唯一，兩筆使用者不得使用相同 Email
  - 若 Email 已存在（不論 Local 或 AD）→ 禁止新增
  - 系統自動產生初始密碼（僅 Local）
  - 自動發送 Email 給新使用者

#### FR-USER-002：使用者停用
- **優先級：** 必須 (Must Have)
- **描述：** 主管可停用使用者
- **驗收標準：**
  - 停用前檢查是否負責未完成測項
  - 若有負責測項顯示警告
  - 停用後無法登入
  - 歷史資料保留

#### FR-USER-003：使用者資料修改
- **優先級：** 必須 (Must Have)
- **描述：** 主管可修改使用者資料
- **驗收標準：**
  - 可修改顯示名稱、Email、角色
  - 帳號不可修改
  - 每週可用工時可調整

#### FR-USER-101：權限與群組管理介面（IAM）
- **優先級：** 應該 (Should Have)  
- **描述：** 具備管理權限的使用者可透過介面管理使用者的角色、權限群組與個別權限
- **驗收標準：**
  - 可將使用者指派至一個或多個權限群組
  - 可為特定使用者額外授予或撤銷個別 Permission
  - 可設定臨時權限的到期日，過期後自動失效
  - 可查詢某一位使用者目前的「有效權限」來源
  - 所有權限變更皆須寫入 AuditLog

---

### 6.8 稽核日誌需求

#### FR-AUDIT-001：自動記錄異動
- **優先級：** 必須 (Must Have)
- **描述：** 系統自動記錄重要資料異動
- **驗收標準：**
  - 記錄資料表名稱
  - 記錄記錄ID
  - 記錄操作類型（Create/Update/Delete）
  - 記錄操作人
  - 記錄操作時間
  - 記錄變更前後值（JSON格式）

#### FR-AUDIT-002：稽核日誌查詢
- **優先級：** 必須 (Must Have)
- **描述：** 主管可查詢稽核日誌
- **驗收標準：**
  - 可依資料表篩選
  - 可依操作類型篩選
  - 可依操作人篩選
  - 可依時間區間篩選
  - 可匯出Excel

#### FR-AUDIT-003：變更比對檢視
- **優先級：** 應該 (Should Have)
- **描述：** 可檢視資料變更前後差異
- **驗收標準：**
  - 雙欄顯示Before/After
  - 變更欄位以顏色標示
  - 支援JSON格式解析

---

### 6.9 報表需求

#### FR-REPORT-001：案件進度報表
- **優先級：** 必須 (Must Have)
- **描述：** 顯示案件整體進度
- **驗收標準：**
  - 顯示案件基本資訊
  - 顯示法規完成度（長條圖）
  - 顯示測項進度明細
  - 顯示工時統計（預估vs實際）
  - 支援Excel匯出

#### FR-REPORT-002：工時統計報表
- **優先級：** 必須 (Must Have)
- **描述：** 提供多維度工時統計
- **驗收標準：**
  - 可依案件統計
  - 可依工程師統計
  - 可依期間統計
  - 提供圖表視覺化
  - 支援Excel匯出

#### FR-REPORT-003：延遲分析報表
- **優先級：** 應該 (Should Have)
- **描述：** 分析延遲原因與趨勢
- **驗收標準：**
  - 延遲原因分布圓餅圖
  - 延遲測項清單
  - 平均延遲天數統計
  - 可依期間篩選

---

## 7. 非功能需求

### 7.1 效能需求 (Performance)

#### NFR-PERF-001：回應時間
- **需求：** 一般查詢操作回應時間<2秒
- **測試條件：** 50筆資料以內
- **優先級：** 必須 (Must Have)

#### NFR-PERF-002：工時查詢效能
- **需求：** 單一案件工時查詢<3秒
- **測試條件：** 500筆WorkLog記錄
- **優先級：** 必須 (Must Have)

#### NFR-PERF-003：Loading計算效能
- **需求：** Loading計算<2秒
- **測試條件：** 15位工程師、30個Active案件
- **優先級：** 必須 (Must Have)

#### NFR-PERF-004：報表產出效能
- **需求：** 報表產出<5秒
- **測試條件：** 單一案件、100個測項
- **優先級：** 應該 (Should Have)

---

### 7.2 安全需求 (Security)

#### NFR-SEC-001：密碼安全
- **需求：** 密碼採Hash不可逆儲存
- **標準：** 使用bcrypt或類似演算法
- **優先級：** 必須 (Must Have)

#### NFR-SEC-002：身份驗證
- **需求：** 所有API需驗證身份
- **標準：** 使用JWT Token機制
- **優先級：** 必須 (Must Have)

#### NFR-SEC-003：權限控管
- **需求：** 依角色控管功能存取
- **標準：** Engineer不能存取Manager功能
- **優先級：** 必須 (Must Have)

#### NFR-SEC-004：資料隔離
- **需求：** 工程師只能看自己的資料
- **標準：** 查詢需加入UserId過濾
- **優先級：** 必須 (Must Have)

#### NFR-SEC-005：密碼複雜度
- **需求：** 密碼需符合複雜度規則
- **標準：** 8-20字元、含英文數字
- **優先級：** 必須 (Must Have)

#### NFR-SEC-006：登入失敗鎖定
- **需求：** 連續失敗5次鎖定10分鐘
- **標準：** IP或帳號層級鎖定
- **優先級：** 應該 (Should Have)

#### NFR-SEC-007：JWT 結構與簽章安全
- **需求：** JWT Token 必須使用對稱簽章演算法 HMAC-SHA256（HS256）
- **說明：**
  - Header.alg = "HS256"，Header.typ = "JWT"
  - Signature = HMAC-SHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), SecretKey)
  - SecretKey 僅存放於伺服器端設定檔
- **優先級：** 必須 (Must Have)

#### NFR-SEC-008：JWT Payload 資料最小化
- **需求：** JWT Payload 只保留授權必須的資訊
- **說明：**
  - Payload 建議欄位：UserId、顯示名稱、角色、權限清單、過期時間
  - 不得在 Payload 放入：密碼、工時詳細資料、Email 內文等敏感資訊
- **優先級：** 必須 (Must Have)

---

### 7.3 可用性需求 (Usability)

#### NFR-USAB-001：學習曲線
- **需求：** 新使用者30分鐘內可完成基本操作
- **衡量：** 使用者測試達成率>80%
- **優先級：** 應該 (Should Have)

#### NFR-USAB-002：錯誤訊息
- **需求：** 錯誤訊息清楚明確
- **標準：** 告知原因與解決方式
- **優先級：** 必須 (Must Have)

#### NFR-USAB-003：操作直覺性
- **需求：** 不需查閱手冊可完成90%操作
- **衡量：** 使用者測試回饋
- **優先級：** 應該 (Should Have)

#### NFR-USAB-004：回饋及時性
- **需求：** 操作後立即顯示結果
- **標準：** 載入中顯示進度指示
- **優先級：** 必須 (Must Have)

---

### 7.4 可靠性需求 (Reliability)

#### NFR-RELI-001：可用性
- **需求：** 系統可用性>99%
- **測試條件：** 每月統計
- **優先級：** 應該 (Should Have)

#### NFR-RELI-002：資料完整性
- **需求：** 資料異動需Transaction保護
- **標準：** 關鍵操作全成功或全失敗
- **優先級：** 必須 (Must Have)

#### NFR-RELI-003：資料備份
- **需求：** 每日自動備份
- **標準：** 保留30天備份
- **優先級：** 必須 (Must Have)

#### NFR-RELI-004：錯誤處理
- **需求：** 系統錯誤不導致資料遺失
- **標準：** 異常回滾、記錄日誌
- **優先級：** 必須 (Must Have)

---

### 7.5 可維護性需求 (Maintainability)

#### NFR-MAIN-001：日誌記錄
- **需求：** 記錄系統運作日誌
- **標準：** 包含錯誤、警告、資訊層級
- **優先級：** 必須 (Must Have)

#### NFR-MAIN-002：版本控制
- **需求：** 程式碼使用版本控管
- **標準：** Git或類似工具
- **優先級：** 必須 (Must Have)

---

### 7.6 擴充性需求 (Scalability)

#### NFR-SCAL-001：使用者數量
- **需求：** 支援至少20名同時使用者
- **測試條件：** 並發操作測試
- **優先級：** 必須 (Must Have)

#### NFR-SCAL-002：資料量
- **需求：** 支援至少500個Active案件
- **測試條件：** 效能不明顯下降
- **優先級：** 應該 (Should Have)

#### NFR-SCAL-003：功能擴充性
- **需求：** 架構支援未來功能擴充
- **標準：** 模組化設計
- **優先級：** 應該 (Should Have)

---

### 7.7 相容性需求 (Compatibility)

#### NFR-COMP-001：作業系統
- **需求：** 支援Windows 10/11
- **優先級：** 必須 (Must Have)

#### NFR-COMP-002：資料庫
- **需求：** 使用SQL Server 2019以上
- **優先級：** 必須 (Must Have)

#### NFR-COMP-003：.NET版本
- **需求：** 使用.NET 8
- **優先級：** 必須 (Must Have)

---

## 8. 業務規則

### 8.1 時間相關規則

#### BR-TIME-001：案件日期邏輯
- **規則：** Project.EndDate必須≥Project.StartDate
- **違反處理：** 前端阻擋送出，顯示錯誤訊息
- **例外：** 無

#### BR-TIME-002：法規日期範圍
- **規則：** Regulation日期需在Project日期±30天內
- **違反處理：** 顯示警告，允許繼續但需確認
- **例外：** 特殊案件經主管核准

#### BR-TIME-003：工時回報日期
- **規則：** WorkLog.WorkDate不可晚於今天
- **違反處理：** 前端阻擋送出
- **例外：** 無

#### BR-TIME-004：工時修改期限
- **規則：** 工程師只能修改7天內的WorkLog
- **違反處理：** 顯示錯誤，建議聯繫主管
- **例外：** 主管可修改任何時間的WorkLog

---

### 8.2 工時相關規則

#### BR-HOUR-001：預估工時下限
- **規則：** TestItem.EstimatedHours必須>0
- **違反處理：** 前端阻擋送出
- **例外：** 無

#### BR-HOUR-002：單日工時上限
- **規則：** 單日WorkLog.ActualHours≤12
- **違反處理：** 顯示警告，允許繼續但需確認
- **例外：** 緊急趕工案件

#### BR-HOUR-003：分配工時合理性
- **規則：** Σ(AssignedHours)與EstimatedHours差異不宜>20%
- **違反處理：** 顯示警告，允許繼續
- **例外：** 複雜案件難以精確預估

#### BR-HOUR-004：累計工時警示
- **規則：** 實際工時累計超過分配工時150%需警示
- **違反處理：** 顯示警告，允許繼續
- **例外：** 案件複雜度超出預期

---

### 8.3 分配相關規則

#### BR-ASSIGN-001：主要工程師必要性
- **規則：** 每個TestItem至少需1位Main工程師
- **違反處理：** 前端阻擋送出
- **例外：** 無

#### BR-ASSIGN-002：工程師唯一性
- **規則：** 同一TestItem不可重複分配同一工程師
- **違反處理：** 前端阻擋送出
- **例外：** 無

#### BR-ASSIGN-003：Loading合理性
- **規則：** 分配工作時顯示工程師當前Loading
- **違反處理：** >100%顯示警告，允許繼續
- **例外：** 緊急案件或短期超載可接受

---

### 8.4 延遲相關規則

#### BR-DELAY-001：延遲原因必填
- **規則：** WorkLog.Status=Delayed時必須選延遲原因
- **違反處理：** 前端阻擋送出
- **例外：** 無

#### BR-DELAY-002：延遲原因多選
- **規則：** 可選擇多個延遲原因
- **說明：** 延遲可能由多種因素造成
- **例外：** 無

#### BR-DELAY-003：延遲原因刪除限制
- **規則：** 已被使用的DelayReason不可刪除
- **違反處理：** 顯示錯誤，建議改為停用
- **例外：** 無

---

### 8.5 刪除相關規則

#### BR-DELETE-001：軟刪除原則
- **規則：** Project / Regulation / TestItem / TestItemEngineer / WorkLog（主管限定） 皆採 Soft Delete。
- **說明：** 設定 IsDeleted = true，資料保留於資料庫中，並寫入 DeletedByUserId、DeletedDate。
- **限制：** 一般工程師 不可刪除 WorkLog，僅主管具有刪除工時的權限。
- **例外：** DelayReason 使用紀錄 > 0 時不可刪除（僅能停用）。

#### BR-DELETE-002：連動刪除
- **規則：** 刪除Project時連動刪除下層Regulation與TestItem
- **說明：** 採Soft Delete，資料皆保留
- **例外：** 無

#### BR-DELETE-003：工時記錄保留
- **規則：** WorkLog不因上層刪除而刪除
- **說明：** 用於稽核與歷史查詢
- **例外：** 無

#### BR-DELETE-004:停用工程師處理
- **規則:** 停用工程師前需檢查負責的未完成測項
- **違反處理:** 顯示警告,列出受影響測項
- **例外:** 可強制停用,但測項需重新分配

**說明:** User 採用 **IsActive 機制**,設定 `IsActive = false` 後:
- 使用者無法登入系統
- 歷史資料(WorkLog/TestItemEngineer)保留
- 負責中的 TestItem 需重新分配或由主管接手

---

### 8.6 狀態轉換規則（更新）

#### BR-STATUS-001：測項狀態計算邏輯
- **規則：** TestItem.Status 依以下優先順序計算
- **邏輯：**
  1. **IF 主管手動設定 OnHold** → **OnHold**（最高優先級，手動狀態應被保留）
  2. **ELSE IF 發生「建立 TestItemRevision」事件** → **InProgress**（補測事件，覆蓋舊狀態）
  3. **ELSE IF WorkLog 中存在 Delayed 狀態** → **Delayed**
  4. **ELSE IF 任一工程師按「Complete TestItem」** → **Completed**
  5. **ELSE IF WorkLog 中存在 InProgress 狀態**（有人開始回報工時）→ **InProgress**
  6. **ELSE** → **NotStarted**（初始狀態，沒有任何工時回報）
- **說明：**
  - 主管手動設定的 OnHold 需額外標記（例如：ManualStatusOverride = true）
  - 狀態變更需記錄至 AuditLog
  - 補測版本建立時，自動將 TestItem.Status 設為 InProgress，並清除 ManualStatusOverride 標記

#### BR-STATUS-002：測項狀態逆向操作
- **規則：** TestItem.Status 允許逆向修改，非單向不可逆
- **工程師權限：**
  - 可取消自己誤按的 Completed 狀態
  - 可將 Completed 改回 InProgress
  - 取消操作需寫入 AuditLog（包含理由）
- **主管權限：**
  - 可覆寫 TestItem 的任何狀態
  - 可進行任意狀態轉換：
    - Completed → InProgress / Delayed / OnHold
    - Delayed → InProgress / Completed / OnHold
    - InProgress → Completed / Delayed / OnHold
    - OnHold → 任何狀態
  - 修改需填寫理由，記錄至 AuditLog
  - 主管修改後自動設定 ManualStatusOverride = true
- **自動狀態覆寫：**
  - 建立 TestItemRevision 自動產生的 InProgress 可由主管手動改為其他狀態
  - 所有自動計算的狀態皆可由主管覆寫
- **稽核記錄：**
  - 所有狀態變更皆需記錄：Who / When / From / To / Why / IsManual

#### BR-STATUS-003：案件狀態自動計算
- **規則：** Project.Status 依 Regulation.Status 自動計算
- **邏輯：**
  - 任一 Regulation = Delayed → Project = Delayed
  - 所有 Regulation = Completed → Project = Completed
  - 任一 Regulation = InProgress → Project = Active
  - 所有 Regulation = NotStarted → Project = Draft
- **例外：** 主管可手動設為 OnHold
- **說明：**
  - Project 狀態計算基於 Regulation 層級，而非直接基於 TestItem
  - 形成三層推算：TestItem → Regulation → Project

#### BR-STATUS-004：Regulation 狀態計算邏輯
- **規則：** Regulation.Status 依優先順序自動計算
- **邏輯：**
  1. **IF 主管手動設定 OnHold** → **OnHold**（最高優先級）
  2. **ELSE IF 任一 TestItem = Delayed** → **Delayed**
  3. **ELSE IF 所有 TestItem = Completed** → **Completed**
  4. **ELSE IF 任一 TestItem = InProgress** → **InProgress**
  5. **ELSE** → **NotStarted**（初始狀態）
- **說明：**
  - 主管手動設定的 OnHold 需額外標記（例如：ManualStatusOverride = true）
  - 狀態變更需記錄至 AuditLog
  - Regulation 狀態變更時會觸發 Project 狀態重算

#### BR-STATUS-005：三層狀態推算連鎖反應
- **規則：** 狀態變更採由下而上推算
- **邏輯：**
```
  WorkLog.Status 變更
    → 觸發 TestItem 狀態重算
      → 觸發 Regulation 狀態重算
        → 觸發 Project 狀態重算
```
- **說明：**
  - 每層狀態計算獨立執行，遵循各自的優先順序規則
  - 手動狀態（ManualStatusOverride = true）阻斷自動推算
  - 所有狀態變更皆記錄 AuditLog（包含觸發來源）

---

## 9. 資料需求概述

### 9.1 主要資料實體

#### 9.1.1 User (使用者)
- **說明:** 系統使用者,包含工程師、主管、系統管理者,支援 Local 帳號與 AD 帳號兩種身份來源
- **關鍵屬性:**
  - UserId (PK)
  - Account(唯一,Local 帳號用)
  - PasswordHash(僅適用於 AuthType = Local)
  - DisplayName
  - Email(唯一,用於登入身份合併)
  - Role(Engineer / Manager / Admin)
  - WeeklyAvailableHours
  - IsActive **(使用者啟用/停用狀態,不使用 Soft Delete)**
  - AuthType(Local / AD)
  - ADAccount(僅 AuthType=AD 需要)
  - LastLoginDate
  - LastLoginIP
  - CreatedByUserId (FK → User)
  - CreatedDate
  - ModifiedByUserId (FK → User)
  - ModifiedDate
- **關係:**
  - 一位 User 可負責多個 TestItem(透過 TestItemEngineer)
  - 一位 User 可有多筆 WorkLog
  - 一位 User 可屬於多個 PermissionGroup(透過 UserGroup)
  - 一位 User 可被授予多個個別 Permission(透過 UserPermission)

**注意:** User 採用 **IsActive 機制**而非 Soft Delete,停用後使用者無法登入,但歷史資料保留。

#### 9.1.2 Project (案件)
- **說明：** RF測試案件
- **關鍵屬性：**
  - ProjectId (PK)
  - ProjectName（唯一）
  - CustomerName
  - Priority（High/Medium/Low）
  - Status（Draft/Active/Completed/OnHold/Delayed）
  - StartDate
  - EndDate
  - Note 
  - CreatedByUserId (FK → User)
  - CreatedDate
  - ModifiedByUserId (FK → User)
  - ModifiedDate
  - IsDeleted
  - DeletedByUserId 
  - DeletedDate
- **關聯：**
  - 一個 Project 有多個 Regulation

#### 9.1.3 Regulation (法規)
- **說明：** 案件需測試的法規，支援狀態自動計算
- **關鍵屬性：**
  - RegulationId (PK)
  - ProjectId (FK → Project)
  - RegulationType（FCC/NCC/CE/IC/TELEC 等）
  - RegulationName（Part 24、PLMN、RED…具體規範名稱）
  - Status（NotStarted/InProgress/Delayed/Completed/OnHold）
  - ManualStatusOverride（標記是否為主管手動設定）
  - StartDate
  - EndDate
  - ManagerNote（主管備註
  - CreatedByUserId (FK)）
  - CreatedDate
  - ModifiedDate
  - ModifiedByUserId (FK → User)
  - IsDeleted
  - DeletedByUserId
  - DeletedDate
- **關聯：**
  - 隸屬於一個 Project
  - 一個 Regulation 有多個 TestItem
  - Regulation.Status 依所有 TestItem 狀態自動計算

#### 9.1.4 TestItem (測試項目)
- **說明：** 具體的測試項目
- **關鍵屬性：**
  - TestItemId (PK)
  - RegulationId (FK → Regulation)
  - TestItemName
  - TestType（Conducted/Radiated等）
  - TestLocation
  - EstimatedHours
  - Status（NotStarted/InProgress/Completed/Delayed/OnHold）
  - ManualStatusOverride（標記是否為主管手動設定）
  - ManagerNote 
  - CreatedByUserId
  - CreatedDate
  - ModifiedByUserId
  - ModifiedDate
  - IsDeleted
  - DeletedByUserId
  - DeletedDate 
- **關聯：**
  - 隸屬於一個 Regulation
  - 可分配給多位工程師（透過 TestItemEngineer）
  - 可有多筆 WorkLog
  - 可有多個補測版本（TestItemRevision）

#### 9.1.5 TestItemEngineer (工程師分配)
- **說明：** 測項與工程師的關聯
- **關鍵屬性：**
  - TestItemEngineerId (PK)
  - TestItemId (FK → TestItem)
  - UserId (FK → User)
  - RoleType（Main/Sub）
  - AssignedHours
  - AssignedDate
  - CreatedByUserId
  - CreatedDate
  - ModifiedByUserId
  - ModifiedDate
  - IsDeleted
  - DeletedByUserId
  - DeletedDate 
- **關聯：**
  - 連結 TestItem 與 User

#### 9.1.6 WorkLog (工時記錄)
- **說明:** 工程師的每日工時記錄,**支援 Soft Delete 以保留稽核軌跡**
- **關鍵屬性:**
  - WorkLogId (PK)
  - TestItemId (FK → TestItem)
  - UserId (FK → User)
  - RevisionId (FK → TestItemRevision, nullable)
  - WorkDate
  - ActualHours
  - Status(InProgress/Completed/Delayed)
  - Comment
  - CreatedByUserId (FK → User)
  - CreatedDate
  - ModifiedByUserId (FK → User)
  - ModifiedDate
  - ModificationReason
  - **IsDeleted** *(新增)*
  - **DeletedByUserId (FK → User)** *(新增)*
  - **DeletedDate** *(新增)*
- **關係:**
  - 隸屬於一個 TestItem
  - 隸屬於一個 User
  - 可對應一個 TestItemRevision
  - 可關聯多個 DelayReason(透過 WorkLogDelayReason)

**注意:** WorkLog 採用 **Soft Delete**,即使 TestItem 被刪除,WorkLog 也會保留以供稽核與歷史查詢。

#### 9.1.7 TestItemRevision (補測版本)
- **說明:** 測項的補測版本記錄,**支援 Soft Delete**
- **關鍵屬性:**
  - RevisionId (PK)
  - TestItemId (FK → TestItem)
  - RevisionNumber(v2/v3/v4...)
  - Reason
  - EstimatedHours
  - Description
  - CreatedByUserId (FK → User)
  - CreatedDate
  - ModifiedByUserId (FK → User)
  - ModifiedDate
  - **IsDeleted** *(新增)*
  - **DeletedByUserId (FK → User)** *(新增)*
  - **DeletedDate** *(新增)*
- **關係:**
  - 隸屬於一個 TestItem
  - 可被多筆 WorkLog 參考

**注意:** TestItemRevision 採用 **Soft Delete**,刪除後不影響已關聯的 WorkLog 查詢。

#### 9.1.8 DelayReason (延遲原因)
- **說明:** 延遲原因字典,**支援停用機制(IsActive),已使用的原因不可刪除**
- **關鍵屬性:**
  - DelayReasonId (PK)
  - ReasonText
  - ReasonType(Equipment/Customer/Engineer/Location)
  - **IsActive** *(用於停用,不使用 IsDeleted)*
  - CreatedByUserId (FK → User)
  - CreatedDate
  - ModifiedByUserId (FK → User)
  - ModifiedDate
- **關係:**
  - 可被多個 WorkLog 參考(透過 WorkLogDelayReason)

**管理規則:**
- 已被 WorkLog 使用的 DelayReason **不可刪除**
- 可透過設定 `IsActive = false` 停用
- 停用後不再顯示於下拉選單,但歷史資料仍可查詢

#### 9.1.9 WorkLogDelayReason (工時延遲原因關聯)
- **說明：** WorkLog 與 DelayReason 的多對多關聯
- **關鍵屬性：**
  - WorkLogDelayReasonId (PK)
  - WorkLogId (FK → WorkLog)
  - DelayReasonId (FK → DelayReason)

#### 9.1.10 AuditLog (稽核日誌)
- **說明：** 資料異動稽核記錄
- **關鍵屬性：**
  - AuditLogId (PK)
  - TableName
  - RecordId
  - ActionType（Create/Update/Delete）
  - OldValue（JSON）
  - NewValue（JSON）
  - ChangedBy (FK → User)
  - ChangedDate
  - Reason

#### 9.1.11 Permission (權限)
- **說明：** 系統內可被授與與檢查的最小權限單位
- **關鍵屬性：**
  - PermissionId (PK)
  - PermissionCode（唯一，如 PROJECT_CREATE, WORKLOG_VIEW_ALL）
  - PermissionName
  - Category（Project / TestItem / WorkLog / User / Report...）
  - Description
  - IsActive
  - CreatedByUserId
  - CreatedDate
  - ModifiedByUserId
  - ModifiedDate

#### 9.1.12 PermissionGroup (權限群組)
- **說明:** 權限的集合,用來對應「工程師」、「主管」、「系統管理者」這類角色,**僅可停用,不可刪除**
- **關鍵屬性:**
  - GroupId (PK)
  - GroupName(例如 Engineer / Manager / Admin / Auditor)
  - Description
  - **IsActive** *(用於停用,不使用 IsDeleted)*
  - CreatedByUserId (FK → User)
  - CreatedDate
  - ModifiedByUserId (FK → User)
  - ModifiedDate
- **關係:**
  - 一個 PermissionGroup 可以包含多個 Permission(透過 PermissionGroupMapping)
  - 一個 User 可以屬於多個 PermissionGroup(透過 UserGroup)

**管理規則:**
- PermissionGroup **不支援刪除**,僅能透過 `IsActive = false` 停用
- 停用後該群組不再可指派給新用戶,但已指派的用戶不受影響
- 系統預設群組(Engineer/Manager/Admin)不可停用

#### 9.1.13 PermissionGroupMapping (群組:權限對應)
- **說明:** 定義「某個群組擁有哪些權限」的對應表
- **關鍵屬性:**
  - MappingId (PK)
  - GroupId (FK → PermissionGroup)
  - PermissionId (FK → Permission)
  - **CreatedByUserId (FK → User)** *(新增)*
  - **CreatedDate** *(新增)*
- **關係:**
  - 連結 PermissionGroup 與 Permission

**管理規則:**
- 新增/移除權限對應時需記錄操作者與時間
- 變更需寫入 AuditLog

#### 9.1.14 UserGroup (使用者群組)
- **說明：** 表示某位使用者屬於哪些權限群組
- **關鍵屬性：**
  - UserGroupId (PK)
  - UserId (FK → User)
  - GroupId (FK → PermissionGroup)
  - AssignedDate

#### 9.1.15 UserPermission (使用者個別權限)
- **說明：** 直接賦予某位使用者的個別權限
- **關鍵屬性：**
  - UserPermissionId (PK)
  - UserId (FK → User)
  - PermissionId (FK → Permission)
  - GrantedByUserId (FK → User)
  - GrantedDate
  - ExpireDate（可為 NULL 表示永久）
  - IsActive

---

### 9.2 資料關聯概念圖

```
User (使用者)
  ├─ 建立/修改 → Project (案件)
  ├─ 負責 → TestItem (測項) [透過TestItemEngineer]
  ├─ 回報 → WorkLog (工時記錄)
  ├─ 屬於 → PermissionGroup [透過UserGroup]
  └─ 授予 → Permission [透過UserPermission]

Project (案件)
  └─ 包含 → Regulation (法規)
      └─ 包含 → TestItem (測項)
          ├─ 分配給 → User (工程師) [透過TestItemEngineer]
          ├─ 記錄 → WorkLog (工時記錄)
          └─ 建立 → TestItemRevision (補測版本)

WorkLog (工時記錄)
  ├─ 屬於 → TestItem
  ├─ 屬於 → User
  ├─ 對應 → TestItemRevision (optional)
  └─ 選擇 → DelayReason [透過WorkLogDelayReason]

PermissionGroup (權限群組)
  ├─ 包含 → Permission [透過PermissionGroupMapping]
  └─ 指派給 → User [透過UserGroup]

AuditLog (稽核日誌)
  └─ 記錄所有實體的異動
```

---

### 9.3 ERD 圖（Entity Relationship Diagram）

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         RF 案件排程系統 ERD                              │
└─────────────────────────────────────────────────────────────────────────┘

┌──────────────────────┐
│       User           │
├──────────────────────┤
│ UserId (PK)          │
│ Account (UNIQUE)     │
│ PasswordHash         │
│ DisplayName          │
│ Email (UNIQUE)       │◄─────────────┐
│ Role                 │              │
│ WeeklyAvailableHours │              │
│ IsActive             │              │
│ AuthType             │              │
│ ADAccount            │              │
│ LastLoginDate        │              │
│ LastLoginIP          │              │
│ CreatedByUserId      │              │
│ CreatedDate          │              │
│ ModifiedDate         │              │
│ ModifiedByUserId     │              │
└──────────────────────┘              │
         │                            │
         │ 1                          │
         │                            │
         │ *                          │
         ▼                            │
┌──────────────────────┐              │
│  TestItemEngineer    │              │
├──────────────────────┤              │
│ TestItemEngineerId(PK)│             │
│ TestItemId (FK)      │              │
│ UserId (FK)          │              │
│ RoleType             │              │
│ AssignedHours        │              │
│ AssignedDate         │              │
│ CreatedByUserId      │              │
│ CreatedDate          │              │
│ ModifiedByUserId     │              │
│ ModifiedDate         │              │
│ IsDeleted            │              │
│ DeletedByUserId      │              │
│ DeletedDate          │              │
└──────────────────────┘              │
         │                            │
         │ *                          │
         │                            │
         │ 1                          │
         ▼                            │
┌──────────────────────┐              │
│     TestItem         │              │
├──────────────────────┤              │
│ TestItemId (PK)      │              │
│ RegulationId (FK)    │              │
│ TestItemName         │              │
│ TestType             │              │
│ TestLocation         │              │
│ EstimatedHours       │              │
│ Status               │              │
│ ManualStatusOverride │              │
│ ManagerNote          │              │
│ CreatedByUserId      │              │
│ CreatedDate          │              │
│ ModifiedByUserId     │              │
│ ModifiedDate         │              │
│ IsDeleted            │              │
│ DeletedByUserId      │              │
│ DeletedDate          │              │      
└──────────────────────┘              │
         │                            │
         │ 1                          │
         │                            │
         │ *                          │
         ▼                            │
┌──────────────────────┐              │
│  TestItemRevision    │              │
├──────────────────────┤              │
│ RevisionId (PK)      │              │
│ TestItemId (FK)      │              │
│ RevisionNumber       │              │
│ Reason               │              │
│ EstimatedHours       │              │
│ Description          │              │
│ CreatedByUserId      │──────────────┘
│ CreatedDate          │              
│ ModifiedByUserId     │              
│ ModifiedDate         │              
│ IsDeleted            │              
│ DeletedByUserId      │              
│ DeletedDate          │
└──────────────────────┘
         │
         │ 1
         │
         │ *
         ▼
┌──────────────────────┐
│      WorkLog         │
├──────────────────────┤
│ WorkLogId (PK)       │
│ TestItemId (FK)      │───────┐
│ UserId (FK)          │───────┼──────────┐
│ RevisionId (FK)      │       │          │
│ WorkDate             │       │          │
│ ActualHours          │       │          │
│ Status               │       │          │
│ Comment              │       │          │
│ CreatedByUserId      │       │          │
│ CreatedDate          │       │          │
│ ModifiedByUserId     │       │          │
│ ModifiedDate         │       │          │
│ ModificationReason   │       │          │
│ IsDeleted            │       │          │       
│ DeletedByUserId      │       │          │       
│ DeletedDate          │       │          │
└──────────────────────┘       │          │
         │                     │          │
         │ *                   │          │
         │                     │          │
         │ *                   │          │
         ▼                     │          │
┌──────────────────────┐       │          │
│ WorkLogDelayReason   │       │          │
├──────────────────────┤       │          │
│ WorkLogDelayReasonId │       │          │
│ WorkLogId (FK)       │       │          │
│ DelayReasonId (FK)   │───┐   │          │
└──────────────────────┘   │   │          │
                           │   │          │
                           │ * │          │
                           │   │          │
                           │   │          │                           
                           │ 1 │          │
                           │   │          │
┌──────────────────────┐   │   │          │
│    DelayReason       │   │   │          │
├──────────────────────┤   ▼   │          │
│ DelayReasonId (PK)   │       │          │
│ ReasonText           │       │          │
│ ReasonType           │       │          │
│ IsActive             │       │          │
│ CreatedByUserId      │       │          │
│ CreatedDate          │       │          │
│ ModifiedByUserId     │       │          │
│ ModifiedDate         │       │          │
└──────────────────────┘       │          │
                               │          │
┌──────────────────────┐       │          │
│     Regulation       │       │          │
├──────────────────────┤       │          │
│ RegulationId (PK)    │       │          │
│ ProjectId (FK)       │◄──┐   │          │
│ RegulationType       │   │   │          │
│ RegulationName       │   │   │          │
│ Status               │   │   │          │
│ ManualStatusOverride │   │   │          │
│ StartDate            │   │   │          │
│ EndDate              │   │ * │          │
│ ManagerNote          │   │   │          │
│ CreatedByUserId (FK) │   │   │          │
│ CreatedDate          │   │ 1 │          │
│ ModifiedDate         │   │   │          │
│ ModifiedByUserId(FK) │   │   │          │
│ IsDeleted            │   │   │          │
│ DeletedByUserId      │   │   │          │
│ DeletedDate          │   │   │          │
└──────────────────────┘   │   │          │
                           │   │          │
                           │   │          │
                           │   │          │
┌──────────────────────┐   │   │          │
│      Project         │   │   │          │
├──────────────────────┤   │   │          │
│ ProjectId (PK)       │───┘   │          │
│ ProjectName (UNIQUE) │       │          │
│ CustomerName         │       │          │
│ Priority             │       │          │
│ Status               │       │          │
│ StartDate            │       │          │
│ EndDate              │       │          │
│ Note                 │       │          │
│ CreatedByUserId (FK) │───────┼──────────┘
│ CreatedDate          │       │
│ ModifiedByUserId (FK)│───────┘
│ ModifiedDate         │
│ IsDeleted            │
│ DeletedByUserId      │
│ DeletedDate          │
└──────────────────────┘


┌──────────────────────┐
│    AuditLog          │
├──────────────────────┤
│ AuditLogId (PK)      │
│ TableName            │
│ RecordId             │
│ ActionType           │
│ OldValue (JSON)      │
│ NewValue (JSON)      │
│ ChangedBy (FK)       │──┐
│ ChangedDate          │  │
│ Reason               │  │
└──────────────────────┘  │
                          │
                          │
┌─────────────────────────┼────────────────────────────────────────┐
│      IAM 權限體系        │                                        │
└─────────────────────────┼────────────────────────────────────────┘
                          │
┌──────────────────────┐  │  ┌──────────────────────┐
│    Permission        │  │  │  PermissionGroup     │
├──────────────────────┤  │  ├──────────────────────┤
│ PermissionId (PK)    │  │  │ GroupId (PK)         │
│ PermissionCode       │  │  │ GroupName            │
│ PermissionName       │  │  │ Description          │
│ Category             │  │  │ IsActive             │
│ Description          │  │  │ CreatedByUserId      │        
│ IsActive             │  │  │ CreatedDate          │  
│ CreatedByUserId      │  │  │ ModifiedByUserId     │                
│ CreatedDate          │  │  │ ModifiedDate         │
│ ModifiedByUserId     │  │  └──────────────────────┘ 
│ ModifiedDate         │  │           │
│ Description          │  │           │
└──────────────────────┘  │           │ 1
         │                │           │
         │ 1              │           │ *
         │                │           ▼
         │ *              │  ┌──────────────────────┐
         ▼                │  │ PermissionGroupMapping│
┌──────────────────────┐  │  ├──────────────────────┤
│ PermissionGroupMapping│ │  │ MappingId (PK)       │
├──────────────────────┤  │  │ GroupId (FK)         │
│ MappingId (PK)       │  │  │ PermissionId (FK)    │
│ GroupId (FK)         │  │  │ CreatedByUserId      │
│ PermissionId (FK)    │  │  │ CreatedDate          │
│ CreatedByUserId      │──┘  └──────────────────────┘
│ CreatedDate          │
└──────────────────────┘              │ *
                                      │
                                      │ 1
                                      ▼
                             ┌──────────────────────┐
                             │     UserGroup        │
                             ├──────────────────────┤
                             │ UserGroupId (PK)     │
                             │ UserId (FK)          │───┐
                             │ GroupId (FK)         │   │
                             │ AssignedDate         │   │
                             └──────────────────────┘   │
                                                        │
┌──────────────────────┐                                │
│  UserPermission      │                                │
├──────────────────────┤                                │
│ UserPermissionId(PK) │                                │
│ UserId (FK)          │────────────────────────────────┘
│ PermissionId (FK)    │
│ GrantedByUserId (FK) │
│ GrantedDate          │
│ ExpireDate           │
│ IsActive             │
└──────────────────────┘
```

---

## 10. 附錄 A：混合登入與 IAM

### A.1 混合登入機制概觀

本系統支援兩種身份來源：

1. **Local 帳號登入**
   - 使用者輸入系統帳號與密碼
   - 後端驗證 PasswordHash
   - 驗證成功後簽發 JWT Token，後續 API 皆須附帶 Token

2. **Windows 驗證登入**
   - 系統透過 Windows Authentication 取得目前登入 Windows 的使用者帳號
   - 伺服器端將 AD 帳號對應到系統 User 紀錄
   - 必要時自動建立預設角色與群組

> 本系統使用 Email 作為跨登入來源（Local / AD）的唯一身份識別鍵。
> 若 AD 帳號與 Local 帳號具有相同 Email，登入時視為同一使用者。

---

### A.2 AD 登入 Email 合併流程

AD 登入流程必須依 Email 進行合併：

1. 取得 ADAccount、DisplayName、Email
2. 若 Email = NULL → 拒絕登入（AUTH-101）
3. 查詢：`SELECT * FROM Users WHERE Email = {Email}`
4. 若已存在：
   - 視為同一使用者
   - 更新 ADAccount、AuthType、LastLoginType
5. 若不存在：
   - 建立新 AD 使用者
6. 簽發 JWT Token

---

### A.3 JWT 與 IAM 的角色

1. **JWT 負責：我是誰？（Who）**
   - 通過登入後，由系統簽發 JWT Token
   - Token 內放 UserId / 顯示名稱 / 角色 / 關鍵 Permission
   - 每次呼叫 API 時，後端會驗證 Token 的：
     - 簽章是否正確
     - 是否過期
     - 是否被系統列為無效

2. **IAM 負責：我可以做什麼？（What）**
   - 使用 Permission / PermissionGroup / UserGroup / UserPermission 四個實體
   - 決定使用者可執行的操作
   - 未來新增功能，只要新增對應的 PermissionCode，不必重寫角色邏輯

---

### A.4 Engineer / Manager / Admin 的對應方式

- **Engineer 群組預期權限：**
  - 可查看自己相關的 Project / Regulation / TestItem
  - 可回報、查詢、修改「自己的」 WorkLog（7 天內）
  - 可取消自己的 TestItem Completed 狀態
  - 可查看自己的 Loading 與報表

- **Manager 群組預期權限：**
  - 可建立 / 修改 / 刪除案件
  - 可分配工程師、建立補測版本
  - 可覆寫任何 TestItem 狀態（含理由）
  - 可查看所有工程師的 Loading、工時統計、延遲分析
  - 可覆寫工時（含理由），可停用使用者
  - 可查詢 AuditLog

- **Admin 群組預期權限：**
  - 擁有大部分或全部 Permission
  - 可維護 Permission / PermissionGroup / UserGroup / SystemSetting 等

---

## 11. 附錄 B：TestItem 狀態轉換流程圖

```
┌─────────────────────────────────────────────────────────────────┐
│              TestItem 狀態計算優先順序流程圖                      │
└─────────────────────────────────────────────────────────────────┘

                        ┌─────────────┐
                        │   START     │
                        └──────┬──────┘
                               │
                               ▼
                   ┌───────────────────────┐
                   │ 主管手動設定 OnHold？  │
                   │ (ManualStatusOverride)│
                   └───────┬───────────────┘
                           │
                     Yes   │   No
                    ┌──────┴──────┐
                    ▼             ▼
            ┌──────────┐   ┌──────────────────┐
            │ OnHold   │   │ 發生建立 Revision │
            │ (最高優先)│   │ 事件？            │
            └──────────┘   └────┬─────────────┘
                                │
                          Yes   │   No
                         ┌──────┴──────┐
                         ▼             ▼
                 ┌──────────┐   ┌──────────────┐
                 │InProgress│   │ WorkLog 中    │
                 │(補測狀態) │   │ 存在 Delayed？│
                 └──────────┘   └────┬─────────┘
                                     │
                               Yes   │   No
                              ┌──────┴──────┐
                              ▼             ▼
                      ┌──────────┐   ┌──────────────┐
                      │ Delayed  │   │ 工程師按      │
                      │          │   │ Complete？    │
                      └──────────┘   └────┬─────────┘
                                          │
                                    Yes   │   No
                                   ┌──────┴──────┐
                                   ▼             ▼
                           ┌──────────┐   ┌──────────────┐
                           │Completed │   │ WorkLog 中    │
                           │          │   │ 存在InProgress│
                           └──────────┘   └────┬─────────┘
                                               │
                                         Yes   │   No
                                        ┌──────┴──────┐
                                        ▼             ▼
                                ┌──────────┐   ┌──────────┐
                                │InProgress│   │NotStarted│
                                │          │   │ (初始)   │
                                └──────────┘   └──────────┘


┌─────────────────────────────────────────────────────────────────┐
│              TestItem 狀態逆向操作權限矩陣                        │
└─────────────────────────────────────────────────────────────────┘

操作者      │  可執行的狀態變更
───────────┼─────────────────────────────────────────────────
Engineer   │  Completed → InProgress (僅限取消自己的完成)
           │  需提供理由，寫入 AuditLog
───────────┼─────────────────────────────────────────────────
Manager    │  任何狀態 → 任何狀態
           │  - Completed → InProgress / Delayed / OnHold
           │  - Delayed → InProgress / Completed / OnHold
           │  - InProgress → Completed / Delayed / OnHold
           │  - OnHold → 任何狀態
           │  需填寫理由，寫入 AuditLog
           │  自動設定 ManualStatusOverride = true
───────────┴─────────────────────────────────────────────────
```

---

## 12. 附錄 C：資料字典摘要

### C.1 枚舉類型定義

#### ProjectStatus（案件狀態）
- `Draft`：草稿
- `Active`：進行中
- `Completed`：已完成
- `OnHold`：暫停
- `Delayed`：延遲

#### RegulationStatus（法規狀態）
- `NotStarted`：未開始
- `InProgress`：進行中
- `Completed`：已完成
- `Delayed`：延遲
- `OnHold`：暫停

#### TestItemStatus（測項狀態）
- `NotStarted`：未開始
- `InProgress`：進行中
- `Completed`：已完成
- `Delayed`：延遲
- `OnHold`：暫停

#### WorkLogStatus（工時狀態）
- `InProgress`：進行中
- `Completed`：已完成
- `Delayed`：延遲

#### Priority（優先順序）
- `High`：高
- `Medium`：中
- `Low`：低

#### RoleType（工程師角色）
- `Main`：主要負責
- `Sub`：支援

#### UserRole（使用者角色）
- `Engineer`：工程師
- `Manager`：主管
- `Admin`：系統管理員

#### AuthType（驗證類型）
- `Local`：本地帳號
- `AD`：Active Directory

#### DelayReasonType（延遲原因類型）
- `Equipment`：設備問題
- `Customer`：客戶因素
- `Engineer`：人力因素
- `Location`：場地問題

#### AuditActionType（稽核操作類型）
- `Create`：新增
- `Update`：修改
- `Delete`：刪除

---

## 13. 版本控制與變更記錄

### 變更摘要（v2.2）

1. **TestItem 狀態計算邏輯明確化**
   - 新增 6 級優先順序狀態計算規則
   - 主管手動 OnHold 為最高優先級
   - 補測事件自動觸發 InProgress

2. **狀態逆向操作支援**
   - 工程師可取消自己的 Completed 狀態
   - 主管可覆寫任何狀態
   - 所有變更需記錄 AuditLog

3. **新增 ManualStatusOverride 欄位**
   - 標記主管手動設定的狀態
   - 避免被自動邏輯覆蓋

4. **完整 ERD 圖**
   - 包含所有實體關聯
   - 標示主鍵與外鍵
   - 顯示 IAM 權限體系

5. **狀態轉換流程圖**
   - 視覺化狀態計算邏輯
   - 權限矩陣說明

---