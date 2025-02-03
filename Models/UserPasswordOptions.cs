namespace TrainingManagement.Models
{
    public class UserPasswordOptions
    {
        public int RequiredPasswordLength { get; set; } = 8;
        public bool RequireDigit { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireNonAlphanumeric { get; set; } = true;
        public int RequiredUniqueChars { get; set; } = 1;
    }
}
