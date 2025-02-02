using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string UserName { get; set; }

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

    public class UserConfigModel
    {
        public required string Id { get; set; }
        public UserDetailsViewModel? EditUserViewModel { get; set; }
        public ChangeUserPasswordViewModel? ChangeUserPasswordViewModel { get; set; }
        public UserSecutityViewModel? UserSecutityViewModel { get; set; }
        public List<UserTraining>? UserTrainings { get; set; }
    }

    public class UserDetailsViewModel
    {

        [Required]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public string? Lastname { get; set; }

    }

    public class ChangeUserPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Hasła nie są identyczne")]
        public string? ConfirmPassword { get; set; }
    }

    public class UserSecutityViewModel
    {
        public bool IsLockedOut { get; set; }
        public UserPasswordOptions? UserPasswordOptions { get; set; }
        public DateTime? PasswordExpirationDate { get; set; }
    }

}
