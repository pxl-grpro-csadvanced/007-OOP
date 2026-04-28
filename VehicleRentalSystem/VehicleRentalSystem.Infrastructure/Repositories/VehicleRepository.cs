using Dapper;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interfaces;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public VehicleRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        const string sql = "SELECT * FROM Vehicles";
        using (var connection = _connectionFactory.CreateConnection())
        {
            var results = await connection.QueryAsync<dynamic>(sql);
            return results.Select(MapToVehicle);
        }
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Vehicles WHERE Id = @Id";
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { Id = id });
        return result == null ? null : MapToVehicle(result);
    }

    public async Task<int> AddAsync(Vehicle vehicle)
    {
        using var connection = _connectionFactory.CreateConnection();

        if (vehicle is Car car)
        {
            const string sql = @"INSERT INTO Vehicles
                (LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, NumberOfDoors, FuelType)
                VALUES (@LicensePlate, @Brand, @Model, @Year, @DailyRate, @IsAvailable, @VehicleType, @NumberOfDoors, @FuelType);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                car.LicensePlate, car.Brand, car.Model, car.Year,
                car.DailyRate, car.IsAvailable,
                VehicleType = "Car",
                car.NumberOfDoors, car.FuelType
            });
        }
        else if (vehicle is Motorcycle moto)
        {
            const string sql = @"INSERT INTO Vehicles
                (LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, EngineCapacity, HasSidecar)
                VALUES (@LicensePlate, @Brand, @Model, @Year, @DailyRate, @IsAvailable, @VehicleType, @EngineCapacity, @HasSidecar);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                moto.LicensePlate, moto.Brand, moto.Model, moto.Year,
                moto.DailyRate, moto.IsAvailable,
                VehicleType = "Motorcycle",
                moto.EngineCapacity, moto.HasSidecar
            });
        }
        else if (vehicle is Truck truck)
        {
            const string sql = @"INSERT INTO Vehicles
                (LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, LoadCapacity, NumberOfAxles)
                VALUES (@LicensePlate, @Brand, @Model, @Year, @DailyRate, @IsAvailable, @VehicleType, @LoadCapacity, @NumberOfAxles);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                truck.LicensePlate, truck.Brand, truck.Model, truck.Year,
                truck.DailyRate, truck.IsAvailable,
                VehicleType = "Truck",
                truck.LoadCapacity, truck.NumberOfAxles
            });
        }

        throw new InvalidOperationException($"Unknown vehicle type: {vehicle.GetType().Name}");
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        const string sql = @"UPDATE Vehicles SET
            LicensePlate = @LicensePlate, Brand = @Brand, Model = @Model,
            Year = @Year, DailyRate = @DailyRate, IsAvailable = @IsAvailable
            WHERE Id = @Id";
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            vehicle.Id, vehicle.LicensePlate, vehicle.Brand, vehicle.Model,
            vehicle.Year, vehicle.DailyRate, vehicle.IsAvailable
        });
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Vehicles WHERE Id = @Id";
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
    {
        const string sql = "SELECT * FROM Vehicles WHERE IsAvailable = 1";
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync(sql);
        return results.Select(MapToVehicle);
    }

    private static Vehicle MapToVehicle(dynamic row)
    {
        string vehicleType = row.VehicleType;

         switch (vehicleType)
        {
            case "Car":
                return new Car
                {
                    Id = row.Id,
                    LicensePlate = row.LicensePlate,
                    Brand = row.Brand,
                    Model = row.Model,
                    Year = row.Year,
                    DailyRate = row.DailyRate,
                    IsAvailable = row.IsAvailable,
                    NumberOfDoors = row.NumberOfDoors ?? 0,
                    FuelType = row.FuelType ?? string.Empty
                };

            case "Motorcycle":
                return new Motorcycle
                {
                    Id = row.Id,
                    LicensePlate = row.LicensePlate,
                    Brand = row.Brand,
                    Model = row.Model,
                    Year = row.Year,
                    DailyRate = row.DailyRate,
                    IsAvailable = row.IsAvailable,
                    EngineCapacity = row.EngineCapacity ?? 0,
                    HasSidecar = row.HasSidecar ?? false
                };

            case "Truck":
                return new Truck
                {
                    Id = row.Id,
                    LicensePlate = row.LicensePlate,
                    Brand = row.Brand,
                    Model = row.Model,
                    Year = row.Year,
                    DailyRate = row.DailyRate,
                    IsAvailable = row.IsAvailable,
                    LoadCapacity = row.LoadCapacity ?? 0m,
                    NumberOfAxles = row.NumberOfAxles ?? 0
                };

            default:
                throw new InvalidOperationException($"Unknown vehicle type: {vehicleType}");
                
        };
    }
}
