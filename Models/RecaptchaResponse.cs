namespace TrainingManagement.Models
{
    public class RecaptchaResponse
    {
        public bool success { get; set; }
        public string challenge_ts { get; set; }
        public string hostname { get; set; }
    }
}
