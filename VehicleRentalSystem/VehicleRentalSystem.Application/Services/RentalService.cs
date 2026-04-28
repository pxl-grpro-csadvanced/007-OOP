using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interfaces;

namespace VehicleRentalSystem.Application.Services
{
    public class RentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICustomerRepository _customerRepository;

        public RentalService(IRentalRepository rentalRepository, IVehicleRepository vehicleRepository, ICustomerRepository customerRepository)
        {
            _rentalRepository = rentalRepository;
            _vehicleRepository = vehicleRepository;
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Rental>> GetAllRentalsAsync()
        {
            return await _rentalRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
        {
            return await _rentalRepository.GetActiveRentalsAsync();
        }

        public async Task<IEnumerable<Rental>> GetRentalsByCustomerIdAsync(int customerId)
        {
            return await _rentalRepository.GetRentalsByCustomerIdAsync(customerId);
        }

        public async Task<Rental?> GetRentalByIdAsync(int id)
        {
            return await _rentalRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateRentalAsync(int customerId, int vehicleId, DateTime startDate, DateTime endDate)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found");
            }

            Vehicle vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                throw new InvalidOperationException("Vehicle not found");
            }

            if (!vehicle.IsAvailable)
            {
                throw new InvalidOperationException("Vehicle is not available for rental");
            }

            int days = (endDate - startDate).Days;
            if (days <= 0)
            {
                throw new InvalidOperationException("Invalid rental period");
            }

            decimal totalCost = vehicle.CalculateRentalCost(days);

            var rental = new Rental(0, customerId, vehicleId, startDate, endDate, totalCost);
            var rentalId = await _rentalRepository.AddAsync(rental);

            vehicle.IsAvailable = false;
            await _vehicleRepository.UpdateAsync(vehicle);

            return rentalId;
        }

        public async Task CompleteRentalAsync(int rentalId)
        {
            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            if (rental == null)
            {
                throw new InvalidOperationException("Rental not found");
            }

            rental.IsActive = false;
            await _rentalRepository.UpdateAsync(rental);

            var vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
            if (vehicle != null)
            {
                vehicle.IsAvailable = true;
                await _vehicleRepository.UpdateAsync(vehicle);
            }
        }

        public async Task DeleteRentalAsync(int id)
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental != null)
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
                if (vehicle != null)
                {
                    vehicle.IsAvailable = true;
                    await _vehicleRepository.UpdateAsync(vehicle);
                }
            }

            await _rentalRepository.DeleteAsync(id);
        }

        public decimal CalculateInsuranceCost(Vehicle vehicle, int days)
        {
            if (vehicle is IInsurable insurable)
            {
                return insurable.CalculateInsuranceCost(days);
            }
            return 0m;
        }
    }
}
