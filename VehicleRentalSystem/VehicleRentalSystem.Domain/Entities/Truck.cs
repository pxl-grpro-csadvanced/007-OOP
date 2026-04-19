namespace VehicleRentalSystem.Domain.Entities
{
    public class Truck : Vehicle
    {
        public decimal LoadCapacity { get; set; }
        public int NumberOfAxles { get; set; }

        public Truck()
        {
        }

        public Truck(int id, string licensePlate, string brand, string model, int year, decimal dailyRate, decimal loadCapacity, int numberOfAxles)
            : base(id, licensePlate, brand, model, year, dailyRate)
        {
            LoadCapacity = loadCapacity;
            NumberOfAxles = numberOfAxles;
        }

        public override string GetVehicleType()
        {
            return "Truck";
        }

        public override decimal CalculateRentalCost(int days)
        {
            decimal cost = DailyRate * days;
            if (LoadCapacity > 5000)
            {
                cost += 50m * days;
            }
            return cost;
        }

        public override string GetDetails()
        {
            return $"{Brand} {Model} ({Year}) - Capacity: {LoadCapacity}kg, {NumberOfAxles} axles";
        }

        public override string ToString()
        {
            return $"{LicensePlate} - {Brand} {Model} ({LoadCapacity}kg)";
        }
    }
}
