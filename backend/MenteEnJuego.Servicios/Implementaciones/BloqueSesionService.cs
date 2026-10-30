using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs.Planificacion;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class BloqueSesionService : IBloqueSesionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearBloqueSesionDTO> _crearValidator;
        private readonly IValidator<ActualizarBloqueSesionDTO> _actualizarValidator;

        public BloqueSesionService(
            ApplicationDbContext context,
            IMapper mapper,
            IValidator<CrearBloqueSesionDTO> crearValidator,
            IValidator<ActualizarBloqueSesionDTO> actualizarValidator)
        {
            _context = context;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        public async Task<List<BloqueSesionDTO>>
            ObtenerPorPlanificacionAsync(int planificacionTecnicaId)
        {
            if (planificacionTecnicaId <= 0)
            {
                return new List<BloqueSesionDTO>();
            }

            var bloques = await _context.BloquesSesion
                .AsNoTracking()
                .Where(b =>
                    b.PlanificacionTecnicaId == planificacionTecnicaId)
                .OrderBy(b => b.Orden)
                .ThenBy(b => b.Id)
                .ToListAsync();

            return _mapper.Map<List<BloqueSesionDTO>>(bloques);
        }

        public async Task<BloqueSesionDTO?>
            ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var bloque = await _context.BloquesSesion
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            return bloque == null
                ? null
                : _mapper.Map<BloqueSesionDTO>(bloque);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            BloqueSesionDTO? Datos)>
            CrearAsync(CrearBloqueSesionDTO solicitud)
        {
            NormalizarSolicitudCreacion(solicitud);

            var validacion =
                await _crearValidator.ValidateAsync(solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var planificacion = await _context.PlanificacionesTecnicas
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Id == solicitud.PlanificacionTecnicaId);

            if (planificacion == null)
            {
                return (
                    false,
                    "No se encontró la planificación técnica indicada.",
                    null);
            }

            if (!EsEstadoEditable(planificacion.Estado))
            {
                return (
                    false,
                    "Solo se pueden agregar bloques cuando la planificación está en estado Borrador o Pendiente.",
                    null);
            }

            var ordenOcupado = await _context.BloquesSesion
                .AsNoTracking()
                .AnyAsync(b =>
                    b.PlanificacionTecnicaId ==
                        solicitud.PlanificacionTecnicaId
                    &&
                    b.Orden == solicitud.Orden);

            if (ordenOcupado)
            {
                return (
                    false,
                    "Ya existe un bloque con el mismo orden dentro de la planificación.",
                    null);
            }

            var bloqueDuplicado = await _context.BloquesSesion
                .AsNoTracking()
                .AnyAsync(b =>
                    b.PlanificacionTecnicaId ==
                        solicitud.PlanificacionTecnicaId
                    &&
                    b.Nombre == solicitud.Nombre
                    &&
                    b.TipoBloque == solicitud.TipoBloque);

            if (bloqueDuplicado)
            {
                return (
                    false,
                    "Ya existe un bloque con el mismo nombre y tipo dentro de la planificación.",
                    null);
            }

            var bloque =
                _mapper.Map<BloqueSesion>(solicitud);

            await _context.BloquesSesion.AddAsync(bloque);
            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<BloqueSesionDTO>(bloque);

            return (
                true,
                "El bloque de sesión fue creado correctamente.",
                respuesta);
        }

        public async Task<(
            bool Exito,
            string Mensaje,
            BloqueSesionDTO? Datos)>
            ActualizarAsync(
                int id,
                ActualizarBloqueSesionDTO solicitud)
        {
            if (id <= 0)
            {
                return (
                    false,
                    "El identificador del bloque no es válido.",
                    null);
            }

            NormalizarSolicitudActualizacion(
            solicitud);

            var validacion =
                await _actualizarValidator.ValidateAsync(solicitud);

            if (!validacion.IsValid)
            {
                return (
                    false,
                    UnirErrores(validacion.Errors),
                    null);
            }

            var bloque = await _context.BloquesSesion
                .Include(b => b.PlanificacionTecnica)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bloque == null)
            {
                return (
                    false,
                    "No se encontró el bloque de sesión solicitado.",
                    null);
            }

            if (!EsEstadoEditable(
                    bloque.PlanificacionTecnica.Estado))
            {
                return (
                    false,
                    "Solo se pueden modificar bloques cuando la planificación está en estado Borrador o Pendiente.",
                    null);
            }

            var ordenOcupado = await _context.BloquesSesion
                .AsNoTracking()
                .AnyAsync(b =>
                    b.PlanificacionTecnicaId ==
                        bloque.PlanificacionTecnicaId
                    &&
                    b.Orden == solicitud.Orden
                    &&
                    b.Id != id);

            if (ordenOcupado)
            {
                return (
                    false,
                    "Ya existe otro bloque con el mismo orden dentro de la planificación.",
                    null);
            }

            var bloqueDuplicado = await _context.BloquesSesion
                .AsNoTracking()
                .AnyAsync(b =>
                    b.PlanificacionTecnicaId ==
                        bloque.PlanificacionTecnicaId
                    &&
                    b.Nombre == solicitud.Nombre
                    &&
                    b.TipoBloque == solicitud.TipoBloque
                    &&
                    b.Id != id);

            if (bloqueDuplicado)
            {
                return (
                    false,
                    "Ya existe otro bloque con el mismo nombre y tipo dentro de la planificación.",
                    null);
            }

            _mapper.Map(solicitud, bloque);

            

            await _context.SaveChangesAsync();

            var respuesta =
                _mapper.Map<BloqueSesionDTO>(bloque);

            return (
                true,
                "El bloque de sesión fue actualizado correctamente.",
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
                    "El identificador del bloque no es válido.");
            }

            var bloque = await _context.BloquesSesion
                .Include(b => b.PlanificacionTecnica)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bloque == null)
            {
                return (
                    false,
                    "No se encontró el bloque de sesión solicitado.");
            }

            if (!EsEstadoEditable(
                    bloque.PlanificacionTecnica.Estado))
            {
                return (
                    false,
                    "Solo se pueden eliminar bloques cuando la planificación está en estado Borrador o Pendiente.");
            }

            _context.BloquesSesion.Remove(bloque);
            await _context.SaveChangesAsync();

            return (
                true,
                "El bloque de sesión fue eliminado correctamente.");
        }

        private static bool EsEstadoEditable(string estado)
        {
            return estado == "Borrador"
                || estado == "Pendiente";
        }

        private static void NormalizarSolicitudCreacion(
    CrearBloqueSesionDTO solicitud)
        {
            solicitud.Nombre =
                solicitud.Nombre.Trim();

            solicitud.TipoBloque =
                solicitud.TipoBloque.Trim();

            solicitud.IntensidadPlanificada =
                LimpiarTextoOpcional(
                    solicitud.IntensidadPlanificada);

            solicitud.Objetivo =
                LimpiarTextoOpcional(
                    solicitud.Objetivo);

            solicitud.Descripcion =
                LimpiarTextoOpcional(
                    solicitud.Descripcion);
        }

        private static void NormalizarSolicitudActualizacion(
            ActualizarBloqueSesionDTO solicitud)
        {
            solicitud.Nombre =
                solicitud.Nombre.Trim();

            solicitud.TipoBloque =
                solicitud.TipoBloque.Trim();

            solicitud.IntensidadPlanificada =
                LimpiarTextoOpcional(
                    solicitud.IntensidadPlanificada);

            solicitud.Objetivo =
                LimpiarTextoOpcional(
                    solicitud.Objetivo);

            solicitud.Descripcion =
                LimpiarTextoOpcional(
                    solicitud.Descripcion);
        }
    

        private static string? LimpiarTextoOpcional(
            string? texto)
        {
            return string.IsNullOrWhiteSpace(texto)
                ? null
                : texto.Trim();
        }

        private static string UnirErrores(
            IEnumerable<
                FluentValidation.Results.ValidationFailure>
                errores)
        {
            return string.Join(
                " ",
                errores.Select(error => error.ErrorMessage));
        }
    }
}