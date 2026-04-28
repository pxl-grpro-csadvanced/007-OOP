using VehicleRentalSystem.Domain.Interfaces;

namespace VehicleRentalSystem.Domain.Entities;

public class Car : Vehicle, IInsurable
{
    public int NumberOfDoors { get; set; }
    public string FuelType { get; set; } = string.Empty;

    public Car() { }

    public Car(int id, string licensePlate, string brand, string model, int year,
        decimal dailyRate, int numberOfDoors, string fuelType)
    {
        Id = id;
        LicensePlate = licensePlate;
        Brand = brand;
        Model = model;
        Year = year;
        DailyRate = dailyRate;
        IsAvailable = true;
        NumberOfDoors = numberOfDoors;
        FuelType = fuelType;
    }

    public override string GetVehicleType() => "Car";

    public override decimal CalculateRentalCost(int days) => DailyRate * days;

    public override string GetDetails() =>
        $"{Brand} {Model} ({Year}) - {NumberOfDoors} doors, {FuelType}";

    public decimal CalculateInsuranceCost(int days) => 15m * days;

    public string GetInsuranceType() => "Standard Car Insurance";
}
