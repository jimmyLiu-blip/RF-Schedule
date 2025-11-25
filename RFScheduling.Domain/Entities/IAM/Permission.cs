using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Enums;
using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities.IAM
{
    public class Permission : ICreatableRequired, IModifiable
    {
        public int PermissionId { get; set; }

        [MaxLength(100)]
        public string PermissionCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string PermissionName { get; set; }  = string.Empty;

        public PermissionCategory Category { get; set; }

        [MaxLength(200)]
        public string? Description {  get; set; }

        public bool IsActive { get; set; } = true;

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        public User CreatedBy { get; set; } = null!;

        public User? ModifiedBy { get; set; }

        public ICollection<PermissionGroupMapping> PermissionGroupMappings { get; set; } = new List<PermissionGroupMapping>();

        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    }
}
