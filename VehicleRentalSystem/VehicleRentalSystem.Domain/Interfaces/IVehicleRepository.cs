using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interfaces;

public interface IVehicleRepository
{
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task<Vehicle?> GetByIdAsync(int id);
    Task<int> AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(int id);
    Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
}
