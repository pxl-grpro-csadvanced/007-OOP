using Dapper;
using System.Data;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interfaces;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class CustomerDbRepository : ICustomerRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public CustomerDbRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Task<int> AddAsync(Customer customer)
    {
        string sql = @"
            INSERT INTO Customers (Name, Email, PhoneNumber)
            VALUES (@Name, @Email, @PhoneNumber);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        using (IDbConnection connection = _connectionFactory.CreateConnection())
        {
            return connection.ExecuteScalarAsync<int>(sql, customer);
        }
    }

    public async Task DeleteAsync(int id)
    {
        string sql = "DELETE FROM Customers WHERE Id = @Id";

        using (IDbConnection connection = _connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        string sql = "SELECT * FROM Customers";
        using(IDbConnection connection = _connectionFactory.CreateConnection())
        {
            return await connection.QueryAsync<Customer>(sql);
        }
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        string sql = "SELECT * FROM Customers WHERE Id = @Id";
        using (IDbConnection connection = _connectionFactory.CreateConnection())
        {
            return await connection.QuerySingleOrDefaultAsync<Customer>(sql, new { Id = id });
        }
    }

    public Task UpdateAsync(Customer customer)
    {
        string sql = @"UPDATE Customers SET
            FirstName = @FirstName, LastName = @LastName, Email = @Email,
            PhoneNumber = @PhoneNumber, DriverLicenseNumber = @DriverLicenseNumber
            WHERE Id = @Id";

        using (IDbConnection connection = _connectionFactory.CreateConnection())
        {
            return connection.ExecuteAsync(sql, customer);
        }
    }
    
}
