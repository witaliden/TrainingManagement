namespace TrainingManagement.Models
{
    public class UserTraining
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public int TrainingId { get; set; }
        public Training Training { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDateTime { get; set; }
    }

    public class AssignTrainingsViewModel
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public List<TrainingAssignmentViewModel>? Trainings { get; set; }
    }

    public class TrainingAssignmentViewModel
    {
        public int TrainingId { get; set; }
        public string? Title { get; set; }
        public bool IsAssigned { get; set; }
    }
}
