namespace VehicleRentalSystem.Domain.Entities
{
    public interface IInsurable
    {
        decimal CalculateInsuranceCost(int days);
        string GetInsuranceType();
    }
}
