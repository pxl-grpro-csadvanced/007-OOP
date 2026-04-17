# Quick Reference - MainWindow Constructor

## What Happens When the Application Starts?

### 1. App.xaml runs
```xml
<Application StartupUri="MainWindow.xaml">
```
This tells WPF to open MainWindow.xaml when the app starts.

### 2. MainWindow Constructor Runs
This is where everything gets set up:

```csharp
public MainWindow()
{
    // STEP 1: Load the XAML (MUST BE FIRST!)
    InitializeComponent();
    
    // STEP 2: Set up the database connection string
    var connectionString = "Server=(localdb)\\mssqllocaldb;Database=EmployeeTrainingDb;Trusted_Connection=True;";
    var dbFactory = new DbConnectionFactory(connectionString);
    
    // STEP 3: Create repositories (these talk to the database)
    var employeeRepository = new EmployeeRepository(dbFactory);
    var trainingRepository = new TrainingRepository(dbFactory);
    var enrollmentRepository = new EnrollmentRepository(dbFactory, employeeRepository, trainingRepository);
    
    // STEP 4: Create services (these contain business logic)
    _employeeService = new EmployeeService(employeeRepository);
    _trainingService = new TrainingService(trainingRepository);
    _enrollmentService = new EnrollmentService(enrollmentRepository, employeeRepository, trainingRepository);
    
    // STEP 5: Load data from the database
    LoadAllData();
}
```

## Why This Order Matters

### ? WRONG - This will crash:
```csharp
public MainWindow()
{
    DataContext = this;  // ERROR! Controls don't exist yet
    InitializeComponent();
}
```

### ? CORRECT:
```csharp
public MainWindow()
{
    InitializeComponent();  // Creates all controls first
    DataContext = this;     // Now controls can bind to data
}
```

## Understanding the Dependencies

### ?? Dependency Chain
```
MainWindow
    ?
Services (EmployeeService, TrainingService, EnrollmentService)
    ?
Repositories (EmployeeRepository, TrainingRepository, EnrollmentRepository)
    ?
DbConnectionFactory (creates database connections)
    ?
Database (SQL Server LocalDB)
```

### Why Create in This Order?

1. **DbConnectionFactory** - Needs connection string only
2. **Repositories** - Need DbConnectionFactory
3. **Services** - Need Repositories
4. **MainWindow** - Needs Services

Each layer depends on the layer below it!

## Simple Example - Just Employees

If you only want to work with employees, here's the minimal code:

```csharp
public MainWindow()
{
    InitializeComponent();
    
    // Create database connection
    var connectionString = "Server=(localdb)\\mssqllocaldb;Database=EmployeeTrainingDb;Trusted_Connection=True;";
    var dbFactory = new DbConnectionFactory(connectionString);
    
    // Create repository and service
    var employeeRepository = new EmployeeRepository(dbFactory);
    _employeeService = new EmployeeService(employeeRepository);
    
    // Load data
    LoadEmployees();
}

private async Task LoadEmployees()
{
    var employees = await _employeeService.GetAllEmployeesAsync();
    EmployeeDataGrid.ItemsSource = employees;
}
```

## Common Questions

### Q: Why do we need all these layers?
**A:** Each layer has a specific job:
- **Domain**: Defines what data looks like (Employee, Training)
- **Infrastructure**: Talks to the database (Repositories)
- **Application**: Contains business rules (Services)
- **Presentation**: Shows data to users (MainWindow)

### Q: Can I skip creating some objects?
**A:** No! Each object depends on the previous ones:
- Services need Repositories
- Repositories need DbConnectionFactory
- If you skip one, the next one won't work

### Q: Why use `new` keyword so much?
**A:** We're creating objects! In C#, `new` creates a new instance of a class:
```csharp
var dbFactory = new DbConnectionFactory(connectionString);
```
This means: "Create a new DbConnectionFactory object and store it in dbFactory variable"

### Q: What does `ItemsSource` do?
**A:** It tells a control (like DataGrid or ComboBox) what data to display:
```csharp
EmployeeDataGrid.ItemsSource = employees;
```
This means: "Show all the employees in this DataGrid"

### Q: Why reload data after adding/deleting?
**A:** The control shows a snapshot of data. After changes, we need to get fresh data:
```csharp
await _employeeService.AddEmployeeAsync(firstName, lastName);  // Add to database
await LoadEmployees();  // Get updated list and refresh the DataGrid
```

### Q: Why do some methods have `await`?
**A:** Database operations take time. Using `await` prevents the UI from freezing:
```csharp
await _employeeService.AddEmployeeAsync(firstName, lastName);
```
The app stays responsive while waiting for the database!

## Experiment Ideas

Try these modifications to learn more:

1. **Add Console Output**: See when constructor runs
```csharp
public MainWindow()
{
    Console.WriteLine("MainWindow constructor started");
    InitializeComponent();
    Console.WriteLine("XAML loaded");
    // ... rest of code
}
```

2. **Add Debug Messages**: See when data loads
```csharp
private async Task LoadEmployees()
{
    var employees = await _employeeService.GetAllEmployeesAsync();
    Console.WriteLine($"Loaded {employees.Count()} employees");
    EmployeeDataGrid.ItemsSource = employees;
}
```

3. **Test Setting ItemsSource**: Try setting ItemsSource multiple times
```csharp
EmployeeDataGrid.ItemsSource = employees;  // First time
// ... later after adding an employee
EmployeeDataGrid.ItemsSource = employees;  // Updates the display
```

