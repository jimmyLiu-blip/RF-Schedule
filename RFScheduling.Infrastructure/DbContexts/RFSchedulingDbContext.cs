using Microsoft.EntityFrameworkCore;
using RFScheduling.Domain.Entities.IAM;
using RFScheduling.Domain.Entities.Scheduling;
using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Entities.System;
using RFScheduling.Domain.Enums;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Soft Delete 全域過濾器

            modelBuilder.Entity<Project>().HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Regulation>().HasQueryFilter(x => !x.IsDeleted);
            
            modelBuilder.Entity<TestItem>().HasQueryFilter(x => !x.IsDeleted);
            
            modelBuilder.Entity<TestItemEngineer>().HasQueryFilter(x => !x.IsDeleted);
           
            modelBuilder.Entity<TestItemRevision>().HasQueryFilter(x => !x.IsDeleted);
            
            modelBuilder.Entity<WorkLog>().HasQueryFilter(x => !x.IsDeleted);

            // 2. User
            modelBuilder.Entity<User>(entity =>
            {
                // Unique Indexes
                entity.HasIndex(x => x.Email).IsUnique();
                entity.HasIndex(x => x.Account).IsUnique();

                // Role (1 → 多)
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)             // Users 是指 Role 這個類別裡的 Navigation Property
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict); // 如果某個 Role 底下還有 User，不允許刪除這個 Role

                // CreatedBy / ModifiedBy（單向一對多）
                entity.HasOne(u => u.CreatedBy)
                      .WithMany()
                      .HasForeignKey(u => u.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.ModifiedBy)
                      .WithMany()
                      .HasForeignKey(u => u.ModifiedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // User 1 → 多 PasswordReset（Chascade）
                entity.HasMany(u => u.PasswordResets)
                      .WithOne(pr => pr.User)
                      .HasForeignKey(pr => pr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // User 1 → 多 TestItemEngineer（Restrict）
                entity.HasMany(u => u.TestItemEngineers)
                      .WithOne(tie => tie.Engineer)
                      .HasForeignKey(tie => tie.EngineerUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // User 1 → 多 WorkLog（Restrict）
                entity.HasMany(u => u.WorkLogs)
                      .WithOne(w => w.Engineer)
                      .HasForeignKey(w => w.EngineerUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // User 1 → 多 UserGroups（Restrict）
                entity.HasMany(u => u.UserGroups)
                      .WithOne(ug => ug.User)
                      .HasForeignKey(ug => ug.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // User 1 → 多 UserPermissions（Restrict）
                entity.HasMany(u => u.UserPermissions)
                      .WithOne()

                // RowVersion（併發控制）
                entity.Property(u => u.RowVersion)
                      .IsRowVersion();

                // Default Values
                entity.Property(u => u.WeeklyAvailableHours)
                      .HasDefaultValue(37.5m);

                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);

                entity.Property(u => u.AuthType)
                      .HasDefaultValue("Local");

                entity.Property(u => u.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");

                // MaxLength 
                entity.Property(u => u.Account).HasMaxLength(50);
                entity.Property(u => u.PasswordHash).HasMaxLength(255);
                entity.Property(u => u.DisplayName).HasMaxLength(100);
                entity.Property(u => u.Email).HasMaxLength(255);
                entity.Property(u => u.AuthType).HasMaxLength(20);
                entity.Property(u => u.ADAccount).HasMaxLength(100);
                entity.Property(u => u.ADDomain).HasMaxLength(100);
                entity.Property(u => u.LastLoginIP).HasMaxLength(50);
            });

            // 3. Role
            modelBuilder.Entity<Role>(entity =>
            {
                // Unique Indexes
                entity.HasIndex(r => r.RoleName).IsUnique();

                // Audit - CreatedBy
                entity.HasOne(r => r.CreatedBy)
                      .WithMany()
                      .HasForeignKey(r => r.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Audit - ModifiedBy
                entity.HasOne(r => r.ModifiedBy)
                      .WithMany()
                      .HasForeignKey(r => r.ModifiedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Role 1 - * Users
                entity.HasMany(r => r.Users)
                      .WithOne(u => u.Role)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Default Values
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true);

                entity.Property(u => u.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");

                // MaxLength
                entity.Property(e => e.RoleName).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(200);

            });

            // 4. Permission
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(x => x.PermissionCode).IsUnique();

                entity.HasIndex(x => x.Category);

                // Default Values
                entity.Property(p => p.IsActive)
                      .HasDefaultValue(true);

                entity.Property(p => p.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");

                // PermissionGroupMapping（一對多關聯）
                entity.HasMany(p => p.PermissionGroupMappings)
                      .WithOne(pgm => pgm.Permission)
                      .HasForeignKey(pgm => pgm.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationships
                entity.HasOne(p => p.CreatedBy)
                      .WithMany()
                      .HasForeignKey(p => p.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.ModifiedBy)
                      .WithMany()
                      .HasForeignKey(p => p.ModifiedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Enum → string 轉換
                entity.Property(p => p.Category)
                      .HasConversion<string>()
                      .HasMaxLength(50);

                // MaxLength 
                entity.Property(p => p.PermissionCode)
                      .HasMaxLength(100);

                entity.Property(p => p.PermissionName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(p => p.Description)
                      .HasMaxLength(200)
                      .IsRequired(false);

                // enum列舉
                entity.ToTable(tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_Permission_Category",
                        "[Category] IN ('Project','Regulation','TestItem','WorkLog','User','Report','System')"
                        );
                });
            });

            // 5. PermissionGroup
            modelBuilder.Entity<PermissionGroup>(entity =>
            {
                // Unique Indexes
                entity.HasIndex(x => x.GroupName).IsUnique();

                // Relationships
                entity.HasOne(pg => pg.CreatedBy)
                      .WithMany()
                      .HasForeignKey(pg => pg.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pg => pg.ModifiedBy)
                      .WithMany()
                      .HasForeignKey(pg => pg.ModifiedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 一對多
                entity.HasMany(pg => pg.PermissionGroupMappings)
                      .WithOne(pgm => pgm.PermissionGroup)
                      .HasForeignKey(pgm => pgm.GroupId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(pg => pg.UserGroups)
                      .WithOne(ug => ug.Group)
                      .HasForeignKey(ug => ug.GroupId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Default Values
                entity.Property(pg => pg.IsActive)
                      .HasDefaultValue(true);

                entity.Property(pg => pg.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(pg => pg.GroupName).IsRequired();

                entity.Property(pg => pg.Description).IsRequired(false);

                // MaxLength 
                entity.Property(pg => pg.GroupName)
                      .HasMaxLength(50);

                entity.Property(pg => pg.Description)
                      .HasMaxLength(200);

            });

            // 6. PermissionGroupMapping
            modelBuilder.Entity<PermissionGroupMapping>(entity =>
            {
                // Unique Indexes
                entity.HasIndex(pgm => new { pgm.GroupId, pgm.PermissionId }).IsUnique();

                // Audit CreatedBy
                entity.HasOne(pgm => pgm.CreatedBy)
                      .WithMany()
                      .HasForeignKey(pgm => pgm.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Default Values
                entity.Property(pgm => pgm.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");

            });

            // 7. UserGroup
            modelBuilder.Entity<UserGroup>(entity =>
            {
                // Unique Indexes
                entity.HasIndex(ug => new { ug.UserId, ug.GroupId }).IsUnique();

                // Audit CreatedBy
                entity.HasOne(ug => ug.CreatedBy)
                      .WithMany()
                      .HasForeignKey(u => u.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Default Values
                entity.Property(ug => ug.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");
            });

            // 8. UserPermission
            modelBuilder.Entity<UserPermission>(entity =>
            {
                // Audit CreatedBy
                entity.HasOne(up => up.CreatedBy)
                      .WithMany()
                      .HasForeignKey(u => u.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Default Values
                entity.Property(up => up.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(up => up.IsActive).HasDefaultValue(true);

            });


            // 9. Project
            modelBuilder.Entity<Project>(entity =>
            {
                // Unique Indexes
                entity.HasIndex(x => x.ProjectName).IsUnique();

                entity.HasIndex(p => new { p.Customer, p.ProjectName }).IsUnique();

                // Relationships
                entity.HasOne(p => p.CreatedBy)
                      .WithMany(u => u.CreatedProjects)
                      .HasForeignKey(p => p.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.ModifiedBy)
                      .WithMany()
                      .HasForeignKey(p => p.ModifiedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // RowVersion（併發控制）
                entity.Property(p => p.RowVersion)
                      .IsRowVersion();

                // Default Values
                entity.Property(p => p.Priority)
                      .HasConversion<string>()
                      .HasMaxLength(20)
                      .HasDefaultValue(PriorityLevel.Medium);

                entity.Property(p => p.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20)
                      .HasDefaultValue(ProjectStatus.Draft);

                entity.Property(p => p.IsDeleted)
                      .HasDefaultValue(false);

                entity.Property(p => p.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(p => p.Note).IsRequired(false);

                // MaxLength 
                entity.Property(p => p.ProjectName)
                      .HasMaxLength(200);

                entity.Property(p => p.Customer)
                      .HasMaxLength(200);

                entity.Property(p => p.Note)
                      .HasMaxLength(1000);

                // enum列舉
                entity.ToTable(tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_Project_Status",
                        "[Status] IN ('Draft','InProgress','Completed','OnHold','Delayed')"
                    );

                    tableBuilder.HasCheckConstraint(
                        "CK_Project_Priority",
                        "[Priority] IN ('High','Medium','Low')"
                    );
                });
            });
        }
    }
}
