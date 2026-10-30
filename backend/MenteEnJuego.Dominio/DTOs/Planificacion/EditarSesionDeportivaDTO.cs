public class EditarSesionDeportivaDTO
{
    public string TipoSesion { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan HoraFin { get; set; }

    public string Lugar { get; set; } = string.Empty;

    public string? Objetivo { get; set; }

    public string? Descripcion { get; set; }
}
