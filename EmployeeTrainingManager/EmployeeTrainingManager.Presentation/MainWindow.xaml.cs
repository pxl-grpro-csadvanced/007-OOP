using System.Windows;
using System.Windows.Controls;
using EmployeeTrainingManager.Application.Services;
using EmployeeTrainingManager.Domain.Entities;
using EmployeeTrainingManager.Infrastructure.Data;
using EmployeeTrainingManager.Infrastructure.Repositories;

namespace EmployeeTrainingManager.Presentation;

public class EmployeeDisplay
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Employee OriginalEmployee { get; set; } = null!;
}

public class EnrollmentDisplay
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string TrainingTitle { get; set; } = string.Empty;
    public bool IsBillable { get; set; }
    public Enrollment OriginalEnrollment { get; set; } = null!;
}

public partial class MainWindow : Window
{
    private readonly EmployeeService _employeeService;
    private readonly TrainingService _trainingService;
    private readonly EnrollmentService _enrollmentService;
    private List<Employee> _allEmployees = new();
    private List<Training> _allTrainings = new();

    public MainWindow()
    {
        InitializeComponent();
        
        var connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=EmployeeTrainingDb;Data Source=.\\sqlexpress;TrustServerCertificate=True";
        var dbFactory = new DbConnectionFactory(connectionString);
        
        var employeeRepository = new EmployeeRepository(dbFactory);
        var trainingRepository = new TrainingRepository(dbFactory);
        var enrollmentRepository = new EnrollmentRepository(dbFactory, employeeRepository, trainingRepository);
        
        _employeeService = new EmployeeService(employeeRepository);
        _trainingService = new TrainingService(trainingRepository);
        _enrollmentService = new EnrollmentService(enrollmentRepository, employeeRepository, trainingRepository);
        
        LoadAllData();
    }

    private async void LoadAllData()
    {
        await LoadEmployees();
        await LoadTrainings();
    }

    private async Task LoadEmployees()
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            _allEmployees = employees.ToList();
            
            var employeeDisplayList = _allEmployees.Select(e => new EmployeeDisplay
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                OriginalEmployee = e
            }).ToList();
            
            EmployeeDataGrid.ItemsSource = employeeDisplayList;
            
            EmployeeComboBox.Items.Clear();
            foreach (var employee in _allEmployees)
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = $"{employee.FirstName} {employee.LastName}",
                    Tag = employee
                };
                EmployeeComboBox.Items.Add(comboBoxItem);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoadTrainings()
    {
        try
        {
            var trainings = await _trainingService.GetAllTrainingsAsync();
            _allTrainings = trainings.ToList();
            
            TrainingListBox.Items.Clear();
            foreach (var training in _allTrainings)
            {
                var displayText = $"{training.Title}\nTrainer: {training.TrainerName}";
                var listBoxItem = new ListBoxItem
                {
                    Content = displayText,
                    Tag = training
                };
                TrainingListBox.Items.Add(listBoxItem);
            }
            
            TrainingComboBox.Items.Clear();
            foreach (var training in _allTrainings)
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = training.Title,
                    Tag = training
                };
                TrainingComboBox.Items.Add(comboBoxItem);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading trainings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoadEmployeeEnrollments(int employeeId)
    {
        try
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByEmployeeIdAsync(employeeId);
            
            var enrollmentDisplayList = enrollments.Select(e => new EnrollmentDisplay
            {
                Id = e.Id,
                EmployeeName = $"{e.Employee.FirstName} {e.Employee.LastName}",
                TrainingTitle = e.Training.Title,
                IsBillable = e.IsBillable,
                OriginalEnrollment = e
            }).ToList();
            
            EnrollmentDataGrid.ItemsSource = enrollmentDisplayList;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading enrollments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
    {
        var firstName = FirstNameTextBox.Text;
        var lastName = LastNameTextBox.Text;

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            MessageBox.Show("Please enter first and last name", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _employeeService.AddEmployeeAsync(firstName, lastName);
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            await LoadEmployees();
            MessageBox.Show("Employee added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedDisplay = EmployeeDataGrid.SelectedItem as EmployeeDisplay;
        if (selectedDisplay == null)
        {
            MessageBox.Show("Please select an employee to delete", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedEmployee = selectedDisplay.OriginalEmployee;
        var result = MessageBox.Show($"Are you sure you want to delete {selectedEmployee.FirstName} {selectedEmployee.LastName}?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(selectedEmployee.Id);
                await LoadEmployees();
                MessageBox.Show("Employee deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void AddTrainingButton_Click(object sender, RoutedEventArgs e)
    {
        var trainingId = TrainingIdTextBox.Text;
        var title = TrainingTitleTextBox.Text;
        var trainerName = TrainerNameTextBox.Text;

        if (string.IsNullOrWhiteSpace(trainingId) || string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(trainerName))
        {
            MessageBox.Show("Please enter training ID, title, and trainer name", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var selectedType = (TrainingTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            
            if (selectedType == "Technical")
            {
                var technology = TechnologyTextBox.Text;
                if (string.IsNullOrWhiteSpace(technology))
                {
                    MessageBox.Show("Please enter technology for technical training", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                await _trainingService.AddTechnicalTrainingAsync(trainingId, title, trainerName, technology);
            }
            else
            {
                var riskLevel = (RiskLevelComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (string.IsNullOrWhiteSpace(riskLevel))
                {
                    MessageBox.Show("Please select risk level for safety training", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                int duration = 0;
                if (!int.TryParse(DurationTextBox.Text, out duration))
                {
                    MessageBox.Show("Please enter a valid duration", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                await _trainingService.AddSafetyTrainingAsync(trainingId, title, trainerName, duration, riskLevel);
            }

            ClearTrainingFields();
            await LoadTrainings();
            MessageBox.Show("Training added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding training: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void DeleteTrainingButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = TrainingListBox.SelectedItem as ListBoxItem;
        if (selectedItem == null)
        {
            MessageBox.Show("Please select a training to delete", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedTraining = selectedItem.Tag as Training;
        if (selectedTraining == null) return;

        var result = MessageBox.Show($"Are you sure you want to delete {selectedTraining.Title}?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _trainingService.DeleteTrainingAsync(selectedTraining.Id);
                await LoadTrainings();
                MessageBox.Show("Training deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting training: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void EnrollButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedEmployeeItem = EmployeeComboBox.SelectedItem as ComboBoxItem;
        var selectedTrainingItem = TrainingComboBox.SelectedItem as ComboBoxItem;

        if (selectedEmployeeItem == null || selectedTrainingItem == null)
        {
            MessageBox.Show("Please select both an employee and a training", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedEmployee = selectedEmployeeItem.Tag as Employee;
        var selectedTraining = selectedTrainingItem.Tag as Training;
        
        if (selectedEmployee == null || selectedTraining == null) return;

        try
        {
            var isBillable = BillableCheckBox.IsChecked ?? false;
            await _enrollmentService.EnrollEmployeeAsync(selectedEmployee.Id, selectedTraining.Id, isBillable);
            await LoadEmployeeEnrollments(selectedEmployee.Id);
            MessageBox.Show($"{selectedEmployee.FirstName} {selectedEmployee.LastName} enrolled in {selectedTraining.Title}!",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error enrolling employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void DeleteEnrollmentButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedDisplay = EnrollmentDataGrid.SelectedItem as EnrollmentDisplay;
        if (selectedDisplay == null)
        {
            MessageBox.Show("Please select an enrollment to delete", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedEnrollment = selectedDisplay.OriginalEnrollment;
        var result = MessageBox.Show("Are you sure you want to delete this enrollment?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _enrollmentService.DeleteEnrollmentAsync(selectedEnrollment.Id);
                var currentEmployeeItem = EmployeeComboBox.SelectedItem as ComboBoxItem;
                if (currentEmployeeItem?.Tag is Employee currentEmployee)
                {
                    await LoadEmployeeEnrollments(currentEmployee.Id);
                }
                MessageBox.Show("Enrollment deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting enrollment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadEmployees();
        await LoadTrainings();
        MessageBox.Show("Data refreshed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void EmployeeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedDisplay = EmployeeDataGrid.SelectedItem as EmployeeDisplay;
        if (selectedDisplay != null)
        {
            await LoadEmployeeEnrollments(selectedDisplay.OriginalEmployee.Id);
        }
    }

    private void TrainingTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TrainingTypeComboBox == null) return;
        
        var selectedType = (TrainingTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        
        if (TechnicalFieldsPanel != null && SafetyFieldsPanel != null)
        {
            if (selectedType == "Technical")
            {
                TechnicalFieldsPanel.Visibility = Visibility.Visible;
                SafetyFieldsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                TechnicalFieldsPanel.Visibility = Visibility.Collapsed;
                SafetyFieldsPanel.Visibility = Visibility.Visible;
            }
        }
    }

    private void ClearTrainingFields()
    {
        TrainingIdTextBox.Clear();
        TrainingTitleTextBox.Clear();
        TrainerNameTextBox.Clear();
        TechnologyTextBox.Clear();
        DurationTextBox.Clear();
        RiskLevelComboBox.SelectedIndex = -1;
    }
}