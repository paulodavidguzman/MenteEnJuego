using AutoMapper;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;
using System;

namespace MenteEnJuego.Servicios.Mapping.Planificacion
{
    public class PlanificacionTecnicaProfile : Profile
    {
        public PlanificacionTecnicaProfile()
        {
            CreateMap<
                CrearPlanificacionTecnicaDTO,
                PlanificacionTecnica>()
                .ForMember(
                    destino => destino.Id,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.UsuarioResponsableId,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.CargaPlanificada,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.Estado,
                    opcion => opcion.MapFrom(_ => "Borrador"))
                .ForMember(
                    destino => destino.FechaCreacion,
                    opcion => opcion.MapFrom(_ => DateTime.Now))
                .ForMember(
                    destino => destino.FechaModificacion,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.SesionDeportiva,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.UsuarioResponsable,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.ObjetivosEspecificos,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.ContenidosPlanificados,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.BloquesSesion,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.IndicadoresPlanificados,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.StaffPlanificaciones,
                    opcion => opcion.Ignore());

            CreateMap<
                ActualizarPlanificacionTecnicaDTO,
                PlanificacionTecnica>()
                .ForMember(
                    destino => destino.Id,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.SesionDeportivaId,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.UsuarioResponsableId,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.CargaPlanificada,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.Estado,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.FechaCreacion,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.FechaModificacion,
                    opcion => opcion.MapFrom(_ => DateTime.Now))
                .ForMember(
                    destino => destino.SesionDeportiva,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.UsuarioResponsable,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.ObjetivosEspecificos,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.ContenidosPlanificados,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.BloquesSesion,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.IndicadoresPlanificados,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.StaffPlanificaciones,
                    opcion => opcion.Ignore());

            CreateMap<
                PlanificacionTecnica,
                PlanificacionTecnicaDTO>()
                .ForMember(
                    destino => destino.CantidadObjetivos,
                    opcion => opcion.MapFrom(
                        origen =>
                            origen.ObjetivosEspecificos.Count))
                .ForMember(
                    destino => destino.CantidadContenidos,
                    opcion => opcion.MapFrom(
                        origen =>
                            origen.ContenidosPlanificados.Count))
                .ForMember(
                    destino => destino.CantidadBloques,
                    opcion => opcion.MapFrom(
                        origen =>
                            origen.BloquesSesion.Count))
                .ForMember(
                    destino => destino.CantidadIndicadores,
                    opcion => opcion.MapFrom(
                        origen =>
                            origen.IndicadoresPlanificados.Count))
                .ForMember(
                    destino => destino.CantidadProfesionales,
                    opcion => opcion.MapFrom(
                        origen =>
                            origen.StaffPlanificaciones.Count));
        }
    }
}