using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
