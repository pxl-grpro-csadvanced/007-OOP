# Vehicle Rental System - C# Advanced Exercise

## Overview
This is a comprehensive exercise demonstrating a **Vehicle Rental System** built with **Domain-Driven Design (DDD)** architecture and modern .NET practices.

## ?? Learning Objectives

This exercise covers the following advanced C# concepts:

### 1. **Inheritance**
- Abstract base class `Vehicle` with three concrete implementations:
  - `Car`
  - `Motorcycle`
  - `Truck`
- Demonstrates polymorphic behavior through abstract methods

### 2. **Interfaces**
- `IInsurable` interface implemented by `Car` and `Motorcycle`
- Repository interfaces (`IVehicleRepository`, `ICustomerRepository`, `IRentalRepository`)

### 3. **Polymorphism**
- Abstract methods in `Vehicle` class overridden by derived classes
- Different rental cost calculations based on vehicle type
- Different insurance calculations for insurable vehicles

### 4. **Abstract Classes**
- `Vehicle` abstract class with shared properties and abstract methods
- Forces derived classes to implement specific behaviors

### 5. **Dapper ORM**
- Lightweight ORM for database access
- Used in all repository implementations
- Demonstrates raw SQL with parameter binding

### 6. **SQL Client (Microsoft.Data.SqlClient)**
- Direct database connectivity
- Connection factory pattern for connection management

## ??? Architecture

The solution follows **Domain-Driven Design (DDD)** with clear layer separation:

```
VehicleRentalSystem/
??? VehicleRentalSystem.Domain/
?   ??? Entities/
?   ?   ??? Vehicle.cs (Abstract)
?   ?   ??? Car.cs
?   ?   ??? Motorcycle.cs
?   ?   ??? Truck.cs
?   ?   ??? Customer.cs
?   ?   ??? Rental.cs
?   ?   ??? IInsurable.cs (Interface)
?   ??? Repositories/ (Interfaces)
?       ??? IVehicleRepository.cs
?       ??? ICustomerRepository.cs
?       ??? IRentalRepository.cs
?
??? VehicleRentalSystem.Infrastructure/
?   ??? Data/
?   ?   ??? DbConnectionFactory.cs
?   ??? Repositories/ (Dapper implementations)
?       ??? VehicleRepository.cs
?       ??? CustomerRepository.cs
?       ??? RentalRepository.cs
?
??? VehicleRentalSystem.Application/
?   ??? Services/
?       ??? VehicleService.cs
?       ??? CustomerService.cs
?       ??? RentalService.cs
?
??? VehicleRentalSystem.Presentation/ (WPF)
    ??? MainWindow.xaml
    ??? MainWindow.xaml.cs
```

## ?? Domain Model

### Vehicle Hierarchy (Inheritance & Polymorphism)

```csharp
public abstract class Vehicle
{
    // Common properties
    public int Id { get; set; }
    public string LicensePlate { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal DailyRate { get; set; }
    public bool IsAvailable { get; set; }
    
    // Abstract methods (polymorphism)
    public abstract string GetVehicleType();
    public abstract decimal CalculateRentalCost(int days);
    public abstract string GetDetails();
}
```

### Car (Implements IInsurable)
- Properties: `NumberOfDoors`, `FuelType`
- Standard rental cost calculation
- Insurance cost: €15/day

### Motorcycle (Implements IInsurable)
- Properties: `EngineCapacity`, `HasSidecar`
- Rental cost increases 20% for engines > 600cc
- Insurance cost: €20/day (1.5x for engines > 600cc)

### Truck (No insurance)
- Properties: `LoadCapacity`, `NumberOfAxles`
- Additional €50/day for capacity > 5000kg
- No insurance available

### IInsurable Interface
```csharp
public interface IInsurable
{
    decimal CalculateInsuranceCost(int days);
    string GetInsuranceType();
}
```

## ??? Database Setup

### Prerequisites
- SQL Server or SQL Server Express
- Connection string configured in `MainWindow.xaml.cs`

### Setup Instructions

1. **Update the connection string** in `VehicleRentalSystem.Presentation/MainWindow.xaml.cs`:
   ```csharp
   var connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=VehicleRentalDb;Data Source=.\\sqlexpress;TrustServerCertificate=True";
   ```

2. **Run the database script**:
   - Open `DatabaseSetup.sql` in SQL Server Management Studio (SSMS)
   - Execute the script
   - This creates the database, tables, and sample data

### Database Schema

**Customers Table:**
- Id, FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber

**Vehicles Table** (Single Table Inheritance):
- Id, LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType
- Car fields: NumberOfDoors, FuelType
- Motorcycle fields: EngineCapacity, HasSidecar
- Truck fields: LoadCapacity, NumberOfAxles

**Rentals Table:**
- Id, CustomerId, VehicleId, StartDate, EndDate, TotalCost, IsActive

## ?? How to Run

1. **Open the solution** in Visual Studio 2022 or later
2. **Set up the database** using `DatabaseSetup.sql`
3. **Set `VehicleRentalSystem.Presentation` as startup project**
4. **Run the application** (F5)

## ?? Key Features

### Vehicle Management
- Add different types of vehicles (Car, Motorcycle, Truck)
- View all vehicles with their specific properties
- Delete vehicles
- Track availability status

### Customer Management
- Add new customers
- View customer details
- Delete customers

### Rental Management
- Create rentals by selecting customer and available vehicle
- Automatic cost calculation based on vehicle type
- Insurance cost calculation for insurable vehicles
- Complete rentals (marks vehicle as available)
- View active rentals

## ?? Important Concepts Demonstrated

### 1. Polymorphism in Action
Different vehicle types calculate rental costs differently:
```csharp
// Car: simple calculation
public override decimal CalculateRentalCost(int days) => DailyRate * days;

// Motorcycle: premium for powerful bikes
public override decimal CalculateRentalCost(int days)
{
    decimal cost = DailyRate * days;
    if (EngineCapacity > 600)
        cost *= 1.2m;
    return cost;
}

// Truck: extra charge for heavy loads
public override decimal CalculateRentalCost(int days)
{
    decimal cost = DailyRate * days;
    if (LoadCapacity > 5000)
        cost += 50m * days;
    return cost;
}
```

### 2. Interface Usage
The `IInsurable` interface allows polymorphic insurance handling:
```csharp
public decimal CalculateInsuranceCost(Vehicle vehicle, int days)
{
    if (vehicle is IInsurable insurable)
    {
        return insurable.CalculateInsuranceCost(days);
    }
    return 0m; // Trucks don't have insurance
}
```

### 3. Dapper ORM Pattern
Repositories use Dapper for clean data access:
```csharp
public async Task<IEnumerable<Customer>> GetAllAsync()
{
    using var connection = _connectionFactory.CreateConnection();
    var sql = "SELECT * FROM Customers";
    return await connection.QueryAsync<Customer>(sql);
}
```

### 4. Single Table Inheritance
All vehicle types stored in one table with type discriminator:
- `VehicleType` column determines the concrete type
- Type-specific fields are nullable
- Repository maps database records to correct C# types

## ?? Exercise Suggestions

### Beginner
1. Add a new method to display vehicle information
2. Create a new vehicle type (e.g., Van)
3. Add validation to prevent negative rental costs

### Intermediate
1. Implement a discount system for long-term rentals
2. Add vehicle maintenance tracking
3. Create a customer loyalty program
4. Add filtering and sorting to vehicle lists

### Advanced
1. Implement the Repository pattern with Unit of Work
2. Add async validation for overlapping rentals
3. Implement CQRS pattern
4. Add event sourcing for rental history
5. Implement soft delete for all entities

## ??? Technologies Used

- **.NET 9.0**
- **C# 13.0**
- **WPF** (Windows Presentation Foundation)
- **Dapper 2.1.72** (Micro ORM)
- **Microsoft.Data.SqlClient 7.0.0**
- **SQL Server**

## ?? Notes

- The solution demonstrates **SOLID principles**
- **Dependency Injection** is manually configured in the presentation layer
- **Async/await** pattern used throughout for database operations
- Repository pattern provides abstraction over data access
- Service layer contains business logic

## ?? Learning Path

1. **Study the Domain layer** - Understand inheritance and interfaces
2. **Examine the Infrastructure layer** - Learn Dapper and repository pattern
3. **Review the Application layer** - See how services orchestrate business logic
4. **Explore the Presentation layer** - WPF data binding and event handling
5. **Run and test** - Create rentals, observe polymorphic behavior
6. **Extend the system** - Add new features to solidify understanding

## ?? Additional Resources

- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [C# Inheritance](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/inheritance)
- [C# Interfaces](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces)
- [SOLID Principles](https://www.digitalocean.com/community/conceptual_articles/s-o-l-i-d-the-first-five-principles-of-object-oriented-design)

---

**Happy Learning! ??**
