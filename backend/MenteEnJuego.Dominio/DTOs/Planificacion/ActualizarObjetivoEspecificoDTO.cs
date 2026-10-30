namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class ActualizarObjetivoEspecificoDTO
    {
        public string Descripcion { get; set; } =
            string.Empty;

        public string Prioridad { get; set; } =
            "Media";

        public int Orden { get; set; }
    }
}