using EmployeeTrainingManager.Domain.Entities;
using EmployeeTrainingManager.Domain.Repositories;

namespace EmployeeTrainingManager.Application.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<int> AddEmployeeAsync(string firstName, string lastName)
        {
            var employee = new Employee(0, firstName, lastName);
            return await _employeeRepository.AddAsync(employee);
        }

        public async Task UpdateEmployeeAsync(int id, string firstName, string lastName)
        {
            var employee = new Employee(id, firstName, lastName);
            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await _employeeRepository.DeleteAsync(id);
        }
    }
}
