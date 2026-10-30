using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Dominio.DTOs
{
    public class EventoTimelineDTO
    {
        public DateTime Fecha { get; set; }
        public string TipoEvento { get; set; } = "";
        public string Titulo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string ModuloOrigen { get; set; } = "";
    }
}
