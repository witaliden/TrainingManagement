namespace TrainingManagement.Models
{
    public class UserTraining
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public int TrainingId { get; set; }
        public Training Training { get; set; }
        public bool IsCompleted { get; set; }
    }
}
