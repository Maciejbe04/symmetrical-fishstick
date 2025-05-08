using System.Runtime.InteropServices.JavaScript;
using Microsoft.Data.SqlClient;
using WebApplication2.Exceptions;
using WebApplication2.Models;
using WebApplication2.Models.DTOs;

namespace WebApplication2.Services;

public interface IDbService
{
    public Task<IEnumerable<TripGetDTO>> GetAllTripsAsync();
    public Task<IEnumerable<TripGetDTO>> GetTripsFromClientIdAsync(int id);
    public Task<Client> CreateNewClientAsync(ClientCreateDTO body);
    public Task<IEnumerable<ClientGetDTO>> GetAllClientsAsync();
    public Task DeleteClientFromTripAsync(int idClient, int idTrip);
    public Task RegisterClientAsync(int idClient, int idTrip);
}



public class DbService(IConfiguration configuration) : IDbService
{
    public async Task<IEnumerable<TripGetDTO>> GetAllTripsAsync()
    {
        var result = new List<TripGetDTO>();
        
        var connectionString = configuration.GetConnectionString("Default");

        await using var connection = new SqlConnection(connectionString);
        
        var sql = "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name FROM Trip t JOIN Country_Trip ct on t.IdTrip = ct.IdTrip JOIN Country c ON c.IdCountry = ct.IdCountry";

        await using var command = new SqlCommand(sql, connection);
        
        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new TripGetDTO
            {
                IdTrip = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5),
                Country = reader.GetString(6),
            });
        }

        return result;
    }
    public async Task<IEnumerable<ClientGetDTO>> GetAllClientsAsync()
    {
        var result = new List<ClientGetDTO>();
        
        var connectionString = configuration.GetConnectionString("Default");

        await using var connection = new SqlConnection(connectionString);
        
        var sql = "SELECT IdClient, FirstName, LastName, Email, Telephone, Pesel FROM Client";

        await using var command = new SqlCommand(sql, connection);
        
        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new ClientGetDTO
            {
                IdClient = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName= reader.GetString(2),
                Email = reader.GetString(3),
                Telephone = reader.GetString(4),
                Pesel = reader.GetString(5)
            });
        }

        return result;
    }

    

    public async Task<IEnumerable<TripGetDTO>> GetTripsFromClientIdAsync(int id)
    {
        var result = new List<TripGetDTO>();
        
        var connectionString = configuration.GetConnectionString("Default");
        await using var connection = new SqlConnection(connectionString);
        
        var sql = "SELECT 1 FROM CLIENT WHERE IdClient = @Id";
        
        await using var command = new SqlCommand(sql, connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        await connection.OpenAsync();
        
        var numOfRows = await command.ExecuteNonQueryAsync();

        if (numOfRows == 0)
        {
            throw new ClientNotFound($"Client with id {id} not found");
        }

        var sql2 =
            "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople FROM Trip t JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip WHERE ct.IdClient = @id;";
        
        
        await using var command2 = new SqlCommand(sql2, connection);
        
        command2.Parameters.AddWithValue("@id", id);
        
        await using var reader = await command2.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new TripGetDTO
            {
                IdTrip = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5)
            });
        }
        return result;
        
    }

    public async Task<Client> CreateNewClientAsync(ClientCreateDTO body)
    {
        var connectionString = configuration.GetConnectionString("Default");
        
        await using var connection = new SqlConnection(connectionString);
        
        var sql = "INSERT INTO CLIENT (FirstName,LastName, Email, Telephone, Pesel) VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel); Select scope_identity();";
        
        await using var command = new SqlCommand(sql, connection);
        
        command.Parameters.AddWithValue("@FirstName", body.FirstName);
        command.Parameters.AddWithValue("@LastName", body.LastName);
        command.Parameters.AddWithValue("@Email", body.Email);
        command.Parameters.AddWithValue("@Telephone", body.Telephone);
        command.Parameters.AddWithValue("@Pesel", body.Pesel);
        await connection.OpenAsync();
        var id = Convert.ToInt32(await command.ExecuteScalarAsync());

        return new Client()
        {
            IdClient = id,
            FirstName = body.FirstName,
            LastName = body.LastName,
            Email = body.Email,
            Telephone = body.Telephone,
            Pesel = body.Pesel
        };
        
    }
    public async Task DeleteClientFromTripAsync(int idClient, int idTrip)
    {
        var connectionString = configuration.GetConnectionString("Default");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var sql2 = "DELETE FROM Client_Trip WHERE IdClient = @idClient AND IdTrip = @idTrip;";
        await using var command2 = new SqlCommand(sql2, connection);
        command2.Parameters.AddWithValue("@idClient", idClient); 
        command2.Parameters.AddWithValue("@idTrip", idTrip);

        var numOfRows = await command2.ExecuteNonQueryAsync();

        if (numOfRows == 0)
        {
            throw new ClientTripNotFound($"Client with id {idClient} and trip id {idTrip} not found");
        }
        
    }

    public async Task RegisterClientAsync(int idClient, int idTrip)
    {
        var connectionString = configuration.GetConnectionString("Default");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        var sql = "SELECT 1 FROM Client WHERE IdClient = @idClient";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idClient", idClient);
        
        
        var numOfRows = await command.ExecuteScalarAsync();

        if (numOfRows == null){
            throw new ClientNotFound($"Client with id {idClient} not found");
        }
        
        var sql2 = "SELECT 1 FROM Trip WHERE IdTrip = @idTrip";
        
        await using var command2 = new SqlCommand(sql2, connection);
        command2.Parameters.AddWithValue("@idTrip", idTrip);
        
        
        var numOfRows2 = await command2.ExecuteScalarAsync();

        if (numOfRows2 == null)
        {
            throw new TripNotFound($"Trip with id {idTrip} not found");
        }
        
        var sql3 = "SELECT MaxPeople FROM Trip WHERE IdTrip = @idTrip;";
        await using var command3 = new SqlCommand(sql3, connection);
        command3.Parameters.AddWithValue("@idTrip", idTrip);
        
        var maxPeople = (int) await command3.ExecuteScalarAsync();
        
        var sql4 = "SELECT COUNT(*) FROM Client_Trip Where IdTrip = @idTrip;";
        await using var command4 = new SqlCommand(sql4, connection);
        command4.Parameters.AddWithValue("@idTrip", idTrip);
        
        var numOfRows3 = (int) await command4.ExecuteScalarAsync();

        if (numOfRows3 >= maxPeople)
        {
            throw new MaxPeopleException("Cant add another person because limit is full");
        }
        
        var date = int.Parse(DateTime.Today.ToString("yyyyMMdd"));

        var sql5 =
            "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate ) VALUES (@idClient, @idTrip, @date, null)";
        await using var command5 = new SqlCommand(sql5, connection);
        command5.Parameters.AddWithValue("@idClient", idClient);
        command5.Parameters.AddWithValue("@idTrip", idTrip);
        command5.Parameters.AddWithValue("@date", date);
        
        var numOfRows4 = await command5.ExecuteNonQueryAsync();

        if (numOfRows4 == 0)
        {
            throw new ClientTripNotFound($"Trip with id {idTrip} not found");
        }

    }
    
}