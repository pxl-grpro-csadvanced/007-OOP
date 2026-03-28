using Dapper;
using EmployeeTrainingManager.Domain.Entities;
using EmployeeTrainingManager.Domain.Repositories;
using EmployeeTrainingManager.Infrastructure.Data;

namespace EmployeeTrainingManager.Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly DbConnectionFactory _connectionFactory;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITrainingRepository _trainingRepository;

        public EnrollmentRepository(DbConnectionFactory connectionFactory, IEmployeeRepository employeeRepository, ITrainingRepository trainingRepository)
        {
            _connectionFactory = connectionFactory;
            _employeeRepository = employeeRepository;
            _trainingRepository = trainingRepository;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, EmployeeId, TrainingId, IsBillable FROM Enrollments";
            var enrollmentData = await connection.QueryAsync<dynamic>(sql);
            var enrollments = new List<Enrollment>();
            foreach (var data in enrollmentData)
            {
                var employee = await _employeeRepository.GetByIdAsync((int)data.EmployeeId);
                var training = await _trainingRepository.GetByIdAsync((string)data.TrainingId);
                if (employee != null && training != null)
                {
                    enrollments.Add(new Enrollment
                    {
                        Id = data.Id,
                        Employee = employee,
                        Training = training,
                        IsBillable = data.IsBillable
                    });
                }
            }
            return enrollments;
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, EmployeeId, TrainingId, IsBillable FROM Enrollments WHERE Id = @Id";
            var data = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });
            if (data == null) return null;
            var employee = await _employeeRepository.GetByIdAsync((int)data.EmployeeId);
            var training = await _trainingRepository.GetByIdAsync((string)data.TrainingId);
            if (employee == null || training == null) return null;
            return new Enrollment
            {
                Id = data.Id,
                Employee = employee,
                Training = training,
                IsBillable = data.IsBillable
            };
        }

        public async Task<IEnumerable<Enrollment>> GetByEmployeeIdAsync(int employeeId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, EmployeeId, TrainingId, IsBillable FROM Enrollments WHERE EmployeeId = @EmployeeId";
            var enrollmentData = await connection.QueryAsync<dynamic>(sql, new { EmployeeId = employeeId });
            var enrollments = new List<Enrollment>();
            foreach (var data in enrollmentData)
            {
                var employee = await _employeeRepository.GetByIdAsync((int)data.EmployeeId);
                var training = await _trainingRepository.GetByIdAsync((string)data.TrainingId);
                if (employee != null && training != null)
                {
                    enrollments.Add(new Enrollment
                    {
                        Id = data.Id,
                        Employee = employee,
                        Training = training,
                        IsBillable = data.IsBillable
                    });
                }
            }
            return enrollments;
        }

        public async Task<int> AddAsync(Enrollment enrollment)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "INSERT INTO Enrollments (EmployeeId, TrainingId, IsBillable) VALUES (@EmployeeId, @TrainingId, @IsBillable); SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(sql, new { EmployeeId = enrollment.Employee.Id, TrainingId = enrollment.Training.Id, enrollment.IsBillable });
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Enrollments SET EmployeeId = @EmployeeId, TrainingId = @TrainingId, IsBillable = @IsBillable WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { enrollment.Id, EmployeeId = enrollment.Employee.Id, TrainingId = enrollment.Training.Id, enrollment.IsBillable });
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Enrollments WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
