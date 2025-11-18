using RFScheduling.Domain.Interfaces;
using RFScheduling.Domain.Enums;

namespace RFScheduling.Domain.Entities
{
    public class Project : ISoftDeletable, ICreatable, IModifiable
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string? Customer {  get; set; }

        public PriorityLevel Priority {  get; set; } = PriorityLevel.Medium;

        public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Note { get; set; }

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ISoftDeletable
        public bool IsDeleted { get; set; }
        public int? DeletedByUserId { get; set; }
        public DateTime? DeletedDate { get; set; }

        // Concurrency Token
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Navigation Properties
        public User CreatedBy { get; set; } = null!;
        public ICollection<Regulation> Regulations { get; set; } = new List<Regulation>();
    }
}
