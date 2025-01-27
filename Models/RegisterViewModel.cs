using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public required string Lastname { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć co najmniej {2} znaków długości.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
        public required string ConfirmPassword { get; set; }
    }
}
