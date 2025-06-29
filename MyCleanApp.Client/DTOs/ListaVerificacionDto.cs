namespace MyCleanApp.Client.DTOs
{
    public class ListaVerificacionDto
    {
        public int Id { get; set; }
        public int NivelAcademicoId { get; set; }
        public string NombreDocumento { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Obligatorio { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }

    public class VerificacionDocumentosDto
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public int ListaVerificacionId { get; set; }
        public string NombreDocumento { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public DateTime? FechaVerificacion { get; set; }
        public string VerificadoPor { get; set; } = string.Empty;
    }
}
