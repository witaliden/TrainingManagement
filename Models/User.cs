using Microsoft.AspNetCore.Identity;

namespace TrainingManagement.Models
{
    public class User : IdentityUser
    {
        public List<UserTraining>? UserTrainings { get; set; }
        public required string Name { get; set; }
        public required string Lastname { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? PasswordExpirationDate { get; set; }
        public List<string> PreviousPasswords { get; set; } = [];
    }
}