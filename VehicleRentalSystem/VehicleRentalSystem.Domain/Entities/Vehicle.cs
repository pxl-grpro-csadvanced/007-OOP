namespace VehicleRentalSystem.Domain.Entities;

public abstract class Vehicle : Object
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal DailyRate { get; set; }
    public bool IsAvailable { get; set; } = true;

    public string VehicleType { get => GetVehicleType(); }

    public abstract string GetVehicleType();
    public abstract decimal CalculateRentalCost(int days);
    public abstract string GetDetails();

    public override string ToString() => $"{Brand} {Model} ({Year}) - {LicensePlate}";
}
