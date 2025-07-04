public class ActualizarEstadoRequest
{
    public int Id { get; set; }               // ID de la solicitud
    public string Estado { get; set; }        // Nuevo estado: APROBADO o RECHAZADO
    public int UsuarioId { get; set; }        // ID del usuario que aprueba/rechaza
}
