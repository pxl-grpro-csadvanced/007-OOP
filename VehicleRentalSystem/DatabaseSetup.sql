-- Vehicle Rental System Database Setup Script
-- Run this script to create the database and tables

-- Create the database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'VehicleRentalDb')
BEGIN
    CREATE DATABASE VehicleRentalDb;
END
GO

USE VehicleRentalDb;
GO

-- Drop tables if they exist (in correct order due to foreign keys)
IF OBJECT_ID('Rentals', 'U') IS NOT NULL
    DROP TABLE Rentals;
IF OBJECT_ID('Vehicles', 'U') IS NOT NULL
    DROP TABLE Vehicles;
IF OBJECT_ID('Customers', 'U') IS NOT NULL
    DROP TABLE Customers;
GO

-- Create Customers table
CREATE TABLE Customers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    DriverLicenseNumber NVARCHAR(50) NOT NULL
);
GO

-- Create Vehicles table (supports all vehicle types with nullable specific fields)
CREATE TABLE Vehicles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    LicensePlate NVARCHAR(20) NOT NULL UNIQUE,
    Brand NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Year INT NOT NULL,
    DailyRate DECIMAL(10,2) NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    VehicleType NVARCHAR(50) NOT NULL, -- 'Car', 'Motorcycle', 'Truck'
    
    -- Car specific fields
    NumberOfDoors INT NULL,
    FuelType NVARCHAR(50) NULL,
    
    -- Motorcycle specific fields
    EngineCapacity INT NULL,
    HasSidecar BIT NULL,
    
    -- Truck specific fields
    LoadCapacity DECIMAL(10,2) NULL,
    NumberOfAxles INT NULL
);
GO

-- Create Rentals table
CREATE TABLE Rentals (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    VehicleId INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    TotalCost DECIMAL(10,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_Rentals_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    CONSTRAINT FK_Rentals_Vehicles FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id)
);
GO

-- Insert sample data

-- Sample Customers
INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber) VALUES
('John', 'Smith', 'john.smith@email.com', '+32-123-456-789', 'DL123456'),
('Emma', 'Johnson', 'emma.johnson@email.com', '+32-234-567-890', 'DL234567'),
('Michael', 'Williams', 'michael.w@email.com', '+32-345-678-901', 'DL345678'),
('Sarah', 'Brown', 'sarah.brown@email.com', '+32-456-789-012', 'DL456789'),
('David', 'Jones', 'david.jones@email.com', '+32-567-890-123', 'DL567890');
GO

-- Sample Cars
INSERT INTO Vehicles (LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, NumberOfDoors, FuelType) VALUES
('ABC-123', 'Toyota', 'Corolla', 2022, 45.00, 1, 'Car', 4, 'Petrol'),
('DEF-456', 'Volkswagen', 'Golf', 2023, 50.00, 1, 'Car', 5, 'Diesel'),
('GHI-789', 'Tesla', 'Model 3', 2024, 85.00, 1, 'Car', 4, 'Electric'),
('JKL-012', 'BMW', '3 Series', 2023, 75.00, 1, 'Car', 4, 'Hybrid');
GO

-- Sample Motorcycles
INSERT INTO Vehicles (LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, EngineCapacity, HasSidecar) VALUES
('MNO-345', 'Harley-Davidson', 'Sportster', 2022, 60.00, 1, 'Motorcycle', 883, 0),
('PQR-678', 'Yamaha', 'YZF-R1', 2023, 70.00, 1, 'Motorcycle', 998, 0),
('STU-901', 'Honda', 'CB500X', 2021, 40.00, 1, 'Motorcycle', 471, 0),
('VWX-234', 'BMW', 'R1250GS', 2024, 90.00, 1, 'Motorcycle', 1254, 1);
GO

-- Sample Trucks
INSERT INTO Vehicles (LicensePlate, Brand, Model, Year, DailyRate, IsAvailable, VehicleType, LoadCapacity, NumberOfAxles) VALUES
('YZA-567', 'Mercedes-Benz', 'Sprinter', 2022, 95.00, 1, 'Truck', 3500, 2),
('BCD-890', 'Ford', 'Transit', 2023, 85.00, 1, 'Truck', 2800, 2),
('EFG-123', 'Volvo', 'FH16', 2021, 150.00, 1, 'Truck', 7500, 3),
('HIJ-456', 'Scania', 'R450', 2024, 180.00, 1, 'Truck', 9000, 4);
GO

-- Sample Rentals
INSERT INTO Rentals (CustomerId, VehicleId, StartDate, EndDate, TotalCost, IsActive) VALUES
(1, 1, '2024-04-10', '2024-04-15', 225.00, 0),
(2, 5, '2024-04-12', '2024-04-14', 120.00, 0),
(3, 9, '2024-04-01', '2024-04-05', 475.00, 0);
GO

-- Verify data
SELECT 'Customers' as TableName, COUNT(*) as RecordCount FROM Customers
UNION ALL
SELECT 'Vehicles', COUNT(*) FROM Vehicles
UNION ALL
SELECT 'Rentals', COUNT(*) FROM Rentals;
GO

PRINT 'Database setup completed successfully!';
PRINT 'Total Customers: ' + CAST((SELECT COUNT(*) FROM Customers) AS NVARCHAR(10));
PRINT 'Total Vehicles: ' + CAST((SELECT COUNT(*) FROM Vehicles) AS NVARCHAR(10));
PRINT 'Total Rentals: ' + CAST((SELECT COUNT(*) FROM Rentals) AS NVARCHAR(10));
GO
