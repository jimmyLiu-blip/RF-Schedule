using RFScheduling.Domain.Interfaces;
using RFScheduling.Domain.Enums;

namespace RFScheduling.Domain.Entities
{
    public class TestItem : ISoftDeletable, ICreatableRequired, IModifiable
    {
        public int TestItemId { get; set; }
        
        public int RegulationId { get; set; }

        public string TestItemName { get; set; } = string.Empty;

        public string TestType {  get; set; } = string.Empty ;

        public string TestLocation {  get; set; } = string.Empty ;

        public decimal EstimateHours { get; set; }

        public TestItemStatus Status { get; set; } = TestItemStatus.NotStarted ;

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
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Navigation Properties
        public Regulation Regulation { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;
        public ICollection<TestItemEngineer> TestItemEngineers { get; set; } = new List<TestItemEngineer>();
        public ICollection<TestItemRevision> Revisions { get; set; } = new List<TestItemRevision>();
        public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
