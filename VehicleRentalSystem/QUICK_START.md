# ?? Quick Start Guide - Vehicle Rental System

## Get Started in 5 Minutes!

### Step 1: Open the Solution (30 seconds)
1. Navigate to: `C:\CourseMaterial\oplossingen-cs-advanced\07-OOP\VehicleRentalSystem\`
2. Double-click `VehicleRentalSystem.sln`
3. Visual Studio will open

### Step 2: Set Up the Database (2 minutes)
1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance (usually `.\sqlexpress`)
3. Click **File ? Open ? File**
4. Select `DatabaseSetup.sql` from the solution folder
5. Click **Execute** (or press F5)
6. ? You should see: "Database setup completed successfully!"

### Step 3: Update Connection String (1 minute)
1. In Visual Studio, open `VehicleRentalSystem.Presentation/MainWindow.xaml.cs`
2. Find line ~17 (in the constructor)
3. Update if needed:
   ```csharp
   var connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=VehicleRentalDb;Data Source=.\\sqlexpress;TrustServerCertificate=True";
   ```
   - If your SQL Server instance is different, change `.\\sqlexpress` to your instance name
   - Default is usually fine!

### Step 4: Run the Application (30 seconds)
1. In Visual Studio, press **F5** (or click the green "Start" button)
2. The Vehicle Rental System window should appear
3. ? You should see sample data loaded!

### Step 5: Test the Application (1 minute)
1. **View Vehicles:**
   - Click the "Vehicles" tab
   - You should see 12 vehicles (cars, motorcycles, trucks)

2. **View Customers:**
   - Click the "Customers" tab
   - You should see 5 sample customers

3. **Create a Rental:**
   - Click the "Rentals" tab
   - Select a customer from the dropdown
   - Select an available vehicle
   - Set dates (start: today, end: 5 days from now)
   - Click "Create Rental"
   - ? A message shows the cost calculation!

---

## ?? What to Do Next?

### If You're a Student:
?? **Read:** `EXERCISE_INSTRUCTIONS.md` - Your complete exercise guide
- Follow Part 1: Understanding the code (30 min)
- Complete Part 4: Extend the system (60-90 min)

### If You're an Instructor:
?? **Review:** `README.md` - Complete project documentation
?? **Share:** `EXERCISE_INSTRUCTIONS.md` with students
?? **Reference:** `OOP_CONCEPTS.md` for teaching

### If You're Self-Learning:
1. Start with `OOP_CONCEPTS.md` - Understand the theory
2. Explore the code - Open and read each file
3. Run the debugger - Set breakpoints, step through
4. Try the exercises - Add the Van vehicle type first

---

## ?? Important Files

| File | Purpose |
|------|---------|
| `README.md` | Complete project overview and architecture |
| `EXERCISE_INSTRUCTIONS.md` | Step-by-step exercise tasks |
| `OOP_CONCEPTS.md` | Deep dive into OOP concepts |
| `PROJECT_SUMMARY.md` | What was created and why |
| `DatabaseSetup.sql` | Database creation script |

---

## ?? Troubleshooting

### Problem: "Cannot connect to database"
**Solution:**
1. Make sure SQL Server is running
2. Check connection string in `MainWindow.xaml.cs`
3. Verify database exists: Run `SELECT name FROM sys.databases WHERE name = 'VehicleRentalDb'`

### Problem: "Tables not found"
**Solution:**
1. Run `DatabaseSetup.sql` in SSMS
2. Verify tables exist: `USE VehicleRentalDb; SELECT * FROM INFORMATION_SCHEMA.TABLES`

### Problem: "No data appears in the application"
**Solution:**
1. Check if database has sample data: `SELECT COUNT(*) FROM Vehicles`
2. Re-run the data insertion part of `DatabaseSetup.sql`

### Problem: "Build errors"
**Solution:**
1. Clean solution: **Build ? Clean Solution**
2. Rebuild: **Build ? Rebuild Solution**
3. Check NuGet packages are restored

---

## ?? Tips for Success

### 1. Use the Debugger
- Set a breakpoint in `CreateRentalButton_Click` method
- Step through to see how polymorphism works
- Watch variables to see different vehicle types

### 2. Experiment
- Add a new vehicle through the UI
- Try different vehicle types
- Notice how costs calculate differently

### 3. Read the Code
- Start with `Vehicle.cs` - the base class
- Then read `Car.cs`, `Motorcycle.cs`, `Truck.cs`
- Compare how they implement abstract methods

### 4. Ask Questions
- Why is `Vehicle` abstract?
- Why does `Truck` not implement `IInsurable`?
- How does `VehicleRepository` know which type to create?

---

## ?? Learning Path

### Beginner (1-2 hours)
1. ? Set up and run
2. ? Understand the structure
3. ? Read `OOP_CONCEPTS.md`
4. ? Complete Exercise Part 1 & 2

### Intermediate (3-4 hours)
1. ? Complete beginner tasks
2. ? Add the Van vehicle type
3. ? Implement distance-based pricing
4. ? Add premium insurance

### Advanced (5+ hours)
1. ? Complete intermediate tasks
2. ? Implement all advanced challenges
3. ? Add unit tests
4. ? Refactor using dependency injection

---

## ?? Key Concepts to Master

### Must Understand:
- ? **Inheritance:** Vehicle ? Car, Motorcycle, Truck
- ? **Abstract Classes:** Vehicle class with abstract methods
- ? **Interfaces:** IInsurable implemented by Car and Motorcycle
- ? **Polymorphism:** Different CalculateRentalCost implementations

### Should Understand:
- ? **Dapper ORM:** Repository implementations
- ? **Repository Pattern:** Data access abstraction
- ? **DDD Architecture:** Layer separation
- ? **Async/Await:** Asynchronous programming

### Nice to Understand:
- ? **SOLID Principles:** In the architecture
- ? **WPF Data Binding:** UI to code connection
- ? **Single Table Inheritance:** Database design
- ? **Service Layer Pattern:** Business logic separation

---

## ?? Success Checklist

Before starting the exercises, make sure:
- [ ] Solution opens in Visual Studio without errors
- [ ] Database is created and has sample data
- [ ] Application runs and shows data
- [ ] You can create a rental successfully
- [ ] You understand the basic structure

After completing exercises:
- [ ] You can explain inheritance in this project
- [ ] You can explain polymorphism in action
- [ ] You can add a new vehicle type
- [ ] You understand the DDD architecture
- [ ] You know when to use abstract classes vs interfaces

---

## ?? Need Help?

### Check These First:
1. **Error messages** - They tell you what's wrong
2. **OOP_CONCEPTS.md** - Explains all concepts
3. **Existing code** - Similar patterns already implemented
4. **Documentation** - README.md has architecture details

### Common Questions:
**Q: Why use an abstract class for Vehicle?**
A: Because all vehicles share common properties (Brand, Model, etc.) but each calculates costs differently. Abstract class provides both shared state and enforces implementation.

**Q: Why doesn't Truck implement IInsurable?**
A: Business rule - in this system, trucks don't offer insurance. This demonstrates that not all related classes need the same capabilities.

**Q: How does the repository know which vehicle type to create?**
A: The `VehicleType` column in the database stores "Car", "Motorcycle", or "Truck". The repository uses a switch statement to map this to the correct C# type.

**Q: Why use Dapper instead of Entity Framework?**
A: This is an educational choice to show raw SQL and understand how ORMs work. In production, either is fine!

---

## ?? Ready to Start!

You're all set! Choose your path:

### Path 1: Understanding First
?? Read `OOP_CONCEPTS.md` ? Explore code ? Try exercises

### Path 2: Hands-On First
?? Run app ? Experiment ? Read when confused ? Try exercises

### Path 3: Guided Learning
?? Follow `EXERCISE_INSTRUCTIONS.md` step by step

---

**Good luck and enjoy learning! ??**

*Remember: The best way to learn is by doing. Don't just read the code—modify it, break it, fix it!*
