# OOP Concepts Quick Reference - Vehicle Rental System

## ?? Table of Contents
1. [Inheritance](#inheritance)
2. [Abstract Classes](#abstract-classes)
3. [Interfaces](#interfaces)
4. [Polymorphism](#polymorphism)
5. [Encapsulation](#encapsulation)

---

## ?? Inheritance

**Definition:** A mechanism where a new class (derived/child) inherits properties and methods from an existing class (base/parent).

### Example in This Project:

```csharp
// Base class
public abstract class Vehicle
{
    public int Id { get; set; }
    public string LicensePlate { get; set; }
    public string Brand { get; set; }
    // ... more properties
}

// Derived classes
public class Car : Vehicle { /* Car-specific members */ }
public class Motorcycle : Vehicle { /* Motorcycle-specific members */ }
public class Truck : Vehicle { /* Truck-specific members */ }
```

### Benefits:
- ? **Code Reuse:** Common properties defined once in `Vehicle`
- ? **Logical Hierarchy:** Models real-world "is-a" relationships
- ? **Maintainability:** Changes to common behavior in one place

### When to Use:
- Use inheritance when there's a clear "is-a" relationship
- Car **is a** Vehicle ?
- Customer **has a** Rental ? (use composition instead)

---

## ?? Abstract Classes

**Definition:** A class that cannot be instantiated and may contain abstract methods that must be implemented by derived classes.

### Example in This Project:

```csharp
public abstract class Vehicle
{
    // Concrete properties (all vehicles have these)
    public string Brand { get; set; }
    public decimal DailyRate { get; set; }
    
    // Abstract methods (each vehicle type implements differently)
    public abstract string GetVehicleType();
    public abstract decimal CalculateRentalCost(int days);
    public abstract string GetDetails();
}
```

### Key Points:
- ? **Cannot create instances:** `new Vehicle()` - compile error
- ? **Can have concrete members:** Regular properties and methods
- ? **Forces implementation:** Derived classes must override abstract methods

### Car Implementation:
```csharp
public class Car : Vehicle
{
    public override string GetVehicleType() => "Car";
    
    public override decimal CalculateRentalCost(int days)
    {
        return DailyRate * days; // Simple calculation
    }
    
    public override string GetDetails()
    {
        return $"{Brand} {Model} ({Year}) - {NumberOfDoors} doors";
    }
}
```

### Motorcycle Implementation:
```csharp
public class Motorcycle : Vehicle
{
    public override string GetVehicleType() => "Motorcycle";
    
    public override decimal CalculateRentalCost(int days)
    {
        decimal cost = DailyRate * days;
        if (EngineCapacity > 600) // Different logic!
            cost *= 1.2m;
        return cost;
    }
    
    public override string GetDetails()
    {
        return $"{Brand} {Model} - {EngineCapacity}cc";
    }
}
```

### Benefits:
- ? **Common Interface:** All vehicles have same methods
- ? **Enforce Contract:** Derived classes must implement abstract methods
- ? **Shared Implementation:** Can include concrete methods

---

## ?? Interfaces

**Definition:** A contract that defines a set of methods/properties that implementing classes must provide.

### Example in This Project:

```csharp
public interface IInsurable
{
    decimal CalculateInsuranceCost(int days);
    string GetInsuranceType();
}
```

### Implementations:

**Car implements IInsurable:**
```csharp
public class Car : Vehicle, IInsurable
{
    public decimal CalculateInsuranceCost(int days)
    {
        return 15m * days; // Standard rate
    }
    
    public string GetInsuranceType()
    {
        return "Standard Car Insurance";
    }
}
```

**Motorcycle implements IInsurable:**
```csharp
public class Motorcycle : Vehicle, IInsurable
{
    public decimal CalculateInsuranceCost(int days)
    {
        decimal baseCost = 20m * days;
        if (EngineCapacity > 600)
            baseCost *= 1.5m; // Higher risk = higher cost
        return baseCost;
    }
    
    public string GetInsuranceType()
    {
        return EngineCapacity > 600 
            ? "Premium Motorcycle Insurance" 
            : "Standard Motorcycle Insurance";
    }
}
```

**Truck does NOT implement IInsurable:**
```csharp
public class Truck : Vehicle
{
    // No insurance methods - trucks aren't insurable in this system
}
```

### Interface vs Abstract Class:

| Feature | Interface | Abstract Class |
|---------|-----------|----------------|
| Multiple inheritance | ? Yes | ? No (single inheritance) |
| Implementation | ? No (before C# 8) | ? Yes |
| Fields | ? No | ? Yes |
| Constructors | ? No | ? Yes |
| Access modifiers | ? Public only | ? Any |

### When to Use:
- **Interface:** Defines a capability (IInsurable, IComparable, IDisposable)
- **Abstract Class:** Defines a type hierarchy (Vehicle, Animal, Shape)

### Rule of Thumb:
- Use interface for "can-do" relationships: Car **can be** insured
- Use abstract class for "is-a" relationships: Car **is a** Vehicle

---

## ?? Polymorphism

**Definition:** The ability of objects of different types to be treated as objects of a common base type, with each type responding differently to the same method call.

### Example in This Project:

```csharp
// In RentalService
public decimal CalculateInsuranceCost(Vehicle vehicle, int days)
{
    if (vehicle is IInsurable insurable)
    {
        return insurable.CalculateInsuranceCost(days);
    }
    return 0m; // Not insurable (e.g., Truck)
}
```

### How It Works:

**Scenario 1: Car Rental**
```csharp
Vehicle vehicle = new Car { DailyRate = 50m, EngineCapacity = 1600 };
int days = 5;

// Polymorphic call - runs Car's implementation
decimal cost = vehicle.CalculateRentalCost(days); // 50 * 5 = €250

// Insurance check
if (vehicle is IInsurable insurable)
{
    decimal insurance = insurable.CalculateInsuranceCost(days); // 15 * 5 = €75
}
```

**Scenario 2: Motorcycle Rental**
```csharp
Vehicle vehicle = new Motorcycle { DailyRate = 60m, EngineCapacity = 750 };
int days = 5;

// Same method call, different implementation!
decimal cost = vehicle.CalculateRentalCost(days); // (60 * 5) * 1.2 = €360

// Different insurance calculation
if (vehicle is IInsurable insurable)
{
    decimal insurance = insurable.CalculateInsuranceCost(days); // (20 * 5) * 1.5 = €150
}
```

**Scenario 3: Truck Rental**
```csharp
Vehicle vehicle = new Truck { DailyRate = 100m, LoadCapacity = 6000 };
int days = 5;

// Different implementation again
decimal cost = vehicle.CalculateRentalCost(days); // (100 * 5) + (50 * 5) = €750

// Not insurable
if (vehicle is IInsurable insurable) // This is false for Truck
{
    // This block won't execute
}
```

### Real-World Usage in the Application:

**In the UI (MainWindow.xaml.cs):**
```csharp
private async void CreateRentalButton_Click(object sender, RoutedEventArgs e)
{
    var selectedVehicle = RentalVehicleComboBox.SelectedItem as Vehicle;
    
    int days = (endDate - startDate).Days;
    
    // Polymorphic call - correct implementation runs based on actual type
    decimal rentalCost = selectedVehicle.CalculateRentalCost(days);
    
    // Polymorphic insurance calculation
    decimal insuranceCost = _rentalService.CalculateInsuranceCost(selectedVehicle, days);
}
```

### Types of Polymorphism:

**1. Compile-Time Polymorphism (Method Overloading):**
```csharp
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public double Add(double a, double b) => a + b;
    public int Add(int a, int b, int c) => a + b + c;
}
```

**2. Runtime Polymorphism (Method Overriding):**
```csharp
// What we use in Vehicle hierarchy
public abstract class Vehicle
{
    public abstract decimal CalculateRentalCost(int days);
}

public class Car : Vehicle
{
    public override decimal CalculateRentalCost(int days) { /* Car logic */ }
}
```

### Benefits:
- ? **Flexibility:** Add new vehicle types without changing existing code
- ? **Maintainability:** Each type manages its own behavior
- ? **Extensibility:** Easy to add new features

---

## ?? Encapsulation

**Definition:** Hiding internal state and requiring all interaction through methods/properties.

### Example in This Project:

```csharp
public class SafetyTraining : Training, ICertificateProvider
{
    private bool _certificateGranted; // Private field - hidden from outside
    
    // Public method to interact with private field
    public void GrantCertificate()
    {
        _certificateGranted = true;
    }
}
```

### Property Encapsulation:
```csharp
public class Customer
{
    // Auto-properties with validation
    private string _email;
    public string Email 
    { 
        get => _email;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty");
            _email = value;
        }
    }
}
```

### Benefits:
- ? **Data Protection:** Control how data is accessed/modified
- ? **Validation:** Ensure data integrity
- ? **Flexibility:** Change internal implementation without affecting users

---

## ?? SOLID Principles in This Project

### S - Single Responsibility Principle
Each class has one reason to change:
- `Vehicle`: Represents vehicle data and behavior
- `VehicleRepository`: Handles vehicle database operations
- `VehicleService`: Manages vehicle business logic

### O - Open/Closed Principle
Open for extension, closed for modification:
- Can add new vehicle type (Van) without modifying Vehicle class
- Can add new insurance type without modifying existing insurables

### L - Liskov Substitution Principle
Derived classes can replace base classes:
- `Car`, `Motorcycle`, `Truck` can all be used where `Vehicle` is expected
- No unexpected behavior when substituting

### I - Interface Segregation Principle
Specific interfaces instead of one large interface:
- `IInsurable` is small and focused
- Not all vehicles need to implement it

### D - Dependency Inversion Principle
Depend on abstractions, not concretions:
- Services depend on `IVehicleRepository` interface
- Not on concrete `VehicleRepository` class

---

## ?? Testing Polymorphism

Try this test code to see polymorphism in action:

```csharp
// Create different vehicle types
List<Vehicle> fleet = new List<Vehicle>
{
    new Car(1, "ABC-123", "Toyota", "Corolla", 2023, 50m, 4, "Petrol"),
    new Motorcycle(2, "MNO-345", "Harley", "Sportster", 2023, 60m, 883, false),
    new Truck(3, "YZA-567", "Mercedes", "Sprinter", 2023, 95m, 3500, 2)
};

int rentalDays = 5;

// Polymorphic iteration
foreach (Vehicle vehicle in fleet)
{
    Console.WriteLine($"Vehicle: {vehicle.GetVehicleType()}");
    Console.WriteLine($"Details: {vehicle.GetDetails()}");
    
    // Each vehicle calculates cost differently!
    decimal cost = vehicle.CalculateRentalCost(rentalDays);
    Console.WriteLine($"Rental Cost for {rentalDays} days: {cost:C}");
    
    // Check if insurable
    if (vehicle is IInsurable insurable)
    {
        decimal insurance = insurable.CalculateInsuranceCost(rentalDays);
        Console.WriteLine($"Insurance Cost: {insurance:C}");
        Console.WriteLine($"Type: {insurable.GetInsuranceType()}");
    }
    else
    {
        Console.WriteLine("Not insurable");
    }
    
    Console.WriteLine();
}
```

**Output:**
```
Vehicle: Car
Details: Toyota Corolla (2023) - 4 doors, Petrol
Rental Cost for 5 days: €250.00
Insurance Cost: €75.00
Type: Standard Car Insurance

Vehicle: Motorcycle
Details: Harley Sportster (2023) - 883cc, no sidecar
Rental Cost for 5 days: €360.00
Insurance Cost: €150.00
Type: Premium Motorcycle Insurance

Vehicle: Truck
Details: Mercedes Sprinter (2023) - Capacity: 3500kg, 2 axles
Rental Cost for 5 days: €475.00
Not insurable
```

---

## ?? Common Pitfalls and Solutions

### Pitfall 1: Using Concrete Types Instead of Abstractions
? **Bad:**
```csharp
public void ProcessRental(Car car) { }
public void ProcessRental(Motorcycle motorcycle) { }
public void ProcessRental(Truck truck) { }
```

? **Good:**
```csharp
public void ProcessRental(Vehicle vehicle) 
{
    // Works for all vehicle types!
}
```

### Pitfall 2: Type Checking Instead of Polymorphism
? **Bad:**
```csharp
decimal cost;
if (vehicle.GetType() == typeof(Car))
    cost = vehicle.DailyRate * days;
else if (vehicle.GetType() == typeof(Motorcycle))
    cost = vehicle.DailyRate * days * 1.2m;
else if (vehicle.GetType() == typeof(Truck))
    cost = vehicle.DailyRate * days + 50m * days;
```

? **Good:**
```csharp
decimal cost = vehicle.CalculateRentalCost(days); // Polymorphism!
```

### Pitfall 3: Breaking Encapsulation
? **Bad:**
```csharp
public class Vehicle
{
    public decimal dailyRate; // Public field - no control
}

vehicle.dailyRate = -100; // Negative rate? Oops!
```

? **Good:**
```csharp
public class Vehicle
{
    private decimal _dailyRate;
    public decimal DailyRate 
    { 
        get => _dailyRate;
        set
        {
            if (value < 0)
                throw new ArgumentException("Rate cannot be negative");
            _dailyRate = value;
        }
    }
}
```

---

## ?? Further Reading

- **Design Patterns:** Gang of Four book
- **Clean Code:** Robert C. Martin
- **Refactoring:** Martin Fowler
- **Domain-Driven Design:** Eric Evans

---

**Remember:** Object-Oriented Programming is about modeling real-world concepts in code. The Vehicle Rental System models the real relationships between vehicles, customers, and rentals!
