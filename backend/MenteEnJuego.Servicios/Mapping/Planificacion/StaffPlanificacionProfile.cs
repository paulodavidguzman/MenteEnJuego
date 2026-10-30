using AutoMapper;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;

namespace MenteEnJuego.Servicios.Mapeos.Planificacion
{
    public class StaffPlanificacionProfile : Profile
    {
        public StaffPlanificacionProfile()
        {
            CreateMap<StaffPlanificacion, StaffPlanificacionDTO>();

            CreateMap<CrearStaffPlanificacionDTO, StaffPlanificacion>();

            CreateMap<ActualizarStaffPlanificacionDTO, StaffPlanificacion>();
        }
    }
}