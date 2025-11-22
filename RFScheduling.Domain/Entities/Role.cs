using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities
{
    public class Role : ICreatableRequired, IModifiable
    {
        public int RoleId { get; set; }

        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description {  get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        public User CreatedBy { get; set; } = null!;

        public User? ModifiedBy { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
