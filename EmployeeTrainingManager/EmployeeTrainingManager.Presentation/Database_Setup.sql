-- =============================================
-- Employee Training Manager Database Setup
-- =============================================
-- This script creates the database and all required tables
-- Run this script in SQL Server Management Studio or Azure Data Studio
-- Connected to: (localdb)\mssqllocaldb
-- =============================================

-- Drop database if it exists (CAUTION: This will delete all data!)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'EmployeeTrainingDb')
BEGIN
    ALTER DATABASE EmployeeTrainingDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EmployeeTrainingDb;
END
GO

-- Create the database
CREATE DATABASE EmployeeTrainingDb;
GO

-- Use the new database
USE EmployeeTrainingDb;
GO

-- =============================================
-- Table: Employees
-- Stores employee information
-- =============================================
CREATE TABLE Employees
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    CONSTRAINT CK_Employees_FirstName CHECK (LEN(FirstName) > 0),
    CONSTRAINT CK_Employees_LastName CHECK (LEN(LastName) > 0)
);
GO

-- =============================================
-- Table: Trainings
-- Stores both Technical and Safety trainings
-- Uses TrainingType to distinguish between types
-- =============================================
CREATE TABLE Trainings
(
    Id NVARCHAR(50) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    TrainerName NVARCHAR(100) NOT NULL,
    DurationInHours INT NOT NULL DEFAULT 0,
    TrainingType NVARCHAR(50) NOT NULL,
    Technology NVARCHAR(100) NULL,      -- For Technical Training only
    RiskLevel NVARCHAR(50) NULL,        -- For Safety Training only
    CONSTRAINT CK_Trainings_Title CHECK (LEN(Title) > 0),
    CONSTRAINT CK_Trainings_TrainerName CHECK (LEN(TrainerName) > 0),
    CONSTRAINT CK_Trainings_Type CHECK (TrainingType IN ('Technical', 'Safety')),
    CONSTRAINT CK_Trainings_RiskLevel CHECK (RiskLevel IS NULL OR RiskLevel IN ('Low', 'Medium', 'High'))
);
GO

-- =============================================
-- Table: Enrollments
-- Links employees to trainings
-- =============================================
CREATE TABLE Enrollments
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    TrainingId NVARCHAR(50) NOT NULL,
    IsBillable BIT NOT NULL DEFAULT 0,
    EnrollmentDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Enrollments_Employees FOREIGN KEY (EmployeeId) 
        REFERENCES Employees(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Enrollments_Trainings FOREIGN KEY (TrainingId) 
        REFERENCES Trainings(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Enrollments_Employee_Training UNIQUE (EmployeeId, TrainingId)
);
GO

-- =============================================
-- Indexes for better query performance
-- =============================================
CREATE INDEX IX_Enrollments_EmployeeId ON Enrollments(EmployeeId);
CREATE INDEX IX_Enrollments_TrainingId ON Enrollments(TrainingId);
GO

-- =============================================
-- Sample Data (Optional - Remove if not needed)
-- =============================================

-- Insert sample employees
INSERT INTO Employees (FirstName, LastName) VALUES
('John', 'Doe'),
('Jane', 'Smith'),
('Michael', 'Johnson'),
('Sarah', 'Williams'),
('David', 'Brown');
GO

-- Insert sample technical trainings
INSERT INTO Trainings (Id, Title, TrainerName, DurationInHours, TrainingType, Technology) VALUES
('TECH-001', '.NET 9 Fundamentals', 'Alice Cooper', 40, 'Technical', 'C#'),
('TECH-002', 'React Development', 'Bob Martin', 32, 'Technical', 'JavaScript'),
('TECH-003', 'SQL Server Advanced', 'Carol White', 24, 'Technical', 'SQL');
GO

-- Insert sample safety trainings
INSERT INTO Trainings (Id, Title, TrainerName, DurationInHours, TrainingType, RiskLevel) VALUES
('SAFE-001', 'Fire Safety Training', 'Tom Anderson', 4, 'Safety', 'High'),
('SAFE-002', 'First Aid Basics', 'Emma Davis', 8, 'Safety', 'Medium'),
('SAFE-003', 'Workplace Ergonomics', 'Lisa Green', 2, 'Safety', 'Low');
GO

-- Insert sample enrollments
INSERT INTO Enrollments (EmployeeId, TrainingId, IsBillable) VALUES
(1, 'TECH-001', 1),  -- John Doe enrolled in .NET 9 Fundamentals (Billable)
(1, 'SAFE-001', 0),  -- John Doe enrolled in Fire Safety Training (Not Billable)
(2, 'TECH-002', 1),  -- Jane Smith enrolled in React Development (Billable)
(3, 'SAFE-002', 0),  -- Michael Johnson enrolled in First Aid Basics (Not Billable)
(4, 'TECH-003', 1),  -- Sarah Williams enrolled in SQL Server Advanced (Billable)
(5, 'SAFE-003', 0);  -- David Brown enrolled in Workplace Ergonomics (Not Billable)
GO

-- =============================================
-- Verification Queries
-- =============================================

-- View all employees
SELECT * FROM Employees;

-- View all trainings
SELECT * FROM Trainings;

-- View all enrollments with details
SELECT 
    e.Id AS EnrollmentId,
    emp.FirstName + ' ' + emp.LastName AS EmployeeName,
    t.Title AS TrainingTitle,
    t.TrainingType,
    CASE WHEN e.IsBillable = 1 THEN 'Yes' ELSE 'No' END AS Billable,
    e.EnrollmentDate
FROM Enrollments e
INNER JOIN Employees emp ON e.EmployeeId = emp.Id
INNER JOIN Trainings t ON e.TrainingId = t.Id
ORDER BY e.EnrollmentDate DESC;

-- =============================================
-- Database Setup Complete!
-- =============================================
PRINT 'Database EmployeeTrainingDb created successfully!';
PRINT 'Tables created: Employees, Trainings, Enrollments';
PRINT 'Sample data inserted (optional - you can delete this if not needed)';
GO
