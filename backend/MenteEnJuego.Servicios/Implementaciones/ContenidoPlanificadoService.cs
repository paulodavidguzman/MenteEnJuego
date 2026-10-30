using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class ContenidoPlanificadoService
        : IContenidoPlanificadoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        private readonly
            IValidator<CrearContenidoPlanificadoDTO>
            _crearValidator;

        private readonly
            IValidator<ActualizarContenidoPlanificadoDTO>
            _actualizarValidator;

        public ContenidoPlanificadoService(
            ApplicationDbContext context,
            IMapper mapper,
            IValidator<CrearContenidoPlanificadoDTO>
                crearValidator,
            IValidator<ActualizarContenidoPlanificadoDTO>
                actualizarValidator)
        {
            _context = context;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        public async Task<List<ContenidoPlanificadoDTO>>
            ObtenerPorPlanificacionAsync(
                int planificacionTecnicaId)
        {
            if (planificacionTecnicaId <= 0)
            {
                return new List<ContenidoPlanificadoDTO>();
            }

            var contenidos =
                await _context.ContenidosPlanificados
                    .AsNoTracking()
                    .Where(c =>
                        c.PlanificacionTecnicaId ==
                        planificacionTecnicaId)
                    .OrderBy(c => c.Orden)
                    .ThenBy(c => c.Id)
                    .ToListAsync();

            return _mapper.Map<
                List<ContenidoPlanificadoDTO>>(
                    contenidos);
        }

        public async Task<ContenidoPlanificadoDTO?>
            ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var contenido =
                await _context.ContenidosPlanificados
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            return contenido == null
                ? null
                : _mapper.Map<
                    ContenidoPlanificadoDTO>(
                        contenido);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            ContenidoPlanificadoDTO? Datos)>
            CrearAsync(
                CrearContenidoPlanificadoDTO solicitud)
        {
            NormalizarSolicitudCreacion(solicitud);

            var validacion =
                await _crearValidator.ValidateAsync(
                    solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var planificacion =
                await _context.PlanificacionesTecnicas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        p =>
                            p.Id ==
                            solicitud.PlanificacionTecnicaId);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica indicada.",
                    null);
            }

            if (!EsEstadoEditable(
                    planificacion.Estado))
            {
                return (
                    false,
                    "Solo se pueden agregar contenidos cuando la planificación está en estado Borrador o Pendiente.",
                    null);
            }

            var ordenOcupado =
                await _context.ContenidosPlanificados
                    .AsNoTracking()
                    .AnyAsync(
                        c =>
                            c.PlanificacionTecnicaId ==
                                solicitud
                                    .PlanificacionTecnicaId
                            &&
                            c.Orden ==
                                solicitud.Orden);

            if (ordenOcupado)
            {
                return (
                    false,
                    "Ya existe un contenido con el mismo orden dentro de la planificación.",
                    null);
            }

            var contenidoDuplicado =
                await _context.ContenidosPlanificados
                    .AsNoTracking()
                    .AnyAsync(
                        c =>
                            c.PlanificacionTecnicaId ==
                                solicitud
                                    .PlanificacionTecnicaId
                            &&
                            c.Categoria ==
                                solicitud.Categoria
                            &&
                            c.Nombre ==
                                solicitud.Nombre);

            if (contenidoDuplicado)
            {
                return (
                    false,
                    "Ya existe un contenido con la misma categoría y nombre dentro de la planificación.",
                    null);
            }

            var contenido =
                _mapper.Map<ContenidoPlanificado>(
                    solicitud);

            /*contenido.Categoria =
                contenido.Categoria.Trim();

            contenido.Nombre =
                contenido.Nombre.Trim();

            contenido.Descripcion =
                LimpiarTextoOpcional(
                    contenido.Descripcion);

            contenido.Prioridad =
                contenido.Prioridad.Trim();*/

            await _context.ContenidosPlanificados
                .AddAsync(contenido);

            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<ContenidoPlanificadoDTO>(
                    contenido);

            return (
                true,
                "El contenido planificado fue creado correctamente.",
                respuesta);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            ContenidoPlanificadoDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarContenidoPlanificadoDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador del contenido no es válido.",
                    null);
            }


            NormalizarSolicitudActualizacion(solicitud);

            var validacion =
                await _actualizarValidator
                    .ValidateAsync(solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var contenido =
                await _context.ContenidosPlanificados
                    .Include(c =>
                        c.PlanificacionTecnica)
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (contenido == null)
            {
                return (
                    false,
                    "No se encontró el contenido planificado solicitado.",
                    null);
            }

            if (!EsEstadoEditable(
                    contenido.PlanificacionTecnica
                        .Estado))
            {
                return (
                    false,
                    "Solo se pueden modificar contenidos cuando la planificación está en estado Borrador o Pendiente.",
                    null);
            }

            var ordenOcupado =
                await _context.ContenidosPlanificados
                    .AsNoTracking()
                    .AnyAsync(
                        c =>
                            c.PlanificacionTecnicaId ==
                                contenido
                                    .PlanificacionTecnicaId
                            &&
                            c.Orden ==
                                solicitud.Orden
                            &&
                            c.Id != id);

            if (ordenOcupado)
            {
                return (
                    false,
                    "Ya existe otro contenido con el mismo orden dentro de la planificación.",
                    null);
            }

            var contenidoDuplicado =
                await _context.ContenidosPlanificados
                    .AsNoTracking()
                    .AnyAsync(
                        c =>
                            c.PlanificacionTecnicaId ==
                                contenido
                                    .PlanificacionTecnicaId
                            &&
                            c.Categoria ==
                                solicitud.Categoria
                            &&
                            c.Nombre ==
                                solicitud.Nombre
                            &&
                            c.Id != id);

            if (contenidoDuplicado)
            {
                return (
                    false,
                    "Ya existe otro contenido con la misma categoría y nombre dentro de la planificación.",
                    null);
            }

            _mapper.Map(
                solicitud,
                contenido);

           /* contenido.Categoria =
                contenido.Categoria.Trim();

            contenido.Nombre =
                contenido.Nombre.Trim();

            contenido.Descripcion =
                LimpiarTextoOpcional(
                    contenido.Descripcion);

            contenido.Prioridad =
                contenido.Prioridad.Trim();*/

            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<ContenidoPlanificadoDTO>(
                    contenido);

            return (
                true,
                "El contenido planificado fue actualizado correctamente.",
                respuesta);
        }

        public async Task<(
            bool Exito,
            string Mensaje)>
            EliminarAsync(int id)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador del contenido no es válido.");
            }

            var contenido =
                await _context.ContenidosPlanificados
                    .Include(c =>
                        c.PlanificacionTecnica)
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (contenido == null)
            {
                return (
                    false,
                    "No se encontró el contenido planificado solicitado.");
            }

            if (!EsEstadoEditable(
                    contenido.PlanificacionTecnica
                        .Estado))
            {
                return (
                    false,
                    "Solo se pueden eliminar contenidos cuando la planificación está en estado Borrador o Pendiente.");
            }

            _context.ContenidosPlanificados
                .Remove(contenido);

            await _context.SaveChangesAsync();

            return (
                true,
                "El contenido planificado fue eliminado correctamente.");
        }

        private static bool EsEstadoEditable(
            string estado)
        {
            return estado == "Borrador"
                || estado == "Pendiente";
        }

        private static void NormalizarSolicitudCreacion(
    CrearContenidoPlanificadoDTO solicitud)
        {
            solicitud.Categoria =
                solicitud.Categoria.Trim();

            solicitud.Nombre =
                solicitud.Nombre.Trim();

            solicitud.Descripcion =
                LimpiarTextoOpcional(
                    solicitud.Descripcion);

            solicitud.Prioridad =
                solicitud.Prioridad.Trim();
        }

        private static void NormalizarSolicitudActualizacion(
            ActualizarContenidoPlanificadoDTO solicitud)
        {
            solicitud.Categoria =
                solicitud.Categoria.Trim();

            solicitud.Nombre =
                solicitud.Nombre.Trim();

            solicitud.Descripcion =
                LimpiarTextoOpcional(
                    solicitud.Descripcion);

            solicitud.Prioridad =
                solicitud.Prioridad.Trim();
        }

        private static string? LimpiarTextoOpcional(
            string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return null;
            }

            return texto.Trim();
        }

        private static string UnirErrores(
            IEnumerable<
                FluentValidation.Results.ValidationFailure>
                errores)
        {
            return string.Join(
                " ",
                errores.Select(
                    error => error.ErrorMessage));
        }
    }
}