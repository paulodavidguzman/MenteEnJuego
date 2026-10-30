using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Dominio.DTOs
{
    public class RiesgoDeportivoDTO
    {
        public string NivelRiesgo { get; set; } = "Bajo";
        public int PuntajeRiesgo { get; set; } = 0;
        public string MotivoPrincipal { get; set; } = "Sin alertas relevantes";
        public bool AlertaFatiga { get; set; }
        public bool AlertaDolor { get; set; }
        public bool AlertaSueno { get; set; }
        public bool AlertaEstres { get; set; }
    }
}