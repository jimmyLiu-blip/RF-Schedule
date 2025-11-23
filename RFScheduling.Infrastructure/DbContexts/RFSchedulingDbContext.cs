using Microsoft.EntityFrameworkCore;
using RFScheduling.Domain.Entities.IAM;
using RFScheduling.Domain.Entities.Scheduling;
using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Entities.System;

namespace RFScheduling.Infrastructure.DbContexts
{
    public class RFSchedulingDbContext : DbContext
    {
        // 把外部傳進來的 options（包含連線字串、資料庫提供者、行為設定）傳給 EF Core 的 DbContext 基底類別。
        public RFSchedulingDbContext(DbContextOptions<RFSchedulingDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<PermissionGroup> PermissionGroups { get; set; }

        public DbSet<PermissionGroupMapping> PermissionGroupMappings { get; set; }

        public DbSet<UserGroup> UserGroups { get; set; }
        
        public DbSet<UserPermission> UserPermissions { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Regulation> Regulations { get; set; }
        
        public DbSet<TestItem> TestItems { get; set; }

        public DbSet<TestItemEngineer> TestItemEngineers { get; set; }

        public DbSet<TestItemRevision> TestItemRevisions { get; set; }

        public DbSet<WorkLog> WorkLogs { get; set; }

        public DbSet<DelayReason> DelayReasons { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<PasswordReset> PasswordResets { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }


    }
}
