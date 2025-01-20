namespace TrainingManagement.Models
{
    public class AssignTrainingViewModel
    {
        public int TrainingId { get; set; }
        public string TrainingTitle { get; set; }
        public List<UserViewModel> Users { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
