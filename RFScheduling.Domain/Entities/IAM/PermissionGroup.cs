using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Interfaces;

namespace RFScheduling.Domain.Entities.IAM
{
    public class PermissionGroup : ICreatableRequired, IModifiable
    {
        public int GroupId {  get; set; }

        public string GroupName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        public User CreatedBy { get; set; } = null!;

        public User? ModifiedBy { get; set; }

        public ICollection<PermissionGroupMapping> PermissionGroupMappings { get; set; }= new List<PermissionGroupMapping>();

        public ICollection<UserGroup> UserGroups { get; set; }= new List<UserGroup>();
    }
}
