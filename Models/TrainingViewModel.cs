namespace TrainingManagement.Models
{
    public class TrainingViewModel
    {
        public int TrainingId { get; set; }
        public string? TrainingTitle { get; set; }
        public List<UserAssignedViewModel>? Users { get; set; }
    }

    public class UserAssignedViewModel
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
