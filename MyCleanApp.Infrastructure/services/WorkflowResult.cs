namespace MyCleanApp.Infrastructure.Services
{
    public class WorkflowResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static WorkflowResult CreateSuccess(string message, string? details = null, object? data = null)
        {
            return new WorkflowResult
            {
                Success = true,
                Message = message,
                Details = details,
                Data = data
            };
        }

        public static WorkflowResult CreateError(string message, string? details = null)
        {
            return new WorkflowResult
            {
                Success = false,
                Message = message,
                Details = details,
                Errors = new List<string> { message }
            };
        }

        public static WorkflowResult CreateError(List<string> errors)
        {
            return new WorkflowResult
            {
                Success = false,
                Message = "Se encontraron errores",
                Errors = errors
            };
        }
    }
}
