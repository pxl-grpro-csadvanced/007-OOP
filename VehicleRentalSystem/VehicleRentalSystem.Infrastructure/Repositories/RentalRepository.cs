using Dapper;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Repositories;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly DbConnectionFactory _connectionFactory;
        private readonly ICustomerRepository _customerRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public RentalRepository(DbConnectionFactory connectionFactory, ICustomerRepository customerRepository, IVehicleRepository vehicleRepository)
        {
            _connectionFactory = connectionFactory;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<IEnumerable<Rental>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, CustomerId, VehicleId, StartDate, EndDate, TotalCost, IsActive FROM Rentals";
            var rentals = await connection.QueryAsync<Rental>(sql);
            
            foreach (var rental in rentals)
            {
                rental.Customer = await _customerRepository.GetByIdAsync(rental.CustomerId);
                rental.Vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
            }
            
            return rentals;
        }

        public async Task<Rental?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, CustomerId, VehicleId, StartDate, EndDate, TotalCost, IsActive FROM Rentals WHERE Id = @Id";
            var rental = await connection.QueryFirstOrDefaultAsync<Rental>(sql, new { Id = id });
            
            if (rental != null)
            {
                rental.Customer = await _customerRepository.GetByIdAsync(rental.CustomerId);
                rental.Vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
            }
            
            return rental;
        }

        public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, CustomerId, VehicleId, StartDate, EndDate, TotalCost, IsActive FROM Rentals WHERE IsActive = 1";
            var rentals = await connection.QueryAsync<Rental>(sql);
            
            foreach (var rental in rentals)
            {
                rental.Customer = await _customerRepository.GetByIdAsync(rental.CustomerId);
                rental.Vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
            }
            
            return rentals;
        }

        public async Task<IEnumerable<Rental>> GetRentalsByCustomerIdAsync(int customerId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, CustomerId, VehicleId, StartDate, EndDate, TotalCost, IsActive FROM Rentals WHERE CustomerId = @CustomerId";
            var rentals = await connection.QueryAsync<Rental>(sql, new { CustomerId = customerId });
            
            foreach (var rental in rentals)
            {
                rental.Customer = await _customerRepository.GetByIdAsync(rental.CustomerId);
                rental.Vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
            }
            
            return rentals;
        }

        public async Task<int> AddAsync(Rental rental)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Rentals (CustomerId, VehicleId, StartDate, EndDate, TotalCost, IsActive) 
                        VALUES (@CustomerId, @VehicleId, @StartDate, @EndDate, @TotalCost, @IsActive); 
                        SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(sql, rental);
        }

        public async Task UpdateAsync(Rental rental)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Rentals SET CustomerId = @CustomerId, VehicleId = @VehicleId, 
                        StartDate = @StartDate, EndDate = @EndDate, TotalCost = @TotalCost, IsActive = @IsActive 
                        WHERE Id = @Id";
            await connection.ExecuteAsync(sql, rental);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Rentals WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
