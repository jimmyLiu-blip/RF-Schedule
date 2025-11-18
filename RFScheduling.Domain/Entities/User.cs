using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities
{
    public class User : ISoftDeletable, ICreatableNullable, IModifiable
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

        public int? CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int? DeletedByUserId { get; set; }

        public DateTime? DeletedDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Navigation Properties
        public Role Role { get; set; } = null!;
        public ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();
        public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
        public ICollection<TestItemEngineer> TestItemEngineers { get; set; } = new List<TestItemEngineer>();
        public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}