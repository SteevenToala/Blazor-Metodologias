namespace MyCleanApp.API.DTOs
{
    public class CompletarVerificacionRequest
    {
        public bool DocumentosValidos { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }
}
