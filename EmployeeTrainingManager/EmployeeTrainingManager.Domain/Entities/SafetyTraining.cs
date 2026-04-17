namespace EmployeeTrainingManager.Domain.Entities
{
    public class SafetyTraining : Training, ICertificateProvider
    {
        public string RiskLevel { get; set; }
        private bool _certificateGranted;

        public SafetyTraining()
        {
            RiskLevel = string.Empty;
        }

        public SafetyTraining(string trainingId, string title, string trainerName, int duration, string riskLevel)
            : base(trainingId, title, trainerName, duration)
        {
            RiskLevel = riskLevel;
        }

        public void GrantCertificate()
        {
            _certificateGranted = true;
        }

        public override string ShowInfo()
        {
            return $"Safety Training: {Title} - Risk Level: {RiskLevel}";
        }

        public override string ToString()
        {
            return $"{Title} (Risk: {RiskLevel})";
        }
    }
}
