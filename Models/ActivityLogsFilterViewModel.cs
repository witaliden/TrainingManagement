namespace TrainingManagement.Models
{
    public class ActivityLogsFilterViewModel
    {
        public string? UserName { get; set; }
        public string? ActionType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? IsSuccess { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }
        public List<UserActivityLog> Logs { get; set; } = new();
    }
}
