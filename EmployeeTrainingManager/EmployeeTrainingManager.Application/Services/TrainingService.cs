using EmployeeTrainingManager.Domain.Entities;
using EmployeeTrainingManager.Infrastructure.Repositories;

namespace EmployeeTrainingManager.Application.Services
{
    public class TrainingService
    {
        private readonly TrainingRepository _trainingRepository;

        public TrainingService(TrainingRepository trainingRepository)
        {
            _trainingRepository = trainingRepository;
        }

        public async Task<IEnumerable<Training>> GetAllTrainingsAsync()
        {
            return await _trainingRepository.GetAllAsync();
        }

        public async Task<Training?> GetTrainingByIdAsync(string id)
        {
            return await _trainingRepository.GetByIdAsync(id);
        }

        public async Task<string> AddTechnicalTrainingAsync(string id, string title, string trainerName, string technology)
        {
            var training = new TechnicalTraining(id, title, trainerName, technology);
            return await _trainingRepository.AddAsync(training);
        }

        public async Task<string> AddSafetyTrainingAsync(string id, string title, string trainerName, int duration, string riskLevel)
        {
            var training = new SafetyTraining(id, title, trainerName, duration, riskLevel);
            return await _trainingRepository.AddAsync(training);
        }

        public async Task UpdateTrainingAsync(Training training)
        {
            await _trainingRepository.UpdateAsync(training);
        }

        public async Task DeleteTrainingAsync(string id)
        {
            await _trainingRepository.DeleteAsync(id);
        }
    }
}
