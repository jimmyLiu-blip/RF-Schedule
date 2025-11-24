using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Interfaces;

namespace RFScheduling.Domain.Entities.IAM
{
    public class PermissionGroupMapping : ICreatableRequired
    {
        public int MappingId { get; set; }

        public int GroupId { get; set; }

        public int PermissionId { get; set; }

        // ICreatable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        // Navigation Properties
        public User CreatedBy { get; set; } = null!;

        public PermissionGroup PermissionGroup { get; set; } = null!;

        public Permission Permission { get; set; } = null!;

    }
}
