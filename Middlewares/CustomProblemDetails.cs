namespace LeaveManagementAPI.Middlewares
{
    internal class CustomProblemDetails
    {
        public object Title { get; set; }
        public int Status { get; set; }
        public object Detail { get; set; }
        public string Type { get; set; }
        public object Errors { get; set; }
    }
}