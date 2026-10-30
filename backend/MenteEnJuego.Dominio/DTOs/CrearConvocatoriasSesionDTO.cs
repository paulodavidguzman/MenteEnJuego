using System.Collections.Generic;

namespace MenteEnJuego.Dominio.DTOs
{
    public class CrearConvocatoriasSesionDTO
    {
        public List<int> JugadoraIds { get; set; } = new();
    }
}