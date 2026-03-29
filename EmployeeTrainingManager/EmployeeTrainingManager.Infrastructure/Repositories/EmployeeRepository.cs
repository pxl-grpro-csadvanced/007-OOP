using Dapper;
using EmployeeTrainingManager.Domain.Entities;
using EmployeeTrainingManager.Infrastructure.Data;

namespace EmployeeTrainingManager.Infrastructure.Repositories
{
    public class EmployeeRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public EmployeeRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, FirstName, LastName FROM Employees";
            return await connection.QueryAsync<Employee>(sql);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, FirstName, LastName FROM Employees WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "INSERT INTO Employees (FirstName, LastName) VALUES (@FirstName, @LastName); SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(sql, employee);
        }

        public async Task UpdateAsync(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Employees SET FirstName = @FirstName, LastName = @LastName WHERE Id = @Id";
            await connection.ExecuteAsync(sql, employee);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Employees WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
