using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenteEnJuego.Dominio.DTOs
{
    public class MiniGraficoRpe
    {
        public DateTime Fecha { get; set; }

        public decimal Valor { get; set; }

        public string Etiqueta { get; set; } = "";
    }
}