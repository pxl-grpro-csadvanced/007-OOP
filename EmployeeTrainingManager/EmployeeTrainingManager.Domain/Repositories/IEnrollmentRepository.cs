using EmployeeTrainingManager.Domain.Entities;

namespace EmployeeTrainingManager.Domain.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int id);
        Task<IEnumerable<Enrollment>> GetByEmployeeIdAsync(int employeeId);
        Task<int> AddAsync(Enrollment enrollment);
        Task UpdateAsync(Enrollment enrollment);
        Task DeleteAsync(int id);
    }
}
