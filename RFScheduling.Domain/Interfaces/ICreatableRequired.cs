namespace RFScheduling.Domain.Interfaces
{
    public interface ICreatableRequired
    {
        int CreatedByUserId { get; set; }

        DateTime CreatedDate { get; set; }
    }
}
