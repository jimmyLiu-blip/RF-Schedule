namespace RFScheduling.Domain.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }

        int? DeletedByUserId { get; set; }

        DateTime? DeletedDate { get; set; }
    }
}
