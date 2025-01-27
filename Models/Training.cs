using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Models
{
    public class Training
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string Link { get; set; }
        public DateTime DueDate { get; set; }
        public List<UserTraining>? UserTrainings { get; set; }
    }
}
