using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities
{
    public class TestItemRevision : ISoftDeletable, ICreatableRequired, IModifiable
    {
        public int RevisionId { get; set; }

        public int TestItemId { get; set; }

        [MaxLength(10)]
        public string RevisionNumber { get; set; } = string.Empty;

        public decimal EstimatedHours { get; set; }

        [MaxLength(200)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description {  get; set; }

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

        public TestItem TestItem { get; set; } = null!;

        public User CreatedBy { get; set; } = null!;

        public ICollection<WorkLog> WorkLogs  { get; set; } = new List<WorkLog>();
    }
}
