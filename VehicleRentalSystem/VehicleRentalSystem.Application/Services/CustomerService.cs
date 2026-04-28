using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interfaces;

namespace VehicleRentalSystem.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<int> AddCustomerAsync(string firstName, string lastName, string email, string phoneNumber, string driverLicenseNumber)
        {
            var customer = new Customer(0, firstName, lastName, email, phoneNumber, driverLicenseNumber);
            return await _customerRepository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(int id, string firstName, string lastName, string email, string phoneNumber, string driverLicenseNumber)
        {
            var customer = new Customer(id, firstName, lastName, email, phoneNumber, driverLicenseNumber);
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _customerRepository.DeleteAsync(id);
        }
    }
}
