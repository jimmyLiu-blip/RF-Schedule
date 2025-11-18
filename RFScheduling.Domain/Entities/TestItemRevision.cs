using RFScheduling.Domain.Interfaces;

namespace RFScheduling.Domain.Entities
{
    public class TestItemRevision : ISoftDeletable, ICreatable, IModifiable
    {
        public int RevisionId { get; set; }

        public int TestItemId { get; set; }

        public string RevisionNumber { get; set; } = string.Empty;

        public decimal EstimatedHours { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string? Description {  get; set; }

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ISoftDeletable
        public bool IsDeleted { get; set; }
        public int? DeletedByUserId { get; set; }
        public DateTime? DeletedDate { get; set; }

        // Navigation Properties

        public TestItem TestItem { get; set; } = null!;

        public User User { get; set; } = null!;

        public ICollection<WorkLog> WorkLogs  { get; set; } = new List<WorkLog>();
    }
}
