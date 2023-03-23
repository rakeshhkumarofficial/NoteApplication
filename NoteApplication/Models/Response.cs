namespace NoteApplication.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool ? IsSuccess { get; set; }
        public object? Data { get; set; }
    }
}
