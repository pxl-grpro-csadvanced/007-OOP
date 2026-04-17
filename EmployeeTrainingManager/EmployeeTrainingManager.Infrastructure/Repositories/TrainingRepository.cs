using Dapper;
using EmployeeTrainingManager.Domain.Entities;
using EmployeeTrainingManager.Domain.Repositories;
using EmployeeTrainingManager.Infrastructure.Data;

namespace EmployeeTrainingManager.Infrastructure.Repositories
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TrainingRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Training>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, Title, TrainerName, DurationInHours, TrainingType, Technology, RiskLevel FROM Trainings";
            var trainings = await connection.QueryAsync<dynamic>(sql);
            return trainings.Select(MapToTraining);
        }

        public async Task<Training?> GetByIdAsync(string id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT Id, Title, TrainerName, DurationInHours, TrainingType, Technology, RiskLevel FROM Trainings WHERE Id = @Id";
            var training = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });
            return training != null ? MapToTraining(training) : null;
        }

        public async Task<string> AddAsync(Training training)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = training is TechnicalTraining ?
                "INSERT INTO Trainings (Id, Title, TrainerName, DurationInHours, TrainingType, Technology) VALUES (@Id, @Title, @TrainerName, @DurationInHours, 'Technical', @Technology)" :
                "INSERT INTO Trainings (Id, Title, TrainerName, DurationInHours, TrainingType, RiskLevel) VALUES (@Id, @Title, @TrainerName, @DurationInHours, 'Safety', @RiskLevel)";
            
            var parameters = training is TechnicalTraining tech ?
                new { tech.Id, tech.Title, tech.TrainerName, tech.DurationInHours, tech.Technology } :
                new { training.Id, training.Title, training.TrainerName, training.DurationInHours, RiskLevel = ((SafetyTraining)training).RiskLevel } as object;
            
            await connection.ExecuteAsync(sql, parameters);
            return training.Id;
        }

        public async Task UpdateAsync(Training training)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = training is TechnicalTraining ?
                "UPDATE Trainings SET Title = @Title, TrainerName = @TrainerName, DurationInHours = @DurationInHours, Technology = @Technology WHERE Id = @Id" :
                "UPDATE Trainings SET Title = @Title, TrainerName = @TrainerName, DurationInHours = @DurationInHours, RiskLevel = @RiskLevel WHERE Id = @Id";
            
            var parameters = training is TechnicalTraining tech ?
                new { tech.Id, tech.Title, tech.TrainerName, tech.DurationInHours, tech.Technology } :
                new { training.Id, training.Title, training.TrainerName, training.DurationInHours, RiskLevel = ((SafetyTraining)training).RiskLevel } as object;
            
            await connection.ExecuteAsync(sql, parameters);
        }

        public async Task DeleteAsync(string id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Trainings WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        private Training MapToTraining(dynamic row)
        {
            string trainingType = row.TrainingType;
            return trainingType == "Technical" ?
                new TechnicalTraining
                {
                    Id = row.Id,
                    Title = row.Title,
                    TrainerName = row.TrainerName,
                    DurationInHours = row.DurationInHours,
                    Technology = row.Technology ?? string.Empty
                } :
                new SafetyTraining
                {
                    Id = row.Id,
                    Title = row.Title,
                    TrainerName = row.TrainerName,
                    DurationInHours = row.DurationInHours,
                    RiskLevel = row.RiskLevel ?? string.Empty
                };
        }
    }
}
