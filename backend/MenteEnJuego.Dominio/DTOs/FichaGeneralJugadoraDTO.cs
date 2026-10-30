using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Dominio.DTOs
{
    public class FichaGeneralJugadoraDTO
    {
        public int IdJugadora { get; set; }
        public string? FotoBase64 { get; set; }
        public string Apellido { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Dni { get; set; } = "";
        public DateTime? FechaNacimiento { get; set; }
        public decimal? AlturaCm { get; set; }
        public decimal? PesoActualKg { get; set; }
        public decimal? ImcActual { get; set; }
        public int Edad { get; set; }
        public string? Posicion { get; set; }
        public string? Equipo { get; set; }
        public string? Lateralidad { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string EstadoDeportivo { get; set; } = "Activa";
    }
}