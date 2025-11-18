namespace RFScheduling.Domain.Interfaces
{
    public interface ICreatable
    {
        int CreatedByUserId { get; set; }

        DateTime CreatedDate { get; set; }
    }
}
