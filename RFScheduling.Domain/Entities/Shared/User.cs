using RFScheduling.Domain.Entities.IAM;
using RFScheduling.Domain.Entities.Scheduling;
using RFScheduling.Domain.Entities.System;
using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities.Shared
{
    public class User : ICreatableNullable, IModifiable
    {
        public int UserId { get; set; }

        [MaxLength(50)]
        public string Account { get; set; } = string.Empty;

        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public decimal WeeklyAvailableHours { get; set; } = 37.5m;

        public bool IsActive { get; set; } = true;

        [MaxLength(20)]
        public string AuthType {  get; set; } = "Local";

        [MaxLength(100)]
        public string? ADAccount {  get; set; }

        [MaxLength(100)]
        public string? ADDomain { get; set; }

        public DateTime? LastLoginDate { get; set; }

        [MaxLength(50)]
        public string LastLoginIP { get; set; } = string.Empty;

        public int? CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Navigation Properties
        public Role Role { get; set; } = null!;

        public User? CreatedBy { get; set; }

        public User? ModifiedBy { get; set; }
       
        public ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();
       
        public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
        
        public ICollection<TestItemEngineer> TestItemEngineers { get; set; } = new List<TestItemEngineer>();
       
        public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();

        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    }
}