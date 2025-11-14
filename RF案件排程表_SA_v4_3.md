# 📘 RF 案件排程系統 — 系統分析文件（SA v4.3）
---

## 1. 系統概要（System Overview）

### 1.1 專案背景

RF 測試案件包含多種法規（FCC / NCC / CE / IC / TELEC）、多個測試項目（Conducted / Radiated / Blocking ...）、多座測試場地，以及不同工程師負責。不論是案件規劃、測試人力安排、工時回報、進度追蹤，都極度依賴統一管理系統。

目前仍以 Excel、口頭協作等方式管理測試案件，造成下列問題：

- **資料分散、不一致、難以查詢與維護**
- **工程師工時難以統計，無法追蹤補測版本**
- **工程師負載（Loading）無法視覺化**
- **測試延遲原因無法標準化與分析**
- **缺乏權限控管，容易誤刪資料或看到不該看的資訊**
- **測試補測版本（v2/v3/v4）無法追蹤**
- **工時回報缺乏稽核（AuditLog）無法知道誰在什麼時間修改過資料**
- **多人維護 Excel 可能產生版本衝突**

本系統旨在建立結構化、可維護、可追蹤且支援 RBAC 與補測版本管理的 RF Scheduling System。

---

## 1.2 系統目標

1. 建立結構化資料：  
   **Project → Regulation → TestItem → Engineer → WorkLog → Revision**
2. 設計 Wizard 建案流程（4 Steps）
3. 導入 RBAC（三角色：Engineer / Manager / Admin）
4. 支援補測版本（TestItemRevision）
5. 支援延遲原因（DelayReason）
6. 全系統採 Soft Delete（含 DeletedByUserId、DeletedDate）
7. AuditLog 放在 Infrastructure 層負責記錄修改
8. 提供工程師 Loading 計算（Assigned / Actual / Remaining）
9. 未來可擴展甘特圖、報表平台、績效分析

---

## 2. 系統角色（RBAC）

### 2.1 角色介紹

- **Engineer**：只回報自己的測試進度與工時
- **Manager**：建案、設定法規、建立測項、分配工程師，查看所有工程師進度
- **Admin**：管理帳號、角色、DelayReason 字典與系統級設定

---

### 2.2 角色權限矩陣（SA v4.3）

| 功能 | Engineer | Manager | Admin |
|------|----------|----------|--------|
| 登入系統 | ✔ | ✔ | ✔ |
| 查看自己項目 | ✔ | ✔ | ✔ |
| 回報進度 / 工時 | ✔（僅自己） | ✖ | ✖ |
| 修改 WorkLog（需修正原因） | ✔（僅自己） | ✖ | ✖ |
| 查看所有工程師回報 | ✖ | ✔（不限案件） | ✔ |
| 建案 Project | ✖ | ✔ | ✔ |
| 編輯 / 刪除 Project | ✖ | ✔（限自己建立） | ✔ |
| 管理 Regulation | ✖ | ✔ | ✔ |
| 管理 TestItem | ✖ | ✔ | ✔ |
| 分配工程師 | ✖ | ✔ | ✔ |
| 建立補測版本 Revision | ✖ | ✔ | ✔ |
| 查看延遲報告 | ✔（自己） | ✔（全部） | ✔ |
| 管理 DelayReason 字典 | ✖ | ✔ | ✔ |
| 管理使用者與角色 | ✖ | ✖ | ✔ |

---

## 3. Wizard 建案流程

Wizard 共有 4 個主要階段：

1. Step 1：建立 Project  
2. Step 2：選擇 Regulations  
3. Step 3：建立 TestItems  
4. Step 4：分配工程師（含 Loading 顯示）

---

### 3.1 Step 1：建立 Project

**輸入欄位：**

| 欄位 | 必填 | 說明 |
|------|------|------|
| ProjectName | ✔ | 案件名稱 |
| Customer | ✖ | 客戶 |
| Priority | ✔ | High / Medium / Low |
| StartDate | ✖ | 預計開始 |
| EndDate | ✖ | 預計結束 |

**驗證：**

- ProjectName 不可空  
- StartDate ≤ EndDate（若都有輸入）

---

### 3.2 Step 2：選擇 Regulation

每個 Regulation 需包含：

| 欄位 | 必填 |
|------|------|
| RegulationName | ✔ |
| StartDate | ✔ |
| EndDate | ✔ |

**驗證：**

- 至少勾選 1 項法規
- StartDate ≤ EndDate

---

### 3.3 Step 3：建立 TestItem

**欄位：**

| 欄位 | 必填 |
|------|------|
| TestItemName | ✔ |
| TestType | ✔ |
| TestLocation | ✔ |
| EstimatedHours | ✔ |
| ManagerNote | ✖ |

**驗證：**

- 每個 Regulation 至少需 1 個 TestItem
- EstimatedHours > 0

---

### 3.4 Step 4：分配工程師（TestItemEngineer）

| 欄位 | 說明 |
|------|------|
| EngineerUserId | 工程師帳號 |
| RoleType | Main / Sub |
| AssignedHours | 分配工時 |

**預設分配比例（建議值，可調整）**

- 1 人 → 100%  
- 2 人 → 70% / 30%  
- 3 人 → 50% / 30% / 20%  

**驗證：**

- 至少一位工程師  
- 至少一位主測（Main）  
- AssignedHours > 0  
- 若 AssignedHours 不等於 EstimatedHours → 警示但允許繼續  
- 顯示工程師 Loading（AssignedHours / AvailableHours）

---

## 4. 補測版本（TestItemRevision）

### 4.1 使用場景

- 測試不通過需補測  
- 客戶要求版本升級  
- 問題重現需重測  
- 不同樣品、不同環境、不同場地需要新版本  

---

### 4.2 欄位

| 欄位 | 說明 |
|------|------|
| RevisionId | 主鍵 |
| TestItemId | 測項編號 |
| RevisionNumber | v2 / v3 / v4 |
| Reason | 補測原因 |
| Description | 補測說明 |
| CreatedByUserId | 建立人 |
| CreatedDate | 時間 |

---

## 5. 延遲原因（DelayReason）

### 5.1 欄位

| 欄位 | 說明 |
|------|------|
| DelayReasonId | 主鍵 |
| ReasonText | 顯示文字 |
| ReasonType | 類型（設備、客戶、工程師、場地） |
| IsActive | 是否啟用 |

---

## 6. AuditLog（非 Domain）

AuditLog 記錄：

- 修改前 / 修改後內容（JSON）
- 修改人
- 修改時間
- 修改原因（工程師修改 WorkLog 必填）
- 物件 ID

放在 Infrastructure 層，不屬於 Domain。

---

## 7. Domain 模型

以下全部採 Soft Delete（含 DeletedByUserId / DeletedDate）。

---

### 7.1 User

- UserId  
- Account  
- PasswordHash  
- DisplayName  
- Email  
- IsActive  
- CreatedDate  

---

### 7.2.1 Role

- RoleId

- RoleName

### 7.2.2 UserRole

- UserRoleId

- UserId

- RoleId

---

### 7.3 Project

- ProjectId  
- ProjectName  
- Customer  
- Priority  
- StartDate  
- EndDate  
- CreatedByUserId  
- CreatedDate  
- IsDeleted  
- DeletedByUserId  
- DeletedDate  

---

### 7.4 Regulation

- RegulationId  
- ProjectId  
- RegulationName  
- StartDate  
- EndDate  
- CreatedByUserId  
- CreatedDate  
- IsDeleted  
- DeletedByUserId  
- DeletedDate  

---

### 7.5 TestItem

- TestItemId  
- RegulationId  
- TestItemName  
- TestType  
- TestLocation  
- EstimatedHours  
- Status  
- ManagerNote  
- CreatedByUserId  
- CreatedDate  
- IsDeleted  
- DeletedByUserId  
- DeletedDate  

---

### 7.6 TestItemEngineer

- TestItemEngineerId  
- TestItemId  
- EngineerUserId  
- RoleType（Main/Sub）  
- AssignedHours  
- CreatedByUserId  
- CreatedDate  

---

### 7.7 WorkLog

- WorkLogId  
- TestItemId  
- RevisionId  
- EngineerUserId  
- WorkDate  
- ActualHours  
- Status  
- DelayReasonId  
- Comment  
- CreatedDate  

---

### 7.8 TestItemRevision

- RevisionId  
- TestItemId  
- RevisionNumber  
- Reason  
- Description  
- CreatedByUserId  
- CreatedDate  

---

### 7.9 DelayReason

- DelayReasonId  
- ReasonText  
- ReasonType  
- IsActive  

---

## 8. ER Diagram

![alt text](image.png)

---

