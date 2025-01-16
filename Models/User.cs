namespace TrainingManagement.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string Email { get; set; }
        public required string Password { get; set; }
        public List<UserTraining>? UserTrainings { get; set; }
    }
}
