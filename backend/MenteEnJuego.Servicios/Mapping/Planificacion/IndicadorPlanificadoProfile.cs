using AutoMapper;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;

namespace MenteEnJuego.Servicios.Mapeos.Planificacion
{
    public class IndicadorPlanificadoProfile : Profile
    {
        public IndicadorPlanificadoProfile()
        {
            CreateMap<IndicadorPlanificado, IndicadorPlanificadoDTO>();

            CreateMap<CrearIndicadorPlanificadoDTO, IndicadorPlanificado>();

            CreateMap<ActualizarIndicadorPlanificadoDTO, IndicadorPlanificado>();
        }
    }
}