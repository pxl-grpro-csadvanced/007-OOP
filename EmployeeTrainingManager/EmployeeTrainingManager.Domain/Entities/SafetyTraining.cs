namespace EmployeeTrainingManager.Domain.Entities
{
    public class SafetyTraining : Training
    {
        public string RiskLevel { get; set; }

        public SafetyTraining()
        {
            RiskLevel = string.Empty;
        }

        public SafetyTraining(string trainingId, string title, string trainerName, int duration, string riskLevel)
            : base(trainingId, title, trainerName, duration)
        {
            RiskLevel = riskLevel;
        }

        public string ShowInfo()
        {
            return $"Safety Training: {Title} - Risk Level: {RiskLevel}";
        }

        public override string ToString()
        {
            return $"{Title} (Risk: {RiskLevel})";
        }
    }
}
