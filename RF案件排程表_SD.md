# 📙 RF案件排程系統 — 系統設計文件 (SD v2.0)

---

## 1. 系統架構設計

### 1.1 整體架構圖

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                   │
│  ┌───────────────────────────────────────────────────┐  │
│  │  WinForms Application (DevExpress)                │  │
│  │  ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐   │  │
│  │  │Wizard  │  │WorkLog │  │Loading │  │Report  │   │  │
│  │  │Forms   │  │Forms   │  │Forms   │  │Forms   │   │  │
│  │  └────────┘  └────────┘  └────────┘  └────────┘   │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                           ↓ HTTPS (JSON)
┌─────────────────────────────────────────────────────────┐
│                   Application Layer                     │
│  ┌───────────────────────────────────────────────────┐  │
│  │        ASP.NET Core Web API (.NET 8)              │  │
│  │  ┌──────────────────────────────────────────────┐ │  │
│  │  │ Controllers                                  │ │  │
│  │  │ ┌────┐ ┌────┐ ┌────┐ ┌────┐ ┌────┐ ┌────┐    │ │  │
│  │  │ │Auth│ │Proj│ │Test│ │Work│ │Load│ │Audit│   │ │  │
│  │  │ └────┘ └────┘ └────┘ └────┘ └────┘ └────┘    │ │  │
│  │  └──────────────────────────────────────────────┘ │  │
│  │  ┌──────────────────────────────────────────────┐ │  │
│  │  │ Filters & Middleware                         │ │  │
│  │  │ • Authentication Filter                      │ │  │
│  │  │ • Authorization Filter                       │ │  │
│  │  │ • AuditLog Filter                            │ │  │
│  │  │ • Exception Handler Middleware               │ │  │
│  │  └──────────────────────────────────────────────┘ │  │
│  │  ┌──────────────────────────────────────────────┐ │  │
│  │  │ Services (Business Logic)                    │ │  │
│  │  │ • ProjectService                             │ │  │
│  │  │ • WorkLogService                             │ │  │
│  │  │ • LoadingService                             │ │  │
│  │  │ • AuditLogService                            │ │  │
│  │  │ • EmailService                               │ │  │
│  │  └──────────────────────────────────────────────┘ │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                   Data Access Layer                     │
│  ┌───────────────────────────────────────────────────┐  │
│  │        Entity Framework Core 8.0                  │  │
│  │  ┌──────────────────────────────────────────────┐ │  │
│  │  │ DbContext: RFSchedulingDbContext             │ │  │
│  │  │ • Query Filters (Soft Delete)                │ │  │
│  │  │ • Change Tracking                            │ │  │
│  │  │ • Transaction Management                     │ │  │
│  │  └──────────────────────────────────────────────┘ │  │
│  │  ┌──────────────────────────────────────────────┐ │  │
│  │  │ Repositories (Optional)                      │ │  │
│  │  │ • Generic Repository Pattern                 │ │  │
│  │  └──────────────────────────────────────────────┘ │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                      Database Layer                     │
│  ┌───────────────────────────────────────────────────┐  │
│  │          SQL Server 2019 Express                  │  │
│  │  • Tables (20+ tables)                            │  │
│  │  • Indexes                                        │  │
│  │  • Foreign Keys                                   │  │
│  │  • Stored Procedures (Optional)                   │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                    External Services                    │
│  ┌───────────────────────────────────────────────────┐  │
│  │  SMTP Server (Email Notifications)                │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

---

### 1.2 分層職責說明

#### 1.2.1 Presentation Layer (WinForms)
**職責：**
- 使用者互動介面
- 輸入驗證（前端驗證）
- 顯示資料與錯誤訊息
- 呼叫Web API

**技術：**
- WinForms (.NET 8)
- DevExpress WinForms Controls
- HttpClient (API通訊)
- Newtonsoft.Json (JSON序列化)

**不包含：**
- 業務邏輯運算
- 直接存取資料庫
- 複雜的資料處理

---

#### 1.2.2 Application Layer (Web API)
**職責：**
- 接收HTTP請求
- 身份驗證與授權
- 業務邏輯處理
- Transaction管理
- 回應HTTP回應

**技術：**
- ASP.NET Core Web API 8.0
- JWT Bearer Authentication
- AutoMapper (DTO映射)
- FluentValidation (驗證)

**關鍵Service：**
- **ProjectService:** 案件建立、修改、狀態計算
- **WorkLogService:** 工時回報、狀態同步
- **LoadingService:** Loading計算
- **AuditLogService:** 稽核日誌記錄
- **EmailService:** Email發送（密碼重設、通知）

---

#### 1.2.3 Data Access Layer (EF Core)
**職責：**
- ORM對應
- Query優化
- Change Tracking
- Transaction管理
- Soft Delete處理

**技術：**
- Entity Framework Core 8.0
- Code First Approach
- Migration管理

**關鍵機制：**
- Soft Delete
- AuditLog自動記錄
- RowVersion
---

#### 1.2.4 Database Layer (SQL Server)
**職責：**
- 資料持久化
- 資料完整性約束
- 索引優化
- 備份與復原

**技術：**
- SQL Server 2019 Express
- Database Edition: Express

---

## 2. 資料庫設計

### 2.1 資料庫架構總覽

**資料庫名稱：** RFScheduling

**字符集：** UTF-8 

**Collation：** Chinese_Taiwan_Stroke_CI_AS

**資料表數量：** 14個主要資料表

---

### 2.2 完整資料表設計

#### 2.2.1 Role (角色)
```sql
CREATE TABLE [dbo].[Role] (
    [RoleId]        INT            IDENTITY(1,1) NOT NULL,
    [RoleName]      NVARCHAR(50)   NOT NULL,
    [Description]   NVARCHAR(50)   NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleId])
);

-- 初始資料
INSERT INTO [Role] (RoleName, Description) VALUES 
    ('Engineer', '工程師'),
    ('Manager', '主管');
```

---

#### 2.2.2 User (使用者)
```sql
CREATE TABLE [dbo].[User] (
    [UserId]                INT          IDENTITY(1,1) NOT NULL,
    [Account]               NVARCHAR(50)   NOT NULL,
    [PasswordHash]          NVARCHAR(255)  NOT NULL,
    [DisplayName]           NVARCHAR(100)  NOT NULL,
    [Email]                 NVARCHAR(255)  NOT NULL,
    [RoleId]                INT            NOT NULL,
    [WeeklyAvailableHours]  DECIMAL(5,2)   NOT NULL DEFAULT 37.5,
    [IsActive]              BIT            NOT NULL DEFAULT 1,
    [CreatedByUserId]       INT            NULL,
    [CreatedDate]           DATETIME       NOT NULL DEFAULT GETDATE(),
    [ModifiedDate]          DATETIME       NULL,
    [IsDeleted]             BIT            NOT NULL DEFAULT 0,
    [DeletedByUserId]       INT            NULL,
    [DeletedDate]           DATETIME       NULL,
    [RowVersion]            ROWVERSION     NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId]),
    CONSTRAINT [FK_User_Role] FOREIGN KEY ([RoleId]) 
        REFERENCES [Role]([RoleId]),
    CONSTRAINT [UQ_User_Account] UNIQUE ([Account]),
    CONSTRAINT [UQ_User_Email] UNIQUE ([Email]),
    CONSTRAINT [CK_User_WeeklyHours] CHECK ([WeeklyAvailableHours] > 0 AND [WeeklyAvailableHours] <= 72)
);

CREATE NONCLUSTERED INDEX [IX_User_RoleId] ON [User]([RoleId]);
CREATE NONCLUSTERED INDEX [IX_User_IsActive] ON [User]([IsActive]) WHERE [IsDeleted] = 0;
```

**欄位說明：**
- `PasswordHash`: 使用bcrypt Hash，長度255足夠
- `WeeklyAvailableHours`: 每週可工作時數，預設37.5（每日7.5小時×5天）
- `RowVersion`: 用於併發控制

---

#### 2.2.3 PasswordReset (密碼重設)
```sql
CREATE TABLE [dbo].[PasswordReset] (
    [PasswordResetId]   INT             IDENTITY(1,1) NOT NULL,
    [UserId]            INT             NOT NULL,
    [Token]             NVARCHAR(255)   NOT NULL,
    [ExpireAt]          DATETIME        NOT NULL,
    [UsedAt]            DATETIME        NULL,
    [CreatedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PasswordReset] PRIMARY KEY CLUSTERED ([PasswordResetId]),
    CONSTRAINT [FK_PasswordReset_User] FOREIGN KEY ([UserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [UQ_PasswordReset_Token] UNIQUE ([Token])
);

CREATE NONCLUSTERED INDEX [IX_PasswordReset_UserId] ON [PasswordReset]([UserId]);
```

**欄位說明：**
- `Token`: GUID + Hash，用於Email連結
- `ExpireAt`: 到期時間（建立後30分鐘）
- `UsedAt`: 使用時間，NULL表示未使用

---

#### 2.2.4 Project (案件)
```sql
CREATE TABLE [dbo].[Project] (
    [ProjectId]         INT             IDENTITY(1,1) NOT NULL,
    [ProjectName]       NVARCHAR(200)   NOT NULL,
    [Customer]          NVARCHAR(200)   NULL,
    [Priority]          NVARCHAR(20)    NOT NULL DEFAULT 'Medium', -- High, Medium, Low
    [Status]            NVARCHAR(20)    NOT NULL DEFAULT 'Draft', -- Draft, Active, Completed, OnHold, Delayed
    [StartDate]         DATE            NULL,
    [EndDate]           DATE            NULL,
    [Note]              NVARCHAR(1000)  NULL,
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME     NOT NULL DEFAULT GETDATE(),
    [ModifiedDate]      DATETIME        NULL,
    [IsDeleted]         BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]   INT             NULL,
    [DeletedDate]       DATETIME        NULL,
    [RowVersion]        ROWVERSION      NOT NULL,
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([ProjectId]),
    CONSTRAINT [FK_Project_CreatedBy] FOREIGN KEY ([CreatedByUserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [UQ_Project_Name] UNIQUE ([ProjectName]) WHERE [IsDeleted] = 0,
    CONSTRAINT [CK_Project_Priority] CHECK ([Priority] IN ('High', 'Medium', 'Low')),
    CONSTRAINT [CK_Project_Status] CHECK ([Status] IN ('Draft', 'Active', 'Completed', 'OnHold', 'Delayed')),
    CONSTRAINT [CK_Project_DateRange] CHECK ([EndDate] IS NULL OR [StartDate] IS NULL OR [EndDate] >= [StartDate])
);

CREATE NONCLUSTERED INDEX [IX_Project_Status] ON [Project]([Status]) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_Project_CreatedDate] ON [Project]([CreatedDate]) WHERE [IsDeleted] = 0;
```

---

#### 2.2.5 Regulation (法規)
```sql
CREATE TABLE [dbo].[Regulation] (
    [RegulationId]      INT             IDENTITY(1,1) NOT NULL,
    [ProjectId]         INT             NOT NULL,
    [RegulationName]    NVARCHAR(100)   NOT NULL, -- FCC, NCC, CE, IC, TELEC
    [StartDate]         DATE            NOT NULL,
    [EndDate]           DATE            NOT NULL,
    [Note]              NVARCHAR(500)   NULL,
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedDate]      DATETIME        NULL,
    [IsDeleted]         BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]   INT             NULL,
    [DeletedDate]       DATETIME        NULL,
    CONSTRAINT [PK_Regulation] PRIMARY KEY CLUSTERED ([RegulationId]),
    CONSTRAINT [FK_Regulation_Project] FOREIGN KEY ([ProjectId]) 
        REFERENCES [Project]([ProjectId]),
    CONSTRAINT [FK_Regulation_CreatedBy] FOREIGN KEY ([CreatedByUserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [CK_Regulation_DateRange] CHECK ([EndDate] >= [StartDate])
);

CREATE NONCLUSTERED INDEX [IX_Regulation_ProjectId] ON [Regulation]([ProjectId]) WHERE [IsDeleted] = 0;
```

---

#### 2.2.6 TestItem (測試項目)
```sql
CREATE TABLE [dbo].[TestItem] (
    [TestItemId]        INT             IDENTITY(1,1) NOT NULL,
    [RegulationId]      INT             NOT NULL,
    [TestItemName]      NVARCHAR(200)   NOT NULL,
    [TestType]          NVARCHAR(100)   NOT NULL, -- Conducted, Radiated, Blocking, DFS, PWS, Adaptivity
    [TestLocation]      NVARCHAR(100)   NOT NULL, -- Lab A, Lab B, Lab C
    [EstimatedHours]    DECIMAL(10,2)   NOT NULL,
    [Status]            NVARCHAR(20)    NOT NULL DEFAULT 'NotStarted',
    [ManagerNote]       NVARCHAR(500)   NULL,
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME     NOT NULL DEFAULT GETDATE(),
    [ModifiedDate]      DATETIME        NULL,
    [IsDeleted]         BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]   INT             NULL,
    [DeletedDate]       DATETIME        NULL,
    [RowVersion]        ROWVERSION      NOT NULL,
    CONSTRAINT [PK_TestItem] PRIMARY KEY CLUSTERED ([TestItemId]),
    CONSTRAINT [FK_TestItem_Regulation] FOREIGN KEY ([RegulationId]) 
        REFERENCES [Regulation]([RegulationId]),
    CONSTRAINT [FK_TestItem_CreatedBy] FOREIGN KEY ([CreatedByUserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [CK_TestItem_Status] CHECK ([Status] IN ('NotStarted', 'InProgress', 'Completed', 'Delayed', 'OnHold')),
    CONSTRAINT [CK_TestItem_EstimatedHours] CHECK ([EstimatedHours] > 0)
);

CREATE NONCLUSTERED INDEX [IX_TestItem_RegulationId] ON [TestItem]([RegulationId]) WHERE [IsDeleted] = 0;
CREATE NONCLUSTERED INDEX [IX_TestItem_Status] ON [TestItem]([Status]) WHERE [IsDeleted] = 0;
```

---

#### 2.2.7 TestItemEngineer (工程師分配)
```sql
CREATE TABLE [dbo].[TestItemEngineer] (
    [TestItemEngineerId]    INT          IDENTITY(1,1) NOT NULL,
    [TestItemId]            INT             NOT NULL,
    [EngineerUserId]        INT             NOT NULL,
    [RoleType]              NVARCHAR(20)    NOT NULL, -- Main, Sub
    [AssignedHours]         DECIMAL(10,2)   NOT NULL,
    [CreatedByUserId]       INT             NOT NULL,
    [CreatedDate]           DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedDate]          DATETIME        NULL,
    [IsDeleted]             BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]       INT             NULL,
    [DeletedDate]           DATETIME        NULL,
    CONSTRAINT [PK_TestItemEngineer] PRIMARY KEY CLUSTERED ([TestItemEngineerId]),
    CONSTRAINT [FK_TestItemEngineer_TestItem] FOREIGN KEY ([TestItemId]) 
        REFERENCES [TestItem]([TestItemId]),
    CONSTRAINT [FK_TestItemEngineer_User] FOREIGN KEY ([EngineerUserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [FK_TestItemEngineer_CreatedBy] FOREIGN KEY ([CreatedByUserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [CK_TestItemEngineer_RoleType] CHECK ([RoleType] IN ('Main', 'Sub')),
    CONSTRAINT [CK_TestItemEngineer_AssignedHours] CHECK ([AssignedHours] > 0)
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_TestItemEngineer] 
    ON [TestItemEngineer]([TestItemId], [EngineerUserId]) 
    WHERE [IsDeleted] = 0;

CREATE NONCLUSTERED INDEX [IX_TestItemEngineer_EngineerUserId] 
    ON [TestItemEngineer]([EngineerUserId]) WHERE [IsDeleted] = 0;
```

**欄位說明：**
- `RoleType`: Main=主要負責，Sub=支援
- `AssignedHours`: 分配給該工程師的工時

---

#### 2.2.8 TestItemRevision (補測版本)
```sql
CREATE TABLE [dbo].[TestItemRevision] (
    [RevisionId]        INT             IDENTITY(1,1) NOT NULL,
    [TestItemId]        INT             NOT NULL,
    [RevisionNumber]    NVARCHAR(10)    NOT NULL, -- v2, v3, v4
    [EstimatedHours]    DECIMAL(10,2)   NOT NULL,
    [Reason]            NVARCHAR(200)   NOT NULL,
    [Description]       NVARCHAR(500)   NULL,
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedDate]      DATETIME        NULL,
    [IsDeleted]         BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]   INT             NULL,
    [DeletedDate]       DATETIME        NULL,
    CONSTRAINT [PK_TestItemRevision] PRIMARY KEY CLUSTERED ([RevisionId]),
    CONSTRAINT [FK_TestItemRevision_TestItem] FOREIGN KEY ([TestItemId]) 
        REFERENCES [TestItem]([TestItemId]),
    CONSTRAINT [FK_TestItemRevision_CreatedBy] FOREIGN KEY ([CreatedByUserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [CK_TestItemRevision_EstimatedHours] CHECK ([EstimatedHours] > 0)
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_TestItemRevision] 
    ON [TestItemRevision]([TestItemId], [RevisionNumber]) 
    WHERE [IsDeleted] = 0;
```

---

#### 2.2.9 WorkLog (工時記錄)
```sql
CREATE TABLE [dbo].[WorkLog] (
    [WorkLogId]             INT          IDENTITY(1,1) NOT NULL,
    [TestItemId]            INT             NOT NULL,
    [RevisionId]            INT             NULL, -- NULL = v1
    [EngineerUserId]        INT             NOT NULL,
    [WorkDate]              DATE            NOT NULL,
    [ActualHours]           DECIMAL(10,2)   NOT NULL,
    [Status]                NVARCHAR(20)    NOT NULL, -- InProgress, Completed, Delayed
    [Comment]               NVARCHAR(500)   NULL,
    [CreatedDate]           DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedDate]          DATETIME        NULL,
    [ModificationReason]    NVARCHAR(500)   NULL, -- Manager覆寫時填寫
    CONSTRAINT [PK_WorkLog] PRIMARY KEY CLUSTERED ([WorkLogId]),
    CONSTRAINT [FK_WorkLog_TestItem] FOREIGN KEY ([TestItemId]) 
        REFERENCES [TestItem]([TestItemId]),
    CONSTRAINT [FK_WorkLog_Revision] FOREIGN KEY ([RevisionId]) 
        REFERENCES [TestItemRevision]([RevisionId]),
    CONSTRAINT [CK_WorkLog_Status] CHECK ([Status] IN ('InProgress', 'Completed', 'Delayed')),
    CONSTRAINT [CK_WorkLog_ActualHours] CHECK ([ActualHours] > 0 AND [ActualHours] <= 12)
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_WorkLog_DateUser] 
    ON [WorkLog]([TestItemId], [EngineerUserId], [WorkDate], [RevisionId]);

CREATE NONCLUSTERED INDEX [IX_WorkLog_EngineerUserId] 
    ON [WorkLog]([EngineerUserId]);

CREATE NONCLUSTERED INDEX [IX_WorkLog_WorkDate] 
    ON [WorkLog]([WorkDate]);
```

**欄位說明：**
- `RevisionId`: NULL表示v1（原始版本），非NULL表示補測版本
- `ModificationReason`: Manager修改工時時填寫理由

---

#### 2.2.10 DelayReason (延遲原因)
```sql
CREATE TABLE [dbo].[DelayReason] (
    [DelayReasonId]     INT             IDENTITY(1,1) NOT NULL,
    [ReasonText]        NVARCHAR(200)   NOT NULL,
    [ReasonType]        NVARCHAR(50)    NOT NULL, -- Equipment, Customer, Engineer, Location, Other
    [IsActive]          BIT             NOT NULL DEFAULT 1,
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME     NOT NULL DEFAULT GETDATE(),
    [ModifiedDate]      DATETIME        NULL,
    CONSTRAINT [PK_DelayReason] PRIMARY KEY CLUSTERED ([DelayReasonId]),
    CONSTRAINT [FK_DelayReason_CreatedBy] FOREIGN KEY ([CreatedByUserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [CK_DelayReason_Type] CHECK ([ReasonType] IN ('Equipment', 'Customer', 'Engineer', 'Location', 'Other')),
    CONSTRAINT [UQ_DelayReason_Text] UNIQUE ([ReasonText])
);

CREATE NONCLUSTERED INDEX [IX_DelayReason_IsActive] ON [DelayReason]([IsActive]);

-- 初始資料
INSERT INTO [DelayReason] (ReasonText, ReasonType, CreatedByUserId) VALUES 
    ('測試設備故障', 'Equipment', 1),
    ('客戶延遲提供樣品', 'Customer', 1),
    ('工程師人力不足', 'Engineer', 1),
    ('測試場地被佔用', 'Location', 1),
    ('其他原因', 'Other', 1);
```

---

#### 2.2.11 WorkLogDelayReason (工時延遲原因關聯)
```sql
CREATE TABLE [dbo].[WorkLogDelayReason] (
    [WorkLogDelayReasonId]  INT         IDENTITY(1,1) NOT NULL,
    [WorkLogId]             INT         NOT NULL,
    [DelayReasonId]         INT         NOT NULL,
    CONSTRAINT [PK_WorkLogDelayReason] PRIMARY KEY CLUSTERED ([WorkLogDelayReasonId]),
    CONSTRAINT [FK_WorkLogDelayReason_WorkLog] FOREIGN KEY ([WorkLogId]) 
        REFERENCES [WorkLog]([WorkLogId]) ON DELETE CASCADE,
    CONSTRAINT [FK_WorkLogDelayReason_DelayReason] FOREIGN KEY ([DelayReasonId]) 
        REFERENCES [DelayReason]([DelayReasonId])
);

CREATE UNIQUE NONCLUSTERED INDEX [UQ_WorkLogDelayReason] 
    ON [WorkLogDelayReason]([WorkLogId], [DelayReasonId]);

CREATE NONCLUSTERED INDEX [IX_WorkLogDelayReason_DelayReasonId] 
    ON [WorkLogDelayReason]([DelayReasonId]);
```

---

#### 2.2.12 AuditLog (稽核日誌)
```sql
CREATE TABLE [dbo].[AuditLog] (
    [AuditLogId]    BIGINT          IDENTITY(1,1) NOT NULL,
    [TableName]     NVARCHAR(50)    NOT NULL,
    [RecordId]      INT             NOT NULL,
    [Action]        NVARCHAR(20)    NOT NULL, -- Create, Update, Delete, PasswordReset
    [OldValue]      NVARCHAR(MAX)   NULL, -- JSON
    [NewValue]      NVARCHAR(MAX)   NULL, -- JSON
    [UserId]        INT             NOT NULL,
    [ModifiedDate]  DATETIME        NOT NULL DEFAULT GETDATE(),
    [Reason]        NVARCHAR(500)   NULL,
    CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([AuditLogId]),
    CONSTRAINT [FK_AuditLog_User] FOREIGN KEY ([UserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [CK_AuditLog_Action] CHECK ([Action] IN ('Create', 'Update', 'Delete', 'PasswordReset'))
);

CREATE NONCLUSTERED INDEX [IX_AuditLog_TableRecord] 
    ON [AuditLog]([TableName], [RecordId]);

CREATE NONCLUSTERED INDEX [IX_AuditLog_UserId] 
    ON [AuditLog]([UserId]);

CREATE NONCLUSTERED INDEX [IX_AuditLog_ModifiedDate] 
    ON [AuditLog]([ModifiedDate] DESC);
```

**欄位說明：**
- `OldValue` / `NewValue`: 儲存JSON格式的變更前後值
- `Reason`: 重要修改需填寫理由（如Manager覆寫工時）

---

#### 2.2.13 SystemSetting (系統設定)
```sql
CREATE TABLE [dbo].[SystemSetting] (
    [SettingId]     INT             IDENTITY(1,1) NOT NULL,
    [SettingKey]    NVARCHAR(100)   NOT NULL,
    [SettingValue]  NVARCHAR(500)   NULL,
    [Description]   NVARCHAR(200)   NULL,
    [ModifiedDate]  DATETIME        NULL,
    CONSTRAINT [PK_SystemSetting] PRIMARY KEY CLUSTERED ([SettingId]),
    CONSTRAINT [UQ_SystemSetting_Key] UNIQUE ([SettingKey])
);

-- 初始資料
INSERT INTO [SystemSetting] (SettingKey, SettingValue, Description) VALUES 
    ('DefaultWeeklyHours', '37.5', '預設每週工作時數'),
    ('WorkLogEditDays', '7', 'WorkLog可修改天數'),
    ('LoginFailLimit', '5', '登入失敗鎖定次數'),
    ('LockoutMinutes', '10', '鎖定時間(分鐘)'),
    ('PasswordResetExpireMinutes', '30', '密碼重設連結有效期限(分鐘)'),
    ('AuditLogRetentionDays', '365', 'AuditLog保留天數'),
    ('DeletedDataRetentionDays', '180', '已刪除資料保留天數'),
    ('SmtpServer', 'smtp.company.com', 'SMTP伺服器'),
    ('SmtpPort', '25', 'SMTP Port'),
    ('SmtpEnableSSL', 'false', '是否啟用SSL'),
    ('SenderEmail', 'noreply@company.com', '寄件者Email'),
    ('SenderName', 'RF排程系統', '寄件者名稱');
```

---

### 2.3 資料庫關聯圖

```
┌──────────┐
│   Role   │
└──────────┘
      ↑
      │ 1:N
      │
┌──────────┐         ┌──────────────────┐
│   User   │────────→│ PasswordReset    │
└──────────┘ 1:N     └──────────────────┘
      ↑
      │ 1:N
      │
┌──────────┐
│ Project  │
└──────────┘
      ↑
      │ 1:N
      │
┌──────────┐
│Regulation│
└──────────┘
      ↑
      │ 1:N
      │
┌──────────┐         ┌──────────────────┐
│ TestItem │────────→│TestItemRevision  │
└──────────┘ 1:N     └──────────────────┘
      ↑                      ↑
      │ N:M                  │
      │                      │ 0:N
┌──────────────────┐         │
│TestItemEngineer  │         │
└──────────────────┘         │
      ↑                      │
      │ 1:N                  │
      │                      │
┌──────────┐                 │
│ WorkLog  │─────────────────┘
└──────────┘
      ↑
      │ N:M
      │
┌──────────────────┐    ┌──────────┐
│WorkLogDelayReason│───→│DelayReason│
└──────────────────┘ N:1└──────────┘

┌──────────┐
│AuditLog  │ (獨立記錄所有異動)
└──────────┘
```

---

### 2.4 索引策略

#### 2.4.1 關鍵查詢的索引

**查詢1：工程師查看自己的測項**
```sql
-- Query: SELECT * FROM TestItem WHERE EngineerUserId = @UserId AND IsDeleted = 0
-- Index: IX_TestItemEngineer_EngineerUserId 
```

**查詢2：Loading計算**
```sql
-- Query: 
-- SELECT EngineerUserId, SUM(AssignedHours) 
-- FROM TestItemEngineer tie
-- JOIN TestItem ti ON tie.TestItemId = ti.TestItemId
-- JOIN Regulation r ON ti.RegulationId = r.RegulationId
-- JOIN Project p ON r.ProjectId = p.ProjectId
-- WHERE p.Status = 'Active' AND tie.IsDeleted = 0
-- GROUP BY EngineerUserId

-- 需要的索引：
CREATE NONCLUSTERED INDEX [IX_Project_Status_Include] 
    ON [Project]([Status]) 
    INCLUDE ([ProjectId])
    WHERE [IsDeleted] = 0;
```

**查詢3：案件進度報表**
```sql
-- Query: 
-- SELECT * FROM Project p
-- LEFT JOIN Regulation r ON p.ProjectId = r.ProjectId
-- LEFT JOIN TestItem ti ON r.RegulationId = ti.RegulationId
-- WHERE p.ProjectId = @ProjectId

-- Index: 已有FK索引，無需額外建立
```

**查詢4：稽核日誌查詢**
```sql
-- Query: 
-- SELECT * FROM AuditLog 
-- WHERE TableName = @TableName AND RecordId = @RecordId
-- ORDER BY ModifiedDate DESC

-- Index: IX_AuditLog_TableRecord (已建立)
```

---

#### 2.4.2 覆蓋索引 (Covering Index)

**工時查詢優化：**
```sql
CREATE NONCLUSTERED INDEX [IX_WorkLog_Covering] 
    ON [WorkLog]([EngineerUserId], [WorkDate])
    INCLUDE ([TestItemId], [ActualHours], [Status])
    WHERE [WorkDate] >= DATEADD(MONTH, -3, GETDATE());
```

**說明：** 針對近3個月的工時查詢優化，包含常用欄位避免Key Lookup

---

## 3. API設計規範

### 3.1 API端點總覽

**Base URL：** `https://api.company.com/api`

**版本控制：** URL Path (`/v1/`)

**完整URL範例：** `https://api.company.com/api/v1/auth/login`

---

### 3.2 認證模組 API

#### 3.2.1 POST /api/v1/auth/login
**功能：** 使用者登入

#### 3.2.2 POST /api/v1/auth/forgot-password
**功能：** 申請密碼重設

#### 3.2.3 GET /api/v1/auth/validate-reset-token
**功能：** 驗證重設Token

#### 3.2.4 POST /api/v1/auth/reset-password
**功能：** 執行密碼重設

---

### 3.3 案件管理 API

#### 3.3.1 POST /api/v1/projects/create-with-wizard
**功能：** Wizard建案（一次性建立完整案件）

#### 3.3.2 GET /api/v1/projects
**功能：** 取得案件清單

#### 3.3.3 GET /api/v1/projects/{id}
**功能：** 取得案件詳細資訊

---

### 3.4 工時管理 API

#### 3.4.1 GET /api/v1/worklogs/my-tasks
**功能：** 取得我的測項清單（工程師專用）

#### 3.4.2 POST /api/v1/worklogs
**功能：** 回報工時

#### 3.4.3 GET /api/v1/worklogs/testitem/{testItemId}
**功能：** 取得測項的工時記錄

---

### 3.5 Loading分析 API

#### 3.5.1 GET /api/v1/loading/engineers
**功能：** 取得所有工程師Loading

#### 3.5.2 GET /api/v1/loading/engineers/{id}
**功能：** 取得工程師Loading明細

---

## 4. 安全設計

### 4.1 認證機制 (Authentication)

#### 4.1.1 JWT Token設計

**Token Payload：**

**Token Generation (C#)：**

---

#### 4.1.2 密碼Hash設計

**使用bcrypt：**

---

#### 4.1.3 密碼重設Token設計

**Token生成：**

---

### 4.2 授權機制 (Authorization)

#### 4.2.1 Role-Based Access Control

**Controller層級授權：**

**Action層級授權：**

---

#### 4.2.2 Resource-Based Authorization

**工程師只能查看/修改自己的WorkLog：**

---

### 4.3 API安全最佳實踐

#### 4.3.1 HTTPS強制

#### 4.3.2 CORS設定

#### 4.3.3 Rate Limiting

### 4.4 SQL Injection防護

**使用Parameterized Query (EF Core自動處理)：**
```csharp
// ✅ 安全：EF Core自動參數化
var projects = await _dbContext.Projects
    .Where(p => p.ProjectName.Contains(keyword))
    .ToListAsync();

// ✅ 如果必須使用原生SQL，使用參數化
var projects = await _dbContext.Projects
    .FromSqlRaw("SELECT * FROM Project WHERE ProjectName LIKE {0}", $"%{keyword}%")
    .ToListAsync();
```
### 4.5 XSS防護

**前端處理：**
```csharp
// WinForms會自動處理TextBox的HTML編碼
// 但在顯示富文本時需注意

public string SanitizeHtml(string input)
{
    // 使用HtmlAgilityPack或類似Library
    return System.Net.WebUtility.HtmlEncode(input);
}
```

**API回應：**
- JSON序列化自動處理特殊字元
- 不回傳原始HTML

---

## 5. Entity Framework Core設計

### 5.1 DbContext設計

```csharp
public class RFSchedulingDbContext : DbContext
{
    public RFSchedulingDbContext(DbContextOptions options)
        : base(options)
    {
    }

    // DbSets
    public DbSet Users { get; set; }
    public DbSet Roles { get; set; }
    public DbSet PasswordResets { get; set; }
    public DbSet Projects { get; set; }
    public DbSet Regulations { get; set; }
    public DbSet TestItems { get; set; }
    public DbSet TestItemEngineers { get; set; }
    public DbSet TestItemRevisions { get; set; }
    public DbSet WorkLogs { get; set; }
    public DbSet DelayReasons { get; set; }
    public DbSet WorkLogDelayReasons { get; set; }
    public DbSet AuditLogs { get; set; }
    public DbSet SystemSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 套用所有Configuration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RFSchedulingDbContext).Assembly);

        // Global Query Filter (Soft Delete)
        modelBuilder.Entity().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity().HasQueryFilter(e => !e.IsDeleted);
    }

    public override async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 自動設定CreatedDate/ModifiedDate
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is ICreatable creatable)
                {
                    creatable.CreatedDate = DateTime.Now;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is IModifiable modifiable)
                {
                    modifiable.ModifiedDate = DateTime.Now;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

---

## 6. Service層設計

### 6.1 Service架構

```
Services/
├── Auth/
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   └── PasswordService.cs
├── Projects/
│   ├── IProjectService.cs
│   └── ProjectService.cs
├── WorkLogs/
│   ├── IWorkLogService.cs
│   └── WorkLogService.cs
├── Loading/
│   ├── ILoadingService.cs
│   └── LoadingService.cs
├── Email/
│   ├── IEmailService.cs
│   └── EmailService.cs
└── Common/
    ├── IAuditLogService.cs
    └── AuditLogService.cs
```
---

## 7. 部署架構

### 7.1 部署拓撲圖

```
┌────────────────────────────────────────────────┐
│            公司內部網路                         │
│                                                │
│  ┌──────────────┐  ┌──────────────┐            │
│  │ Client PC 1  │  │ Client PC 2  │  ...       │
│  │              │  │              │            │
│  │ WinForms App │  │ WinForms App │            │
│  └──────────────┘  └──────────────┘            │
│         │                  │                   │
│         └──────────┬───────┘                   │
│                    │ HTTPS                     │
│                    ↓                           │
│  ┌─────────────────────────────────────────┐   │
│  │  Application Server                     │   │
│  │  ┌───────────────────────────────────┐  │   │
│  │  │  IIS / Kestrel                    │  │   │
│  │  │  ┌─────────────────────────────┐  │  │   │
│  │  │  │  ASP.NET Core Web API       │  │  │   │
│  │  │  │  (.NET 8)                   │  │  │   │
│  │  │  └─────────────────────────────┘  │  │   │
│  │  └───────────────────────────────────┘  │   │
│  └─────────────────────────────────────────┘   │ 
│                    │                           │
│                    ↓                           │
│  ┌─────────────────────────────────────────┐   │
│  │  Database Server                        │   │
│  │  ┌───────────────────────────────────┐  │   │
│  │  │  SQL Server 2019 Express          │  │   │
│  │  │  Database: RFScheduling           │  │   │
│  │  └───────────────────────────────────┘  │   │
│  └─────────────────────────────────────────┘   │
│                                                │
└────────────────────────────────────────────────┘
                    │ SMTP
                    ↓
          ┌───────────────────┐
          │  Email Server     │
          │  (公司SMTP Server)│
          └───────────────────┘
```

