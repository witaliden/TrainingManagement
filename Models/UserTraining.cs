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
}
