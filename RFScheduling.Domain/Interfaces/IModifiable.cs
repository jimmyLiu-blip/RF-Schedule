namespace RFScheduling.Domain.Interfaces
{
    public interface IModifiable
    {
        int? ModifiedByUserId { get; set; }
        DateTime? ModifiedDate { get; set; }
    }
}
