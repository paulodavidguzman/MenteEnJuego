using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Dominio.DTOs
{
    public class IndicePreventivoResultadoDTO
    {
        public bool Disponible { get; set; }

        public int IdJugadora { get; set; }

        public string Fecha { get; set; } = "";

        public double? IndicePreventivo { get; set; }

        public string NivelPreventivo { get; set; } = "SIN_DATOS";

        public List<string> FactoresPrincipales { get; set; } = new();

        public List<string> ContextoPreventivo { get; set; } = new();

        public bool GpsDisponible { get; set; }
    }
}