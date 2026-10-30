namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class ActualizarContenidoPlanificadoDTO
    {
        public string Categoria { get; set; } =
            string.Empty;

        public string Nombre { get; set; } =
            string.Empty;

        public string? Descripcion { get; set; }

        public string Prioridad { get; set; } =
            "Media";

        public int Orden { get; set; }
    }
}