namespace WebApplication2.Models;

public class Client_Trip
{
    public int IdTrip { get; set; }
    public int IdClient { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}