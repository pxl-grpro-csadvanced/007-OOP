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
('David', 'Brown'),
('Emily', 'Davis'),
('Robert', 'Miller'),
('Lisa', 'Wilson'),
('James', 'Moore'),
('Maria', 'Taylor');
GO

-- Insert sample technical trainings
INSERT INTO Trainings (Id, Title, TrainerName, DurationInHours, TrainingType, Technology) VALUES
('TECH-001', '.NET 9 Fundamentals', 'Alice Cooper', 40, 'Technical', 'C#'),
('TECH-002', 'React Development', 'Bob Martin', 32, 'Technical', 'JavaScript'),
('TECH-003', 'SQL Server Advanced', 'Carol White', 24, 'Technical', 'SQL'),
('TECH-004', 'Angular Essentials', 'Bob Martin', 28, 'Technical', 'TypeScript'),
('TECH-005', 'SQL Server Advanced', 'Carol White', 24, 'Technical', 'SQL'),
('TECH-006', 'Azure Cloud Services', 'David Lee', 36, 'Technical', 'Azure'),
('TECH-007', 'Docker & Kubernetes', 'Emma Stone', 20, 'Technical', 'Docker'),
('TECH-008', 'Python for Data Science', 'Frank Ocean', 40, 'Technical', 'Python');
GO

-- Insert sample safety trainings
INSERT INTO Trainings (Id, Title, TrainerName, DurationInHours, TrainingType, RiskLevel) VALUES
('SAFE-001', 'Fire Safety Training', 'Tom Anderson', 4, 'Safety', 'High'),
('SAFE-002', 'First Aid Basics', 'Emma Davis', 8, 'Safety', 'Medium'),
('SAFE-003', 'Workplace Ergonomics', 'Lisa Green', 2, 'Safety', 'Low'),
('SAFE-004', 'CPR Certification', 'Emma Davis', 6, 'Safety', 'Medium'),
('SAFE-005', 'Workplace Ergonomics', 'Lisa Green', 2, 'Safety', 'Low'),
('SAFE-006', 'Chemical Safety', 'Mark Johnson', 5, 'Safety', 'High'),
('SAFE-007', 'Electrical Safety', 'Nina Patel', 4, 'Safety', 'High'),
('SAFE-008', 'Slip and Fall Prevention', 'Oliver Brown', 2, 'Safety', 'Low');
GO

-- Insert sample enrollments
INSERT INTO Enrollments (EmployeeId, TrainingId, IsBillable) VALUES
-- John Doe's enrollments
(1, 'TECH-001', 1),  -- .NET 9 Fundamentals (Billable)
(1, 'SAFE-001', 0),  -- Fire Safety Training (Not Billable)
(1, 'SAFE-003', 0),  -- First Aid Basics (Not Billable)

-- Jane Smith's enrollments
(2, 'TECH-002', 1),  -- ASP.NET Core Web API (Billable)
(2, 'TECH-003', 1),  -- React Development (Billable)
(2, 'SAFE-001', 0),  -- Fire Safety Training (Not Billable)

-- Michael Johnson's enrollments
(3, 'TECH-005', 1),  -- SQL Server Advanced (Billable)
(3, 'SAFE-002', 0),  -- Emergency Evacuation (Not Billable)

-- Sarah Williams's enrollments
(4, 'TECH-006', 1),  -- Azure Cloud Services (Billable)
(4, 'SAFE-005', 0),  -- Workplace Ergonomics (Not Billable)

-- David Brown's enrollments
(5, 'TECH-007', 1),  -- Docker & Kubernetes (Billable)
(5, 'SAFE-003', 0),  -- First Aid Basics (Not Billable)

-- Emily Davis's enrollments
(6, 'TECH-008', 1),  -- Python for Data Science (Billable)
(6, 'SAFE-006', 0),  -- Chemical Safety (Not Billable)

-- Robert Miller's enrollments
(7, 'TECH-004', 1),  -- Angular Essentials (Billable)
(7, 'SAFE-007', 0),  -- Electrical Safety (Not Billable)

-- Lisa Wilson's enrollments
(8, 'TECH-001', 1),  -- .NET 9 Fundamentals (Billable)
(8, 'SAFE-008', 0),  -- Slip and Fall Prevention (Not Billable)

-- James Moore's enrollments
(9, 'TECH-003', 1),  -- React Development (Billable)

-- Maria Taylor's enrollments
(10, 'SAFE-004', 0); -- CPR Certification (Not Billable)
GO

-- =============================================
-- Verification Queries
-- =============================================

-- Count records
SELECT 'Employees' AS TableName, COUNT(*) AS RecordCount FROM Employees
UNION ALL
SELECT 'Trainings', COUNT(*) FROM Trainings
UNION ALL
SELECT 'Enrollments', COUNT(*) FROM Enrollments;
GO

-- View enrollments with full details
SELECT 
    e.Id AS EnrollmentId,
    emp.FirstName + ' ' + emp.LastName AS EmployeeName,
    t.Title AS TrainingTitle,
    t.TrainingType,
    CASE 
        WHEN t.TrainingType = 'Technical' THEN 'Tech: ' + t.Technology
        WHEN t.TrainingType = 'Safety' THEN 'Risk: ' + t.RiskLevel
    END AS Details,
    CASE WHEN e.IsBillable = 1 THEN 'Yes' ELSE 'No' END AS Billable,
    e.EnrollmentDate
FROM Enrollments e
INNER JOIN Employees emp ON e.EmployeeId = emp.Id
INNER JOIN Trainings t ON e.TrainingId = t.Id
ORDER BY e.EnrollmentDate DESC;
GO

-- =============================================
-- Database Setup Complete!
-- =============================================
PRINT 'Database EmployeeTrainingDb created successfully!';
PRINT 'Tables created: Employees, Trainings, Enrollments';
PRINT 'Sample data inserted successfully!';
PRINT '10 Employees, 16 Trainings (8 Technical + 8 Safety), 20 Enrollments';
GO