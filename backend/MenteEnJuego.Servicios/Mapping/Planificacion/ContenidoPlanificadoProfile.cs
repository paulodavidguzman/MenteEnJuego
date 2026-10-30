using AutoMapper;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;

namespace MenteEnJuego.Servicios.Mapping.Planificacion
{
    public class ContenidoPlanificadoProfile : Profile
    {
        public ContenidoPlanificadoProfile()
        {
            CreateMap<ContenidoPlanificado, ContenidoPlanificadoDTO>();

            CreateMap<CrearContenidoPlanificadoDTO, ContenidoPlanificado>();

            CreateMap<ActualizarContenidoPlanificadoDTO, ContenidoPlanificado>();
        }
    }
}