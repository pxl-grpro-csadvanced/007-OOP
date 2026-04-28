using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interfaces;

public interface IRentalRepository
{
    Task<IEnumerable<Rental>> GetAllAsync();
    Task<Rental?> GetByIdAsync(int id);
    Task<int> AddAsync(Rental rental);
    Task UpdateAsync(Rental rental);
    Task DeleteAsync(int id);
    Task<IEnumerable<Rental>> GetActiveRentalsAsync();
    Task<IEnumerable<Rental>> GetRentalsByCustomerIdAsync(int customerId);
}
