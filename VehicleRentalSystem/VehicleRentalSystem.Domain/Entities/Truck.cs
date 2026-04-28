namespace VehicleRentalSystem.Domain.Entities;

public class Truck : Vehicle
{
    public decimal LoadCapacity { get; set; }
    public int NumberOfAxles { get; set; }

    public Truck() { }

    public Truck(int id, string licensePlate, string brand, string model, int year,
        decimal dailyRate, decimal loadCapacity, int numberOfAxles)
    {
        Id = id;
        LicensePlate = licensePlate;
        Brand = brand;
        Model = model;
        Year = year;
        DailyRate = dailyRate;
        IsAvailable = true;
        LoadCapacity = loadCapacity;
        NumberOfAxles = numberOfAxles;
    }

    public override string GetVehicleType() => "Truck";

    public override decimal CalculateRentalCost(int days)
    {
        decimal cost = DailyRate * days;
        if (LoadCapacity > 5000)
            cost += 50m * days;
        return cost;
    }

    public override string GetDetails() =>
        $"{Brand} {Model} ({Year}) - Capacity: {LoadCapacity}kg, {NumberOfAxles} axles";
}
