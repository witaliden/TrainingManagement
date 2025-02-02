namespace TrainingManagement.Models
{
    public class UserPasswordOptions
    {
        public int RequiredPasswordLength { get; set; } = 6;
        public bool RequireDigit { get; set; } = false;
        public bool RequireLowercase { get; set; } = false;
        public bool RequireUppercase { get; set; } = false;
        public bool RequireNonAlphanumeric { get; set; } = false;
        public int RequiredUniqueChars { get; set; } = 1;
    }
}
