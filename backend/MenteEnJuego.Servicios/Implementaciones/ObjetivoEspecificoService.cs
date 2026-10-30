using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class ObjetivoEspecificoService
        : IObjetivoEspecificoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        private readonly
            IValidator<CrearObjetivoEspecificoDTO>
            _crearValidator;

        private readonly
            IValidator<ActualizarObjetivoEspecificoDTO>
            _actualizarValidator;

        private readonly
            IValidator<EvaluarObjetivoEspecificoDTO>
            _evaluarValidator;

        public ObjetivoEspecificoService(
            ApplicationDbContext context,
            IMapper mapper,
            IValidator<CrearObjetivoEspecificoDTO>
                crearValidator,
            IValidator<ActualizarObjetivoEspecificoDTO>
                actualizarValidator,
            IValidator<EvaluarObjetivoEspecificoDTO>
                evaluarValidator)
        {
            _context = context;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
            _evaluarValidator = evaluarValidator;
        }

        public async Task<List<ObjetivoEspecificoDTO>>
            ObtenerPorPlanificacionAsync(
                int planificacionTecnicaId)
        {
            if (planificacionTecnicaId <= 0)
            {
                return new List<ObjetivoEspecificoDTO>();
            }

            var objetivos =
                await _context.ObjetivosEspecificos
                    .AsNoTracking()
                    .Where(o =>
                        o.PlanificacionTecnicaId ==
                        planificacionTecnicaId)
                    .OrderBy(o => o.Orden)
                    .ThenBy(o => o.Id)
                    .ToListAsync();

            return _mapper.Map<
                List<ObjetivoEspecificoDTO>>(
                    objetivos);
        }

        public async Task<ObjetivoEspecificoDTO?>
            ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var objetivo =
                await _context.ObjetivosEspecificos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            return objetivo == null
                ? null
                : _mapper.Map<
                    ObjetivoEspecificoDTO>(
                        objetivo);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            ObjetivoEspecificoDTO? Datos)>
            CrearAsync(
                CrearObjetivoEspecificoDTO solicitud)
        {
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
                    "Solo se pueden agregar objetivos cuando la planificación está en estado Borrador o Pendiente.",
                    null);
            }

            var ordenOcupado =
                await _context.ObjetivosEspecificos
                    .AsNoTracking()
                    .AnyAsync(
                        o =>
                            o.PlanificacionTecnicaId ==
                                solicitud
                                    .PlanificacionTecnicaId
                            &&
                            o.Orden ==
                                solicitud.Orden);

            if (ordenOcupado)
            {
                return (
                    false,
                    "Ya existe un objetivo con el mismo orden dentro de la planificación.",
                    null);
            }

            var objetivo =
                _mapper.Map<ObjetivoEspecifico>(
                    solicitud);

            await _context.ObjetivosEspecificos
                .AddAsync(objetivo);

            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<ObjetivoEspecificoDTO>(
                    objetivo);

            return (
                true,
                "El objetivo específico fue creado correctamente.",
                respuesta);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            ObjetivoEspecificoDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarObjetivoEspecificoDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador del objetivo no es válido.",
                    null);
            }

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

            var objetivo =
                await _context.ObjetivosEspecificos
                    .Include(o =>
                        o.PlanificacionTecnica)
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            if (objetivo == null)
            {
                return (
                    false,
                    "No se encontró el objetivo específico solicitado.",
                    null);
            }

            if (!EsEstadoEditable(
                    objetivo.PlanificacionTecnica
                        .Estado))
            {
                return (
                    false,
                    "Solo se pueden modificar objetivos cuando la planificación está en estado Borrador o Pendiente.",
                    null);
            }

            var ordenOcupado =
                await _context.ObjetivosEspecificos
                    .AsNoTracking()
                    .AnyAsync(
                        o =>
                            o.PlanificacionTecnicaId ==
                                objetivo
                                    .PlanificacionTecnicaId
                            &&
                            o.Orden ==
                                solicitud.Orden
                            &&
                            o.Id != id);

            if (ordenOcupado)
            {
                return (
                    false,
                    "Ya existe otro objetivo con el mismo orden dentro de la planificación.",
                    null);
            }

            _mapper.Map(
                solicitud,
                objetivo);

            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<ObjetivoEspecificoDTO>(
                    objetivo);

            return (
                true,
                "El objetivo específico fue actualizado correctamente.",
                respuesta);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            ObjetivoEspecificoDTO? Datos)>
            EvaluarAsync(
                int id,
                EvaluarObjetivoEspecificoDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador del objetivo no es válido.",
                    null);
            }

            var validacion =
                await _evaluarValidator
                    .ValidateAsync(solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var objetivo =
                await _context.ObjetivosEspecificos
                    .Include(o =>
                        o.PlanificacionTecnica)
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            if (objetivo == null)
            {
                return (
                    false,
                    "No se encontró el objetivo específico solicitado.",
                    null);
            }

            if (!EsEstadoEvaluable(
                    objetivo.PlanificacionTecnica
                        .Estado))
            {
                return (
                    false,
                    "El objetivo solo puede evaluarse cuando la planificación está EnEjecucion o Ejecutada.",
                    null);
            }

            _mapper.Map(
                solicitud,
                objetivo);

            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<ObjetivoEspecificoDTO>(
                    objetivo);

            return (
                true,
                "El objetivo específico fue evaluado correctamente.",
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
                    "El identificador del objetivo no es válido.");
            }

            var objetivo =
                await _context.ObjetivosEspecificos
                    .Include(o =>
                        o.PlanificacionTecnica)
                    .FirstOrDefaultAsync(
                        o => o.Id == id);

            if (objetivo == null)
            {
                return (
                    false,
                    "No se encontró el objetivo específico solicitado.");
            }

            if (!EsEstadoEditable(
                    objetivo.PlanificacionTecnica
                        .Estado))
            {
                return (
                    false,
                    "Solo se pueden eliminar objetivos cuando la planificación está en estado Borrador o Pendiente.");
            }

            _context.ObjetivosEspecificos
                .Remove(objetivo);

            await _context.SaveChangesAsync();

            return (
                true,
                "El objetivo específico fue eliminado correctamente.");
        }

        private static bool EsEstadoEditable(
            string estado)
        {
            return estado == "Borrador"
                || estado == "Pendiente";
        }

        private static bool EsEstadoEvaluable(
            string estado)
        {
            return estado == "EnEjecucion"
                || estado == "Ejecutada";
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