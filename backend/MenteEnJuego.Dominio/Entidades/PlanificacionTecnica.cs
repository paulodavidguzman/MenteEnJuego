using System;
using System.Collections.Generic;

namespace MenteEnJuego.Dominio.Entidades
{
    public class PlanificacionTecnica
    {
        public int Id { get; set; }

        public int SesionDeportivaId { get; set; }

        public int? UsuarioResponsableId { get; set; }

        public string? ObjetivoGeneral { get; set; }

        public int? DuracionPlanificadaMinutos { get; set; }

        public string? IntensidadPlanificada { get; set; }

        public byte? RpeObjetivo { get; set; }

        public int? CargaPlanificada { get; private set; }

        public string? MaterialesNecesarios { get; set; }

        public string? Precauciones { get; set; }

        public string? Observaciones { get; set; }

        public string Estado { get; set; } =
            "Borrador";

        public DateTime FechaCreacion { get; set; } =
            DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        public SesionDeportiva SesionDeportiva
        {
            get;
            set;
        } = null!;

        public Usuario? UsuarioResponsable
        {
            get;
            set;
        }

        public ICollection<ObjetivoEspecifico>
            ObjetivosEspecificos
        { get; set; } =
            new List<ObjetivoEspecifico>();

            public ICollection<ContenidoPlanificado>
                 ContenidosPlanificados
             { get; set; } =
                 new List<ContenidoPlanificado>();

             public ICollection<BloqueSesion>
                 BloquesSesion
             { get; set; } =
                 new List<BloqueSesion>();

             public ICollection<IndicadorPlanificado>
                 IndicadoresPlanificados
             { get; set; } =
                 new List<IndicadorPlanificado>();

             public ICollection<StaffPlanificacion>
                 StaffPlanificaciones
             { get; set; } =
                 new List<StaffPlanificacion>();
    }


}