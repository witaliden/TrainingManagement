namespace TrainingManagement.Models
{
    public class DashboardViewModel
    {
        public List<string> Labels { get; set; }
        public List<int> CompletedTrainings { get; set; }
        public int TotalUsers { get; set; }
        public int TotalTrainings { get; set; }
        public int CompletedTrainingsCount { get; set; }
        public List<Training> UpcomingTrainings { get; set; }
    }
}
