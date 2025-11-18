using RFScheduling.Domain.Interfaces;

namespace RFScheduling.Domain.Entities
{
    public class Regulation : ISoftDeletable, ICreatableRequired, IModifiable
    {
        public int RegulationId { get; set; }

        public int ProjectId { get; set; }

        public string RegulationName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Note {  get; set; }

        // ICreatable & IModifiable
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ISoftDeletable
        public bool IsDeleted { get; set; }
        public int? DeletedByUserId { get; set; }
        public DateTime? DeletedDate { get; set; }

        // Navigation Properties
        public Project Project { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;
        public ICollection<TestItem> TestItems { get; set; } = new List<TestItem>();
    }
}
