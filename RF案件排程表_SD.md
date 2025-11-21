# 📙 RF案件排程系統 — 系統設計文件 (SD v3.0)

---

## 📖 文件說明

**版本歷程:**
- v1.0 (2025-11-14): 初版系統設計文件
- v2.0 (2025-11-17): 調整架構設計,新增API規範
- v2.1 (2025-11-19): 補充混合登入機制與JWT安全性
- v3.0 (2025-11-20): 
  - 同步SA v2.3最新需求
  - 新增完整UI介面設計規範
  - 更新狀態計算邏輯(三層推算)
  - 新增IAM權限體系設計
  - 補充Email合併機制
  - 新增Soft Delete與IsActive機制說明

---

## 1. 系統架構設計

### 1.1 整體架構圖

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                   │
│  ┌───────────────────────────────────────────────────┐  │
│  │  WinForms Application (DevExpress)                │  │
│  │  ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐   │  │
│  │  │Login   │  │Engineer│  │Manager │  │Admin   │   │  │
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
│  │  │ │Auth│ │Proj│ │Test│ │Work│ │Load│ │User│    │ │  │
│  │  │ └────┘ └────┘ └────┘ └────┘ └────┘ └────┘    │ │  │
│  │  └──────────────────────────────────────────────┘ │  │
│  │  ┌──────────────────────────────────────────────┐ │  │
│  │  │ Filters & Middleware                         │ │  │
│  │  │ • JWT Authentication Middleware              │ │  │
│  │  │ • Authorization Filter (Permission-Based)    │ │  │
│  │  │ • AuditLog Filter                            │ │  │
│  │  │ • Exception Handler Middleware               │ │  │
│  │  └──────────────────────────────────────────────┘ │  │
│  │  ┌──────────────────────────────────────────────┐ │  │
│  │  │ Services (Business Logic)                    │ │  │
│  │  │ • AuthService (Local + AD)                   │ │  │
│  │  │ • ProjectService (狀態計算)                   │ │  │
│  │  │ • RegulationService (狀態計算)                │ │  │
│  │  │ • TestItemService (狀態計算+逆向操作)          │ │  │
│  │  │ • WorkLogService (RevisionId檢查)             │ │  │
│  │  │ • LoadingService                             │ │  │
│  │  │ • PermissionService (IAM)                    │ │  │
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
│  │  │ • Query Filters (Soft Delete: IsDeleted)     │ │  │
│  │  │ • Global Filters (IsActive for User/etc)     │ │  │
│  │  │ • Change Tracking                            │ │  │
│  │  │ • Transaction Management                     │ │  │
│  │  └──────────────────────────────────────────────┘ │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                      Database Layer                     │
│  ┌───────────────────────────────────────────────────┐  │
│  │          SQL Server 2019 Express                  │  │
│  │  • Tables (15+ tables)                            │  │
│  │  • Indexes                                        │  │
│  │  • Foreign Keys                                   │  │
│  │  • Unique Constraints (Email-based merge)        │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                    External Services                    │
│  ┌───────────────────────────────────────────────────┐  │
│  │  • SMTP Server (Email Notifications)              │  │
│  │  • Active Directory (Windows Authentication)      │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

---

### 1.2 分層職責說明

#### 1.2.1 Presentation Layer (WinForms)

**職責:**
- 使用者互動介面
- 輸入驗證(前端驗證)
- 顯示資料與錯誤訊息
- 呼叫Web API
- JWT Token管理

**技術:**
- WinForms (.NET 8)
- DevExpress WinForms Controls
- HttpClient (API通訊)
- Newtonsoft.Json (JSON序列化)

**主要表單模組:**
- 登入表單(Local + AD)
- 工程師工作台
- 主管管理介面
- 系統管理介面

**不包含:**
- 業務邏輯運算
- 直接存取資料庫
- 複雜的資料處理

---

#### 1.2.2 Application Layer (Web API)

**API 專案:** `RFScheduling.Api`

**主要責任:**
- 暴露 RESTful API 給 WinForms Client
- 實作混合登入(Local 帳號 + AD 帳號):
  - 兩種登入流程最終都會找到/建立同一筆 `User` 資料(以 Email 為唯一識別)
  - 統一由 `IAuthService` 簽發 JWT,WinForms 之後一律用 JWT 呼叫 API
- 實作 Permission-Based 權限檢查(AuthorizeAttribute + Policy / Claim)
- 統一處理例外(Exception Middleware)、回傳標準錯誤格式
- 實作 JWT 簽發與驗證 Middleware(Bearer Authentication)

**關鍵元件:**

1. **Controllers:**
   - `AuthController`: 處理 Local 登入、AD 登入、取得使用者資訊
   - `ProjectController`: 案件查詢、建立、狀態查詢
   - `RegulationController`: 法規層資料與狀態
   - `TestItemController`: 測試項目維護與狀態更新
   - `WorkLogController`: 工時記錄新增、查詢
   - `UserController`: 使用者管理(新增、停用、調整工時)
   - `PermissionController`: IAM權限管理
   - `LoadingController`: Loading分析
   - `ReportController`: 報表產出

2. **Service 介面:**
   - `IAuthService`: 混合登入流程、Email 正規化(轉小寫)、JWT Token 簽發
   - `IUserService`: User CRUD、重設密碼、AD 使用者同步
   - `IProjectService`: Project 建立、狀態計算(由 Regulation.Status 彙總)
   - `IRegulationService`: Regulation 建立、狀態計算(由 TestItem.Status 彙總)
   - `ITestItemService`: TestItem / TestItemRevision 維護、狀態更新(6級優先順序)、狀態逆向操作
   - `IWorkLogService`: WorkLog 新增/修改/刪除,包含 RevisionId 檢查
   - `IPermissionService`: Permission CRUD、PermissionGroup管理、使用者權限指派
   - `ILoadingService`: Loading計算(Assigned/Actual)
   - `IAuditLogService`: 稽核日誌記錄與查詢

3. **DTO / ViewModel:**
   - 登入 Request / Response DTO(含 JWT Token)
   - 案件清單、法規 + 測試項目樹狀結構 DTO
   - 工時回報 DTO(含 RevisionId)
   - Loading報表 DTO
   - 權限管理 DTO

---

#### 1.2.3 Data Access Layer (EF Core)

**職責:**
- ORM對應
- Query優化
- Change Tracking
- Transaction管理
- Soft Delete處理(IsDeleted)
- IsActive處理(User、DelayReason、PermissionGroup)

**技術:**
- Entity Framework Core 8.0
- Code First Approach
- Migration管理

**關鍵機制:**
- Soft Delete (IsDeleted = true)
- IsActive (User停用、DelayReason停用、PermissionGroup停用)
- AuditLog自動記錄
- RowVersion併發控制
- Email唯一性約束(不區分大小寫)

---

#### 1.2.4 Database Layer (SQL Server)

**職責:**
- 資料持久化
- 資料完整性約束
- 索引優化
- 備份與復原

**技術:**
- SQL Server 2019 Express
- Collation: Chinese_Taiwan_Stroke_CI_AS

**關鍵特性:**
- Email唯一索引(用於身份合併)
- Soft Delete機制
- IsActive機制
- 三層狀態計算支援

---

## 2. 資料庫設計

### 2.1 核心資料表設計

#### 2.1.1 User (使用者)

```sql
CREATE TABLE [dbo].[User] (
    [UserId]                INT IDENTITY(1,1) NOT NULL,
    [Account]               NVARCHAR(50)   NOT NULL,  -- 顯示帳號
    [PasswordHash]          NVARCHAR(255)  NULL,      -- Local 才使用
    [DisplayName]           NVARCHAR(100)  NOT NULL,
    [Email]                 NVARCHAR(255)  NOT NULL,  -- 唯一識別(Local/AD)
    [RoleId]                INT            NOT NULL,  -- FK → Role
    
    [WeeklyAvailableHours]  DECIMAL(5,2)   NOT NULL DEFAULT 37.5,
    [IsActive]              BIT            NOT NULL DEFAULT 1,  -- 啟用/停用

    -- AD 支援欄位
    [AuthType]              NVARCHAR(20)   NOT NULL DEFAULT 'Local',  -- Local/AD
    [ADAccount]             NVARCHAR(100)  NULL,
    [ADDomain]              NVARCHAR(100)  NULL,

    -- 登入紀錄欄位
    [LastLoginDate]         DATETIME       NULL,
    [LastLoginIP]           NVARCHAR(50)   NULL,

    -- 審計欄位
    [CreatedByUserId]       INT            NULL,
    [CreatedDate]           DATETIME       NOT NULL DEFAULT GETDATE(),
    [ModifiedByUserId]      INT            NULL,
    [ModifiedDate]          DATETIME       NULL,
    [RowVersion]            ROWVERSION     NOT NULL,

    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId]),
    CONSTRAINT [FK_User_Role] FOREIGN KEY ([RoleId]) REFERENCES [Role]([RoleId]),
    CONSTRAINT [UQ_User_Account] UNIQUE ([Account]),
    CONSTRAINT [UQ_User_Email] UNIQUE ([Email])  -- Email 唯一識別
);

-- Email唯一索引(活躍用戶)
CREATE UNIQUE NONCLUSTERED INDEX [UX_User_Email] 
    ON [User]([Email]) WHERE [IsActive] = 1;

-- Account唯一索引(活躍用戶)
CREATE UNIQUE NONCLUSTERED INDEX [UX_User_Account] 
    ON [User]([Account]) WHERE [IsActive] = 1;
```

**重要說明:**
- User 使用 **IsActive** 機制,不使用 IsDeleted
- Email 必須唯一,用於 Local/AD 登入身份合併
- 停用用戶 IsActive = false,無法登入但保留歷史資料

---

#### 2.1.2 Project (案件)

```sql
CREATE TABLE [dbo].[Project] (
    [ProjectId]         INT             IDENTITY(1,1) NOT NULL,
    [ProjectName]       NVARCHAR(200)   NOT NULL,
    [Customer]          NVARCHAR(200)   NULL,
    [Priority]          NVARCHAR(20)    NOT NULL DEFAULT 'Medium',
    [Status]            NVARCHAR(20)    NOT NULL DEFAULT 'Draft',
    [StartDate]         DATE            NULL,
    [EndDate]           DATE            NULL,
    [Note]              NVARCHAR(1000)  NULL,
    
    -- 審計欄位
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedByUserId]  INT             NULL,
    [ModifiedDate]      DATETIME        NULL,
    
    -- Soft Delete
    [IsDeleted]         BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]   INT             NULL,
    [DeletedDate]       DATETIME        NULL,
    [RowVersion]        ROWVERSION      NOT NULL,
    
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([ProjectId]),
    CONSTRAINT [FK_Project_CreatedBy] FOREIGN KEY ([CreatedByUserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [UQ_Project_Name] UNIQUE ([ProjectName]) WHERE [IsDeleted] = 0,
    CONSTRAINT [CK_Project_Priority] CHECK ([Priority] IN ('High', 'Medium', 'Low')),
    CONSTRAINT [CK_Project_Status] CHECK ([Status] IN ('Draft', 'Active', 'Completed', 'OnHold', 'Delayed'))
);
```

**狀態計算邏輯:**
- 由 Regulation 狀態彙總而來
- 任一 Regulation = Delayed → Project = Delayed
- 所有 Regulation = Completed → Project = Completed
- 任一 Regulation = InProgress → Project = Active
- 所有 Regulation = NotStarted → Project = Draft
- 主管可手動設為 OnHold

---

#### 2.1.3 Regulation (法規)

```sql
CREATE TABLE [dbo].[Regulation] (
    [RegulationId]          INT             IDENTITY(1,1) NOT NULL,
    [ProjectId]             INT             NOT NULL,
    [RegulationName]        NVARCHAR(100)   NOT NULL,
    [StartDate]             DATE            NOT NULL,
    [EndDate]               DATE            NOT NULL,
    [Status]                NVARCHAR(20)    NOT NULL DEFAULT 'NotStarted',
    [ManualStatusOverride]  BIT             NOT NULL DEFAULT 0,  -- 手動狀態標記
    [Note]                  NVARCHAR(500)   NULL,
    
    -- 審計欄位
    [CreatedByUserId]       INT             NOT NULL,
    [CreatedDate]           DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedByUserId]      INT             NULL,
    [ModifiedDate]          DATETIME        NULL,
    
    -- Soft Delete
    [IsDeleted]             BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]       INT             NULL,
    [DeletedDate]           DATETIME        NULL,
    
    CONSTRAINT [PK_Regulation] PRIMARY KEY CLUSTERED ([RegulationId]),
    CONSTRAINT [FK_Regulation_Project] FOREIGN KEY ([ProjectId]) 
        REFERENCES [Project]([ProjectId]),
    CONSTRAINT [CK_Regulation_Status] CHECK ([Status] IN ('NotStarted', 'InProgress', 'Completed', 'Delayed', 'OnHold'))
);
```

**狀態計算邏輯:**
1. IF 主管手動設定 OnHold (ManualStatusOverride = true) → OnHold
2. ELSE IF 任一 TestItem = Delayed → Delayed
3. ELSE IF 所有 TestItem = Completed → Completed
4. ELSE IF 任一 TestItem = InProgress → InProgress
5. ELSE → NotStarted

---

#### 2.1.4 TestItem (測試項目)

```sql
CREATE TABLE [dbo].[TestItem] (
    [TestItemId]            INT             IDENTITY(1,1) NOT NULL,
    [RegulationId]          INT             NOT NULL,
    [TestItemName]          NVARCHAR(200)   NOT NULL,
    [TestType]              NVARCHAR(100)   NOT NULL,
    [TestLocation]          NVARCHAR(100)   NOT NULL,
    [EstimatedHours]        DECIMAL(10,2)   NOT NULL,
    [Status]                NVARCHAR(20)    NOT NULL DEFAULT 'NotStarted',
    [ManualStatusOverride]  BIT             NOT NULL DEFAULT 0,  -- 手動狀態標記
    [ManagerNote]           NVARCHAR(500)   NULL,
    
    -- 審計欄位
    [CreatedByUserId]       INT             NOT NULL,
    [CreatedDate]           DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedByUserId]      INT             NULL,
    [ModifiedDate]          DATETIME        NULL,
    
    -- Soft Delete
    [IsDeleted]             BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]       INT             NULL,
    [DeletedDate]           DATETIME        NULL,
    [RowVersion]            ROWVERSION      NOT NULL,
    
    CONSTRAINT [PK_TestItem] PRIMARY KEY CLUSTERED ([TestItemId]),
    CONSTRAINT [FK_TestItem_Regulation] FOREIGN KEY ([RegulationId]) 
        REFERENCES [Regulation]([RegulationId]),
    CONSTRAINT [CK_TestItem_Status] CHECK ([Status] IN ('NotStarted', 'InProgress', 'Completed', 'Delayed', 'OnHold'))
);
```

**狀態計算邏輯(6級優先順序):**
1. IF 主管手動設定 OnHold (ManualStatusOverride = true) → **OnHold** (最高優先級)
2. ELSE IF 發生「建立 TestItemRevision」事件 → **InProgress** (補測事件)
3. ELSE IF WorkLog 中存在 Delayed 狀態 → **Delayed**
4. ELSE IF 任一工程師按「Complete TestItem」→ **Completed**
5. ELSE IF WorkLog 中存在 InProgress 狀態 → **InProgress**
6. ELSE → **NotStarted** (初始狀態)

**狀態逆向操作:**
- **工程師權限:** 可取消自己誤按的 Completed 狀態
- **主管權限:** 可覆寫任何狀態,需填寫理由

---

#### 2.1.5 WorkLog (工時記錄)

```sql
CREATE TABLE [dbo].[WorkLog] (
    [WorkLogId]             INT             IDENTITY(1,1) NOT NULL,
    [TestItemId]            INT             NOT NULL,
    [RevisionId]            INT             NULL,  -- NULL = v1
    [EngineerUserId]        INT             NOT NULL,
    [WorkDate]              DATE            NOT NULL,
    [ActualHours]           DECIMAL(10,2)   NOT NULL,
    [Status]                NVARCHAR(20)    NOT NULL,
    [Comment]               NVARCHAR(500)   NULL,
    
    -- 審計欄位
    [CreatedByUserId]       INT             NOT NULL,
    [CreatedDate]           DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedByUserId]      INT             NULL,
    [ModifiedDate]          DATETIME        NULL,
    [ModificationReason]    NVARCHAR(500)   NULL,
    
    -- Soft Delete (保留稽核軌跡)
    [IsDeleted]             BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]       INT             NULL,
    [DeletedDate]           DATETIME        NULL,
    
    CONSTRAINT [PK_WorkLog] PRIMARY KEY CLUSTERED ([WorkLogId]),
    CONSTRAINT [FK_WorkLog_TestItem] FOREIGN KEY ([TestItemId]) 
        REFERENCES [TestItem]([TestItemId]),
    CONSTRAINT [FK_WorkLog_Revision] FOREIGN KEY ([RevisionId]) 
        REFERENCES [TestItemRevision]([RevisionId]),
    CONSTRAINT [CK_WorkLog_Status] CHECK ([Status] IN ('InProgress', 'Completed', 'Delayed')),
    CONSTRAINT [CK_WorkLog_ActualHours] CHECK ([ActualHours] > 0 AND [ActualHours] <= 12)
);
```

**重要說明:**
- WorkLog 支援 **Soft Delete**,保留稽核軌跡
- RevisionId = NULL 表示 v1(原始版本)
- 主管修改工時需填寫 ModificationReason

---

#### 2.1.6 TestItemRevision (補測版本)  // 文件沒有正式定義 TestItemRevision 的欄位、用途、UI、流程 

```sql
CREATE TABLE [dbo].[TestItemRevision] (
    [RevisionId]        INT             IDENTITY(1,1) NOT NULL,
    [TestItemId]        INT             NOT NULL,
    [RevisionNumber]    NVARCHAR(10)    NOT NULL,  -- v2, v3, v4
    [EstimatedHours]    DECIMAL(10,2)   NOT NULL,
    [Reason]            NVARCHAR(200)   NOT NULL,
    [Description]       NVARCHAR(500)   NULL,
    
    -- 審計欄位
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedByUserId]  INT             NULL,
    [ModifiedDate]      DATETIME        NULL,
    
    -- Soft Delete
    [IsDeleted]         BIT             NOT NULL DEFAULT 0,
    [DeletedByUserId]   INT             NULL,
    [DeletedDate]       DATETIME        NULL,
    
    CONSTRAINT [PK_TestItemRevision] PRIMARY KEY CLUSTERED ([RevisionId]),
    CONSTRAINT [FK_TestItemRevision_TestItem] FOREIGN KEY ([TestItemId]) 
        REFERENCES [TestItem]([TestItemId]),
    CONSTRAINT [UQ_TestItemRevision] UNIQUE ([TestItemId], [RevisionNumber]) WHERE [IsDeleted] = 0
);
```

---

#### 2.1.7 DelayReason (延遲原因)

```sql
CREATE TABLE [dbo].[DelayReason] (
    [DelayReasonId]     INT             IDENTITY(1,1) NOT NULL,
    [ReasonText]        NVARCHAR(200)   NOT NULL,
    [ReasonType]        NVARCHAR(50)    NOT NULL,
    [IsActive]          BIT             NOT NULL DEFAULT 1,  -- 啟用/停用
    
    -- 審計欄位
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedByUserId]  INT             NULL,
    [ModifiedDate]      DATETIME        NULL,
    
    CONSTRAINT [PK_DelayReason] PRIMARY KEY CLUSTERED ([DelayReasonId]),
    CONSTRAINT [CK_DelayReason_Type] CHECK ([ReasonType] IN ('Equipment', 'Customer', 'Engineer', 'Location', 'Other')),
    CONSTRAINT [UQ_DelayReason_Text] UNIQUE ([ReasonText])
);
```

**重要說明:**
- DelayReason 使用 **IsActive** 機制,不使用 IsDeleted
- 已使用的 DelayReason 不可刪除,僅能停用(IsActive = false)
- 停用後不再顯示於下拉選單,但歷史資料仍可查詢

---

#### 2.1.8 IAM 權限體系資料表

##### Permission (權限)

```sql
CREATE TABLE [dbo].[Permission] (
    [PermissionId]      INT             IDENTITY(1,1) NOT NULL,
    [PermissionCode]    NVARCHAR(100)   NOT NULL,  -- PROJECT_CREATE, WORKLOG_VIEW_ALL
    [PermissionName]    NVARCHAR(100)   NOT NULL,  -- 給 UI 顯示的「中文名稱」 → 使用者看得懂的名稱
    [Category]          NVARCHAR(50)    NOT NULL,  -- Project/TestItem/WorkLog/User/Report
    [Description]       NVARCHAR(200)   NULL,      -- 權限補充說明（用途 / 會影響哪個功能）
    [IsActive]          BIT             NOT NULL DEFAULT 1,
    
    -- 審計欄位
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedByUserId]  INT             NULL,
    [ModifiedDate]      DATETIME        NULL,
    
    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([PermissionId]),
    CONSTRAINT [UQ_Permission_Code] UNIQUE ([PermissionCode])
);
```

##### PermissionGroup (權限群組)

```sql
CREATE TABLE [dbo].[PermissionGroup] (
    [GroupId]           INT             IDENTITY(1,1) NOT NULL,
    [GroupName]         NVARCHAR(50)    NOT NULL,  -- Engineer/Manager/Admin
    [Description]       NVARCHAR(200)   NULL,
    [IsActive]          BIT             NOT NULL DEFAULT 1,  -- 啟用/停用
    
    -- 審計欄位
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    [ModifiedByUserId]  INT             NULL,
    [ModifiedDate]      DATETIME        NULL,
    
    CONSTRAINT [PK_PermissionGroup] PRIMARY KEY CLUSTERED ([GroupId]),
    CONSTRAINT [UQ_PermissionGroup_Name] UNIQUE ([GroupName])
);
```

**重要說明:**
- PermissionGroup 使用 **IsActive** 機制
- 系統預設群組(Engineer/Manager/Admin)不可停用
- 停用後該群組不再可指派給新用戶

##### PermissionGroupMapping (群組權限對應)

```sql
CREATE TABLE [dbo].[PermissionGroupMapping] (
    [MappingId]         INT             IDENTITY(1,1) NOT NULL,
    [GroupId]           INT             NOT NULL,
    [PermissionId]      INT             NOT NULL,
    
    -- 審計欄位
    [CreatedByUserId]   INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [PK_PermissionGroupMapping] PRIMARY KEY CLUSTERED ([MappingId]),
    CONSTRAINT [FK_PGM_Group] FOREIGN KEY ([GroupId]) 
        REFERENCES [PermissionGroup]([GroupId]),
    CONSTRAINT [FK_PGM_Permission] FOREIGN KEY ([PermissionId]) 
        REFERENCES [Permission]([PermissionId]),
    CONSTRAINT [UQ_PGM] UNIQUE ([GroupId], [PermissionId])
);
```

##### UserGroup (使用者群組)

```sql
CREATE TABLE [dbo].[UserGroup] (
    [UserGroupId]       INT             IDENTITY(1,1) NOT NULL,
    [UserId]            INT             NOT NULL,
    [GroupId]           INT             NOT NULL,
    [AssignedDate]      DATETIME        NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED ([UserGroupId]),
    CONSTRAINT [FK_UG_User] FOREIGN KEY ([UserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [FK_UG_Group] FOREIGN KEY ([GroupId]) 
        REFERENCES [PermissionGroup]([GroupId]),
    CONSTRAINT [UQ_UserGroup] UNIQUE ([UserId], [GroupId])
);
```

##### UserPermission (使用者個別權限)

```sql
CREATE TABLE [dbo].[UserPermission] (
    [UserPermissionId]  INT             IDENTITY(1,1) NOT NULL,
    [UserId]            INT             NOT NULL,
    [PermissionId]      INT             NOT NULL,
    [GrantedByUserId]   INT             NOT NULL,
    [GrantedDate]       DATETIME        NOT NULL DEFAULT GETDATE(),
    [ExpireDate]        DATETIME        NULL,  -- NULL表示永久
    [IsActive]          BIT             NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_UserPermission] PRIMARY KEY CLUSTERED ([UserPermissionId]),
    CONSTRAINT [FK_UP_User] FOREIGN KEY ([UserId]) 
        REFERENCES [User]([UserId]),
    CONSTRAINT [FK_UP_Permission] FOREIGN KEY ([PermissionId]) 
        REFERENCES [Permission]([PermissionId]),
    CONSTRAINT [FK_UP_GrantedBy] FOREIGN KEY ([GrantedByUserId]) 
        REFERENCES [User]([UserId])
);
```

---

### 2.2 資料表關聯總覽

```
User (使用者) ←─ IsActive機制
  ├─ 建立/修改 → Project (案件) ←─ Soft Delete
  ├─ 負責 → TestItem (測項) [透過TestItemEngineer] ←─ Soft Delete
  ├─ 回報 → WorkLog (工時記錄) ←─ Soft Delete
  ├─ 屬於 → PermissionGroup [透過UserGroup] ←─ IsActive機制
  └─ 授予 → Permission [透過UserPermission]

Project → Regulation → TestItem → WorkLog
(三層狀態推算: TestItem → Regulation → Project)

TestItem ─ 建立 → TestItemRevision ←─ Soft Delete
WorkLog ─ 選擇 → DelayReason [透過WorkLogDelayReason] ←─ IsActive機制

PermissionGroup ←─ IsActive機制
  ├─ 包含 → Permission [透過PermissionGroupMapping]
  └─ 指派給 → User [透過UserGroup]
```

---

## 3. 混合登入與身份合併機制

### 3.1 Email-Based Identity Merge 原則

**核心概念:**
- Email 作為跨登入來源(Local/AD)的唯一身份識別鍵
- 同一 Email 無論透過 Local 或 AD 登入,都視為同一使用者
- Email 欄位必須唯一,且不區分大小寫

**實作方式:**
```csharp
// Email正規化函數
public string NormalizeEmail(string email)
{
    return email?.Trim().ToLower();
}

// 身份合併邏輯
public async Task<User> GetOrCreateUser(string email, string authType)
{
    var normalizedEmail = NormalizeEmail(email);
    
    // 查詢現有用戶
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
    
    if (user != null)
    {
        // 更新登入類型與時間
        user.AuthType = authType;
        user.LastLoginDate = DateTime.Now;
        return user;
    }
    
    // 建立新用戶
    return await CreateNewUser(normalizedEmail, authType);
}
```

---

### 3.2 Local 登入流程

```
┌─────────────────────────────────────────────────┐
│         Local 登入流程                          │
└─────────────────────────────────────────────────┘

1. 用戶輸入 Email + Password
   ↓
2. Client 呼叫: POST /api/auth/login-local
   {
     "email": "user@example.com",
     "password": "P@ssw0rd!"
   }
   ↓
3. Server端處理:
   - Email 正規化: user@example.com → user@example.com
   - 查詢: SELECT * FROM Users WHERE Email = 'user@example.com'
   - 驗證 PasswordHash
   - 檢查 IsActive = 1
   - 檢查 AuthType 包含 'Local'
   ↓
4. 驗證成功:
   - 更新 LastLoginDate, LastLoginIP
   - 生成 JWT Token (含 UserId, Email, Role, Permissions)
   ↓
5. 回傳給 Client:
   {
     "token": "eyJhbGc...",
     "userId": 123,
     "displayName": "張三",
     "role": "Engineer",
     "permissions": ["WORKLOG_VIEW_OWN", ...]
   }
```

---

### 3.3 AD 登入流程

```
┌─────────────────────────────────────────────────┐
│         AD 登入流程                             │
└─────────────────────────────────────────────────┘

1. 用戶點擊「Windows 驗證登入」
   ↓
2. Client 呼叫: POST /api/auth/login-ad
   (可能需要傳入當前Windows用戶資訊)
   ↓
3. Server端處理:
   - 透過 AD/LDAP 驗證 Windows 帳密
   - 從 AD 取得: Email, DisplayName, sAMAccountName, Domain
   - Email 正規化: User@Company.com → user@company.com
   ↓
4. Email 合併邏輯:
   - 查詢: SELECT * FROM Users WHERE Email = 'user@company.com'
   
   IF 存在:
     - 視為同一使用者
     - 更新: AuthType = 'AD', ADAccount, ADDomain
     - 更新: LastLoginDate, LastLoginIP
   
   ELSE:
     - 建立新使用者:
       * Email = 'user@company.com'
       * DisplayName 來自 AD
       * AuthType = 'AD'
       * ADAccount, ADDomain 填入
       * Role 採預設角色(Engineer)
   ↓
5. 生成 JWT Token (同 Local 登入)
   ↓
6. 回傳給 Client (同 Local 登入)
```

**重要規則:**
- AD 登入時若 Email = NULL → 拒絕登入
- Email 已存在 → 不新增,僅更新 AD 相關欄位
- Email 不存在 → 建立新用戶並預設為 Engineer 角色

---

### 3.4 JWT Token 設計

**Token 結構:**
```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "123",                    // UserId
    "email": "user@example.com",     // Email (小寫)
    "name": "張三",                   // DisplayName
    "role": "Engineer",              // Role
    "permissions": [                 // 有效權限清單
      "WORKLOG_VIEW_OWN",
      "WORKLOG_CREATE",
      "TESTITEM_VIEW_ASSIGNED"
    ],
    "iss": "RFSchedulingAPI",        // Issuer
    "aud": "RFSchedulingClient",     // Audience
    "iat": 1700000000,               // Issued At
    "exp": 1700028800                // Expires (8小時後)
  },
  "signature": "..."
}
```

**JWT 驗證流程:**
```
每次 API 呼叫:
1. Client 在 Header 加上: Authorization: Bearer {token}
2. Server Middleware 驗證:
   - 簽章是否正確 (HMAC-SHA256)
   - Token 是否過期 (exp)
   - Issuer/Audience 是否正確
3. 驗證通過 → 將 Claims 映射到 HttpContext.User
4. Controller/Action 可取得:
   - UserId: User.FindFirst("sub").Value
   - Email: User.FindFirst("email").Value
   - Role: User.IsInRole("Engineer")
   - Permissions: 透過 Policy 檢查
```

---

## 4. 狀態計算與逆向操作設計

### 4.1 三層狀態推算架構

```
┌─────────────────────────────────────────────────┐
│           三層狀態推算架構                       │
└─────────────────────────────────────────────────┘

WorkLog.Status 變更
  ↓ 觸發
TestItem 狀態重算 (6級優先順序)
  ↓ 觸發
Regulation 狀態重算
  ↓ 觸發
Project 狀態重算

每層狀態計算獨立執行,遵循各自的優先順序規則
手動狀態(ManualStatusOverride = true)阻斷自動推算
所有狀態變更記錄 AuditLog(包含觸發來源)
```

---

### 4.2 TestItem 狀態計算邏輯 (6級優先順序)

**優先順序規則:**

```csharp
public class TestItemStatusCalculator
{
    public TestItemStatus CalculateStatus(TestItem testItem, 
        List<WorkLog> workLogs, 
        List<TestItemRevision> revisions)
    {
        // 1. 主管手動設定 OnHold (最高優先級)
        if (testItem.ManualStatusOverride && testItem.Status == TestItemStatus.OnHold)
        {
            return TestItemStatus.OnHold;
        }
        
        // 2. 發生「建立 TestItemRevision」事件
        if (revisions.Any(r => r.CreatedDate > testItem.ModifiedDate))
        {
            return TestItemStatus.InProgress;
        }
        
        // 3. WorkLog 中存在 Delayed 狀態
        if (workLogs.Any(w => w.Status == WorkLogStatus.Delayed && !w.IsDeleted))
        {
            return TestItemStatus.Delayed;
        }
        
        // 4. 任一工程師按「Complete TestItem」
        if (testItem.CompletedByUserId != null)
        {
            return TestItemStatus.Completed;
        }
        
        // 5. WorkLog 中存在 InProgress 狀態
        if (workLogs.Any(w => w.Status == WorkLogStatus.InProgress && !w.IsDeleted))
        {
            return TestItemStatus.InProgress;
        }
        
        // 6. 初始狀態
        return TestItemStatus.NotStarted;
    }
}
```

**狀態變更觸發時機:**
- WorkLog 新增/修改/刪除時
- TestItemRevision 建立時
- 工程師按「Complete TestItem」時
- 主管手動覆寫狀態時

---

### 4.3 TestItem 狀態逆向操作

**工程師權限:**
```csharp
// 取消自己的完成狀態
public async Task<Result> CancelCompletion(int testItemId, int engineerUserId, string reason)
{
    var testItem = await _context.TestItems.FindAsync(testItemId);
    
    // 權限檢查: 只能取消自己的完成
    if (testItem.CompletedByUserId != engineerUserId)
    {
        return Result.Fail("只能取消自己的完成狀態");
    }
    
    // 執行取消
    testItem.CompletedByUserId = null;
    testItem.Status = TestItemStatus.InProgress;
    
    // 記錄 AuditLog
    await _auditLogService.LogAsync(new AuditLog
    {
        TableName = "TestItem",
        RecordId = testItemId,
        Action = "CancelCompletion",
        OldValue = JsonConvert.SerializeObject(new { Status = "Completed" }),
        NewValue = JsonConvert.SerializeObject(new { Status = "InProgress" }),
        UserId = engineerUserId,
        Reason = reason
    });
    
    await _context.SaveChangesAsync();
    
    // 觸發 Regulation 狀態重算
    await RecalculateRegulationStatus(testItem.RegulationId);
    
    return Result.Success();
}
```

**主管權限:**
```csharp
// 覆寫任何狀態
public async Task<Result> OverrideStatus(int testItemId, 
    TestItemStatus newStatus, 
    int managerId, 
    string reason)
{
    var testItem = await _context.TestItems.FindAsync(testItemId);
    var oldStatus = testItem.Status;
    
    // 主管權限檢查
    if (!await _permissionService.HasPermission(managerId, "TESTITEM_STATUS_OVERRIDE"))
    {
        return Result.Fail("無權限覆寫狀態");
    }
    
    // 執行覆寫
    testItem.Status = newStatus;
    testItem.ManualStatusOverride = true;
    testItem.ModifiedByUserId = managerId;
    testItem.ModifiedDate = DateTime.Now;
    
    // 記錄 AuditLog (必須包含理由)
    await _auditLogService.LogAsync(new AuditLog
    {
        TableName = "TestItem",
        RecordId = testItemId,
        Action = "StatusOverride",
        OldValue = JsonConvert.SerializeObject(new { Status = oldStatus }),
        NewValue = JsonConvert.SerializeObject(new { Status = newStatus, ManualOverride = true }),
        UserId = managerId,
        Reason = reason  // 必填
    });
    
    await _context.SaveChangesAsync();
    
    // 觸發 Regulation 狀態重算
    await RecalculateRegulationStatus(testItem.RegulationId);
    
    return Result.Success();
}
```

---

### 4.4 Regulation 狀態計算邏輯

```csharp
public class RegulationStatusCalculator
{
    public RegulationStatus CalculateStatus(Regulation regulation, 
        List<TestItem> testItems)
    {
        // 1. 主管手動設定 OnHold (最高優先級)
        if (regulation.ManualStatusOverride && regulation.Status == RegulationStatus.OnHold)
        {
            return RegulationStatus.OnHold;
        }
        
        // 2. 任一 TestItem = Delayed
        if (testItems.Any(t => t.Status == TestItemStatus.Delayed && !t.IsDeleted))
        {
            return RegulationStatus.Delayed;
        }
        
        // 3. 所有 TestItem = Completed
        if (testItems.All(t => t.Status == TestItemStatus.Completed || t.IsDeleted))
        {
            return RegulationStatus.Completed;
        }
        
        // 4. 任一 TestItem = InProgress
        if (testItems.Any(t => t.Status == TestItemStatus.InProgress && !t.IsDeleted))
        {
            return RegulationStatus.InProgress;
        }
        
        // 5. 所有 TestItem = NotStarted
        return RegulationStatus.NotStarted;
    }
}
```

---

### 4.5 Project 狀態計算邏輯

```csharp
public class ProjectStatusCalculator
{
    public ProjectStatus CalculateStatus(Project project, 
        List<Regulation> regulations)
    {
        // 1. 任一 Regulation = Delayed
        if (regulations.Any(r => r.Status == RegulationStatus.Delayed && !r.IsDeleted))
        {
            return ProjectStatus.Delayed;
        }
        
        // 2. 所有 Regulation = Completed
        if (regulations.All(r => r.Status == RegulationStatus.Completed || r.IsDeleted))
        {
            return ProjectStatus.Completed;
        }
        
        // 3. 任一 Regulation = InProgress
        if (regulations.Any(r => r.Status == RegulationStatus.InProgress && !r.IsDeleted))
        {
            return ProjectStatus.Active;
        }
        
        // 4. 所有 Regulation = NotStarted
        if (regulations.All(r => r.Status == RegulationStatus.NotStarted || r.IsDeleted))
        {
            return ProjectStatus.Draft;
        }
        
        // 5. 主管可手動設為 OnHold
        return project.Status;  // 保持現有狀態
    }
}
```

---

## 5. IAM 權限體系設計

### 5.1 Permission-Based 架構

**核心概念:**
- 以 **Permission** 為最小授權單位
- **PermissionGroup** 為 Permission 的集合
- User 透過 **UserGroup** 繼承群組權限
- User 可透過 **UserPermission** 獲得個別權限

**權限計算公式:**
```
使用者有效權限 = UserGroup繼承權限 ∪ UserPermission個別權限

檢查邏輯:
1. 查詢 User 所屬的所有 PermissionGroup (WHERE IsActive = 1)
2. 查詢這些 Group 的所有 Permission (透過 PermissionGroupMapping)
3. 查詢 User 的個別 Permission (WHERE IsActive = 1 AND (ExpireDate IS NULL OR ExpireDate > NOW()))
4. 合併去重,得到最終權限清單
```

---

### 5.2 預設權限群組定義

#### Engineer 群組預期權限

| PermissionCode | 說明 |
|---------------|------|
| PROJECT_VIEW_ASSIGNED | 查看自己相關的案件 |
| REGULATION_VIEW_ASSIGNED | 查看自己相關的法規 |
| TESTITEM_VIEW_ASSIGNED | 查看自己負責的測項 |
| WORKLOG_VIEW_OWN | 查看自己的工時記錄 |
| WORKLOG_CREATE | 回報工時 |
| WORKLOG_EDIT_7DAYS | 修改7天內的工時 |
| TESTITEM_COMPLETE | 標記測項完成 |
| TESTITEM_CANCEL_COMPLETION | 取消自己的完成狀態 |
| LOADING_VIEW_OWN | 查看自己的Loading |
| REPORT_VIEW_OWN | 查看自己的報表 |

#### Manager 群組預期權限

| PermissionCode | 說明 |
|---------------|------|
| PROJECT_* | 所有案件管理權限 |
| REGULATION_* | 所有法規管理權限 |
| TESTITEM_* | 所有測項管理權限 |
| TESTITEM_STATUS_OVERRIDE | 覆寫測項狀態 |
| WORKLOG_VIEW_ALL | 查看所有工時記錄 |
| WORKLOG_EDIT_ALL | 修改任何工時(需理由) |
| WORKLOG_DELETE | 刪除工時(需理由) |
| LOADING_VIEW_ALL | 查看所有工程師Loading |
| REPORT_VIEW_ALL | 查看所有報表 |
| USER_VIEW | 查看使用者資料 |
| USER_DISABLE | 停用使用者 |
| AUDITLOG_VIEW | 查看稽核日誌 |
| DELAYREASON_MANAGE | 管理延遲原因 |

#### Admin 群組預期權限

| PermissionCode | 說明 |
|---------------|------|
| * | 所有權限 |
| USER_MANAGE | 使用者管理 |
| PERMISSION_MANAGE | 權限管理 |
| PERMISSION_GROUP_MANAGE | 權限群組管理 |
| SYSTEM_SETTING_MANAGE | 系統設定管理 |

---

### 5.3 權限檢查實作

**Controller 層級檢查:**
```csharp
[Authorize]  // 需要登入
[RequirePermission("PROJECT_CREATE")]  // 自訂 Attribute
public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
{
    // 權限已由 Attribute 檢查
    var result = await _projectService.CreateAsync(dto);
    return Ok(result);
}
```

**Service 層級檢查:**
```csharp
public async Task<Result> DeleteWorkLog(int workLogId, int userId, string reason)
{
    // 檢查權限
    var hasPermission = await _permissionService.HasPermission(userId, "WORKLOG_DELETE");
    if (!hasPermission)
    {
        return Result.Fail("無權限刪除工時記錄");
    }
    
    // 檢查理由
    if (string.IsNullOrWhiteSpace(reason))
    {
        return Result.Fail("刪除工時必須填寫理由");
    }
    
    // 執行刪除 (Soft Delete)
    var workLog = await _context.WorkLogs.FindAsync(workLogId);
    workLog.IsDeleted = true;
    workLog.DeletedByUserId = userId;
    workLog.DeletedDate = DateTime.Now;
    
    // 記錄 AuditLog
    await _auditLogService.LogAsync(new AuditLog
    {
        TableName = "WorkLog",
        RecordId = workLogId,
        Action = "Delete",
        OldValue = JsonConvert.SerializeObject(workLog),
        UserId = userId,
        Reason = reason
    });
    
    await _context.SaveChangesAsync();
    return Result.Success();
}
```

---

## 6. UI 介面設計規範

### 6.1 登入介面設計

#### 6.1.1 登入表單 (Login Form)

**佈局:**
```
┌──────────────────────────────────────────┐
│                                          │
│         RF案件排程系統                    │
│         RF Scheduling System             │
│                                          │
│  ┌────────────────────────────────────┐  │
│  │  Email:                            │  │
│  │  [_____________________________]   │  │
│  │                                    │  │
│  │  Password:                         │  │
│  │  [_____________________________]   │  │
│  │                                    │  │
│  │  [ ] Remember Me                   │  │
│  │                                    │  │
│  │  [    Login    ] [Windows Login]   │  │
│  │                                    │  │
│  │          Forgot Password?          │  │
│  └────────────────────────────────────┘  │
│                                          │
│              Version 1.0.0               │
└──────────────────────────────────────────┘
```

**控制項規格:**
- **Email TextBox**: 
  - 必填,格式驗證
  - Placeholder: "請輸入Email"
- **Password TextBox**:
  - 必填,PasswordChar = '*'
  - Placeholder: "請輸入密碼"
- **Remember Me CheckBox**:
  - 記住登入資訊(僅記 Email)
- **Login Button**:
  - Primary Button
  - 呼叫 API: POST /api/auth/login-local
- **Windows Login Button**:
  - Secondary Button
  - 呼叫 API: POST /api/auth/login-ad
- **Forgot Password Link**:
  - 開啟密碼重設對話框

**驗證規則:**
- Email 格式驗證
- 密碼長度 8-20 字元
- 連續失敗 5 次鎖定 10 分鐘

---

### 6.2 工程師介面設計

#### 6.2.1 工程師主畫面 (Engineer Dashboard)

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│ RF排程系統 | 歡迎, 張三 (Engineer) | Loading: 75% | 登出      │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  我的工作  │  工時記錄  │  Loading分析  │  報表               │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  === 我的測項清單 ===                                        │
│                                                             │
│  篩選: [專案▼] [法規▼] [狀態▼] [測試類型▼] [搜尋___]           │
│                                                             │
│  ┌────────────────────────────────────────────────────┐     │
│  │ 專案: Project A | 法規: FCC Part 24                │      │
│  │ 測項: Conducted Emission                           │      │
│  │ 狀態: InProgress | 預估: 40h | 實際: 30h | 剩餘: 10h│      │
│  │ 主要負責: 張三 | 支援: 李四                         │      │
│  │ [回報工時] [查看詳情] [標記完成]                     │     │
│  └────────────────────────────────────────────────────┘     │
│                                                             │
│  ┌────────────────────────────────────────────────────┐     │
│  │ 專案: Project B | 法規: NCC PLMN                   │      │
│  │ 測項: Radiated Spurious                            │      │
│  │ 狀態: NotStarted | 預估: 60h | 實際: 0h | 剩餘: 60h │      │
│  │ 主要負責: 張三                                      │     │
│  │ [回報工時] [查看詳情]                               │     │
│  └────────────────────────────────────────────────────┘     │
│                                                             │
│  [第 1 頁 / 共 3 頁]  [上一頁] [下一頁]                       │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**功能說明:**
1. **頁面標題列**:
   - 顯示使用者名稱、角色
   - 即時 Loading 百分比(顏色編碼: ≤60%綠色, 61-80%黃色, 81-100%橘色, >100%紅色)
   - 登出按鈕

2. **Tab 導航**:
   - 我的工作
   - 工時記錄
   - Loading分析
   - 報表

3. **測項卡片**:
   - 顯示專案、法規、測項名稱
   - 顯示狀態(色塊標示)
   - 顯示預估/實際/剩餘工時
   - 顯示負責工程師
   - 操作按鈕:
     - 回報工時
     - 查看詳情
     - 標記完成(僅 InProgress 狀態)

---

#### 6.2.2 工時回報對話框 (WorkLog Dialog)

**佈局:**
```
┌──────────────────────────────────────────┐
│  回報工時                            [X]  │
├──────────────────────────────────────────┤
│                                          │
│  測項: Conducted Emission                │
│  版本: ○ v1 (原始)  ○ v2 (補測)          │
│                                          │
│  工作日期: [2025-11-20▼]                 │
│                                          │
│  實際工時: [____] 小時 (0.5 - 12)        │
│                                          │
│  狀態: ○ 進行中  ○ 完成  ○ 延遲          │
│                                          │
│  ┌─ 延遲原因 (狀態=延遲時必填) ────┐     │
│  │ □ 測試設備故障                  │     │
│  │ □ 客戶延遲提供樣品              │     │
│  │ □ 工程師人力不足                │     │
│  │ □ 測試場地被佔用                │     │
│  │ □ 其他原因                      │     │
│  └─────────────────────────────────┘     │
│                                          │
│  備註:                                   │
│  [________________________________]      │
│  [________________________________]      │
│  [________________________________]      │
│                                          │
│  [取消]              [確定送出]           │
│                                          │
└──────────────────────────────────────────┘
```

**驗證規則:**
- 工作日期不可晚於今天
- 實際工時 0.5 - 12 小時
- 狀態=延遲時必須至少選擇一個延遲原因
- 同一測項同一日期不可重複回報

**API 呼叫:**
```json
POST /api/worklogs
{
  "testItemId": 123,
  "revisionId": null,  // v1
  "workDate": "2025-11-20",
  "actualHours": 8.0,
  "status": "InProgress",
  "delayReasonIds": [],
  "comment": "今日完成初步測試"
}
```

---

#### 6.2.3 工時記錄查詢介面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 我的工時記錄 ===                                        │
│                                                             │
│  篩選條件:                                                   │
│  日期範圍: [2025-11-01] ~ [2025-11-30]                       │
│  專案: [全部▼]  測項: [全部▼]  版本: [全部▼]                  │
│  [查詢] [匯出Excel]                                          │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐    │
│  │日期      │測項              │版本│工時│狀態  │操作    │    │
│  ├─────────────────────────────────────────────────────┤    │
│  │11/20    │Conducted Emission│v1  │8.0 │進行中│[編輯]  │    │
│  │11/19    │Conducted Emission│v1  │7.5 │進行中│[編輯]  │    │
│  │11/18    │Radiated Spurious │v1  │6.0 │延遲  │       │     │
│  │11/15    │Conducted Emission│v1  │8.0 │進行中│       │     │
│  │         │                  │    │    │      │       │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                             │
│  統計資料:                                                   │
│  本月總工時: 156.5 小時                                      │
│  本週總工時: 37.5 小時                                       │
│  今日總工時: 8.0 小時                                        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**功能說明:**
- 僅能查看自己的工時記錄
- 7天內的工時可編輯
- 超過7天顯示"無法編輯"提示
- 支援Excel匯出

---

#### 6.2.4 Loading分析介面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 我的Loading分析 ===                                     │
│                                                             │
│  ┌───────────────────────────────────────────────────┐      │
│  │  本週可用工時: 37.5h                               │      │
│  │  已分配工時: 28.0h (74.7%)                         │      │
│  │  實際工時: 26.5h (70.7%)                           │      │
│  │                                                   │      │
│  │  ████████████████░░░░ 74.7% (已分配)              │      │
│  │  ██████████████░░░░░░ 70.7% (實際)                │      │
│  └───────────────────────────────────────────────────┘      │
│                                                             │
│  === 測項工時明細 ===                                        │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐    │
│  │測項              │預估 │分配 │實際 │剩餘 │完成度│狀態│      │
│  ├─────────────────────────────────────────────────────┤    │
│  │Conducted Emission│40.0 │30.0 │25.5 │4.5  │85%  │進行│    │
│  │Radiated Spurious │60.0 │50.0 │35.0 │15.0 │70%  │進行│    │
│  │Blocking Test     │30.0 │20.0 │0.0  │20.0 │0%   │未開│    │
│  │                  │     │     │     │     │     │    │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                             │
│  [匯出Loading報表]                                           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Loading計算說明:**
- **已分配工時:** 所有 Active 專案中分配給該工程師的工時總和
- **實際工時:** 該工程師實際回報的工時總和
- **完成度:** 實際工時 / 分配工時 × 100%
- **Loading百分比:** 已分配工時 / 本週可用工時 × 100%

---

### 6.3 主管介面設計

#### 6.3.1 主管主畫面 (Manager Dashboard)

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│ RF排程系統 | 歡迎, 王主管 (Manager) | 登出                   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  案件管理 │ 工時審核 │ Loading監控 │ 延遲分析 │ 報表 │ 用戶 │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  === 案件總覽 ===                      [+ 新增專案]          │
│                                                             │
│  篩選: [狀態▼] [優先級▼] [客戶▼] [搜尋___] [Wizard建案]    │
│                                                             │
│  ┌────────────────────────────────────────────────────┐    │
│  │ Project A | 客戶: ABC公司 | 優先級: High           │    │
│  │ 狀態: Active | 進度: 65% | 預計完成: 2025-12-31    │    │
│  │                                                    │    │
│  │ 法規進度:                                          │    │
│  │ • FCC Part 24: 80% (InProgress) ████████░░        │    │
│  │ • NCC PLMN: 50% (InProgress)    █████░░░░░        │    │
│  │ • CE RED: 0% (NotStarted)       ░░░░░░░░░░        │    │
│  │                                                    │    │
│  │ [查看詳情] [編輯] [狀態管理]                        │    │
│  └────────────────────────────────────────────────────┘    │
│                                                             │
│  ┌────────────────────────────────────────────────────┐    │
│  │ Project B | 客戶: XYZ公司 | 優先級: Medium         │    │
│  │ 狀態: Delayed ⚠ | 進度: 40% | 預計完成: 2025-11-30  │    │
│  │                                                    │    │
│  │ 延遲原因: 測試設備故障、工程師人力不足              │    │
│  │                                                    │    │
│  │ [查看詳情] [編輯] [狀態管理]                        │    │
│  └────────────────────────────────────────────────────┘    │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

#### 6.3.2 Wizard建案流程

**Step 1: 基本資訊**
```
┌──────────────────────────────────────────┐
│  新增專案 - Step 1/4: 基本資訊      [X]  │
├──────────────────────────────────────────┤
│                                          │
│  專案名稱*: [____________________]       │
│                                          │
│  客戶名稱: [____________________]        │
│                                          │
│  優先級: ○ High  ● Medium  ○ Low        │
│                                          │
│  預計開始日期*: [2025-11-20▼]           │
│  預計結束日期*: [2025-12-31▼]           │
│                                          │
│  備註:                                   │
│  [________________________________]      │
│  [________________________________]      │
│                                          │
│  [取消]          [上一步]    [下一步>]   │
│                                          │
└──────────────────────────────────────────┘
```

**Step 2: 法規選擇**
```
┌──────────────────────────────────────────┐
│  新增專案 - Step 2/4: 法規選擇      [X]  │
├──────────────────────────────────────────┤
│                                          │
│  請選擇需要測試的法規:                   │
│                                          │
│  ☑ FCC Part 24                          │
│     開始: [2025-11-20] 結束: [2025-12-15]│
│                                          │
│  ☑ NCC PLMN                             │
│     開始: [2025-12-01] 結束: [2025-12-20]│
│                                          │
│  □ CE RED                               │
│     開始: [__________] 結束: [__________]│
│                                          │
│  □ IC RSS                               │
│     開始: [__________] 結束: [__________]│
│                                          │
│  □ TELEC                                │
│     開始: [__________] 結束: [__________]│
│                                          │
│  [取消]        [<上一步]    [下一步>]    │
│                                          │
└──────────────────────────────────────────┘
```

**Step 3: 測試項目定義**
```
┌──────────────────────────────────────────┐
│  新增專案 - Step 3/4: 測試項目      [X]    │
├──────────────────────────────────────────┤
│                                          │
│  法規: FCC Part 24                       │
│  ┌──────────────────────────────────┐   │
│  │測試項目      │類型      │預估工時│     │
│  ├──────────────────────────────────┤   │
│  │Conducted     │Conducted │[40.0]  │   │
│  │Radiated      │Radiated  │[60.0]  │   │
│  │Blocking      │Blocking  │[30.0]  │   │
│  │[+ 新增測項]                       │   │
│  └──────────────────────────────────┘   │
│                                          │
│  法規: NCC PLMN                          │
│  ┌──────────────────────────────────┐   │
│  │測試項目      │類型      │預估工時│   │
│  ├──────────────────────────────────┤   │
│  │Conducted     │Conducted │[40.0]  │   │
│  │Radiated      │Radiated  │[60.0]  │   │
│  │[+ 新增測項]                       │   │
│  └──────────────────────────────────┘   │
│                                          │
│  [取消]        [<上一步]    [下一步>]    │
│                                          │
└──────────────────────────────────────────┘
```

**Step 4: 工程師分配**
```
┌──────────────────────────────────────────┐
│  新增專案 - Step 4/4: 工程師分配    [X]  │
├──────────────────────────────────────────┤
│                                          │
│  測項: Conducted Emission (FCC)          │
│  預估工時: 40.0h                         │
│                                          │
│  主要負責 (Main):                        │
│  [張三▼]  分配工時: [30.0] h             │
│  目前Loading: 65% (可接受)               │
│                                          │
│  支援 (Sub):                             │
│  [李四▼]  分配工時: [10.0] h             │
│  目前Loading: 70% (可接受)               │
│                                          │
│  [+ 新增支援工程師]                      │
│                                          │
│  ⚠ 工時分配總和與預估工時差異: 0h (0%)   │
│                                          │
│  ─────────────────────────────────       │
│                                          │
│  [顯示其他測項分配...]                   │
│                                          │
│  [取消]        [<上一步]    [完成建立]   │
│                                          │
└──────────────────────────────────────────┘
```

---

#### 6.3.3 測項狀態管理介面

**佈局:**
```
┌──────────────────────────────────────────┐
│  測項狀態管理                        [X]  │
├──────────────────────────────────────────┤
│                                          │
│  測項: Conducted Emission                │
│  目前狀態: InProgress                    │
│                                          │
│  === 狀態歷程 ===                        │
│  ┌──────────────────────────────────┐   │
│  │ 2025-11-20 10:30 | 張三回報工時  │   │
│  │ 狀態: InProgress → InProgress    │   │
│  │                                  │   │
│  │ 2025-11-19 15:00 | 首次回報工時  │   │
│  │ 狀態: NotStarted → InProgress    │   │
│  └──────────────────────────────────┘   │
│                                          │
│  === 手動覆寫狀態 ===                    │
│                                          │
│  新狀態*:                                │
│  ○ NotStarted ○ InProgress              │
│  ● OnHold     ○ Delayed   ○ Completed   │
│                                          │
│  覆寫理由*: (必填)                       │
│  [________________________________]      │
│  [________________________________]      │
│  [________________________________]      │
│                                          │
│  ⚠ 此操作將設定 ManualStatusOverride     │
│  ⚠ 自動狀態計算將被停用                  │
│  ⚠ 所有變更將記錄於稽核日誌              │
│                                          │
│  [取消]              [確定覆寫]           │
│                                          │
└──────────────────────────────────────────┘
```

---

#### 6.3.4 工時審核與覆寫介面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 工時審核 ===                                            │
│                                                             │
│  篩選: [工程師▼] [專案▼] [日期範圍] [異常工時only □]        │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │日期  │工程師│測項              │工時│狀態  │操作    │  │
│  ├─────────────────────────────────────────────────────┤  │
│  │11/20│張三  │Conducted Emission│12.0│進行中│[編輯]⚠│  │
│  │11/19│李四  │Radiated Spurious │7.5 │進行中│[編輯] │  │
│  │11/18│張三  │Blocking Test     │0.5 │延遲  │[編輯]⚠│  │
│  │     │      │                  │    │      │       │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
│  ⚠ = 異常工時 (>10h 或 <1h)                                 │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**工時編輯對話框 (主管專用):**
```
┌──────────────────────────────────────────┐
│  編輯工時 (主管權限)                [X]  │
├──────────────────────────────────────────┤
│                                          │
│  工程師: 張三                            │
│  測項: Conducted Emission                │
│  日期: 2025-11-20                        │
│                                          │
│  原始工時: 12.0 小時                     │
│  修改後工時*: [____] 小時 (0.5 - 12)    │
│                                          │
│  原始狀態: InProgress                    │
│  修改後狀態: ○ 進行中 ○ 完成 ○ 延遲     │
│                                          │
│  修改理由*: (必填)                       │
│  [________________________________]      │
│  [________________________________]      │
│  [________________________________]      │
│                                          │
│  ⚠ 此操作將寫入稽核日誌                  │
│  ⚠ 工程師將收到通知                      │
│                                          │
│  [取消]              [確定修改]           │
│                                          │
└──────────────────────────────────────────┘
```

---

#### 6.3.5 Loading監控介面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│  === Loading監控 ===                        [匯出報表]       │
│                                                             │
│  時間範圍: [本週▼]  篩選: [全部工程師▼]                     │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │工程師│可用工時│分配│實際│Loading│狀態      │詳情    │  │
│  ├─────────────────────────────────────────────────────┤  │
│  │張三  │37.5   │35.0│32.5│93.3%  │[████████]│[查看] │  │
│  │李四  │37.5   │28.0│26.5│74.7%  │[██████░░]│[查看] │  │
│  │王五  │37.5   │42.0│38.0│112.0% │[████████]│[查看]⚠│  │
│  │趙六  │37.5   │20.0│18.0│53.3%  │[████░░░░]│[查看] │  │
│  │      │       │    │    │       │          │       │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
│  ⚠ = 超載預警 (>100%)                                       │
│                                                             │
│  === Loading趨勢圖 ===                                      │
│  ┌─────────────────────────────────────────────────────┐  │
│  │ 120%│                                       ╱╲       │  │
│  │     │                               ╱╲    ╱  ╲      │  │
│  │ 100%│───────────────────────────────────────────── │  │
│  │     │            ╱╲    ╱╲    ╱╲  ╱    ╲╱          │  │
│  │  80%│    ╱╲    ╱  ╲  ╱  ╲  ╱  ╲╱                  │  │
│  │     │  ╱    ╲╱      ╲╱    ╲╱                      │  │
│  │  60%│╱                                             │  │
│  │     └───────────────────────────────────────────  │  │
│  │     W1   W2   W3   W4   W5   W6   W7   W8        │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Loading詳情對話框:**
```
┌──────────────────────────────────────────┐
│  工程師Loading詳情 - 張三            [X]  │
├──────────────────────────────────────────┤
│                                          │
│  本週可用工時: 37.5h                     │
│  已分配工時: 35.0h (93.3%)               │
│  實際工時: 32.5h (86.7%)                 │
│                                          │
│  ████████████████████░ 93.3% (已分配)   │
│  ████████████████░░░░░ 86.7% (實際)     │
│                                          │
│  === 工作明細 ===                        │
│  ┌──────────────────────────────────┐   │
│  │專案    │測項        │分配│實際│%  │   │
│  ├──────────────────────────────────┤   │
│  │Proj A │Conducted   │20.0│18.5│93%│   │
│  │Proj A │Radiated    │15.0│14.0│93%│   │
│  │Proj B │Blocking    │0.0 │0.0 │0% │   │
│  │       │            │    │    │   │   │
│  └──────────────────────────────────┘   │
│                                          │
│  [調整可用工時] [重新分配]  [關閉]       │
│                                          │
└──────────────────────────────────────────┘
```

---

#### 6.3.6 延遲分析介面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 延遲分析 ===                          [匯出報表]        │
│                                                             │
│  時間範圍: [本月▼]  專案: [全部▼]                           │
│                                                             │
│  === 延遲原因分布 ===                                        │
│  ┌─────────────────────────────────────────────────────┐  │
│  │                                                     │  │
│  │          測試設備故障 (35%)                         │  │
│  │          ██████████████████                         │  │
│  │                                                     │  │
│  │          工程師人力不足 (25%)                       │  │
│  │          ████████████                               │  │
│  │                                                     │  │
│  │          客戶延遲提供樣品 (20%)                     │  │
│  │          ██████████                                 │  │
│  │                                                     │  │
│  │          測試場地被佔用 (15%)                       │  │
│  │          ███████                                    │  │
│  │                                                     │  │
│  │          其他原因 (5%)                              │  │
│  │          ██                                         │  │
│  │                                                     │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
│  === 延遲測項清單 ===                                        │
│  ┌─────────────────────────────────────────────────────┐  │
│  │專案    │測項              │工程師│延遲天數│原因      │  │
│  ├─────────────────────────────────────────────────────┤  │
│  │Proj A │Conducted Emission│張三  │5      │設備故障  │  │
│  │Proj B │Radiated Spurious │李四  │3      │人力不足  │  │
│  │Proj C │Blocking Test     │王五  │7      │場地佔用  │  │
│  │       │                  │      │       │          │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
│  平均延遲天數: 5.2 天                                        │
│  延遲測項數: 15 項                                           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

#### 6.3.7 用戶管理介面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 用戶管理 ===                          [+ 新增用戶]      │
│                                                             │
│  篩選: [角色▼] [狀態▼] [認證類型▼] [搜尋___]                │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │帳號  │姓名│Email          │角色    │狀態  │操作    │  │
│  ├─────────────────────────────────────────────────────┤  │
│  │zhang3│張三│zhang@ex.com   │Engineer│啟用  │[編輯]  │  │
│  │li4   │李四│li@example.com │Engineer│啟用  │[編輯]  │  │
│  │wang5 │王五│wang@ex.com    │Manager │啟用  │[編輯]  │  │
│  │zhao6 │趙六│zhao@ex.com    │Engineer│停用⊗│[編輯]  │  │
│  │      │    │               │        │      │        │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**新增/編輯用戶對話框:**
```
┌──────────────────────────────────────────┐
│  編輯用戶                            [X]  │
├──────────────────────────────────────────┤
│                                          │
│  帳號: zhang3 (不可修改)                 │
│                                          │
│  姓名*: [張三___]                        │
│                                          │
│  Email*: [zhang@example.com]            │
│  (用於登入身份識別,不可與他人重複)        │
│                                          │
│  角色*: [Engineer▼]                     │
│                                          │
│  認證類型: ☑ Local  ☑ AD                │
│                                          │
│  === Local 帳號設定 ===                  │
│  密碼: [重設密碼...]                     │
│                                          │
│  === AD 帳號設定 ===                     │
│  AD帳號: [zhang3___]                    │
│  AD網域: [COMPANY___]                   │
│                                          │
│  每週可用工時: [37.5] 小時               │
│                                          │
│  狀態: ○ 啟用  ○ 停用                   │
│                                          │
│  ⚠ 停用前請確認無未完成測項              │
│                                          │
│  [取消]              [儲存]               │
│                                          │
└──────────────────────────────────────────┘
```

---

### 6.4 系統管理員介面設計

#### 6.4.1 系統管理主畫面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│ RF排程系統 | 歡迎, 系統管理員 (Admin) | 登出                │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  權限管理 │ 用戶管理 │ 延遲原因 │ 系統設定 │ 稽核日誌      │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  [權限管理 Tab 內容...]                                      │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

#### 6.4.2 權限管理介面

**權限清單:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 權限管理 ===                          [+ 新增權限]      │
│                                                             │
│  篩選: [類別▼] [狀態▼] [搜尋___]                            │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │權限代碼              │名稱          │類別  │狀態│操作│  │
│  ├─────────────────────────────────────────────────────┤  │
│  │PROJECT_CREATE       │建立案件      │Project│啟用│[編]│  │
│  │PROJECT_VIEW_ALL     │查看所有案件  │Project│啟用│[編]│  │
│  │WORKLOG_VIEW_OWN     │查看自己工時  │WorkLog│啟用│[編]│  │
│  │WORKLOG_EDIT_7DAYS   │修改7天內工時 │WorkLog│啟用│[編]│  │
│  │TESTITEM_STATUS_OVERRIDE│覆寫測項狀態│TestItem│啟用│[編]│  │
│  │                     │              │       │    │    │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**權限群組管理:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 權限群組 ===                          [+ 新增群組]      │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │群組名稱  │說明              │權限數│狀態  │操作    │  │
│  ├─────────────────────────────────────────────────────┤  │
│  │Engineer │工程師預設權限    │10    │啟用  │[編輯]  │  │
│  │Manager  │主管預設權限      │25    │啟用  │[編輯]  │  │
│  │Admin    │系統管理員權限    │50    │啟用  │[編輯]  │  │
│  │Auditor  │稽核人員權限      │5     │啟用  │[編輯]  │  │
│  │         │                  │      │      │        │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**編輯權限群組對話框:**
```
┌──────────────────────────────────────────┐
│  編輯權限群組 - Engineer             [X]  │
├──────────────────────────────────────────┤
│                                          │
│  群組名稱: Engineer (不可修改)           │
│                                          │
│  說明: [工程師預設權限_______________]   │
│                                          │
│  狀態: ● 啟用  ○ 停用                   │
│                                          │
│  === 群組權限 ===                        │
│                                          │
│  [依類別顯示▼]                           │
│                                          │
│  Project:                               │
│  ☑ PROJECT_VIEW_ASSIGNED                │
│  □ PROJECT_CREATE                       │
│  □ PROJECT_EDIT                         │
│                                          │
│  TestItem:                              │
│  ☑ TESTITEM_VIEW_ASSIGNED               │
│  ☑ TESTITEM_COMPLETE                    │
│  ☑ TESTITEM_CANCEL_COMPLETION           │
│  □ TESTITEM_STATUS_OVERRIDE             │
│                                          │
│  WorkLog:                               │
│  ☑ WORKLOG_VIEW_OWN                     │
│  ☑ WORKLOG_CREATE                       │
│  ☑ WORKLOG_EDIT_7DAYS                   │
│  □ WORKLOG_EDIT_ALL                     │
│  □ WORKLOG_DELETE                       │
│                                          │
│  [展開更多...]                           │
│                                          │
│  [取消]              [儲存]               │
│                                          │
└──────────────────────────────────────────┘
```

**用戶權限指派:**
```
┌──────────────────────────────────────────┐
│  用戶權限管理 - 張三                 [X]  │
├──────────────────────────────────────────┤
│                                          │
│  基本資訊:                               │
│  帳號: zhang3                            │
│  姓名: 張三                              │
│  角色: Engineer                          │
│                                          │
│  === 群組權限 (繼承) ===                 │
│  ┌──────────────────────────────────┐   │
│  │ Engineer 群組                    │   │
│  │ • PROJECT_VIEW_ASSIGNED         │   │
│  │ • TESTITEM_VIEW_ASSIGNED        │   │
│  │ • WORKLOG_VIEW_OWN              │   │
│  │ • WORKLOG_CREATE                │   │
│  │ [共10項權限...]                  │   │
│  └──────────────────────────────────┘   │
│                                          │
│  === 個別權限 (額外授予) ===             │
│  ┌──────────────────────────────────┐   │
│  │權限               │到期日 │操作  │   │
│  ├──────────────────────────────────┤   │
│  │REPORT_VIEW_ALL   │永久   │[撤銷]│   │
│  │TESTITEM_STATUS_  │2025/12│[撤銷]│   │
│  │  OVERRIDE        │  /31  │      │   │
│  │                  │       │      │   │
│  └──────────────────────────────────┘   │
│                                          │
│  [+ 授予新權限]                          │
│                                          │
│  [關閉]                                  │
│                                          │
└──────────────────────────────────────────┘
```

---

#### 6.4.3 延遲原因管理介面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 延遲原因管理 ===                      [+ 新增原因]      │
│                                                             │
│  篩選: [類型▼] [狀態▼] [搜尋___]                            │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │原因文字          │類型      │使用次數│狀態  │操作    │  │
│  ├─────────────────────────────────────────────────────┤  │
│  │測試設備故障      │Equipment │45     │啟用  │[編輯]  │  │
│  │客戶延遲提供樣品  │Customer  │32     │啟用  │[編輯]  │  │
│  │工程師人力不足    │Engineer  │28     │啟用  │[編輯]  │  │
│  │測試場地被佔用    │Location  │20     │啟用  │[編輯]  │  │
│  │其他原因          │Other     │15     │啟用  │[編輯]  │  │
│  │舊設備故障        │Equipment │5      │停用⊗│[編輯]  │  │
│  │                  │          │       │      │        │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
│  ⚠ 已使用的延遲原因不可刪除,僅能停用                         │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**新增/編輯延遲原因對話框:**
```
┌──────────────────────────────────────────┐
│  編輯延遲原因                        [X]  │
├──────────────────────────────────────────┤
│                                          │
│  原因文字*:                              │
│  [測試設備故障____________________]      │
│                                          │
│  類型*:                                  │
│  ● Equipment  ○ Customer                │
│  ○ Engineer   ○ Location  ○ Other      │
│                                          │
│  狀態: ● 啟用  ○ 停用                   │
│                                          │
│  使用次數: 45 次                         │
│                                          │
│  ⚠ 此原因已被使用,無法刪除               │
│  ⚠ 停用後將不再顯示於工時回報選單        │
│  ⚠ 已記錄的歷史資料不受影響              │
│                                          │
│  [取消]    [刪除] [儲存]                 │
│                                          │
└──────────────────────────────────────────┘
```

---

#### 6.4.4 系統設定介面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 系統設定 ===                                            │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │設定項目              │值        │說明          │操作│  │
│  ├─────────────────────────────────────────────────────┤  │
│  │DefaultWeeklyHours   │37.5      │預設週工時    │[編]│  │
│  │WorkLogEditDays      │7         │工時可修改天數│[編]│  │
│  │LoginFailLimit       │5         │登入失敗限制  │[編]│  │
│  │LockoutMinutes       │10        │鎖定時間(分)  │[編]│  │
│  │PasswordResetExpire  │30        │密碼重設期限  │[編]│  │
│  │AuditLogRetentionDays│365       │稽核日誌保留  │[編]│  │
│  │DeletedDataRetention │180       │已刪資料保留  │[編]│  │
│  │SmtpServer           │smtp.co...│SMTP伺服器    │[編]│  │
│  │SmtpPort             │25        │SMTP Port     │[編]│  │
│  │SenderEmail          │noreply...│寄件者Email   │[編]│  │
│  │                     │          │              │    │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**編輯設定對話框:**
```
┌──────────────────────────────────────────┐
│  編輯系統設定                        [X]  │
├──────────────────────────────────────────┤
│                                          │
│  設定項目: WorkLogEditDays               │
│                                          │
│  設定值*: [7___] (天)                    │
│                                          │
│  說明: 工程師可修改自己工時記錄的天數    │
│                                          │
│  ⚠ 修改此設定將立即生效                  │
│  ⚠ 建議範圍: 3-14 天                    │
│                                          │
│  [取消]              [儲存]               │
│                                          │
└──────────────────────────────────────────┘
```

---

#### 6.4.5 稽核日誌介面

**佈局:**
```
┌─────────────────────────────────────────────────────────────┐
│  === 稽核日誌查詢 ===                      [匯出Excel]       │
│                                                             │
│  篩選條件:                                                   │
│  資料表: [全部▼]  操作類型: [全部▼]  操作人: [全部▼]        │
│  日期範圍: [2025-11-01] ~ [2025-11-30]                      │
│  [查詢]                                                     │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │時間      │操作人│資料表  │記錄ID│操作  │詳情    │  │
│  ├─────────────────────────────────────────────────────┤  │
│  │11/20 15:30│王主管│TestItem│123  │狀態覆│[查看]  │  │
│  │           │      │        │     │寫    │        │  │
│  │11/20 14:20│張三  │WorkLog │456  │新增  │[查看]  │  │
│  │11/20 10:15│李四  │WorkLog │455  │修改  │[查看]  │  │
│  │11/19 16:45│王主管│User    │10   │停用  │[查看]  │  │
│  │           │      │        │     │      │        │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**稽核日誌詳情對話框:**
```
┌──────────────────────────────────────────┐
│  稽核日誌詳情                        [X]  │
├──────────────────────────────────────────┤
│                                          │
│  操作時間: 2025-11-20 15:30:25           │
│  操作人: 王主管 (Manager)                │
│  資料表: TestItem                        │
│  記錄ID: 123                             │
│  操作類型: StatusOverride (狀態覆寫)     │
│                                          │
│  === 變更前 ===                          │
│  {                                       │
│    "Status": "InProgress",              │
│    "ManualStatusOverride": false        │
│  }                                       │
│                                          │
│  === 變更後 ===                          │
│  {                                       │
│    "Status": "OnHold",                  │
│    "ManualStatusOverride": true         │
│  }                                       │
│                                          │
│  理由:                                   │
│  等待客戶提供新版樣品,暫停測試           │
│                                          │
│  [關閉]                                  │
│                                          │
└──────────────────────────────────────────┘
```

---

## 7. API 設計規範

### 7.1 RESTful API 端點總覽

**Base URL:** `https://api.company.com/api/v1`

#### 7.1.1 認證模組

| Method | Endpoint | 說明 |
|--------|----------|------|
| POST | /auth/login-local | Local 帳號登入 |
| POST | /auth/login-ad | AD 驗證登入 |
| POST | /auth/forgot-password | 申請密碼重設 |
| GET | /auth/validate-reset-token | 驗證重設 Token |
| POST | /auth/reset-password | 執行密碼重設 |
| GET | /auth/me | 取得當前使用者資訊 |
| POST | /auth/refresh | 刷新 JWT Token |

#### 7.1.2 案件管理

| Method | Endpoint | 說明 |
|--------|----------|------|
| POST | /projects | 建立案件 |
| POST | /projects/wizard | Wizard 建案 |
| GET | /projects | 取得案件清單 |
| GET | /projects/{id} | 取得案件詳情 |
| PUT | /projects/{id} | 更新案件 |
| DELETE | /projects/{id} | 刪除案件 (Soft Delete) |
| GET | /projects/{id}/regulations | 取得案件的法規清單 |

#### 7.1.3 法規管理

| Method | Endpoint | 說明 |
|--------|----------|------|
| POST | /regulations | 建立法規 |
| GET | /regulations/{id} | 取得法規詳情 |
| PUT | /regulations/{id} | 更新法規 |
| DELETE | /regulations/{id} | 刪除法規 (Soft Delete) |
| PUT | /regulations/{id}/status | 覆寫法規狀態 |

#### 7.1.4 測試項目管理

| Method | Endpoint | 說明 |
|--------|----------|------|
| POST | /testitems | 建立測項 |
| GET | /testitems/{id} | 取得測項詳情 |
| PUT | /testitems/{id} | 更新測項 |
| DELETE | /testitems/{id} | 刪除測項 (Soft Delete) |
| PUT | /testitems/{id}/status | 覆寫測項狀態 |
| POST | /testitems/{id}/complete | 標記測項完成 |
| POST | /testitems/{id}/cancel-completion | 取消測項完成 |
| POST | /testitems/{id}/revisions | 建立補測版本 |
| GET | /testitems/{id}/engineers | 取得測項工程師分配 |
| POST | /testitems/{id}/engineers | 分配工程師 |

#### 7.1.5 工時管理

| Method | Endpoint | 說明 |
|--------|----------|------|
| POST | /worklogs | 回報工時 |
| GET | /worklogs | 查詢工時記錄 |
| GET | /worklogs/{id} | 取得工時詳情 |
| PUT | /worklogs/{id} | 修改工時 |
| DELETE | /worklogs/{id} | 刪除工時 (Soft Delete, 主管權限) |
| GET | /worklogs/my-tasks | 取得我的測項清單 |
| GET | /worklogs/testitem/{testItemId} | 取得測項的工時記錄 |

#### 7.1.6 Loading 分析

| Method | Endpoint | 說明 |
|--------|----------|------|
| GET | /loading/engineers | 取得所有工程師 Loading |
| GET | /loading/engineers/{id} | 取得工程師 Loading 明細 |
| GET | /loading/summary | 取得 Loading 總覽 |

#### 7.1.7 使用者管理

| Method | Endpoint | 說明 |
|--------|----------|------|
| POST | /users | 建立使用者 |
| GET | /users | 取得使用者清單 |
| GET | /users/{id} | 取得使用者詳情 |
| PUT | /users/{id} | 更新使用者 |
| PUT | /users/{id}/status | 啟用/停用使用者 |
| POST | /users/{id}/reset-password | 重設密碼 |

#### 7.1.8 權限管理

| Method | Endpoint | 說明 |
|--------|----------|------|
| GET | /permissions | 取得權限清單 |
| GET | /permissions/groups | 取得權限群組清單 |
| GET | /permissions/groups/{id} | 取得權限群組詳情 |
| POST | /permissions/groups | 建立權限群組 |
| PUT | /permissions/groups/{id} | 更新權限群組 |
| GET | /permissions/users/{userId} | 取得使用者有效權限 |
| POST | /permissions/users/{userId}/grant | 授予個別權限 |
| DELETE | /permissions/users/{userId}/revoke/{permissionId} | 撤銷個別權限 |

#### 7.1.9 延遲原因管理

| Method | Endpoint | 說明 |
|--------|----------|------|
| GET | /delayreasons | 取得延遲原因清單 |
| POST | /delayreasons | 建立延遲原因 |
| PUT | /delayreasons/{id} | 更新延遲原因 |
| PUT | /delayreasons/{id}/status | 啟用/停用延遲原因 |

#### 7.1.10 稽核日誌

| Method | Endpoint | 說明 |
|--------|----------|------|
| GET | /auditlogs | 查詢稽核日誌 |
| GET | /auditlogs/{id} | 取得稽核日誌詳情 |

#### 7.1.11 報表

| Method | Endpoint | 說明 |
|--------|----------|------|
| GET | /reports/project-progress | 案件進度報表 |
| GET | /reports/worklog-summary | 工時統計報表 |
| GET | /reports/delay-analysis | 延遲分析報表 |
| GET | /reports/loading | Loading 報表 |

---

### 7.2 API 請求/回應範例

#### 7.2.1 Local 登入

**請求:**
```http
POST /api/v1/auth/login-local
Content-Type: application/json

{
  "email": "zhang@example.com",
  "password": "P@ssw0rd!"
}
```

**成功回應:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": 123,
    "account": "zhang3",
    "displayName": "張三",
    "email": "zhang@example.com",
    "role": "Engineer",
    "permissions": [
      "WORKLOG_VIEW_OWN",
      "WORKLOG_CREATE",
      "TESTITEM_VIEW_ASSIGNED"
    ],
    "expiresAt": "2025-11-21T02:30:00Z"
  }
}
```

**失敗回應:**
```json
{
  "success": false,
  "error": {
    "code": "AUTH_INVALID_CREDENTIALS",
    "message": "帳號或密碼錯誤"
  }
}
```

---

#### 7.2.2 回報工時

**請求:**
```http
POST /api/v1/worklogs
Authorization: Bearer {token}
Content-Type: application/json

{
  "testItemId": 123,
  "revisionId": null,
  "workDate": "2025-11-20",
  "actualHours": 8.0,
  "status": "InProgress",
  "delayReasonIds": [],
  "comment": "今日完成初步測試"
}
```

**成功回應:**
```json
{
  "success": true,
  "data": {
    "workLogId": 456,
    "testItemId": 123,
    "revisionId": null,
    "workDate": "2025-11-20",
    "actualHours": 8.0,
    "status": "InProgress",
    "comment": "今日完成初步測試",
    "createdDate": "2025-11-20T15:30:00Z"
  },
  "message": "工時回報成功,測項狀態已更新為 InProgress"
}
```

---

#### 7.2.3 Wizard 建案

**請求:**
```http
POST /api/v1/projects/wizard
Authorization: Bearer {token}
Content-Type: application/json

{
  "project": {
    "projectName": "Project A - WiFi Module",
    "customer": "ABC Company",
    "priority": "High",
    "startDate": "2025-11-20",
    "endDate": "2025-12-31",
    "note": "WiFi 6E 模組認證專案"
  },
  "regulations": [
    {
      "regulationName": "FCC Part 24",
      "startDate": "2025-11-20",
      "endDate": "2025-12-15",
      "testItems": [
        {
          "testItemName": "Conducted Emission",
          "testType": "Conducted",
          "testLocation": "Lab A",
          "estimatedHours": 40.0,
          "engineers": [
            {
              "userId": 123,
              "roleType": "Main",
              "assignedHours": 30.0
            },
            {
              "userId": 124,
              "roleType": "Sub",
              "assignedHours": 10.0
            }
          ]
        }
      ]
    }
  ]
}
```

**成功回應:**
```json
{
  "success": true,
  "data": {
    "projectId": 789,
    "projectName": "Project A - WiFi Module",
    "status": "Draft",
    "regulationCount": 1,
    "testItemCount": 1,
    "totalEstimatedHours": 40.0
  },
  "message": "專案建立成功"
}
```

---

#### 7.2.4 覆寫測項狀態

**請求:**
```http
PUT /api/v1/testitems/123/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "newStatus": "OnHold",
  "reason": "等待客戶提供新版樣品,暫停測試"
}
```

**成功回應:**
```json
{
  "success": true,
  "data": {
    "testItemId": 123,
    "oldStatus": "InProgress",
    "newStatus": "OnHold",
    "manualStatusOverride": true,
    "modifiedBy": "王主管",
    "modifiedDate": "2025-11-20T15:30:00Z",
    "reason": "等待客戶提供新版樣品,暫停測試"
  },
  "message": "測項狀態已覆寫,相關法規與專案狀態已重新計算"
}
```

---

### 7.3 錯誤碼設計

| 錯誤碼 | HTTP Status | 說明 |
|--------|-------------|------|
| AUTH_INVALID_CREDENTIALS | 401 | 帳號或密碼錯誤 |
| AUTH_ACCOUNT_LOCKED | 403 | 帳號已鎖定 |
| AUTH_ACCOUNT_DISABLED | 403 | 帳號已停用 |
| AUTH_TOKEN_EXPIRED | 401 | Token 已過期 |
| AUTH_TOKEN_INVALID | 401 | Token 無效 |
| PERMISSION_DENIED | 403 | 權限不足 |
| VALIDATION_ERROR | 400 | 輸入驗證失敗 |
| RESOURCE_NOT_FOUND | 404 | 資源不存在 |
| RESOURCE_ALREADY_EXISTS | 409 | 資源已存在 |
| WORKLOG_EDIT_DEADLINE_EXCEEDED | 400 | 超過工時編輯期限 |
| WORKLOG_DUPLICATE_DATE | 409 | 該日期已有工時記錄 |
| TESTITEM_ALREADY_COMPLETED | 400 | 測項已完成 |
| TESTITEM_CANNOT_CANCEL_COMPLETION | 403 | 無法取消完成狀態 |
| LOADING_OVERLOAD_WARNING | 400 | Loading 超載警告 |
| DELAYREASON_IN_USE | 409 | 延遲原因使用中,無法刪除 |
| EMAIL_DUPLICATE | 409 | Email 已被使用 |
| INTERNAL_SERVER_ERROR | 500 | 伺服器內部錯誤 |

---

## 8. 部署架構

### 8.1 部署拓撲圖

```
┌────────────────────────────────────────────────┐
│            公司內部網路                         │
│                                                │
│  ┌──────────────┐  ┌──────────────┐            │
│  │ Client PC 1  │  │ Client PC 2  │  ...       │
│  │ (WinForms)   │  │ (WinForms)   │            │
│  └──────────────┘  └──────────────┘            │
│         │                  │                   │
│         └──────────┬───────┘                   │
│                    │ HTTPS (JWT)               │
│                    ↓                           │
│  ┌─────────────────────────────────────────┐   │
│  │  Application Server (IIS / Kestrel)    │   │
│  │  ┌───────────────────────────────────┐  │   │
│  │  │  ASP.NET Core Web API (.NET 8)    │  │   │
│  │  │  - JWT Authentication             │  │   │
│  │  │  - Permission-Based Authorization │  │   │
│  │  │  - Status Calculation Engine      │  │   │
│  │  └───────────────────────────────────┘  │   │
│  └─────────────────────────────────────────┘   │
│                    │                           │
│                    ↓                           │
│  ┌─────────────────────────────────────────┐   │
│  │  Database Server                        │   │
│  │  ┌───────────────────────────────────┐  │   │
│  │  │  SQL Server 2019 Express          │  │   │
│  │  │  - Database: RFScheduling         │  │   │
│  │  │  - Email Unique Constraint        │  │   │
│  │  │  - Soft Delete Support            │  │   │
│  │  └───────────────────────────────────┘  │   │
│  └─────────────────────────────────────────┘   │
│                    │                           │
│                    ↓                           │
│  ┌─────────────────────────────────────────┐   │
│  │  Active Directory Server                │   │
│  │  - Windows Authentication               │   │
│  │  - User Email Sync                      │   │
│  └─────────────────────────────────────────┘   │
│                                                │
└────────────────────────────────────────────────┘
                    │ SMTP
                    ↓
          ┌───────────────────┐
          │  Email Server     │
          │  (Password Reset) │
          └───────────────────┘
```

---

### 8.2 部署環境規格

#### 8.2.1 Client 端需求

| 項目 | 規格 |
|------|------|
| 作業系統 | Windows 10/11 Professional |
| .NET Runtime | .NET 8.0 Desktop Runtime |
| RAM | 4GB 以上 |
| 硬碟空間 | 500MB 以上 |
| 螢幕解析度 | 1920x1080 建議 |
| 網路 | 連接公司內網 |

#### 8.2.2 Application Server

| 項目 | 規格 |
|------|------|
| 作業系統 | Windows Server 2019/2022 |
| .NET Runtime | .NET 8.0 ASP.NET Core Runtime |
| CPU | 4 Core 以上 |
| RAM | 8GB 以上 |
| 硬碟空間 | 100GB 以上 |
| Web Server | IIS 10 或 Kestrel |

#### 8.2.3 Database Server

| 項目 | 規格 |
|------|------|
| 作業系統 | Windows Server 2019/2022 |
| 資料庫 | SQL Server 2019 Express |
| CPU | 4 Core 以上 |
| RAM | 16GB 以上 |
| 硬碟空間 | 500GB 以上 (含備份空間) |
| 備份策略 | 每日完整備份 + 交易記錄備份 |

---

### 8.3 安全設定

#### 8.3.1 JWT 設定

```json
{
  "Jwt": {
    "Key": "{STRONG_SECRET_KEY_256_BIT}",
    "Issuer": "RFSchedulingAPI",
    "Audience": "RFSchedulingClient",
    "ExpiresInHours": 8,
    "RefreshTokenExpiresInDays": 30
  }
}
```

**重要提醒:**
- `Key` 必須為 256 位元以上的強密鑰
- 不得提交到版本控制系統
- 定期更換密鑰 (建議每季)

#### 8.3.2 連線字串加密

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DB_SERVER;Database=RFScheduling;User Id=sa;Password={ENCRYPTED_PASSWORD};TrustServerCertificate=True;"
  }
}
```

**加密方式:**
- 使用 ASP.NET Core Data Protection API
- 或使用 Azure Key Vault / Windows DPAPI

#### 8.3.3 HTTPS 設定

- 必須使用 HTTPS (TLS 1.2+)
- 部署有效的 SSL 憑證
- 強制 HTTPS Redirect

---

## 9. 技術棧總覽

### 9.1 後端技術

| 技術 | 版本 | 用途 |
|------|------|------|
| .NET | 8.0 | 應用程式框架 |
| ASP.NET Core | 8.0 | Web API 框架 |
| Entity Framework Core | 8.0 | ORM 框架 |
| SQL Server | 2019 Express | 資料庫 |
| JWT | - | 身份驗證 |
| BCrypt.Net | - | 密碼雜湊 |
| Newtonsoft.Json | 13.0+ | JSON 序列化 |
| AutoMapper | 12.0+ | 物件映射 |
| FluentValidation | 11.0+ | 輸入驗證 |
| Serilog | 3.0+ | 日誌記錄 |

### 9.2 前端技術

| 技術 | 版本 | 用途 |
|------|------|------|
| WinForms | .NET 8.0 | UI 框架 |
| DevExpress | 23.2+ | UI 控制項 |
| HttpClient | .NET 8.0 | HTTP 通訊 |
| Newtonsoft.Json | 13.0+ | JSON 處理 |

---

## 10. 開發規範

### 10.1 命名規範

#### 10.1.1 C# 命名規範

```csharp
// Class / Interface / Enum: PascalCase
public class ProjectService { }
public interface IProjectService { }
public enum ProjectStatus { }

// Method: PascalCase
public async Task<Result> CreateProjectAsync(CreateProjectDto dto) { }

// Property: PascalCase
public string ProjectName { get; set; }

// Private Field: _camelCase
private readonly IProjectService _projectService;

// Parameter / Local Variable: camelCase
public void ProcessProject(int projectId) 
{
    var projectName = "...";
}

// Constant: PascalCase
public const int MaxWorkHoursPerDay = 12;
```

#### 10.1.2 資料庫命名規範

```sql
-- Table: PascalCase
CREATE TABLE [dbo].[ProjectWorkLog]

-- Column: PascalCase
[ProjectId], [WorkDate], [ActualHours]

-- Index: IX_TableName_ColumnName
CREATE INDEX [IX_Project_Status]

-- Foreign Key: FK_ChildTable_ParentTable
CONSTRAINT [FK_WorkLog_TestItem]

-- Unique Constraint: UQ_TableName_ColumnName
CONSTRAINT [UQ_User_Email]
```

#### 10.1.3 API 端點命名規範

```
// RESTful 風格
GET    /api/v1/projects          取得清單
GET    /api/v1/projects/{id}     取得單筆
POST   /api/v1/projects          新增
PUT    /api/v1/projects/{id}     更新
DELETE /api/v1/projects/{id}     刪除

// 特殊操作使用動詞
POST   /api/v1/projects/wizard   Wizard建案
POST   /api/v1/testitems/{id}/complete  完成測項
PUT    /api/v1/testitems/{id}/status    覆寫狀態
```

---

### 10.2 程式碼規範

#### 10.2.1 Service 層設計範例

```csharp
public class TestItemService : ITestItemService
{
    private readonly RFSchedulingDbContext _context;
    private readonly IPermissionService _permissionService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<TestItemService> _logger;

    public TestItemService(
        RFSchedulingDbContext context,
        IPermissionService permissionService,
        IAuditLogService auditLogService,
        ILogger<TestItemService> logger)
    {
        _context = context;
        _permissionService = permissionService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<Result<TestItemDto>> CompleteTestItemAsync(
        int testItemId, 
        int userId)
    {
        try
        {
            // 1. 權限檢查
            if (!await _permissionService.HasPermission(userId, "TESTITEM_COMPLETE"))
            {
                return Result<TestItemDto>.Fail("權限不足");
            }

            // 2. 資料驗證
            var testItem = await _context.TestItems
                .Include(t => t.Regulation)
                .FirstOrDefaultAsync(t => t.TestItemId == testItemId && !t.IsDeleted);

            if (testItem == null)
            {
                return Result<TestItemDto>.Fail("測項不存在");
            }

            if (testItem.Status == TestItemStatus.Completed)
            {
                return Result<TestItemDto>.Fail("測項已完成");
            }

            // 3. 執行業務邏輯
            var oldStatus = testItem.Status;
            testItem.Status = TestItemStatus.Completed;
            testItem.CompletedByUserId = userId;
            testItem.ModifiedDate = DateTime.Now;
            testItem.ModifiedByUserId = userId;

            // 4. 記錄稽核日誌
            await _auditLogService.LogAsync(new AuditLog
            {
                TableName = "TestItem",
                RecordId = testItemId,
                Action = "Complete",
                OldValue = JsonConvert.SerializeObject(new { Status = oldStatus }),
                NewValue = JsonConvert.SerializeObject(new { Status = TestItemStatus.Completed }),
                UserId = userId,
                Reason = "工程師標記完成"
            });

            // 5. 儲存變更
            await _context.SaveChangesAsync();

            // 6. 觸發狀態重算
            await RecalculateRegulationStatusAsync(testItem.RegulationId);

            _logger.LogInformation(
                "TestItem {TestItemId} completed by User {UserId}", 
                testItemId, userId);

            return Result<TestItemDto>.Success(MapToDto(testItem));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error completing TestItem {TestItemId}", testItemId);
            return Result<TestItemDto>.Fail("完成測項時發生錯誤");
        }
    }

    private async Task RecalculateRegulationStatusAsync(int regulationId)
    {
        // 狀態重算邏輯...
    }

    private TestItemDto MapToDto(TestItem testItem)
    {
        // 物件映射邏輯...
    }
}
```

#### 10.2.2 Controller 層設計範例

```csharp
[ApiController]
[Route("api/v1/testitems")]
[Authorize]
public class TestItemController : ControllerBase
{
    private readonly ITestItemService _testItemService;
    private readonly ILogger<TestItemController> _logger;

    public TestItemController(
        ITestItemService testItemService,
        ILogger<TestItemController> logger)
    {
        _testItemService = testItemService;
        _logger = logger;
    }

    /// <summary>
    /// 標記測項完成
    /// </summary>
    [HttpPost("{id}/complete")]
    [RequirePermission("TESTITEM_COMPLETE")]
    [ProducesResponseType(typeof(ApiResponse<TestItemDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 403)]
    public async Task<IActionResult> CompleteTestItem(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _testItemService.CompleteTestItemAsync(id, userId);

        if (!result.Success)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Error = new ErrorInfo
                {
                    Code = "TESTITEM_COMPLETE_FAILED",
                    Message = result.ErrorMessage
                }
            });
        }

        return Ok(new ApiResponse<TestItemDto>
        {
            Success = true,
            Data = result.Data,
            Message = "測項已標記完成"
        });
    }

    /// <summary>
    /// 覆寫測項狀態 (主管權限)
    /// </summary>
    [HttpPut("{id}/status")]
    [RequirePermission("TESTITEM_STATUS_OVERRIDE")]
    [ProducesResponseType(typeof(ApiResponse<TestItemDto>), 200)]
    public async Task<IActionResult> OverrideStatus(
        int id, 
        [FromBody] OverrideStatusDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _testItemService.OverrideStatusAsync(
            id, dto.NewStatus, userId, dto.Reason);

        if (!result.Success)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Error = new ErrorInfo
                {
                    Code = "STATUS_OVERRIDE_FAILED",
                    Message = result.ErrorMessage
                }
            });
        }

        return Ok(new ApiResponse<TestItemDto>
        {
            Success = true,
            Data = result.Data,
            Message = "測項狀態已覆寫,相關狀態已重新計算"
        });
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
}
```

---

### 10.3 錯誤處理規範

#### 10.3.1 全域錯誤處理 Middleware

```csharp
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleUnauthorizedExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(
        HttpContext context, 
        ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse
        {
            Success = false,
            Error = new ErrorInfo
            {
                Code = "VALIDATION_ERROR",
                Message = "輸入驗證失敗",
                Details = ex.Errors.Select(e => e.ErrorMessage).ToList()
            }
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleUnauthorizedExceptionAsync(
        HttpContext context, 
        UnauthorizedAccessException ex)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse
        {
            Success = false,
            Error = new ErrorInfo
            {
                Code = "PERMISSION_DENIED",
                Message = "權限不足"
            }
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleGenericExceptionAsync(
        HttpContext context, 
        Exception ex)
    {
        _logger.LogError(ex, "未處理的例外發生");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse
        {
            Success = false,
            Error = new ErrorInfo
            {
                Code = "INTERNAL_SERVER_ERROR",
                Message = "伺服器發生錯誤,請稍後再試"
            }
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
```

---

## 11. 測試策略

### 11.1 單元測試

**測試框架:** xUnit + Moq

**測試範例:**
```csharp
public class TestItemServiceTests
{
    private readonly Mock<RFSchedulingDbContext> _mockContext;
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<IAuditLogService> _mockAuditLogService;
    private readonly TestItemService _service;

    public TestItemServiceTests()
    {
        _mockContext = new Mock<RFSchedulingDbContext>();
        _mockPermissionService = new Mock<IPermissionService>();
        _mockAuditLogService = new Mock<IAuditLogService>();
        _service = new TestItemService(
            _mockContext.Object,
            _mockPermissionService.Object,
            _mockAuditLogService.Object,
            Mock.Of<ILogger<TestItemService>>());
    }

    [Fact]
    public async Task CompleteTestItem_WithValidData_ShouldSucceed()
    {
        // Arrange
        var testItemId = 123;
        var userId = 456;
        var testItem = new TestItem
        {
            TestItemId = testItemId,
            Status = TestItemStatus.InProgress,
            RegulationId = 789
        };

        _mockPermissionService
            .Setup(x => x.HasPermission(userId, "TESTITEM_COMPLETE"))
            .ReturnsAsync(true);

        _mockContext.Setup(x => x.TestItems.FindAsync(testItemId))
            .ReturnsAsync(testItem);

        // Act
        var result = await _service.CompleteTestItemAsync(testItemId, userId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(TestItemStatus.Completed, testItem.Status);
        Assert.Equal(userId, testItem.CompletedByUserId);
        _mockAuditLogService.Verify(
            x => x.LogAsync(It.IsAny<AuditLog>()), 
            Times.Once);
    }

    [Fact]
    public async Task CompleteTestItem_WithoutPermission_ShouldFail()
    {
        // Arrange
        var testItemId = 123;
        var userId = 456;

        _mockPermissionService
            .Setup(x => x.HasPermission(userId, "TESTITEM_COMPLETE"))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CompleteTestItemAsync(testItemId, userId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("權限不足", result.ErrorMessage);
    }
}
```

---

### 11.2 整合測試

**測試框架:** xUnit + WebApplicationFactory

**測試範例:**
```csharp
public class TestItemControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TestItemControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteTestItem_WithValidToken_ShouldReturn200()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync("/api/v1/testitems/123/complete", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
        Assert.True(apiResponse.Success);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var loginDto = new
        {
            email = "test@example.com",
            password = "Test@123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login-local", loginDto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        return result.Data.Token;
    }
}
```

---

## 12. 版本控制與變更管理

### 12.1 版本編號規則

**格式:** `Major.Minor.Patch`

**範例:** `1.2.3`

- **Major (主版本):** 重大功能變更或架構調整
- **Minor (次版本):** 新增功能或中等規模變更
- **Patch (修補版本):** Bug 修復或小幅調整

---

### 12.2 變更記錄

#### v3.0 (2025-11-20)
- ✅ 同步 SA v2.3 最新需求
- ✅ 新增完整 UI 介面設計規範
- ✅ 更新狀態計算邏輯(三層推算)
- ✅ 新增 IAM 權限體系設計
- ✅ 補充 Email 合併機制
- ✅ 新增 Soft Delete 與 IsActive 機制說明
- ✅ 新增 API 端點完整定義
- ✅ 新增工程師/主管/Admin 介面設計
- ✅ 補充部署架構與技術棧
- ✅ 新增開發規範與測試策略

#### v2.1 (2025-11-19)
- 補充混合登入機制與JWT安全性

#### v2.0 (2025-11-17)
- 調整架構設計,新增API規範

#### v1.0 (2025-11-14)
- 初版系統設計文件

---

## 13. 附錄

### 13.1 常用 SQL 查詢範例

#### 13.1.1 查詢工程師 Loading

```sql
-- 查詢工程師本週 Loading
WITH EngineerLoading AS (
    SELECT 
        u.UserId,
        u.DisplayName,
        u.WeeklyAvailableHours,
        SUM(tie.AssignedHours) AS AssignedHours,
        SUM(wl.ActualHours) AS ActualHours
    FROM [User] u
    LEFT JOIN TestItemEngineer tie ON u.UserId = tie.EngineerUserId 
        AND tie.IsDeleted = 0
    LEFT JOIN TestItem ti ON tie.TestItemId = ti.TestItemId 
        AND ti.IsDeleted = 0
    LEFT JOIN Regulation r ON ti.RegulationId = r.RegulationId 
        AND r.IsDeleted = 0
    LEFT JOIN Project p ON r.ProjectId = p.ProjectId 
        AND p.IsDeleted = 0 
        AND p.Status = 'Active'
    LEFT JOIN WorkLog wl ON ti.TestItemId = wl.TestItemId 
        AND wl.EngineerUserId = u.UserId
        AND wl.IsDeleted = 0
        AND wl.WorkDate >= DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()), 0)  -- 本週
    WHERE u.IsActive = 1 
        AND u.RoleId = (SELECT RoleId FROM Role WHERE RoleName = 'Engineer')
    GROUP BY u.UserId, u.DisplayName, u.WeeklyAvailableHours
)
SELECT 
    UserId,
    DisplayName,
    WeeklyAvailableHours,
    ISNULL(AssignedHours, 0) AS AssignedHours,
    ISNULL(ActualHours, 0) AS ActualHours,
    CAST(ISNULL(AssignedHours, 0) / WeeklyAvailableHours * 100 AS DECIMAL(5,2)) AS LoadingPercentage
FROM EngineerLoading
ORDER BY LoadingPercentage DESC;
```

#### 13.1.2 查詢延遲測項統計

```sql
-- 查詢本月延遲測項與原因分布
SELECT 
    dr.ReasonType,
    dr.ReasonText,
    COUNT(DISTINCT wldr.WorkLogId) AS DelayCount,
    CAST(COUNT(DISTINCT wldr.WorkLogId) * 100.0 / 
        (SELECT COUNT(DISTINCT WorkLogId) 
         FROM WorkLogDelayReason wldr2
         JOIN WorkLog wl2 ON wldr2.WorkLogId = wl2.WorkLogId
         WHERE wl2.WorkDate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)
           AND wl2.IsDeleted = 0) 
    AS DECIMAL(5,2)) AS Percentage
FROM DelayReason dr
JOIN WorkLogDelayReason wldr ON dr.DelayReasonId = wldr.DelayReasonId
JOIN WorkLog wl ON wldr.WorkLogId = wl.WorkLogId
WHERE wl.Status = 'Delayed'
  AND wl.WorkDate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)  -- 本月
  AND wl.IsDeleted = 0
  AND dr.IsActive = 1
GROUP BY dr.ReasonType, dr.ReasonText
ORDER BY DelayCount DESC;
```

---

### 13.2 DevExpress 控制項清單

**常用控制項:**

| 控制項 | 用途 |
|--------|------|
| GridControl | 資料表格顯示 |
| TreeList | 樹狀結構顯示 (專案-法規-測項) |
| DateEdit | 日期選擇器 |
| ComboBoxEdit | 下拉選單 |
| TextEdit | 文字輸入框 |
| MemoEdit | 多行文字輸入 |
| CheckEdit | 核取方塊 |
| SimpleButton | 按鈕 |
| LabelControl | 文字標籤 |
| PanelControl | 容器面板 |
| LayoutControl | 表單佈局控制 |
| ChartControl | 圖表顯示 (Loading趨勢) |
| ProgressBarControl | 進度條 (Loading百分比) |
| TabControl | 分頁控制項 |
| WizardControl | 精靈控制項 (建案流程) |

---




