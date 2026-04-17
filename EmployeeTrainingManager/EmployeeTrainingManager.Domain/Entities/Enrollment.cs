namespace EmployeeTrainingManager.Domain.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public Training Training { get; set; }
        public bool IsBillable { get; set; }

        public Enrollment()
        {
            Employee = new Employee();
            Training = null!;
        }

        public Enrollment(int id, Training training, Employee employee)
        {
            Id = id;
            Training = training;
            Employee = employee;
        }
    }
}
