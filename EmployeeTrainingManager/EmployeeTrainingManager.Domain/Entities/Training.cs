namespace EmployeeTrainingManager.Domain.Entities
{
    public class Training
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TrainerName { get; set; }
        public int DurationInHours { get; set; }

        protected Training()
        {
            Id = string.Empty;
            Title = string.Empty;
            TrainerName = string.Empty;
        }

        protected Training(string id, string title, string trainerName, int durationInHours)
        {
            Id = id;
            Title = title;
            TrainerName = trainerName;
            DurationInHours = durationInHours;
        }
    }
}
