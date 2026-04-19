# Vehicle Rental System - Project Summary

## ? Project Created Successfully!

A complete **Vehicle Rental System** exercise has been created in:
```
C:\CourseMaterial\oplossingen-cs-advanced\07-OOP\VehicleRentalSystem\
```

---

## ?? Solution Structure

### ?? VehicleRentalSystem.Domain (7 entities + 3 repository interfaces)
**Entities:**
- `Vehicle.cs` - Abstract base class with polymorphic methods
- `Car.cs` - Implements IInsurable interface
- `Motorcycle.cs` - Implements IInsurable interface
- `Truck.cs` - Does not implement IInsurable
- `Customer.cs` - Customer entity
- `Rental.cs` - Rental relationship entity
- `IInsurable.cs` - Interface for insurance capability

**Repository Interfaces:**
- `IVehicleRepository.cs`
- `ICustomerRepository.cs`
- `IRentalRepository.cs`

### ??? VehicleRentalSystem.Infrastructure (4 files)
**Data:**
- `DbConnectionFactory.cs` - SQL connection factory using Microsoft.Data.SqlClient

**Repositories (Dapper implementations):**
- `VehicleRepository.cs` - Handles polymorphic vehicle mapping
- `CustomerRepository.cs` - Basic CRUD operations
- `RentalRepository.cs` - Rental management with relationships

### ?? VehicleRentalSystem.Application (3 services)
**Services:**
- `VehicleService.cs` - Vehicle business logic
- `CustomerService.cs` - Customer business logic
- `RentalService.cs` - Rental business logic with polymorphic calculations

### ??? VehicleRentalSystem.Presentation (WPF Application)
**UI Files:**
- `App.xaml` + `App.xaml.cs` - Application entry point
- `MainWindow.xaml` - UI with 3 tabs (Vehicles, Customers, Rentals)
- `MainWindow.xaml.cs` - Event handlers and UI logic
- `AssemblyInfo.cs` - Assembly configuration

---

## ?? Documentation Files

### Core Documentation
1. **README.md** - Complete project overview and architecture guide
2. **EXERCISE_INSTRUCTIONS.md** - Step-by-step exercise guide with tasks
3. **OOP_CONCEPTS.md** - Deep dive into OOP concepts used in the project
4. **DatabaseSetup.sql** - Complete database creation script with sample data

---

## ?? Key Features Implemented

### ? Inheritance
- Abstract `Vehicle` base class
- Three concrete implementations: `Car`, `Motorcycle`, `Truck`
- Common properties and behavior in base class
- Specific properties and behavior in derived classes

### ? Interfaces
- `IInsurable` interface for vehicles that can be insured
- `Car` and `Motorcycle` implement it
- `Truck` does not implement it (demonstrates selective implementation)
- Repository interfaces for abstraction

### ? Polymorphism
- `GetVehicleType()` - returns different types
- `CalculateRentalCost()` - different calculations per vehicle type
  - Car: Simple daily rate
  - Motorcycle: 20% premium for powerful engines (>600cc)
  - Truck: €50 extra for heavy loads (>5000kg)
- `GetDetails()` - different information per type
- Insurance cost calculation varies by implementation

### ? Abstract Classes
- `Vehicle` cannot be instantiated
- Forces derived classes to implement abstract methods
- Provides common implementation and state

### ? Dapper ORM
- All repositories use Dapper for database access
- Async/await pattern throughout
- Clean, readable SQL queries
- Parameter binding for security

### ? SQL Client
- `DbConnectionFactory` manages connections
- Uses `Microsoft.Data.SqlClient`
- Connection string configuration
- Proper resource disposal with `using` statements

---

## ??? Database Schema

### Tables Created
1. **Customers**
   - Id, FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber

2. **Vehicles** (Single Table Inheritance)
   - Common: Id, LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType
   - Car: NumberOfDoors, FuelType
   - Motorcycle: EngineCapacity, HasSidecar
   - Truck: LoadCapacity, NumberOfAxles

3. **Rentals**
   - Id, CustomerId, VehicleId, StartDate, EndDate, TotalCost, IsActive
   - Foreign keys to Customers and Vehicles

### Sample Data Included
- 5 Customers
- 12 Vehicles (4 cars, 4 motorcycles, 4 trucks)
- 3 Sample rentals (completed)

---

## ?? How to Use This Exercise

### For Instructors:
1. **Review the README.md** for project overview
2. **Share EXERCISE_INSTRUCTIONS.md** with students
3. **Use OOP_CONCEPTS.md** as a teaching reference
4. Students should complete the exercise in 2-3 hours

### For Students:
1. **Start with README.md** to understand the project
2. **Follow EXERCISE_INSTRUCTIONS.md** step by step
3. **Reference OOP_CONCEPTS.md** when you need concept clarification
4. **Complete all tasks** in Part 4 of the instructions

### For Self-Study:
1. Read all documentation first
2. Set up the database
3. Run and explore the application
4. Try to add the Van vehicle type as your first extension
5. Complete the advanced challenges

---

## ??? Technical Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Framework | .NET | 9.0 |
| Language | C# | 13.0 |
| Database | SQL Server | Any version |
| ORM | Dapper | 2.1.72 |
| Data Access | Microsoft.Data.SqlClient | 7.0.0 |
| UI | WPF | .NET 9.0 |
| Architecture | DDD | 4 layers |

---

## ?? Code Statistics

- **Projects:** 4 (Domain, Infrastructure, Application, Presentation)
- **Classes:** 17 (7 entities, 3 repositories, 3 services, 4 UI-related)
- **Interfaces:** 4 (1 domain interface + 3 repository interfaces)
- **Abstract Classes:** 1 (Vehicle)
- **Lines of Code:** ~1,500 (excluding documentation)
- **Documentation:** ~1,800 lines across 4 markdown files

---

## ?? Learning Outcomes

After completing this exercise, students will be able to:

### Understand and Apply:
? **Inheritance** - Create hierarchies with base and derived classes
? **Abstract Classes** - Define contracts with partial implementation
? **Interfaces** - Define contracts without implementation
? **Polymorphism** - Write code that works with multiple types
? **Encapsulation** - Hide implementation details

### Work With:
? **Dapper** - Lightweight ORM for .NET
? **SQL Client** - Database connectivity
? **Async/Await** - Asynchronous programming
? **WPF** - Desktop UI development

### Design Using:
? **DDD Architecture** - Domain-Driven Design principles
? **Repository Pattern** - Data access abstraction
? **Service Layer** - Business logic separation
? **SOLID Principles** - Clean code practices

---

## ?? Where to Find Things

### Need to understand...
- **How inheritance works?** ? See `VehicleRentalSystem.Domain/Entities/Vehicle.cs` and its derived classes
- **How interfaces work?** ? See `IInsurable.cs` and its implementations in `Car.cs` and `Motorcycle.cs`
- **How Dapper works?** ? See any repository in `VehicleRentalSystem.Infrastructure/Repositories/`
- **How polymorphism works?** ? See `RentalService.CalculateInsuranceCost()` method
- **How the UI connects to backend?** ? See `MainWindow.xaml.cs` constructor and event handlers

### Need examples of...
- **Abstract methods** ? `Vehicle.GetVehicleType()`, `CalculateRentalCost()`, `GetDetails()`
- **Interface implementation** ? `Car.CalculateInsuranceCost()`, `GetInsuranceType()`
- **Polymorphic behavior** ? Different `CalculateRentalCost()` implementations
- **Dapper queries** ? Any method in repository classes
- **Async operations** ? All repository and service methods

---

## ? Special Features

### 1. Single Table Inheritance
The `Vehicles` table stores all vehicle types in one table with a discriminator column (`VehicleType`). This demonstrates:
- Database design patterns
- How to map to different C# types
- Nullable columns for type-specific data

### 2. Polymorphic Cost Calculation
Each vehicle type calculates rental costs differently:
```csharp
Car:        DailyRate × Days
Motorcycle: (DailyRate × Days) × 1.2 if EngineCapacity > 600cc
Truck:      (DailyRate × Days) + (€50 × Days) if LoadCapacity > 5000kg
```

### 3. Conditional Interface Implementation
Not all vehicles implement `IInsurable`:
- Cars and Motorcycles: ? Insurable
- Trucks: ? Not insurable (business rule)

### 4. Complete CRUD Operations
All entities support:
- Create (Add)
- Read (GetAll, GetById)
- Update
- Delete

---

## ?? Exercise Difficulty Levels

### Beginner Tasks (30 min)
- ? Set up and run the application
- ? Add a vehicle through the UI
- ? Create a rental
- ? Understand the code structure

### Intermediate Tasks (60 min)
- ? Add the Van vehicle type
- ? Implement distance-based pricing
- ? Add premium insurance for cars
- ? Implement maintenance tracking

### Advanced Tasks (90+ min)
- ? Implement seasonal pricing
- ? Add customer rating system
- ? Create reservation system
- ? Add validation throughout
- ? Implement Unit of Work pattern

---

## ?? Support Resources

### If you get stuck:
1. **Read the error message** - It usually tells you what's wrong
2. **Check OOP_CONCEPTS.md** - Comprehensive explanations of all concepts
3. **Review the existing code** - Similar patterns are already implemented
4. **Use the debugger** - Set breakpoints and step through code
5. **Check the database** - Run queries to verify data

### Common Issues:
- **Database connection fails** ? Update connection string in `MainWindow.xaml.cs`
- **Tables don't exist** ? Run `DatabaseSetup.sql`
- **Build errors** ? Check all projects have correct references
- **UI doesn't show data** ? Check connection string and database setup

---

## ?? What's Next?

After completing this exercise:

1. **Try the advanced challenges** in EXERCISE_INSTRUCTIONS.md
2. **Add unit tests** for the service layer
3. **Implement dependency injection** using Microsoft.Extensions.DependencyInjection
4. **Add logging** using Serilog or NLog
5. **Create an API** version using ASP.NET Core
6. **Add authentication** and user management
7. **Implement CQRS** pattern for better separation

---

## ?? Feedback and Improvement

This exercise can be extended with:
- [ ] Vehicle images/photos
- [ ] PDF invoice generation
- [ ] Email notifications
- [ ] Payment processing
- [ ] Fleet analytics dashboard
- [ ] Mobile app version
- [ ] Multi-language support
- [ ] Report generation

---

## ?? Congratulations!

You now have a complete, working Vehicle Rental System that demonstrates:
- ? All required OOP concepts
- ? Clean architecture
- ? Real-world patterns
- ? Professional code structure
- ? Comprehensive documentation

**Happy Learning and Coding! ??**

---

*Created for C# Advanced Course - Object-Oriented Programming Module*
*Target Framework: .NET 9.0 | C# 13.0*
