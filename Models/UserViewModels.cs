using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserDetailsViewModel
    {
        public User User { get; set; }
        public List<UserTraining> UserTrainings { get; set; }
        public bool IsLockedOut { get; set; }
    }

    public class AssignTrainingsViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<TrainingAssignmentViewModel> Trainings { get; set; }
    }

    public class TrainingAssignmentViewModel
    {
        public int TrainingId { get; set; }
        public string Title { get; set; }
        public bool IsAssigned { get; set; }
    }

}
