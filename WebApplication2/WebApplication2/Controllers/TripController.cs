using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication2.Exceptions;
using WebApplication2.Models.DTOs;
using WebApplication2.Services;

namespace WebApplication2.Controllers;
using System.Data.SqlClient;



[ApiController]
[Route("trips")]
public class TripController(IDbService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllTrips()
    {
        return Ok(await service.GetAllTripsAsync());
    }

    
}