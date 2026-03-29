namespace EmployeeTrainingManager.Domain.Entities
{
    public class TechnicalTraining : Training
    {
        public string Technology { get; set; }

        public TechnicalTraining()
        {
            Technology = string.Empty;
        }

        public TechnicalTraining(string trainingId, string title, string trainerName, string technology)
            : base(trainingId, title, trainerName, 0)
        {
            Technology = technology;
        }

        public string ShowInfo()
        {
            return $"Technical Training: {Title} - Technology: {Technology}";
        }

        public override string ToString()
        {
            return $"{Title} ({Technology})";
        }
    }
}
