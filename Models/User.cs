using Microsoft.AspNetCore.Identity;

namespace TrainingManagement.Models
{
    public class User : IdentityUser
    {
        public List<UserTraining>? UserTrainings { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
    }
}