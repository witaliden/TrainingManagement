using Microsoft.AspNetCore.Identity;

namespace TrainingManagement.Models
{
    public class User : IdentityUser
    {
        public List<UserTraining>? UserTrainings { get; set; }
        public required string Name { get; set; }
        public required string Lastname { get; set; }

        public required UserPasswordOptions UserPasswordOptions { get; set; } = new UserPasswordOptions();
        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? PasswordExpirationDate { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public List<string> PreviousPasswords { get; set; } = [];
    }
}