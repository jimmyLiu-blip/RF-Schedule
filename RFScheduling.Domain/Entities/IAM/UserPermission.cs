using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Interfaces;


namespace RFScheduling.Domain.Entities.IAM
{
    public class UserPermission : ICreatableRequired
    {
        public int UserPermissionId { get; set; }

        public int UserId { get; set; }

        public int PermissionId { get; set; }

        public DateTime? ExpireDate { get; set; }

        public bool IsActive { get; set; } = true;

        // ICreatable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        // Navigation
        public User User { get; set; } = null!;       // 被授權者

        public User CreatedBy { get; set; } = null!;  // 授權者

        public Permission Permission { get; set; } = null!;
    }
}
