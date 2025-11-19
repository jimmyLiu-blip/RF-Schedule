using RFScheduling.Domain.Enums;
using RFScheduling.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RFScheduling.Domain.Entities
{
    public class TestItem : ISoftDeletable, ICreatableRequired, IModifiable
    {
        public int TestItemId { get; set; }
        
        public int RegulationId { get; set; }

        [MaxLength(200)]
        public string TestItemName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string TestType {  get; set; } = string.Empty ;

        [MaxLength(100)]
        public string TestLocation {  get; set; } = string.Empty ;

        public decimal EstimatedHours { get; set; }

        public TestItemStatus Status { get; set; } = TestItemStatus.NotStarted ;

        [MaxLength(500)]
        public string? ManagerNote { get; set; }

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
        public Regulation Regulation { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;
        public ICollection<TestItemEngineer> TestItemEngineers { get; set; } = new List<TestItemEngineer>();
        public ICollection<TestItemRevision> Revisions { get; set; } = new List<TestItemRevision>();
        public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
