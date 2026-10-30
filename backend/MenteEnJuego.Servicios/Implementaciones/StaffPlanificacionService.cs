using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class StaffPlanificacionService
        : IStaffPlanificacionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        private readonly IValidator<CrearStaffPlanificacionDTO>
            _crearValidator;

        private readonly IValidator<ActualizarStaffPlanificacionDTO>
            _actualizarValidator;

        public StaffPlanificacionService(
            ApplicationDbContext context,
            IMapper mapper,
            IValidator<CrearStaffPlanificacionDTO> crearValidator,
            IValidator<ActualizarStaffPlanificacionDTO> actualizarValidator)
        {
            _context = context;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        public async Task<List<StaffPlanificacionDTO>>
     ObtenerPorPlanificacionAsync(
         int planificacionTecnicaId)
        {
            if (planificacionTecnicaId <= 0)
            {
                return new List<StaffPlanificacionDTO>();
            }

            var asignaciones =
                await _context.StaffPlanificaciones
                    .AsNoTracking()
                    .Where(x =>
                        x.PlanificacionTecnicaId ==
                        planificacionTecnicaId)
                    .OrderByDescending(x =>
                        x.EsResponsablePrincipal)
                    .ThenBy(x =>
                        x.FuncionEnSesion)
                    .ThenBy(x => x.Id)
                    .ToListAsync();

            return _mapper.Map<
                List<StaffPlanificacionDTO>>(
                    asignaciones);
        }
        public async Task<StaffPlanificacionDTO?>
            ObtenerPorIdAsync(int id)
        {
            var asignacion =
                await _context.StaffPlanificaciones
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == id);

            if (asignacion == null)
            {
                return null;
            }

            return _mapper.Map<StaffPlanificacionDTO>(
                asignacion);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            StaffPlanificacionDTO? Datos)>
            CrearAsync(
                CrearStaffPlanificacionDTO solicitud)
        {
            NormalizarSolicitudCreacion(solicitud);

            var resultadoValidacion =
                await _crearValidator
                    .ValidateAsync(solicitud);

            if (!resultadoValidacion.IsValid)
            {
                return (
                    false,
                    ObtenerErroresValidacion(
                        resultadoValidacion.Errors),
                    null);
            }

            

            var planificacion =
                await _context.PlanificacionesTecnicas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id ==
                        solicitud.PlanificacionTecnicaId);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica indicada.",
                    null);
            }

            if (!EsPlanificacionEditable(
                    planificacion.Estado))
            {
                return (
                    false,
                    "La planificación técnica no puede modificarse porque su estado no es Borrador ni Pendiente.",
                    null);
            }

            var profesional =
                await _context.StaffProfesionales
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id ==
                        solicitud.StaffProfesionalId);

            if (profesional == null)
            {
                return (
                    false,
                    "No se encontró el profesional del staff indicado.",
                    null);
            }

            var profesionalDuplicado =
                await _context.StaffPlanificaciones
                    .AnyAsync(x =>
                        x.PlanificacionTecnicaId ==
                            solicitud.PlanificacionTecnicaId
                        &&
                        x.StaffProfesionalId ==
                            solicitud.StaffProfesionalId);

            if (profesionalDuplicado)
            {
                return (
                    false,
                    "Ya existe una asignación para este profesional dentro de la planificación.",
                    null);
            }

            if (solicitud.EsResponsablePrincipal)
            {
                var responsableExistente =
                    await _context.StaffPlanificaciones
                        .AnyAsync(x =>
                            x.PlanificacionTecnicaId ==
                                solicitud.PlanificacionTecnicaId
                            &&
                            x.EsResponsablePrincipal);

                if (responsableExistente)
                {
                    return (
                        false,
                        "Ya existe un responsable principal en esta planificación.",
                        null);
                }
            }

            var asignacion =
                _mapper.Map<StaffPlanificacion>(
                    solicitud);

            _context.StaffPlanificaciones.Add(
                asignacion);

            await _context.SaveChangesAsync();

            var asignacionDTO =
                _mapper.Map<StaffPlanificacionDTO>(
                    asignacion);

            return (
                true,
                "El profesional se asignó correctamente a la planificación.",
                asignacionDTO);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            StaffPlanificacionDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarStaffPlanificacionDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador de la asignación no es válido.",
                    null);
            }

            NormalizarSolicitudActualizacion(
             solicitud);

            var resultadoValidacion =
                await _actualizarValidator
                    .ValidateAsync(solicitud);

            if (!resultadoValidacion.IsValid)
            {
                return (
                    false,
                    ObtenerErroresValidacion(
                        resultadoValidacion.Errors),
                    null);
            }

            NormalizarSolicitudActualizacion(solicitud);

            var asignacion =
                await _context.StaffPlanificaciones
                    .FirstOrDefaultAsync(x =>
                        x.Id == id);

            if (asignacion == null)
            {
                return (
                    false,
                    "No se encontró la asignación de staff solicitada.",
                    null);
            }

            var planificacion =
                await _context.PlanificacionesTecnicas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id ==
                        asignacion.PlanificacionTecnicaId);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica asociada.",
                    null);
            }

            if (!EsPlanificacionEditable(
                    planificacion.Estado))
            {
                return (
                    false,
                    "La planificación técnica no puede modificarse porque su estado no es Borrador ni Pendiente.",
                    null);
            }

            if (solicitud.EsResponsablePrincipal)
            {
                var responsableExistente =
                    await _context.StaffPlanificaciones
                        .AnyAsync(x =>
                            x.Id != id
                            &&
                            x.PlanificacionTecnicaId ==
                                asignacion.PlanificacionTecnicaId
                            &&
                            x.EsResponsablePrincipal);

                if (responsableExistente)
                {
                    return (
                        false,
                        "Ya existe otro responsable principal en esta planificación.",
                        null);
                }
            }

            _mapper.Map(
                solicitud,
                asignacion);

            await _context.SaveChangesAsync();

            var asignacionDTO =
                _mapper.Map<StaffPlanificacionDTO>(
                    asignacion);

            return (
                true,
                "La asignación de staff se actualizó correctamente.",
                asignacionDTO);
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
                    "El identificador de la asignación no es válido.");
            }

            var asignacion =
                await _context.StaffPlanificaciones
                    .FirstOrDefaultAsync(x =>
                        x.Id == id);

            if (asignacion == null)
            {
                return (
                    false,
                    "No se encontró la asignación de staff solicitada.");
            }

            var planificacion =
                await _context.PlanificacionesTecnicas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id ==
                        asignacion.PlanificacionTecnicaId);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica asociada.");
            }

            if (!EsPlanificacionEditable(
                    planificacion.Estado))
            {
                return (
                    false,
                    "La asignación no puede eliminarse porque la planificación no está en estado Borrador ni Pendiente.");
            }

            _context.StaffPlanificaciones.Remove(
                asignacion);

            await _context.SaveChangesAsync();

            return (
                true,
                "El profesional fue retirado correctamente de la planificación.");
        }

        private static bool EsPlanificacionEditable(
            string? estado)
        {
            return string.Equals(
                       estado,
                       "Borrador",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   string.Equals(
                       estado,
                       "Pendiente",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static void NormalizarSolicitudCreacion(
            CrearStaffPlanificacionDTO solicitud)
        {
            solicitud.FuncionEnSesion =
                solicitud.FuncionEnSesion.Trim();

            solicitud.Observaciones =
                NormalizarTextoOpcional(
                    solicitud.Observaciones);
        }

        private static void NormalizarSolicitudActualizacion(
            ActualizarStaffPlanificacionDTO solicitud)
        {
            solicitud.FuncionEnSesion =
                solicitud.FuncionEnSesion.Trim();

            solicitud.Observaciones =
                NormalizarTextoOpcional(
                    solicitud.Observaciones);
        }

        private static string? NormalizarTextoOpcional(
            string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static string ObtenerErroresValidacion(
            IEnumerable<
                FluentValidation.Results.ValidationFailure> errores)
        {
            return string.Join(
                " ",
                errores.Select(x =>
                    x.ErrorMessage));
        }
    }
}