namespace VehicleRentalSystem.Domain.Entities
{
    public class Motorcycle : Vehicle, IInsurable
    {
        public int EngineCapacity { get; set; }
        public bool HasSidecar { get; set; }

        public Motorcycle()
        {
        }

        public Motorcycle(int id, string licensePlate, string brand, string model, int year, decimal dailyRate, int engineCapacity, bool hasSidecar)
            : base(id, licensePlate, brand, model, year, dailyRate)
        {
            EngineCapacity = engineCapacity;
            HasSidecar = hasSidecar;
        }

        public override string GetVehicleType()
        {
            return "Motorcycle";
        }

        public override decimal CalculateRentalCost(int days)
        {
            decimal cost = DailyRate * days;
            if (EngineCapacity > 600)
            {
                cost *= 1.2m;
            }
            return cost;
        }

        public override string GetDetails()
        {
            var sidecarInfo = HasSidecar ? "with sidecar" : "no sidecar";
            return $"{Brand} {Model} ({Year}) - {EngineCapacity}cc, {sidecarInfo}";
        }

        public decimal CalculateInsuranceCost(int days)
        {
            decimal baseCost = 20m * days;
            if (EngineCapacity > 600)
            {
                baseCost *= 1.5m;
            }
            return baseCost;
        }

        public string GetInsuranceType()
        {
            return EngineCapacity > 600 ? "Premium Motorcycle Insurance" : "Standard Motorcycle Insurance";
        }

        public override string ToString()
        {
            return $"{LicensePlate} - {Brand} {Model} ({EngineCapacity}cc)";
        }
    }
}
