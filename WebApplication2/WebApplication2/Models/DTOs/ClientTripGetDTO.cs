namespace WebApplication2.Models.DTOs;

public class ClientTripGetDTO
{
    public int IdTrip { get; set; }
    public int IdClient { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}