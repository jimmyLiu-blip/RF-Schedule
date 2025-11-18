using RFScheduling.Domain.Enums;
using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities
{
    public class Project : ISoftDeletable, ICreatableRequired, IModifiable
    {
        public int ProjectId { get; set; }

        [MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Customer {  get; set; }

        public PriorityLevel Priority {  get; set; } = PriorityLevel.Medium;

        public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(1000)]
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
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Navigation Properties
        public User CreatedBy { get; set; } = null!;
        public ICollection<Regulation> Regulations { get; set; } = new List<Regulation>();
    }
}
