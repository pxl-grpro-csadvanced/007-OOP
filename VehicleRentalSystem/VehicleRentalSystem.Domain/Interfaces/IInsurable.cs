namespace VehicleRentalSystem.Domain.Interfaces;

public interface IInsurable
{
    decimal CalculateInsuranceCost(int days);
    string GetInsuranceType();
}
