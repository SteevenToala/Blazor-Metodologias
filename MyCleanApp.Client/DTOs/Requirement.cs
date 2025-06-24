namespace MyCleanApp.Client.DTOs
{
    public class Requirement
    {
        public string Name { get; set; } = string.Empty;
        public int Current { get; set; }
        public int Required { get; set; }
        public bool Completed { get; set; }
    }
}
