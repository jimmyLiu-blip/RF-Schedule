📘 RF 案件排程系統 — 系統分析文件（SA v4.6 企業級完整版）

1. 系統目標（Goals）

RF 測試案件涉及多種法規、多種測試項目、跨工程師分配、人力排程、補測版本、延遲處理與大量工時資料，是一套跨部門的流程管理系統。本系統 v4.6 的目標是以 WinForms（DevExpress）+ .NET 8 Web API 的方式建置可維護、高一致性的 RF Scheduling System。

1.1 系統要解決的問題

以往使用 Excel 管理案件會遇到：

- 資料分散、難以同步
- 工程師負載不明確（難排班）
- 補測版本無法追蹤
- 延遲原因不透明
- Excel 人工管理導致版本衝突
- 缺少稽核（Audit）
- 主管無法快速統計 Project / Regulation / TestItem 進度

1.2 系統最終功能目標

- 完整資料結構（Project → Regulation → TestItem → Engineer → WorkLog → Revision）
- Wizard 建案流程（4 Steps）
- RBAC：Engineer / Manager
- 補測版本 TestItemRevision
- 延遲原因 DelayReason（多選）
- 所有主資料支援 Soft Delete
- AuditLog 記錄所有重要修改，含理由
- 工程師 Loading 計算
- 採用 WinForms（DevExpress）+ Web API（.NET 8 + EF Core）架構

2. 系統角色（RBAC）

本系統包含兩種角色：

- Engineer（工程師）
- Manager（主管）

2.1 角色權限矩陣

| 功能 | Engineer | Manager | 說明 |
|------|----------|---------|------|
| 查看自己負責的測項 | ✔ | ✔ | Engineer 僅能看到自己；Manager 可看到全部 |
| 回報工時 WorkLog | ✔ | ✖ | Manager 無法代回報 |
| 修改工時（7 天內） | ✔ | ✔（須理由） | Engineer 可 7 天內修改；Manager 修改皆需填理由 |
| 查看所有工程師回報紀錄 | ✖ | ✔ | Manager 可跨專案查詢 |
| 建案 Project | ✖ | ✔ | Step 1 |
| 編輯 / 刪除 Project | ✖ | ✔ | Manager 修改需填理由 |
| 設定 Regulation | ✖ | ✔ | Step 2 |
| 新增 TestItem | ✖ | ✔ | Step 3 |
| 工程師分配 | ✖ | ✔ | Step 4 |
| 建立補測版本 Revision | ✖ | ✔ | 自動切回 InProgress |
| 管理 DelayReason | ✖ | ✔ | 已使用不可刪除 |
| 管理使用者 | ✖ | ✔ | 僅能新增工程師、主管 |

2.2 Token 鑑權與 Web API

- 使用 JWT Token
- WinForms 端將 Token 儲存在記憶體（不寫入硬碟）
- API 依 Role 驗證 Claims

3. Wizard 建案流程（4 Steps）

Wizard 使用 DevExpress WizardControl。

後端採「逐步儲存草稿」。

---

3.1 Step 1：Project（建立案件）

欄位與規則：

| 欄位 | 必填 | 說明 |
|------|------|------|
| ProjectName | ✔ | 不可重複 |
| Customer | ✖ | 可留空 |
| Priority | ✔ | High / Medium / Low |
| StartDate | ✖ | 可為 null |
| EndDate | ✖ | 可為 null |

驗證：

- ProjectName 不可空
- 若 Start/End 都填 → Start ≤ End
- 新建時 ProjectStatus = Draft

---

3.2 Step 2：Regulation（設定法規）

欄位：

- RegulationName（必填）
- StartDate（必填）
- EndDate（必填）

驗證：

- 至少 1 筆 Regulation
- Start ≤ End
- 若 Project.StartDate 有填：Regulation.StartDate ≥ Project.StartDate
- 若 Project.EndDate 有填：Regulation.EndDate ≤ Project.EndDate + 30 天緩衝

---

3.3 Step 3：TestItem（建立測項）

欄位：

- TestItemName
- TestType（Conducted / Radiated / Blocking / …）
- TestLocation
- EstimatedHours
- ManagerNote

驗證：

- 每個 Regulation 至少 1 個 TestItem
- EstimatedHours > 0
- 預設 Status = NotStarted

---

3.4 Step 4：工程師分配（TestItemEngineer）

欄位：

- EngineerUserId
- RoleType（Main / Sub）
- AssignedHours

驗證：

- 至少 1 位工程師
- 必須有 Main 工程師
- AssignedHours > 0
- 不可重複指派
- 若 Σ AssignedHours ≠ EstimatedHours → 顯示黃色警示
- 顯示工程師 Loading

---

3.5 完成 Wizard

完成後：

- ProjectStatus = Active

並觸發：

- 記錄 AuditLog（動作：CreateProject）
- 未來可擴展通知工程師

---

4. 補測版本（TestItemRevision）

補測版本為 TestItem 的 v2 / v3 / v4。

4.1 欄位：

- RevisionId
- TestItemId
- RevisionNumber（v2, v3 …）
- EstimatedHours
- Reason（必填）
- Description（可選）

4.2 規則：

- 建立 Revision → TestItem.Status = InProgress
- 預設複製原工程師分配
- WorkLog 必須指定 RevisionId（null = v1）
- UI 支援切換版本工時、版本歷史

---

5. 延遲原因（DelayReason）

欄位：

- DelayReasonId
- ReasonText
- ReasonType（Equipment / Customer / Engineer / Location）
- IsActive

規則：

- WorkLog.Status = Delayed → 必選 DelayReason
- 多選（WorkLogDelayReason）
- 已使用 → 不可刪除，只能停用
- 報表顯示停用項目需標註「（已停用）」

---

6. 狀態機（State Machine）

TestItem 狀態：

NotStarted → InProgress → Completed  
      ↓                     ↑  
      Delayed ←———————┘  
      ↓  
     OnHold（手動）

自動規則：

- 第一筆 WorkLog → InProgress
- 任一 WorkLog = Delayed → TestItem = Delayed
- 所有工程師 WorkLog = Completed → TestItem = Completed
- 建立 Revision → InProgress
- Manager 手動更改 Status → 必填理由（寫入 AuditLog）

---

7. WorkLog（工時回報）規則

基本規則：

| 規則 | 說明 |
|------|------|
| WorkDate ≤ 今天 | 不可回報未來工時 |
| 0 < Hours ≤ 24 | 單日最多 24 |
| 超過 AssignedHours | 警示但允許 |
| Status = Delayed | 必填 DelayReason |

修改規則：

- Engineer：僅能修改 7 天內
- Manager：可修改任何日期，但必須填寫理由
- 所有修改 → AuditLog

---

8. Loading（負載計算）

公式：

```
AssignedLoading = Σ(AssignedHours) / WeeklyAvailableHours
```

顏色：

- 🟢 < 80%
- 🟡 80–100%
- 🔴 > 100%

---

9. Soft Delete

主資料包含欄位：

- IsDeleted
- DeletedByUserId
- DeletedDate

規則：

- 刪除 Project → 連動 Regulation、TestItem、TestItemEngineer
- WorkLog 永不刪
- DelayReason 若已被使用 → 不可刪除（只能停用）

---

10. AuditLog

欄位：

- TableName
- RecordId
- Action（Create / Update / Delete）
- OldValue（JSON）
- NewValue（JSON）
- UserId
- ModifiedDate
- Reason（Manager 修改 TestItem / WorkLog 必填）


