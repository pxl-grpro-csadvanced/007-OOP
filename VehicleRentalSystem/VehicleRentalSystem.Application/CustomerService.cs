using System.Reflection.Metadata.Ecma335;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interfaces;

namespace VehicleRentalSystem.Application;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers;
    }

    public async Task<int> AddCustomerAsync(string firstName, string lastName,
        string email, string phoneNumber, string driverLicenseNumber)
    {
        var customer = new Customer(0, firstName, lastName, email, phoneNumber, driverLicenseNumber);
        return await _customerRepository.AddAsync(customer);
    }

    public Task DeleteCustomerAsync(int id) => _customerRepository.DeleteAsync(id);
}
