using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Models
{
    public class ProfileViewModel
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [DataType(DataType.Text)]
        public required string Lastname { get; set; }

        [Required(ErrorMessage = "Aktualne hasło jest wymagane")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "Nowe hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć co najmniej {2} znaków długości.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Nowe hasła nie są identyczne.")]
        public string? ConfirmPassword { get; set; }
    }

}
