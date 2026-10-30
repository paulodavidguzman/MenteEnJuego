using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Dominio.DTOs
{
    public class MatrizHistoricaLegajoDTO
    {
        // Historial deportivo
        public int TotalSesionesConvocadas { get; set; }
        public int TotalAsistencias { get; set; }
        public decimal PorcentajeAsistencia { get; set; }

        // Historial salud
        public int TotalLesiones { get; set; }
        public int TotalDiasBaja { get; set; }
        public decimal PromedioDiasRecuperacion { get; set; }

        // Historial bienestar
        public int TotalWellness { get; set; }
        public decimal PromedioEstres { get; set; }
        public decimal PromedioHumor { get; set; }
        public decimal PromedioSueno { get; set; }
        public decimal PromedioRpe { get; set; }

        // Historial nutricional
        public int TotalControlesNutricionales { get; set; }
        public decimal? PesoMinimoKg { get; set; }
        public decimal? PesoMaximoKg { get; set; }
        public decimal? PesoPromedioKg { get; set; }

        // Estado general interpretado
        public string LecturaGeneral { get; set; } = "Sin datos suficientes";
    }
}