namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class CrearObjetivoEspecificoDTO
    {
        public int PlanificacionTecnicaId { get; set; }

        public string Descripcion { get; set; } =
            string.Empty;

        public string Prioridad { get; set; } =
            "Media";

        public int Orden { get; set; }
    }
}