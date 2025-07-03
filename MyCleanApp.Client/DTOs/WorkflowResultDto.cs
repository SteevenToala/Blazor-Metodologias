public class WorkflowResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public string? Details { get; set; }
    public object? Data { get; set; }
}
