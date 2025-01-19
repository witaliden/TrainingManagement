namespace TrainingManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalTrainings { get; set; }
        public int CompletedTrainingsCount { get; set; }
        public List<Training> UpcomingTrainings { get; set; }
        public List<UserProgressViewModel> TopUsers { get; set; }
        public UserProgressViewModel UserProgress { get; set; }
    }
}
