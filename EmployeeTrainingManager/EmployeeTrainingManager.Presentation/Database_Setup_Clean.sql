-- =============================================
-- Employee Training Manager Database - CLEAN SETUP
-- =============================================
-- This script creates the database and tables WITHOUT sample data
-- Use this if you want to start with an empty database
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
-- =============================================
CREATE TABLE Trainings
(
    Id NVARCHAR(50) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    TrainerName NVARCHAR(100) NOT NULL,
    DurationInHours INT NOT NULL DEFAULT 0,
    TrainingType NVARCHAR(50) NOT NULL,
    Technology NVARCHAR(100) NULL,      -- For Technical Training
    RiskLevel NVARCHAR(50) NULL,        -- For Safety Training
    CONSTRAINT CK_Trainings_Title CHECK (LEN(Title) > 0),
    CONSTRAINT CK_Trainings_TrainerName CHECK (LEN(TrainerName) > 0),
    CONSTRAINT CK_Trainings_Type CHECK (TrainingType IN ('Technical', 'Safety')),
    CONSTRAINT CK_Trainings_RiskLevel CHECK (RiskLevel IS NULL OR RiskLevel IN ('Low', 'Medium', 'High'))
);
GO

-- =============================================
-- Table: Enrollments
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
-- Indexes for better performance
-- =============================================
CREATE INDEX IX_Enrollments_EmployeeId ON Enrollments(EmployeeId);
CREATE INDEX IX_Enrollments_TrainingId ON Enrollments(TrainingId);
GO

PRINT 'Database EmployeeTrainingDb created successfully!';
PRINT 'Tables: Employees, Trainings, Enrollments';
PRINT 'Database is ready to use!';
GO
