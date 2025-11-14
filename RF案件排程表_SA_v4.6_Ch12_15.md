# SA v4.6 — Ch.11~15 
# 11. Domain Model

## 11.1 User（使用者）
**用途**  
系統的登入帳號，包含工程師與主管。

**欄位結構**
| 欄位 | 型別 | 說明 |
|------|------|------|
| UserId | int | 主鍵 |
| Account | string | 登入帳號（唯一） |
| PasswordHash | string | Hash 後密碼 |
| DisplayName | string | 顯示名稱 |
| Email | string | 可選 |
| RoleId | int | 角色（Engineer / Manager） |
| WeeklyAvailableHours | int | 預設 40 |
| IsActive | bool | 是否可登入 |
| CreatedDate | DateTime | 建立時間 |
| LastLoginDate | DateTime? | 最後登入 |
| IsDeleted | bool | 軟刪除 |
| DeletedByUserId | int? | 刪除人 |
| DeletedDate | DateTime? | 刪除時間 |

**驗證規則**
- Account 不可重複  
- WeeklyAvailableHours > 0  
- IsActive = false → 無法登入  
- RoleId 僅能為 Engineer 或 Manager  

**業務邏輯**
- Manager 可建立 User  
- 停用工程師 → 所有負責 TestItem 標註需重新分配  
- User 刪除 → Soft Delete  

---

## 11.2 Project（案件）
**用途**  
案件主資料，Wizard Step 1 建立。

**欄位**
| 欄位 | 型別 | 說明 |
|------|------|------|
| ProjectId | int | 主鍵 |
| ProjectName | string | 名稱 |
| Customer | string? | 客戶 |
| Priority | string | High / Medium / Low |
| StartDate | DateTime? | 可空 |
| EndDate | DateTime? | 可空 |
| Status | string | Draft / Active / Completed / OnHold / Delayed |
| CreatedByUserId | int | 建立人 |
| CreatedDate | DateTime | 時間 |
| IsDeleted | bool | 軟刪除 |
| DeletedByUserId | int? | 刪除人 |
| DeletedDate | DateTime? | 刪除時間 |

**驗證**
- ProjectName 必填且不可重複  
- StartDate ≤ EndDate（若都有填）  

**自動狀態**
| 條件 | Project.Status |
|------|----------------|
| 所有 TestItem = NotStarted | Draft |
| 任一 = InProgress | Active |
| 任一 = Delayed | Delayed |
| 全部 Completed | Completed |
| Manager 手動 | OnHold |

**狀態機觸發**
- TestItem 狀態改變 → 自動更新 Project.Status  
- Manager 修改 Project → 必填理由  

---

## 11.3 Regulation（法規）
| 欄位 | 型別 |
|------|------|
| RegulationId | int |
| ProjectId | int |
| RegulationName | string |
| StartDate | DateTime |
| EndDate | DateTime |
| CreatedByUserId | int |
| CreatedDate | DateTime |
| IsDeleted | bool |
| DeletedByUserId | int? |
| DeletedDate | DateTime? |

**驗證**
- StartDate ≤ EndDate  
- StartDate ≥ Project.StartDate  
- EndDate ≤ Project.EndDate + 30 天  

**邏輯**
- 刪除 → 連動刪除 TestItem  
- Manager 修改 → 必填理由  

---

## 11.4 TestItem（測項）
| 欄位 | 型別 |
|------|------|
| TestItemId | int |
| RegulationId | int |
| TestItemName | string |
| TestType | string |
| TestLocation | string |
| EstimatedHours | decimal |
| Status | string |
| ManagerNote | string? |
| CreatedByUserId | int |
| CreatedDate | DateTime |
| IsDeleted | bool |
| DeletedByUserId | int? |
| DeletedDate | DateTime? |

**驗證**
- EstimatedHours > 0  
- Name 不可空  

**自動狀態**
| 事件 | 新狀態 |
|------|--------|
| 第一筆 WorkLog | InProgress |
| 任一 WorkLog.Delayed | Delayed |
| 所有 WorkLog.Completed | Completed |
| 建立 Revision | InProgress |

**Manager 編輯必填理由：**
- EstimatedHours  
- 日期（未來可能增加）  
- Status  

---

## 11.5 TestItemEngineer（工程師分配）
| 欄位 | 型別 |
|------|------|
| TestItemEngineerId | int |
| TestItemId | int |
| EngineerUserId | int |
| RoleType | string |
| AssignedHours | decimal |
| CreatedByUserId | int |
| CreatedDate | DateTime |
| IsDeleted | bool |
| DeletedByUserId | int? |
| DeletedDate | DateTime? |

**驗證**
- AssignedHours > 0  
- 不可重複指派  
- 至少 1 位 Main  

---

## 11.6 WorkLog（工時）
| 欄位 | 型別 |
|------|------|
| WorkLogId | int |
| TestItemId | int |
| RevisionId | int? |
| EngineerUserId | int |
| WorkDate | Date |
| ActualHours | decimal |
| Status | string |
| Comment | string? |
| CreatedDate | DateTime |
| ModifiedDate | DateTime? |
| ModificationReason | string? |

**驗證**
- WorkDate ≤ 今天  
- 0 < Hours ≤ 24  
- Status = Delayed → 必選 DelayReason  

**編輯規則**
- Engineer：7 天內可改  
- Manager：任何時間，但需理由  

---

## 11.7 WorkLogDelayReason（多對多）
| 欄位 | 型別 |
|------|------|
| WorkLogDelayReasonId | int |
| WorkLogId | int |
| DelayReasonId | int |

---

## 11.8 TestItemRevision（補測版本）
| 欄位 | 型別 |
|------|------|
| RevisionId | int |
| TestItemId | int |
| RevisionNumber | string |
| EstimatedHours | decimal |
| Reason | string |
| Description | string? |
| CreatedByUserId | int |
| CreatedDate | DateTime |

**事件**
- TestItem.Status = InProgress  
- 預設複製工程師  
- WorkLog 需選 RevisionId  

---

## 11.9 DelayReason
| 欄位 | 型別 |
|------|------|
| DelayReasonId | int |
| ReasonText | string |
| ReasonType | string |
| IsActive | bool |

已使用 → 不可刪除，只能停用。

---

## 11.10 AuditLog
| 欄位 | 說明 |
|------|------|
| AuditLogId | 主鍵 |
| TableName | 表名 |
| RecordId | 主鍵值 |
| Action | CRUD |
| OldValue | JSON |
| NewValue | JSON |
| UserId | 誰改的 |
| ModifiedDate | 何時 |
| Reason | 修改原因（需要時） |



## 12. Web API 設計
核心原則：RESTful、分層、可版本化、支援 RBAC 與 AuditLog。

### 12.1 API 結構
- /api/auth
- /api/projects
- /api/regulations
- /api/testitems
- /api/testitemengineers
- /api/worklogs
- /api/revisions
- /api/delayreasons
- /api/loading
- /api/auditlogs

### 12.2 鑑權
- JWT Token（登入後由 WinForms 保存於記憶體）
- 每個端點依 Role 進行 Claims 驗證
- Manager 端點需 Manager/ Admin Claims

### 12.3 重要端點
#### Auth
- POST /api/auth/login
- GET /api/auth/me

#### Project
- GET/POST/PUT/DELETE（採 SoftDelete）
- 編輯需填寫理由 → AuditLog

#### WorkLog
- GET /api/testitems/{id}/worklogs
- POST /api/worklogs
- PUT /api/worklogs/{id}（含 ModificationReason）

#### Revision
- POST /api/testitems/{id}/revisions → 自動複製工程師

---

## 13. 架構（Architecture）

### 13.1 方案架構
WinForms（DevExpress）
→ 呼叫 Web API（.NET 8）
→ Service Layer（商業邏輯）
→ Repository（EF Core）
→ SQL Server

### 13.2 分層說明
- **WinForms**：僅呈現 UI、呼叫 API
- **API Controller**：驗證、接收請求
- **Service**：業務邏輯（驗證、計算、狀態機）
- **Repository**：資料 CRUD
- **EF Core**：交易、Migration
- **Infrastructure**：AuditLog、SoftDelete、RowVersion

### 13.3 垂直切片（Vertical Slice）
每個資料夾依功能切片，如：
- Project/
- Regulation/
- TestItem/
- WorkLog/

---

## 14. 非功能需求（Non-Functional Requirements）

### 14.1 效能
- Loading 計算 < 2 秒
- Wizard 各 Step < 1 秒
- WorkLog 查詢 100 筆 < 3 秒

### 14.2 安全性
- HTTPS 全站
- 密碼 Hash：BCrypt
- Session idle 30 分鐘自動登出
- SQL Injection 防護（全採用 Parameterized Query）
- XSS 防護（輸入皆 Encode）

### 14.3 可用性
- 每日自動備份
- RTO < 4 小時（災難復原）
- RPO < 1 小時（資料復原點）

### 14.4 可擴展性
- API Versioning
- Redis 快取
- 多語系（i18n）

---

## 15. 例外與併發控制

### 15.1 併發編輯
- 採用 RowVersion（樂觀鎖）
- 資料異動衝突 → 回傳 409 Conflict
- 前端顯示：「資料已被他人修改」

### 15.2 網路中斷
- WinForms 先寫入 LocalStorage（暫存）
- 恢復後自動重新送出
- 若資料逾期 7 天 → 提示刪除

### 15.3 資料不一致
每日排程檢查：
- Orphan WorkLog
- Orphan TestItemEngineer
- Orphan Regulation
結果寫入 Log + Email Admin

### 15.4 工程師離職流程
- 停用帳號（IsActive=false）
- 所有 TestItemEngineer 標記為「需重新分配」
- Manager 收到通知清單

---
