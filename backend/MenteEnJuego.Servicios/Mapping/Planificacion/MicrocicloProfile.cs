using System;
using AutoMapper;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;

namespace MenteEnJuego.Servicios.Mapping.Planificacion
{
    public class MicrocicloProfile : Profile
    {
        public MicrocicloProfile()
        {
            CreateMap<CrearMicrocicloDTO, Microciclo>()
                .ForMember(
                    destino => destino.Id,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.Estado,
                    opcion => opcion.MapFrom(_ => "Planificado"))
                .ForMember(
                    destino => destino.FechaCreacion,
                    opcion => opcion.MapFrom(_ => DateTime.Now))
                .ForMember(
                    destino => destino.FechaModificacion,
                    opcion => opcion.Ignore())
                .ForMember(
                    destino => destino.SesionesDeportivas,
                    opcion => opcion.Ignore());

            CreateMap<ActualizarMicrocicloDTO, Microciclo>()
                .ForMember(
                    destino => destino.Id,
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
                    destino => destino.SesionesDeportivas,
                    opcion => opcion.Ignore());

            CreateMap<Microciclo, MicrocicloDTO>()
                .ForMember(
                    destino => destino.CantidadSesiones,
                    opcion => opcion.MapFrom(
                        origen =>
                            origen.SesionesDeportivas.Count));
        }
    }
}