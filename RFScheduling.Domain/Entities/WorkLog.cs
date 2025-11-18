using RFScheduling.Domain.Interfaces;
using RFScheduling.Domain.Enums;

// 檢查要多注意
namespace RFScheduling.Domain.Entities
{
    public class WorkLog : ICreatable, IModifiable
    {
        public int WorkLogId { get; set; }

        public int TestItemId { get; set; }

        public int? RevisionId { get; set; }

        public int EngineerUserId { get; set; }

        public DateTime WorkDate { get; set; }

        public decimal ActualHours { get; set; }

        public WorkLogStatus Status { get; set; }

        public string? Comment { get; set; }

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModificationReason { get; set; }

        // Navigation Properties

        public TestItem TestItem { get; set; } = null!;

        public TestItemRevision? TestItemRevision { get; set; }

        public User Engineer { get; set; } = null!;

        public ICollection<WorkLogDelayReason> WorkLogDelayReasons {  get; set; } = new List<WorkLogDelayReason>();
    }
}
