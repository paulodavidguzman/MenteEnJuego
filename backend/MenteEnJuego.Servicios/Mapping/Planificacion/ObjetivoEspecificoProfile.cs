using AutoMapper;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;

namespace MenteEnJuego.Servicios.Mapping.Planificacion
{
    public class ObjetivoEspecificoProfile : Profile
    {
        public ObjetivoEspecificoProfile()
        {
            CreateMap<
                CrearObjetivoEspecificoDTO,
                ObjetivoEspecifico>()
                .ForMember(
                    destino => destino.Id,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.Cumplido,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.ObservacionResultado,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.PlanificacionTecnica,
                    opcion => opcion.Ignore());

            CreateMap<
                ActualizarObjetivoEspecificoDTO,
                ObjetivoEspecifico>()
                .ForMember(
                    destino => destino.Id,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.PlanificacionTecnicaId,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.Cumplido,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.ObservacionResultado,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.PlanificacionTecnica,
                    opcion => opcion.Ignore());

            CreateMap<
                EvaluarObjetivoEspecificoDTO,
                ObjetivoEspecifico>()
                .ForMember(
                    destino => destino.Id,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.PlanificacionTecnicaId,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.Descripcion,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.Prioridad,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.Orden,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.PlanificacionTecnica,
                    opcion => opcion.Ignore());

            CreateMap<
                ObjetivoEspecifico,
                ObjetivoEspecificoDTO>();
        }
    }
}