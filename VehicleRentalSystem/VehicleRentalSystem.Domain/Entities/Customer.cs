namespace VehicleRentalSystem.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DriverLicenseNumber { get; set; } = string.Empty;

    public Customer() { }

    public Customer(int id, string firstName, string lastName, string email,
        string phoneNumber, string driverLicenseNumber)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DriverLicenseNumber = driverLicenseNumber;
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public override string ToString() => GetFullName();
}
