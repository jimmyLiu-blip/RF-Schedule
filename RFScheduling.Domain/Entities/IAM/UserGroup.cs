using RFScheduling.Domain.Entities.Shared;
using RFScheduling.Domain.Interfaces;

namespace RFScheduling.Domain.Entities.IAM
{
    public class UserGroup : ICreatableRequired
    {
        public int UserGroupId { get; set; }
        
        public int UserId { get; set; }

        public int GroupId { get; set; }

        // ICreatable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        // Navigation
        public User User { get; set; } = null!;

        public User CreatedBy { get; set; } = null!;

        public PermissionGroup Group { get; set; } = null!;


    }
}
