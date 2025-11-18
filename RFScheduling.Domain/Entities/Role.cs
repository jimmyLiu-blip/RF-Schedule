using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities
{
    public class Role
    {
        public int RoleId { get; set; }

        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Description {  get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
