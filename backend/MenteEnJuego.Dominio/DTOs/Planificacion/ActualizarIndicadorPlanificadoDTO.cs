namespace MenteEnJuego.Dominio.DTOs.Planificacion
{
    public class ActualizarIndicadorPlanificadoDTO
    {
        public string Nombre { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public string? UnidadMedida { get; set; }

        public decimal? ValorObjetivo { get; set; }

        public decimal? ValorMinimo { get; set; }

        public decimal? ValorMaximo { get; set; }

        public string FuenteDatos { get; set; } = string.Empty;

        public bool EsObligatorio { get; set; }

        public int Orden { get; set; }
    }
}