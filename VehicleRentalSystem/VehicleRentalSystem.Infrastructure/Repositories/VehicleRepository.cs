using Dapper;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Repositories;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public VehicleRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT Id, LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, 
                        NumberOfDoors, FuelType, EngineCapacity, HasSidecar, LoadCapacity, NumberOfAxles 
                        FROM Vehicles";
            
            var vehicles = await connection.QueryAsync<dynamic>(sql);
            return MapToVehicles(vehicles);
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT Id, LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, 
                        NumberOfDoors, FuelType, EngineCapacity, HasSidecar, LoadCapacity, NumberOfAxles 
                        FROM Vehicles WHERE Id = @Id";
            
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });
            if (result == null) return null;
            
            return MapToVehicle(result);
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT Id, LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, 
                        NumberOfDoors, FuelType, EngineCapacity, HasSidecar, LoadCapacity, NumberOfAxles 
                        FROM Vehicles WHERE IsAvailable = 1";
            
            var vehicles = await connection.QueryAsync<dynamic>(sql);
            return MapToVehicles(vehicles);
        }

        public async Task<int> AddAsync(Vehicle vehicle)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"INSERT INTO Vehicles (LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, 
                        NumberOfDoors, FuelType, EngineCapacity, HasSidecar, LoadCapacity, NumberOfAxles) 
                        VALUES (@LicensePlate, @Brand, @Model, @Year, @DailyRate, @IsAvailable, @VehicleType, 
                        @NumberOfDoors, @FuelType, @EngineCapacity, @HasSidecar, @LoadCapacity, @NumberOfAxles); 
                        SELECT CAST(SCOPE_IDENTITY() as int)";
            
            var parameters = CreateParameters(vehicle);
            return await connection.ExecuteScalarAsync<int>(sql, parameters);
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"UPDATE Vehicles SET LicensePlate = @LicensePlate, Brand = @Brand, Model = @Model, 
                        Year = @Year, DailyRate = @DailyRate, IsAvailable = @IsAvailable, VehicleType = @VehicleType,
                        NumberOfDoors = @NumberOfDoors, FuelType = @FuelType, EngineCapacity = @EngineCapacity, 
                        HasSidecar = @HasSidecar, LoadCapacity = @LoadCapacity, NumberOfAxles = @NumberOfAxles 
                        WHERE Id = @Id";
            
            var parameters = CreateParameters(vehicle);
            await connection.ExecuteAsync(sql, parameters);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Vehicles WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        private IEnumerable<Vehicle> MapToVehicles(IEnumerable<dynamic> results)
        {
            var vehicles = new List<Vehicle>();
            foreach (var result in results)
            {
                vehicles.Add(MapToVehicle(result));
            }
            return vehicles;
        }

        private Vehicle MapToVehicle(dynamic result)
        {
            string vehicleType = result.VehicleType;
            
            return vehicleType switch
            {
                "Car" => new Car
                {
                    Id = result.Id,
                    LicensePlate = result.LicensePlate,
                    Brand = result.Brand,
                    Model = result.Model,
                    Year = result.Year,
                    DailyRate = result.DailyRate,
                    IsAvailable = result.IsAvailable,
                    NumberOfDoors = result.NumberOfDoors ?? 0,
                    FuelType = result.FuelType ?? string.Empty
                },
                "Motorcycle" => new Motorcycle
                {
                    Id = result.Id,
                    LicensePlate = result.LicensePlate,
                    Brand = result.Brand,
                    Model = result.Model,
                    Year = result.Year,
                    DailyRate = result.DailyRate,
                    IsAvailable = result.IsAvailable,
                    EngineCapacity = result.EngineCapacity ?? 0,
                    HasSidecar = result.HasSidecar ?? false
                },
                "Truck" => new Truck
                {
                    Id = result.Id,
                    LicensePlate = result.LicensePlate,
                    Brand = result.Brand,
                    Model = result.Model,
                    Year = result.Year,
                    DailyRate = result.DailyRate,
                    IsAvailable = result.IsAvailable,
                    LoadCapacity = result.LoadCapacity ?? 0,
                    NumberOfAxles = result.NumberOfAxles ?? 0
                },
                _ => throw new InvalidOperationException($"Unknown vehicle type: {vehicleType}")
            };
        }

        private object CreateParameters(Vehicle vehicle)
        {
            return new
            {
                vehicle.Id,
                vehicle.LicensePlate,
                vehicle.Brand,
                vehicle.Model,
                vehicle.Year,
                vehicle.DailyRate,
                vehicle.IsAvailable,
                VehicleType = vehicle.GetVehicleType(),
                NumberOfDoors = (vehicle as Car)?.NumberOfDoors,
                FuelType = (vehicle as Car)?.FuelType,
                EngineCapacity = (vehicle as Motorcycle)?.EngineCapacity,
                HasSidecar = (vehicle as Motorcycle)?.HasSidecar,
                LoadCapacity = (vehicle as Truck)?.LoadCapacity,
                NumberOfAxles = (vehicle as Truck)?.NumberOfAxles
            };
        }
    }
}
