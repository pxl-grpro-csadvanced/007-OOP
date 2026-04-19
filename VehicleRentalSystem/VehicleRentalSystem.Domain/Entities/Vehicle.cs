namespace VehicleRentalSystem.Domain.Entities
{
    public abstract class Vehicle
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsAvailable { get; set; }

        protected Vehicle()
        {
            LicensePlate = string.Empty;
            Brand = string.Empty;
            Model = string.Empty;
            IsAvailable = true;
        }

        protected Vehicle(int id, string licensePlate, string brand, string model, int year, decimal dailyRate)
        {
            Id = id;
            LicensePlate = licensePlate;
            Brand = brand;
            Model = model;
            Year = year;
            DailyRate = dailyRate;
            IsAvailable = true;
        }

        public abstract string GetVehicleType();
        public abstract decimal CalculateRentalCost(int days);
        public abstract string GetDetails();
    }
}
