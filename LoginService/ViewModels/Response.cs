namespace LoginService.ViewModels
{
    public class Response
    {
        public string? Service { get; set; }

        public object? Args { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Success { get; set; }

        public object? Data { get; set; }

        public string? Message { get; set; }
    }
}
