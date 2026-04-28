using Dapper;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interfaces;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly DbConnectionFactory _connectionFactory;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public RentalRepository(DbConnectionFactory connectionFactory,
        ICustomerRepository customerRepository, IVehicleRepository vehicleRepository)
    {
        _connectionFactory = connectionFactory;
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        const string sql = "SELECT * FROM Rentals";
        using var connection = _connectionFactory.CreateConnection();
        var rentals = (await connection.QueryAsync<Rental>(sql)).ToList();
        await HydrateRentalsAsync(rentals);
        return rentals;
    }

    public async Task<Rental?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Rentals WHERE Id = @Id";
        using var connection = _connectionFactory.CreateConnection();
        var rental = await connection.QuerySingleOrDefaultAsync<Rental>(sql, new { Id = id });
        if (rental != null)
            await HydrateRentalsAsync([rental]);
        return rental;
    }

    public async Task<int> AddAsync(Rental rental)
    {
        const string sql = @"INSERT INTO Rentals
            (CustomerId, VehicleId, StartDate, EndDate, TotalCost, IsActive)
            VALUES (@CustomerId, @VehicleId, @StartDate, @EndDate, @TotalCost, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS INT)";
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, rental);
    }

    public async Task UpdateAsync(Rental rental)
    {
        const string sql = @"UPDATE Rentals SET
            CustomerId = @CustomerId, VehicleId = @VehicleId,
            StartDate = @StartDate, EndDate = @EndDate,
            TotalCost = @TotalCost, IsActive = @IsActive
            WHERE Id = @Id";
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, rental);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Rentals WHERE Id = @Id";
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
    {
        const string sql = "SELECT * FROM Rentals WHERE IsActive = 1";
        using var connection = _connectionFactory.CreateConnection();
        var rentals = (await connection.QueryAsync<Rental>(sql)).ToList();
        await HydrateRentalsAsync(rentals);
        return rentals;
    }

    public async Task<IEnumerable<Rental>> GetRentalsByCustomerIdAsync(int customerId)
    {
        const string sql = "SELECT * FROM Rentals WHERE CustomerId = @CustomerId";
        using var connection = _connectionFactory.CreateConnection();
        var rentals = (await connection.QueryAsync<Rental>(sql, new { CustomerId = customerId })).ToList();
        await HydrateRentalsAsync(rentals);
        return rentals;
    }

    private async Task HydrateRentalsAsync(List<Rental> rentals)
    {
        foreach (var rental in rentals)
        {
            rental.Customer = await _customerRepository.GetByIdAsync(rental.CustomerId);
            rental.Vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
        }
    }
}
