using System.ComponentModel.DataAnnotations;

namespace TrainingManagement.Models
{
    public class UserActivityLog
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public bool IsSuccess { get; set; }
    }
}