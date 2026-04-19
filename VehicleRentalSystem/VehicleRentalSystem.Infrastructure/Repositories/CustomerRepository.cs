using Dapper;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Repositories;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public CustomerRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber FROM Customers";
            return await connection.QueryAsync<Customer>(sql);
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber FROM Customers WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(Customer customer)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber) 
                        VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DriverLicenseNumber); 
                        SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(sql, customer);
        }

        public async Task UpdateAsync(Customer customer)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Customers SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                        PhoneNumber = @PhoneNumber, DriverLicenseNumber = @DriverLicenseNumber WHERE Id = @Id";
            await connection.ExecuteAsync(sql, customer);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Customers WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
