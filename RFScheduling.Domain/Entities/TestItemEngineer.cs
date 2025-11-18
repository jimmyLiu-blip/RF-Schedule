using RFScheduling.Domain.Interfaces;
using RFScheduling.Domain.Enums;

namespace RFScheduling.Domain.Entities
{
    public class TestItemEngineer : ISoftDeletable, ICreatableRequired, IModifiable
    {
        public int TestItemEngineerId { get; set; }

        public int TestItemId { get; set; }

        public int EngineerUserId { get; set; }

        public RoleType RoleType { get; set; }

        public decimal AssignedHours { get; set; }

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

        public User Engineer { get; set; } = null!;

        public User CreatedBy { get; set; } = null!;

    }
}
