namespace OMC.Models
{
    public class ErrorLog
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionType { get; set; }
        public string Source { get; set; }
    }
}
