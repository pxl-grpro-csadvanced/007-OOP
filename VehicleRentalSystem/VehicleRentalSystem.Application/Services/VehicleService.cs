using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Repositories;

namespace VehicleRentalSystem.Application.Services
{
    public class VehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _vehicleRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _vehicleRepository.GetAvailableVehiclesAsync();
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            return await _vehicleRepository.GetByIdAsync(id);
        }

        public async Task<int> AddCarAsync(string licensePlate, string brand, string model, int year, decimal dailyRate, int numberOfDoors, string fuelType)
        {
            var car = new Car(0, licensePlate, brand, model, year, dailyRate, numberOfDoors, fuelType);
            return await _vehicleRepository.AddAsync(car);
        }

        public async Task<int> AddMotorcycleAsync(string licensePlate, string brand, string model, int year, decimal dailyRate, int engineCapacity, bool hasSidecar)
        {
            var motorcycle = new Motorcycle(0, licensePlate, brand, model, year, dailyRate, engineCapacity, hasSidecar);
            return await _vehicleRepository.AddAsync(motorcycle);
        }

        public async Task<int> AddTruckAsync(string licensePlate, string brand, string model, int year, decimal dailyRate, decimal loadCapacity, int numberOfAxles)
        {
            var truck = new Truck(0, licensePlate, brand, model, year, dailyRate, loadCapacity, numberOfAxles);
            return await _vehicleRepository.AddAsync(truck);
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            await _vehicleRepository.UpdateAsync(vehicle);
        }

        public async Task DeleteVehicleAsync(int id)
        {
            await _vehicleRepository.DeleteAsync(id);
        }

        public async Task SetVehicleAvailabilityAsync(int id, bool isAvailable)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle != null)
            {
                vehicle.IsAvailable = isAvailable;
                await _vehicleRepository.UpdateAsync(vehicle);
            }
        }
    }
}
