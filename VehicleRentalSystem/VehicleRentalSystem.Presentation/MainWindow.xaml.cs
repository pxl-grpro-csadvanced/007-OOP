using System.Windows;
using System.Windows.Controls;
using VehicleRentalSystem.Application.Services;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Infrastructure.Data;
using VehicleRentalSystem.Infrastructure.Repositories;

namespace VehicleRentalSystem.Presentation;

public partial class MainWindow : Window
{
    private readonly VehicleService _vehicleService;
    private readonly CustomerService _customerService;
    private readonly RentalService _rentalService;

    public MainWindow()
    {
        InitializeComponent();

        var connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=VehicleRentalDb;Data Source=.\\sqlexpress;TrustServerCertificate=True";
        var dbFactory = new DbConnectionFactory(connectionString);

        var vehicleRepository = new VehicleRepository(dbFactory);
        var customerRepository = new CustomerRepository(dbFactory);
        var rentalRepository = new RentalRepository(dbFactory, customerRepository, vehicleRepository);

        _vehicleService = new VehicleService(vehicleRepository);
        _customerService = new CustomerService(customerRepository);
        _rentalService = new RentalService(rentalRepository, vehicleRepository, customerRepository);

        StartDatePicker.SelectedDate = DateTime.Today;
        EndDatePicker.SelectedDate = DateTime.Today.AddDays(1);

        LoadAllData();
    }

    private async void LoadAllData()
    {
        await LoadVehicles();
        await LoadCustomers();
        await LoadRentals();
    }

    #region Vehicle Management

    private async Task LoadVehicles()
    {
        try
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            VehicleDataGrid.ItemsSource = vehicles;

            var availableVehicles = await _vehicleService.GetAvailableVehiclesAsync();
            RentalVehicleComboBox.ItemsSource = availableVehicles;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading vehicles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void AddVehicleButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var licensePlate = LicensePlateTextBox.Text;
            var brand = BrandTextBox.Text;
            var model = ModelTextBox.Text;

            if (string.IsNullOrWhiteSpace(licensePlate) || string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(model))
            {
                MessageBox.Show("Please fill in all required fields", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(YearTextBox.Text, out int year) || year < 1900 || year > DateTime.Now.Year + 1)
            {
                MessageBox.Show("Please enter a valid year", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(DailyRateTextBox.Text, out decimal dailyRate) || dailyRate <= 0)
            {
                MessageBox.Show("Please enter a valid daily rate", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedType = (VehicleTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            switch (selectedType)
            {
                case "Car":
                    if (!int.TryParse(NumberOfDoorsTextBox.Text, out int doors) || doors < 2 || doors > 5)
                    {
                        MessageBox.Show("Please enter a valid number of doors (2-5)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    var fuelType = (FuelTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Petrol";
                    await _vehicleService.AddCarAsync(licensePlate, brand, model, year, dailyRate, doors, fuelType);
                    break;

                case "Motorcycle":
                    if (!int.TryParse(EngineCapacityTextBox.Text, out int capacity) || capacity < 50 || capacity > 2000)
                    {
                        MessageBox.Show("Please enter a valid engine capacity (50-2000)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    var hasSidecar = HasSidecarCheckBox.IsChecked ?? false;
                    await _vehicleService.AddMotorcycleAsync(licensePlate, brand, model, year, dailyRate, capacity, hasSidecar);
                    break;

                case "Truck":
                    if (!decimal.TryParse(LoadCapacityTextBox.Text, out decimal loadCapacity) || loadCapacity <= 0)
                    {
                        MessageBox.Show("Please enter a valid load capacity", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (!int.TryParse(NumberOfAxlesTextBox.Text, out int axles) || axles < 2 || axles > 10)
                    {
                        MessageBox.Show("Please enter a valid number of axles (2-10)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    await _vehicleService.AddTruckAsync(licensePlate, brand, model, year, dailyRate, loadCapacity, axles);
                    break;
            }

            ClearVehicleFields();
            await LoadVehicles();
            MessageBox.Show("Vehicle added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding vehicle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void DeleteVehicleButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedVehicle = VehicleDataGrid.SelectedItem as Vehicle;
        if (selectedVehicle == null)
        {
            MessageBox.Show("Please select a vehicle to delete", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Are you sure you want to delete {selectedVehicle.Brand} {selectedVehicle.Model}?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _vehicleService.DeleteVehicleAsync(selectedVehicle.Id);
                await LoadVehicles();
                MessageBox.Show("Vehicle deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting vehicle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void RefreshVehiclesButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadVehicles();
    }

    private void VehicleTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (VehicleTypeComboBox == null) return;

        var selectedType = (VehicleTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

        if (CarFieldsPanel != null && MotorcycleFieldsPanel != null && TruckFieldsPanel != null)
        {
            CarFieldsPanel.Visibility = selectedType == "Car" ? Visibility.Visible : Visibility.Collapsed;
            MotorcycleFieldsPanel.Visibility = selectedType == "Motorcycle" ? Visibility.Visible : Visibility.Collapsed;
            TruckFieldsPanel.Visibility = selectedType == "Truck" ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void ClearVehicleFields()
    {
        LicensePlateTextBox.Clear();
        BrandTextBox.Clear();
        ModelTextBox.Clear();
        YearTextBox.Clear();
        DailyRateTextBox.Clear();
        NumberOfDoorsTextBox.Clear();
        FuelTypeComboBox.SelectedIndex = 0;
        EngineCapacityTextBox.Clear();
        HasSidecarCheckBox.IsChecked = false;
        LoadCapacityTextBox.Clear();
        NumberOfAxlesTextBox.Clear();
    }

    #endregion

    #region Customer Management

    private async Task LoadCustomers()
    {
        try
        {
            var customers = await _customerService.GetAllCustomersAsync();
            CustomerDataGrid.ItemsSource = customers;
            RentalCustomerComboBox.ItemsSource = customers;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void AddCustomerButton_Click(object sender, RoutedEventArgs e)
    {
        var firstName = CustomerFirstNameTextBox.Text;
        var lastName = CustomerLastNameTextBox.Text;
        var email = EmailTextBox.Text;
        var phone = PhoneNumberTextBox.Text;
        var license = DriverLicenseTextBox.Text;

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) ||
            string.IsNullOrWhiteSpace(license))
        {
            MessageBox.Show("Please fill in all fields", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _customerService.AddCustomerAsync(firstName, lastName, email, phone, license);
            ClearCustomerFields();
            await LoadCustomers();
            MessageBox.Show("Customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedCustomer = CustomerDataGrid.SelectedItem as Customer;
        if (selectedCustomer == null)
        {
            MessageBox.Show("Please select a customer to delete", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Are you sure you want to delete {selectedCustomer.GetFullName()}?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(selectedCustomer.Id);
                await LoadCustomers();
                MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void RefreshCustomersButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadCustomers();
    }

    private void ClearCustomerFields()
    {
        CustomerFirstNameTextBox.Clear();
        CustomerLastNameTextBox.Clear();
        EmailTextBox.Clear();
        PhoneNumberTextBox.Clear();
        DriverLicenseTextBox.Clear();
    }

    #endregion

    #region Rental Management

    private async Task LoadRentals()
    {
        try
        {
            var rentals = await _rentalService.GetActiveRentalsAsync();
            RentalDataGrid.ItemsSource = rentals;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading rentals: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void CreateRentalButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedCustomer = RentalCustomerComboBox.SelectedItem as Customer;
        var selectedVehicle = RentalVehicleComboBox.SelectedItem as Vehicle;

        if (selectedCustomer == null || selectedVehicle == null)
        {
            MessageBox.Show("Please select both a customer and a vehicle", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Please select start and end dates", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var startDate = StartDatePicker.SelectedDate.Value;
        var endDate = EndDatePicker.SelectedDate.Value;

        if (endDate <= startDate)
        {
            MessageBox.Show("End date must be after start date", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _rentalService.CreateRentalAsync(selectedCustomer.Id, selectedVehicle.Id, startDate, endDate);
            await LoadRentals();
            await LoadVehicles();

            int days = (endDate - startDate).Days;
            decimal rentalCost = selectedVehicle.CalculateRentalCost(days);
            decimal insuranceCost = _rentalService.CalculateInsuranceCost(selectedVehicle, days);
            decimal totalCost = rentalCost + insuranceCost;

            var message = $"Rental created successfully!\n\n" +
                         $"Customer: {selectedCustomer.GetFullName()}\n" +
                         $"Vehicle: {selectedVehicle.Brand} {selectedVehicle.Model}\n" +
                         $"Days: {days}\n" +
                         $"Rental Cost: {rentalCost:C}\n" +
                         $"Insurance Cost: {insuranceCost:C}\n" +
                         $"Total Cost: {totalCost:C}";

            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating rental: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void CompleteRentalButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedRental = RentalDataGrid.SelectedItem as Rental;
        if (selectedRental == null)
        {
            MessageBox.Show("Please select a rental to complete", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show("Are you sure you want to complete this rental?",
            "Confirm Complete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _rentalService.CompleteRentalAsync(selectedRental.Id);
                await LoadRentals();
                await LoadVehicles();
                MessageBox.Show("Rental completed successfully! Vehicle is now available.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error completing rental: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void RefreshRentalsButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadRentals();
    }

    #endregion
}
