using AutoMapper;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;

namespace MenteEnJuego.Servicios.Mapping.Planificacion
{
    public class BloqueSesionProfile : Profile
    {
        public BloqueSesionProfile()
        {
            CreateMap<BloqueSesion, BloqueSesionDTO>();

            CreateMap<CrearBloqueSesionDTO, BloqueSesion>();

            CreateMap<ActualizarBloqueSesionDTO, BloqueSesion>();
        }
    }
}