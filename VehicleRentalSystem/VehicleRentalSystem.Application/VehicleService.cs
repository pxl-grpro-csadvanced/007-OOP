using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interfaces;

namespace VehicleRentalSystem.Application;

public class VehicleService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public Task<IEnumerable<Vehicle>> GetAllVehiclesAsync() =>
        _vehicleRepository.GetAllAsync();

    public Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync() =>
        _vehicleRepository.GetAvailableVehiclesAsync();

    public async Task<int> AddCarAsync(string licensePlate, string brand,
        string model, int year, decimal dailyRate, int numberOfDoors, string fuelType)
    {
        var car = new Car(0, licensePlate, brand, model, year, dailyRate, numberOfDoors, fuelType);
        return await _vehicleRepository.AddAsync(car);
    }

    public async Task<int> AddMotorcycleAsync(string licensePlate, string brand,
        string model, int year, decimal dailyRate, int engineCapacity, bool hasSidecar)
    {
        var motorcycle = new Motorcycle(0, licensePlate, brand, model, year, dailyRate, engineCapacity, hasSidecar);
        return await _vehicleRepository.AddAsync(motorcycle);
    }

    public async Task<int> AddTruckAsync(string licensePlate, string brand,
        string model, int year, decimal dailyRate, decimal loadCapacity, int numberOfAxles)
    {
        var truck = new Truck(0, licensePlate, brand, model, year, dailyRate, loadCapacity, numberOfAxles);
        return await _vehicleRepository.AddAsync(truck);
    }

    public Task DeleteVehicleAsync(int id) => _vehicleRepository.DeleteAsync(id);
}
