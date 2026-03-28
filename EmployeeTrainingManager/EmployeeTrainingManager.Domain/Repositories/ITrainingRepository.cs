using EmployeeTrainingManager.Domain.Entities;

namespace EmployeeTrainingManager.Domain.Repositories
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<Training>> GetAllAsync();
        Task<Training?> GetByIdAsync(string id);
        Task<string> AddAsync(Training training);
        Task UpdateAsync(Training training);
        Task DeleteAsync(string id);
    }
}
