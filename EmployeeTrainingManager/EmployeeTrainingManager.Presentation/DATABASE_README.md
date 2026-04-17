# Database Setup Instructions

## Overview
The Employee Training Manager application uses SQL Server LocalDB for data storage. You need to create the database before running the application.

## Prerequisites
- SQL Server LocalDB (comes with Visual Studio)
- SQL Server Management Studio (SSMS) or Azure Data Studio (recommended for students)

## Option 1: Quick Setup with Sample Data (Recommended for Testing)

Run this single script to create the database with sample data:
- **File**: `Database_Setup.sql`
- **Contains**: Database, tables, indexes, AND sample data
- **Result**: Ready to test immediately with 10 employees, 16 trainings, and 16 enrollments

## Option 2: Clean Setup (Recommended for Production)

Run these scripts in order:

1. **Create Database and Tables**: Run `Database_Setup_Clean.sql`
   - Creates empty database
   - Creates all tables with constraints
   - No sample data

2. **Add Sample Data** (Optional): Run `Database_SampleData.sql`
   - Adds test employees, trainings, and enrollments
   - Only run this if you want sample data for testing

## How to Run the Scripts

### Using SQL Server Management Studio (SSMS)

1. Open SSMS
2. Connect to: `(localdb)\mssqllocaldb`
3. Click **File > Open > File**
4. Select `Database_Setup.sql` (or `Database_Setup_Clean.sql`)
5. Click **Execute** (or press F5)
6. Wait for "Command(s) completed successfully" message

### Using Azure Data Studio

1. Open Azure Data Studio
2. Click **New Connection**
3. Server: `(localdb)\mssqllocaldb`
4. Authentication: Windows Authentication
5. Click **Connect**
6. Click **File > Open File**
7. Select `Database_Setup.sql` (or `Database_Setup_Clean.sql`)
8. Click **Run** (or press F5)

### Using Visual Studio

1. Open **View > SQL Server Object Explorer**
2. Expand **SQL Server > (localdb)\mssqllocaldb**
3. Right-click on **(localdb)\mssqllocaldb**
4. Select **New Query**
5. Copy and paste contents of `Database_Setup.sql`
6. Click **Execute** button (green arrow)

### Using Command Line (sqlcmd)

```bash
sqlcmd -S "(localdb)\mssqllocaldb" -i "Database_Setup.sql"
```

## Database Schema

### Table: Employees
| Column | Type | Description |
|--------|------|-------------|
| Id | INT (Identity) | Primary Key |
| FirstName | NVARCHAR(100) | Employee's first name |
| LastName | NVARCHAR(100) | Employee's last name |

### Table: Trainings
| Column | Type | Description |
|--------|------|-------------|
| Id | NVARCHAR(50) | Primary Key (e.g., "TECH-001") |
| Title | NVARCHAR(200) | Training title |
| TrainerName | NVARCHAR(100) | Name of the trainer |
| DurationInHours | INT | Training duration |
| TrainingType | NVARCHAR(50) | 'Technical' or 'Safety' |
| Technology | NVARCHAR(100) | For Technical trainings only |
| RiskLevel | NVARCHAR(50) | For Safety trainings (Low/Medium/High) |

### Table: Enrollments
| Column | Type | Description |
|--------|------|-------------|
| Id | INT (Identity) | Primary Key |
| EmployeeId | INT | Foreign Key to Employees |
| TrainingId | NVARCHAR(50) | Foreign Key to Trainings |
| IsBillable | BIT | Is this enrollment billable? |
| EnrollmentDate | DATETIME | When enrolled (auto-set) |

## Key Features

### Constraints
- **Check Constraints**: Ensure data validity (e.g., names not empty, valid risk levels)
- **Foreign Keys**: Maintain referential integrity with CASCADE DELETE
- **Unique Constraint**: Prevents duplicate enrollments (same employee + training)

### Indexes
- Index on EmployeeId in Enrollments for faster queries
- Index on TrainingId in Enrollments for faster queries

## Verification

After running the script, verify the setup:

```sql
-- Check if database exists
USE EmployeeTrainingDb;

-- Count records in each table
SELECT 'Employees' AS TableName, COUNT(*) AS RecordCount FROM Employees
UNION ALL
SELECT 'Trainings', COUNT(*) FROM Trainings
UNION ALL
SELECT 'Enrollments', COUNT(*) FROM Enrollments;
```

## Connection String

The application uses this connection string (in `MainWindow.xaml.cs`):
```
Server=(localdb)\mssqllocaldb;Database=EmployeeTrainingDb;Trusted_Connection=True;
```

Make sure your database name matches this connection string!

## Troubleshooting

### Error: Cannot open database
- Make sure you ran the SQL script successfully
- Check that LocalDB is installed: Run `sqllocaldb info` in command prompt

### Error: LocalDB not found
- Install SQL Server Express LocalDB (comes with Visual Studio)
- Or change connection string to use your SQL Server instance

### Error: Login failed
- LocalDB uses Windows Authentication
- Make sure you're running as the same user who created the database

### Start Over
If you need to recreate the database, just run the script again. It will drop and recreate everything.

## Sample Data Summary

If you run `Database_Setup.sql` or `Database_SampleData.sql`, you'll get:

- **10 Employees**: John Doe, Jane Smith, etc.
- **8 Technical Trainings**: .NET, React, SQL Server, Azure, Docker, Python, Angular, ASP.NET
- **8 Safety Trainings**: Fire Safety, First Aid, Ergonomics, Chemical Safety, etc.
- **16 Enrollments**: Various employees enrolled in different trainings

This gives you data to test all features immediately!
