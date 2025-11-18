namespace RFScheduling.Domain.Entities
{
    public class Role
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string Description {  get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
