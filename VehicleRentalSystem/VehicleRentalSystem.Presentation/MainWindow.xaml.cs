using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VehicleRentalSystem.Application.Services;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interfaces;
using VehicleRentalSystem.Infrastructure;
using VehicleRentalSystem.Infrastructure.Repositories;

namespace VehicleRentalSystem.Presentation;

public partial class MainWindow : Window
{
    private readonly VehicleService _vehicleService;
    private readonly CustomerService _customerService;
    private readonly RentalService _rentalService;

    private List<Vehicle> _vehicles = [];
    private List<Customer> _customers = [];
    private List<Rental> _rentals = [];

    public MainWindow()
    {
        InitializeComponent();

        var connectionString =
            "Server=.\\sqlexpress;Database=VehicleRentalDb;Trusted_Connection=True;" +
            "TrustServerCertificate=True;Encrypt=False;";
        var dbFactory = new DbConnectionFactory(connectionString);

        IVehicleRepository vehicleRepository = new VehicleRepository(dbFactory);
        ICustomerRepository customerRepository = new CustomerDbRepository(dbFactory);
        //ICustomerRepository customerRepository = new CustomerApiRepository();
        IRentalRepository rentalRepository = new RentalRepository(dbFactory, customerRepository, vehicleRepository);

        _vehicleService = new VehicleService(vehicleRepository);
        _customerService = new CustomerService(customerRepository);
        _rentalService = new RentalService(rentalRepository, vehicleRepository, customerRepository);


    }



    // ──────────────────────────────────────────────
    // Data loading
    // ──────────────────────────────────────────────

    private async void LoadAllDataAsync(object sender, RoutedEventArgs e)
    {
        await LoadAllDataAsync();
    }
    private async Task LoadAllDataAsync()
    {
        await LoadVehiclesAsync();
        await LoadCustomersAsync();
        await LoadRentalsAsync();
    }

    private async Task LoadVehiclesAsync()
    {
        try
        {
            _vehicles = (await _vehicleService.GetAllVehiclesAsync()).ToList();

            VehicleDataGrid.ItemsSource = _vehicles.ToList();
        }
        catch (Exception ex)
        {
            ShowError("Loading vehicles failed", ex);
        }
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            _customers = (await _customerService.GetAllCustomersAsync()).ToList();

            CustomerDataGrid.ItemsSource = _customers.ToList();

            RentalCustomerComboBox.ItemsSource = _customers.ToList();
            //RentalCustomerComboBox.DisplayMemberPath = "Display";
        }
        catch (Exception ex)
        {
            ShowError("Loading customers failed", ex);
        }
    }

    private async Task LoadRentalsAsync()
    {
        try
        {
            var available = (await _vehicleService.GetAvailableVehiclesAsync()).ToList();

            RentalVehicleComboBox.ItemsSource = available.ToList();

            _rentals = (await _rentalService.GetActiveRentalsAsync()).ToList();

            RentalDataGrid.ItemsSource = _rentals.ToList();
                
        }
        catch (Exception ex)
        {
            ShowError("Loading rentals failed", ex);
        }
    }

    // ──────────────────────────────────────────────
    // Vehicles tab
    // ──────────────────────────────────────────────

    private void VehicleTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CarFieldsPanel == null) return;
        var selected = (VehicleTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
        CarFieldsPanel.Visibility      = selected == "Car"        ? Visibility.Visible : Visibility.Collapsed;
        MotorcycleFieldsPanel.Visibility = selected == "Motorcycle" ? Visibility.Visible : Visibility.Collapsed;
        TruckFieldsPanel.Visibility    = selected == "Truck"      ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void AddVehicleButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var type  = (VehicleTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var plate = LicensePlateTextBox.Text.Trim();
            var brand = BrandTextBox.Text.Trim();
            var model = ModelTextBox.Text.Trim();

            if (!int.TryParse(YearTextBox.Text, out int year))
                throw new ArgumentException("Year must be a whole number.");
            if (!decimal.TryParse(DailyRateTextBox.Text, out decimal rate) || rate <= 0)
                throw new ArgumentException("Daily rate must be a positive number.");

            switch (type)
            {
                case "Car":
                    if (!int.TryParse(NumberOfDoorsTextBox.Text, out int doors))
                        throw new ArgumentException("Number of doors must be a whole number.");
                    var fuel = (FuelTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Petrol";
                    await _vehicleService.AddCarAsync(plate, brand, model, year, rate, doors, fuel);
                    break;

                case "Motorcycle":
                    if (!int.TryParse(EngineCapacityTextBox.Text, out int cc))
                        throw new ArgumentException("Engine capacity must be a whole number.");
                    await _vehicleService.AddMotorcycleAsync(plate, brand, model, year, rate, cc,
                        HasSidecarCheckBox.IsChecked == true);
                    break;

                case "Truck":
                    if (!decimal.TryParse(LoadCapacityTextBox.Text, out decimal load) || load <= 0)
                        throw new ArgumentException("Load capacity must be a positive number.");
                    if (!int.TryParse(NumberOfAxlesTextBox.Text, out int axles))
                        throw new ArgumentException("Number of axles must be a whole number.");
                    await _vehicleService.AddTruckAsync(plate, brand, model, year, rate, load, axles);
                    break;
            }

            ClearVehicleForm();
            await LoadVehiclesAsync();
            MessageBox.Show("Vehicle added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ShowError("Adding vehicle failed", ex);
        }
    }

    private async void DeleteVehicleButton_Click(object sender, RoutedEventArgs e)
    {
        int index = VehicleDataGrid.SelectedIndex;
        if (index < 0) return;

        var vehicle = _vehicles[index];

        var result = MessageBox.Show(
            $"Delete {vehicle.Brand} {vehicle.Model} ({vehicle.LicensePlate})?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _vehicleService.DeleteVehicleAsync(vehicle.Id);
            await LoadVehiclesAsync();
        }
        catch (Exception ex)
        {
            ShowError("Deleting vehicle failed", ex);
        }
    }

    private async void RefreshVehiclesButton_Click(object sender, RoutedEventArgs e) =>
        await LoadVehiclesAsync();

    private void ClearVehicleForm()
    {
        LicensePlateTextBox.Clear();
        BrandTextBox.Clear();
        ModelTextBox.Clear();
        YearTextBox.Clear();
        DailyRateTextBox.Clear();
        NumberOfDoorsTextBox.Clear();
        EngineCapacityTextBox.Clear();
        HasSidecarCheckBox.IsChecked = false;
        LoadCapacityTextBox.Clear();
        NumberOfAxlesTextBox.Clear();
    }

    // ──────────────────────────────────────────────
    // Customers tab
    // ──────────────────────────────────────────────

    private async void AddCustomerButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var firstName = CustomerFirstNameTextBox.Text.Trim();
            var lastName  = CustomerLastNameTextBox.Text.Trim();
            var email     = EmailTextBox.Text.Trim();
            var phone     = PhoneNumberTextBox.Text.Trim();
            var license   = DriverLicenseTextBox.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                throw new ArgumentException("First name and last name are required.");
            if (!email.Contains('@'))
                throw new ArgumentException("A valid email address is required.");

            await _customerService.AddCustomerAsync(firstName, lastName, email, phone, license);

            ClearCustomerForm();
            await LoadCustomersAsync();
            MessageBox.Show("Customer added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ShowError("Adding customer failed", ex);
        }
    }

    private async void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
    {
        int index = CustomerDataGrid.SelectedIndex;
        if (index < 0) return;

        var customer = _customers[index];

        var result = MessageBox.Show(
            $"Delete customer {customer.GetFullName()}?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _customerService.DeleteCustomerAsync(customer.Id);
            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            ShowError("Deleting customer failed", ex);
        }
    }

    private async void RefreshCustomersButton_Click(object sender, RoutedEventArgs e) =>
        await LoadCustomersAsync();

    private void ClearCustomerForm()
    {
        CustomerFirstNameTextBox.Clear();
        CustomerLastNameTextBox.Clear();
        EmailTextBox.Clear();
        PhoneNumberTextBox.Clear();
        DriverLicenseTextBox.Clear();
    }

    // ──────────────────────────────────────────────
    // Rentals tab
    // ──────────────────────────────────────────────

    private async void CreateRentalButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (RentalCustomerComboBox.SelectedIndex < 0)
                throw new ArgumentException("Please select a customer.");
            if (RentalVehicleComboBox.SelectedIndex < 0)
                throw new ArgumentException("Please select a vehicle.");
            if (StartDatePicker.SelectedDate == null)
                throw new ArgumentException("Please select a start date.");
            if (EndDatePicker.SelectedDate == null)
                throw new ArgumentException("Please select an end date.");

            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate   = EndDatePicker.SelectedDate.Value;

            if (endDate <= startDate)
                throw new ArgumentException("End date must be after start date.");

            // Retrieve the actual domain objects by index
            Customer customerItem = (Customer)RentalCustomerComboBox.SelectedItem!;
            Vehicle vehicleItem  = (Vehicle)RentalVehicleComboBox.SelectedItem!;
            int customerId   = customerItem.Id;
            int vehicleId    = vehicleItem.Id;

            var vehicle = _vehicles.First(v => v.Id == vehicleId);

            await _rentalService.CreateRentalAsync(customerId, vehicleId, startDate, endDate);

            int days          = (endDate - startDate).Days;
            decimal rentalCost    = vehicle.CalculateRentalCost(days);
            decimal insuranceCost = _rentalService.CalculateInsuranceCost(vehicle, days);
            decimal totalCost     = rentalCost + insuranceCost;

            var message =
                $"Rental created!\n\n" +
                $"Customer: {customerItem.GetFullName()}\n" +
                $"Vehicle: {vehicle.Brand} {vehicle.Model}\n" +
                $"Days: {days}\n" +
                $"Rental cost: {rentalCost:C}\n" +
                $"Insurance cost: {insuranceCost:C}\n" +
                $"Total cost: {totalCost:C}";

            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadRentalsAsync();
            await LoadVehiclesAsync();
        }
        catch (Exception ex)
        {
            ShowError("Creating rental failed", ex);
        }
    }

    private async void CompleteRentalButton_Click(object sender, RoutedEventArgs e)
    {
        int index = RentalDataGrid.SelectedIndex;
        if (index < 0) return;

        var rental = _rentals[index];

        if (!rental.IsActive)
        {
            MessageBox.Show("This rental is already completed.", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show(
            $"Complete rental #{rental.Id} for {rental.Customer?.GetFullName()}?",
            "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _rentalService.CompleteRentalAsync(rental.Id);
            await LoadRentalsAsync();
            await LoadVehiclesAsync();
        }
        catch (Exception ex)
        {
            ShowError("Completing rental failed", ex);
        }
    }

    private async void RefreshRentalsButton_Click(object sender, RoutedEventArgs e) =>
        await LoadRentalsAsync();

    // ──────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────

    private static void ShowError(string title, Exception ex) =>
        MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);


}
