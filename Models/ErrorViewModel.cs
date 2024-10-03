namespace First_MVC_Project.Models
{
    public class ErrorViewModel
    {
        public string? Error { get; set; }

        public string? Message { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(Message);
    }
}
