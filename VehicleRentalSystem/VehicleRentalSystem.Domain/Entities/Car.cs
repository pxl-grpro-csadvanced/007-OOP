namespace VehicleRentalSystem.Domain.Entities
{
    public class Car : Vehicle, IInsurable
    {
        public int NumberOfDoors { get; set; }
        public string FuelType { get; set; }

        public Car()
        {
            FuelType = string.Empty;
        }

        public Car(int id, string licensePlate, string brand, string model, int year, decimal dailyRate, int numberOfDoors, string fuelType)
            : base(id, licensePlate, brand, model, year, dailyRate)
        {
            NumberOfDoors = numberOfDoors;
            FuelType = fuelType;
        }

        public override string GetVehicleType()
        {
            return "Car";
        }

        public override decimal CalculateRentalCost(int days)
        {
            return DailyRate * days;
        }

        public override string GetDetails()
        {
            return $"{Brand} {Model} ({Year}) - {NumberOfDoors} doors, {FuelType}";
        }

        public decimal CalculateInsuranceCost(int days)
        {
            return 15m * days;
        }

        public string GetInsuranceType()
        {
            return "Standard Car Insurance";
        }

        public override string ToString()
        {
            return $"{LicensePlate} - {Brand} {Model}";
        }
    }
}
