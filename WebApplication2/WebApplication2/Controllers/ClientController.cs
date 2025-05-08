using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication2.Exceptions;
using WebApplication2.Models.DTOs;
using WebApplication2.Services;

namespace WebApplication2.Controllers;


[ApiController]
[Route("clients")]

public class ClientController(IDbService service) : ControllerBase
{
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTripsFromClientId([FromRoute] int id)
    {
        try
        {
            return Ok(await service.GetTripsFromClientIdAsync(id));
        }
        catch (ClientNotFound ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        return Ok(await service.GetAllClientsAsync());
    }
    

    [HttpPost]
    public async Task<IActionResult> CreateNewClient([FromBody] ClientCreateDTO body)
    {
        var client = await service.CreateNewClientAsync(body);
        return Created($"client/{client.IdClient}", client);
    }

    [HttpDelete("{idClient}/trips/{tripId}")]
    public async Task<IActionResult> DeleteClient([FromRoute] int idClient, int tripId)
    {
        try
        {
            await service.DeleteClientFromTripAsync(idClient, tripId);
            return NoContent();
        }
        catch (ClientTripNotFound ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{idClient}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClient([FromRoute] int idClient, [FromRoute] int tripId)
    {
        try
        {
            await service.RegisterClientAsync(idClient, tripId);
            return NoContent();
        }
        catch (ClientTripNotFound ex)
        {
            return NotFound(ex.Message);
        }
        catch (TripNotFound ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    
    
    
    
}