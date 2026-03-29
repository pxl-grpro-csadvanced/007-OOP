using EmployeeTrainingManager.Domain.Entities;
using EmployeeTrainingManager.Infrastructure.Repositories;

namespace EmployeeTrainingManager.Application.Services
{
    public class EnrollmentService
    {
        private readonly EnrollmentRepository _enrollmentRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly TrainingRepository _trainingRepository;

        public EnrollmentService(EnrollmentRepository enrollmentRepository, EmployeeRepository employeeRepository, TrainingRepository trainingRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _employeeRepository = employeeRepository;
            _trainingRepository = trainingRepository;
        }

        public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
        {
            return await _enrollmentRepository.GetAllAsync();
        }

        public async Task<Enrollment?> GetEnrollmentByIdAsync(int id)
        {
            return await _enrollmentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByEmployeeIdAsync(int employeeId)
        {
            return await _enrollmentRepository.GetByEmployeeIdAsync(employeeId);
        }

        public async Task<int> EnrollEmployeeAsync(int employeeId, string trainingId, bool isBillable)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            var training = await _trainingRepository.GetByIdAsync(trainingId);
            if (employee == null || training == null)
                throw new InvalidOperationException("Employee or Training not found");
            var enrollment = new Enrollment(0, training, employee) { IsBillable = isBillable };
            return await _enrollmentRepository.AddAsync(enrollment);
        }

        public async Task DeleteEnrollmentAsync(int id)
        {
            await _enrollmentRepository.DeleteAsync(id);
        }
    }
}
