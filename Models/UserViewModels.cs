using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Lastname { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
        public required string ConfirmPassword { get; set; }
    }

    public class EditUserViewModel
    {
        public required string Id { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public required string Lastname { get; set; }

        // Opcjonalne pole na nowe hasło
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Hasła nie są identyczne")]
        public string? ConfirmPassword { get; set; }
    }

    public class UserDetailsViewModel
    {
        public required User User { get; set; }
        public List<UserTraining>? UserTrainings { get; set; }
        public bool IsLockedOut { get; set; }
    }

}
