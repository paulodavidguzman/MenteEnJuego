using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs;
using MenteEnJuego.Servicios.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MenteEnJuego.Dominio.Entidades;
using MenteEnJuego.Servicios.Seguridad;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class LegajoDeportivoService : ILegajoDeportivoService
    {
        private readonly ApplicationDbContext _context;

        public LegajoDeportivoService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LegajoDeportivoDTO?> ObtenerLegajoAsync(
    int idJugadora,
    LegajoPermissions permisos)
        {

            var jugadora = await _context.Jugadoras
                .FirstOrDefaultAsync(j => j.Id == idJugadora);

            if (jugadora == null)
                return null;

            var edad = 0;

            if (jugadora.FechaNacimiento != default)
            {
                edad = DateTime.Today.Year - jugadora.FechaNacimiento.Year;
                if (DateTime.Today.DayOfYear < jugadora.FechaNacimiento.DayOfYear)
                    edad--;
            }

            var ultimaNutricion = await _context.EvaluacionesNutricionales
               .Where(n => n.IdJugadora == idJugadora)
               .OrderByDescending(n => n.Fecha)
               .FirstOrDefaultAsync();

            decimal? pesoActual = ultimaNutricion?.PesoKg;
            decimal? imc = null;

            if (pesoActual.HasValue && jugadora.AlturaCm.HasValue && jugadora.AlturaCm.Value > 0)
            {
                var alturaMetros = jugadora.AlturaCm.Value / 100;
                imc = Math.Round(pesoActual.Value / (alturaMetros * alturaMetros), 2);
            }

            var ficha = new FichaGeneralJugadoraDTO
            {
                IdJugadora = jugadora.Id,
                Apellido = jugadora.Apellido,
                Nombre = jugadora.Nombre,
                Dni = jugadora.Dni,
                FechaNacimiento = jugadora.FechaNacimiento,
                Edad = edad,
                Posicion = jugadora.Posicion,
                Equipo = jugadora.Equipo,
                Lateralidad = jugadora.Lateralidad,
                Telefono = jugadora.Telefono,
                Email = jugadora.Email,
                FotoBase64 = jugadora.FotoBase64,
                AlturaCm = jugadora.AlturaCm,
                PesoActualKg = pesoActual,
                ImcActual = imc,
                EstadoDeportivo = jugadora.Activo ? "Activa" : "Inactiva"
            };

            var timeline = new List<EventoTimelineDTO>();

            var ultimasEvaluaciones = await _context.EvaluacionesDiarias
                .Where(e => e.IdJugadora == idJugadora)
                .OrderByDescending(e => e.Fecha)
                .Take(5)
                .ToListAsync();



            foreach (var e in ultimasEvaluaciones)
            {
                timeline.Add(new EventoTimelineDTO
                {
                    Fecha = e.Fecha,
                    TipoEvento = "Wellness",
                    Titulo = $"Evaluación {e.TipoSesion} ({(string.IsNullOrWhiteSpace(e.MomentoEvaluacion) ? "Histórica" : e.MomentoEvaluacion)})",
                    Descripcion = string.Join(" | ", new[]
                    {
                        e.Estres.HasValue ? $"Estrés: {e.Estres.Value}" : null,
                        e.Sueño.HasValue ? $"Sueño: {e.Sueño.Value}" : null,
                        e.Humor.HasValue ? $"Humor: {e.Humor.Value}" : null,
                        e.EsfuerzoRpe.HasValue ? $"RPE: {e.EsfuerzoRpe.Value}" : null
                    }.Where(x => x != null)),
                    ModuloOrigen = "Bienestar Integral"
                });
            }



            var totalEvaluacionesWellness = await _context.EvaluacionesDiarias
    .CountAsync(e => e.IdJugadora == idJugadora);

            var totalLesiones = await _context.HistorialLesiones
                .CountAsync(l => l.IdJugadora == idJugadora);

            var totalControlesNutricionales = await _context.EvaluacionesNutricionales
                .CountAsync(n => n.IdJugadora == idJugadora);

            var ultimaLesion = await _context.HistorialLesiones
                .Where(l => l.IdJugadora == idJugadora)
                .OrderByDescending(l => l.FechaInicio)
                .FirstOrDefaultAsync();

            var lesionesJugadora = await _context.HistorialLesiones
    .Where(l => l.IdJugadora == idJugadora)
    .ToListAsync();

            var totalDiasBajaMedica = lesionesJugadora.Sum(l =>
            {
                var fechaFin = l.FechaAltaReal ?? DateTime.Today;
                return (fechaFin.Date - l.FechaInicio.Date).Days;
            });

            var totalEventosHistoricos =
                totalEvaluacionesWellness +
                totalControlesNutricionales +
                totalLesiones;

            var estadoHistoricoGeneral = totalEventosHistoricos switch
            {
                0 => "Sin historial registrado",
                <= 5 => "Historial inicial",
                <= 20 => "Historial en desarrollo",
                _ => "Historial consolidado"
            };

            var resumenDeportivo = new ResumenDeportivoDTO
            {
                TotalEvaluacionesWellness = totalEvaluacionesWellness,
                TotalLesiones = totalLesiones,
                TotalControlesNutricionales = totalControlesNutricionales,
                TotalEventosTimeline = timeline.Count,
                UltimaEvaluacionWellness = ultimasEvaluaciones.FirstOrDefault()?.Fecha,
                UltimoControlNutricional = ultimaNutricion?.Fecha,
                UltimaLesion = ultimaLesion?.FechaInicio,
                TotalDiasBajaMedica = totalDiasBajaMedica,
                EstadoHistoricoGeneral = estadoHistoricoGeneral
            };

            var convocatorias = await _context.ConvocatoriasSesion
    .Where(c => c.JugadoraId == idJugadora)
    .ToListAsync();

            var totalSesionesConvocadas = convocatorias.Count;

            var totalAsistencias = convocatorias.Count(c =>
                c.EstadoAsistencia != null &&
                c.EstadoAsistencia.ToLower().Contains("presente"));

            decimal porcentajeAsistencia = 0;

            if (totalSesionesConvocadas > 0)
            {
                porcentajeAsistencia = Math.Round(
                    ((decimal)totalAsistencias / totalSesionesConvocadas) * 100,
                    2
                );
            }

            decimal promedioDiasRecuperacion = 0;

            if (lesionesJugadora.Any())
            {
                promedioDiasRecuperacion = Math.Round(
                    (decimal)lesionesJugadora.Average(l =>
                    {
                        var fechaFin = l.FechaAltaReal ?? DateTime.Today;
                        return (fechaFin.Date - l.FechaInicio.Date).Days;
                    }),
                    2
                );
            }

            decimal promedioEstres = 0;
            decimal promedioHumor = 0;
            decimal promedioSueno = 0;
            decimal promedioRpe = 0;

            var todasEvaluaciones = await _context.EvaluacionesDiarias
                .Where(e => e.IdJugadora == idJugadora)
                .ToListAsync();
            // Cada serie utiliza únicamente evaluaciones que contienen ese indicador.
            // Los registros históricos sin MomentoEvaluacion se conservan.
            var evolucionRpe = todasEvaluaciones
                .Where(e => e.EsfuerzoRpe.HasValue)
                .OrderBy(e => e.Fecha)
                .ThenBy(e => e.Id)
                .TakeLast(10)
                .Select(e => new MiniGraficoRpe
                {
                    Fecha = e.Fecha,
                    Valor = e.EsfuerzoRpe!.Value,
                    Etiqueta = $"RPE {e.EsfuerzoRpe.Value}"
                })
                .ToList();

            var estresRegistrado = todasEvaluaciones
                .Where(e => e.Estres.HasValue).ToList();
            var humorRegistrado = todasEvaluaciones
                .Where(e => e.Humor.HasValue).ToList();
            var suenoRegistrado = todasEvaluaciones
                .Where(e => e.Sueño.HasValue).ToList();
            var rpeRegistrado = todasEvaluaciones
                .Where(e => e.EsfuerzoRpe.HasValue).ToList();

            if (estresRegistrado.Count > 0)
                promedioEstres = Math.Round((decimal)estresRegistrado.Average(e => e.Estres!.Value), 2);
            if (humorRegistrado.Count > 0)
                promedioHumor = Math.Round((decimal)humorRegistrado.Average(e => e.Humor!.Value), 2);
            if (suenoRegistrado.Count > 0)
                promedioSueno = Math.Round((decimal)suenoRegistrado.Average(e => e.Sueño!.Value), 2);
            if (rpeRegistrado.Count > 0)
                promedioRpe = Math.Round((decimal)rpeRegistrado.Average(e => e.EsfuerzoRpe!.Value), 2);

            var controlesNutricionales = await _context.EvaluacionesNutricionales
                .Where(n => n.IdJugadora == idJugadora)
                .ToListAsync();

            decimal? pesoMinimo = controlesNutricionales.Any()
                ? controlesNutricionales.Min(n => n.PesoKg)
                : null;

            decimal? pesoMaximo = controlesNutricionales.Any()
                ? controlesNutricionales.Max(n => n.PesoKg)
                : null;

            decimal? pesoPromedio = controlesNutricionales.Any()
                ? Math.Round(controlesNutricionales.Average(n => n.PesoKg), 2)
                : null;

            var lecturaGeneral = "Sin datos suficientes";

            if (totalLesiones == 0 && porcentajeAsistencia >= 85 && rpeRegistrado.Count > 0 && promedioRpe <= 7)
                lecturaGeneral = "Perfil estable y disponible";

            else if (totalLesiones > 0 && totalDiasBajaMedica > 30)
                lecturaGeneral = "Historial con impacto médico relevante";

            else if ((rpeRegistrado.Count > 0 && promedioRpe >= 8) ||
                     (suenoRegistrado.Count > 0 && promedioSueno <= 3))
                lecturaGeneral = "Requiere seguimiento por carga o recuperación";

            else if (totalEventosHistoricos >= 10)
                lecturaGeneral = "Historial en seguimiento regular";

            var notasPsicologicas = await _context.NotasPsicologicas
    .Where(p => p.IdJugadora == idJugadora)
    .OrderByDescending(p => p.Fecha)
    .Take(5)
    .ToListAsync();

            foreach (var n in controlesNutricionales
    .OrderByDescending(n => n.Fecha)
    .Take(5))
            {
                timeline.Add(new EventoTimelineDTO
                {
                    Fecha = n.Fecha,
                    TipoEvento = "Nutricion",
                    Titulo = "Evaluación nutricional",
                    Descripcion =
                        $"Peso: {n.PesoKg} kg\nGrasa: {n.PorcentajeGrasa}%\nMúsculo: {n.PorcentajeMusculo}%",
                    ModuloOrigen = "Nutrición"
                });
            }

            foreach (var p in notasPsicologicas)
            {
                timeline.Add(new EventoTimelineDTO
                {
                    Fecha = p.Fecha,
                    TipoEvento = "Psicologia",
                    Titulo = "Seguimiento psicológico",
                    Descripcion = "Sesión registrada por el área de Psicología.",
                    ModuloOrigen = "Psicología"
                });
            }

            foreach (var l in lesionesJugadora
      .OrderByDescending(l => l.FechaInicio)
      .Take(5))
            {
                var fechaFin = l.FechaAltaReal.HasValue
                    ? l.FechaAltaReal.Value.ToString("dd/MM/yyyy")
                    : "Sin alta registrada";

                timeline.Add(new EventoTimelineDTO
                {
                    Fecha = l.FechaInicio,
                    TipoEvento = "Lesion",
                    Titulo = "Lesión muscular",
                    Descripcion =
                        $"{l.Descripcion}\nReadaptación: {l.EstadoReadaptacion}%\nAlta: {fechaFin}",
                    ModuloOrigen = "Salud / Kinesiología"
                });
            }

            foreach (var l in lesionesJugadora
    .Where(l => l.FechaAltaReal.HasValue)
    .OrderByDescending(l => l.FechaAltaReal)
    .Take(5))
            {
                timeline.Add(new EventoTimelineDTO
                {
                    Fecha = l.FechaAltaReal!.Value,
                    TipoEvento = "AltaMedica",
                    Titulo = "Alta médica",
                    Descripcion =
                        $"Apta para volver progresivamente a la actividad.\nLesión previa: {l.Descripcion}",
                    ModuloOrigen = "Salud / Médico Clínico"
                });
            }

            var matrizHistorica = new MatrizHistoricaLegajoDTO
            {
                TotalSesionesConvocadas = totalSesionesConvocadas,
                TotalAsistencias = totalAsistencias,
                PorcentajeAsistencia = porcentajeAsistencia,

                TotalLesiones = totalLesiones,
                TotalDiasBaja = totalDiasBajaMedica,
                PromedioDiasRecuperacion = promedioDiasRecuperacion,

                TotalWellness = totalEvaluacionesWellness,
                PromedioEstres = promedioEstres,
                PromedioHumor = promedioHumor,
                PromedioSueno = promedioSueno,
                PromedioRpe = promedioRpe,

                TotalControlesNutricionales = totalControlesNutricionales,
                PesoMinimoKg = pesoMinimo,
                PesoMaximoKg = pesoMaximo,
                PesoPromedioKg = pesoPromedio,

                LecturaGeneral = lecturaGeneral
            };

            var ultimaEvaluacion = ultimasEvaluaciones.FirstOrDefault();
            var ultimaEvaluacionConRpe = todasEvaluaciones
                .Where(e => e.EsfuerzoRpe.HasValue)
                .OrderByDescending(e => e.Fecha)
                .ThenByDescending(e => e.Id)
                .FirstOrDefault();
            var ultimaEvaluacionBienestar = todasEvaluaciones
                .Where(e => e.Estres.HasValue || e.Humor.HasValue ||
                            e.Sueño.HasValue || e.DolorDoms.HasValue)
                .OrderByDescending(e => e.Fecha)
                .ThenByDescending(e => e.Id)
                .FirstOrDefault();

            var riesgo = CalcularRiesgoBasico(ultimaEvaluacion);

            var legajo = new LegajoDeportivoDTO
            {
                FichaGeneral = ficha,
                RiesgoActual = riesgo,

                Timeline = timeline
         .OrderByDescending(t => t.Fecha)
         .ToList(),

                DisponibilidadActual =
         CalcularDisponibilidad(
             riesgo,
             lesionesJugadora),

                UltimoEstadoBienestar =
         ultimaEvaluacionBienestar == null
             ? "Sin datos"
             : "Con evaluación registrada",

                UltimoEstadoCarga =
         ultimaEvaluacionConRpe == null
             ? "Sin datos"
             : $"RPE: {ultimaEvaluacionConRpe.EsfuerzoRpe.Value}",

                ResumenDeportivo = resumenDeportivo,
                EvolucionRpe = evolucionRpe,
                MatrizHistorica = matrizHistorica
            };

            /*
             * El backend no envía la línea de tiempo cuando
             * el usuario autenticado no posee ese permiso.
             */
            /*
 * =====================================================
 * FILTROS DE AUTORIZACIÓN DEL LEGAJO DEPORTIVO
 * =====================================================
 */

            /* ---------- Timeline ---------- */

            if (!permisos.PuedeVerTimeline)
            {
                legajo.Timeline = new List<EventoTimelineDTO>();
            }
            else
            {
                legajo.Timeline = legajo.Timeline
                    .Where(evento =>
                    {
                        var tipo = evento.TipoEvento?
                            .Trim()
                            .ToLowerInvariant();

                        if (tipo == "wellness")
                        {
                            return permisos.PuedeVerTimelineWellness;
                        }

                        if (tipo == "psicologia")
                        {
                            return permisos.PuedeVerTimelinePsicologia;
                        }

                        if (tipo == "nutricion")
                        {
                            return permisos.PuedeVerTimelineNutricion;
                        }

                        if (tipo == "lesion" ||
                            tipo == "altamedica")
                        {
                            return permisos.PuedeVerTimelineLesiones;
                        }

                        // Los eventos nuevos quedan ocultos por defecto
                        // hasta que se defina expresamente su permiso.
                        return false;
                    })
                    .ToList();
            }

            /* ---------- Datos de contacto ---------- */

            if (!permisos.PuedeVerDatosContacto)
            {
                legajo.FichaGeneral.Telefono = null;
                legajo.FichaGeneral.Email = null;
            }

            /* ---------- Peso, altura e IMC ---------- */

            if (!permisos.PuedeVerMedidasCorporalesActuales)
            {
                legajo.FichaGeneral.AlturaCm = null;
                legajo.FichaGeneral.PesoActualKg = null;
                legajo.FichaGeneral.ImcActual = null;
            }

            /* ---------- Resumen deportivo ---------- */

            if (!permisos.PuedeVerResumen)
            {
                legajo.ResumenDeportivo =
                    new ResumenDeportivoDTO();
            }
            else
            {
                // Debe reflejar solamente los eventos autorizados.
                legajo.ResumenDeportivo.TotalEventosTimeline =
                    legajo.Timeline.Count;

                if (!permisos.PuedeVerIndicadoresWellness)
                {
                    legajo.ResumenDeportivo
                        .TotalEvaluacionesWellness = 0;

                    legajo.ResumenDeportivo
                        .UltimaEvaluacionWellness = null;
                }

                if (!permisos.PuedeVerIndicadoresLesiones)
                {
                    legajo.ResumenDeportivo.TotalLesiones = 0;
                    legajo.ResumenDeportivo.UltimaLesion = null;
                    legajo.ResumenDeportivo.TotalDiasBajaMedica = 0;
                }

                if (!permisos.PuedeVerDetalleNutricional)
                {
                    legajo.ResumenDeportivo
                        .TotalControlesNutricionales = 0;

                    legajo.ResumenDeportivo
                        .UltimoControlNutricional = null;
                }
            }

            /* ---------- Matriz histórica ---------- */

            if (!permisos.PuedeVerMatrizHistorica)
            {
                legajo.MatrizHistorica =
                    new MatrizHistoricaLegajoDTO();
            }
            else
            {
                if (!permisos.PuedeVerIndicadoresWellness)
                {
                    legajo.MatrizHistorica.TotalWellness = 0;
                    legajo.MatrizHistorica.PromedioEstres = 0;
                    legajo.MatrizHistorica.PromedioHumor = 0;
                    legajo.MatrizHistorica.PromedioSueno = 0;
                    legajo.MatrizHistorica.PromedioRpe = 0;
                }

                if (!permisos.PuedeVerIndicadoresLesiones)
                {
                    legajo.MatrizHistorica.TotalLesiones = 0;
                    legajo.MatrizHistorica.TotalDiasBaja = 0;
                    legajo.MatrizHistorica
                        .PromedioDiasRecuperacion = 0;
                }

                if (!permisos.PuedeVerDetalleNutricional)
                {
                    legajo.MatrizHistorica
                        .TotalControlesNutricionales = 0;

                    legajo.MatrizHistorica.PesoMinimoKg = null;
                    legajo.MatrizHistorica.PesoMaximoKg = null;
                    legajo.MatrizHistorica.PesoPromedioKg = null;
                }
            }

            /* ---------- Riesgo ---------- */

            if (!permisos.PuedeVerRiesgo)
            {
                legajo.RiesgoActual = new RiesgoDeportivoDTO
                {
                    NivelRiesgo = "Restringido",
                    PuntajeRiesgo = 0,
                    MotivoPrincipal =
                        "Información no disponible para este perfil."
                };
            }

            /* ---------- Disponibilidad ---------- */

            if (!permisos.PuedeVerDisponibilidad)
            {
                legajo.DisponibilidadActual =
                    "Información restringida";
            }

            /* ---------- Evolución RPE ---------- */

            if (!permisos.PuedeVerEvolucionRpe)
            {
                legajo.EvolucionRpe =
                    new List<MiniGraficoRpe>();
            }

            /* ---------- Indicadores Wellness actuales ---------- */

            if (!permisos.PuedeVerIndicadoresWellness)
            {
                legajo.UltimoEstadoBienestar =
                    "Información restringida";

                legajo.UltimoEstadoCarga =
                    "Información restringida";
            }

            return legajo;

        }

        private RiesgoDeportivoDTO CalcularRiesgoBasico(EvaluacionDiaria? evaluacion)
        {
            if (evaluacion == null ||
                (!evaluacion.EsfuerzoRpe.HasValue && !evaluacion.DolorDoms.HasValue &&
                 !evaluacion.Sueño.HasValue && !evaluacion.Estres.HasValue))
                return new RiesgoDeportivoDTO
                {
                    NivelRiesgo = "Sin datos",
                    MotivoPrincipal = "Sin indicadores disponibles para este cálculo."
                };

            int puntaje = 0;

            bool alertaFatiga = evaluacion.EsfuerzoRpe.HasValue && evaluacion.EsfuerzoRpe.Value >= 8;
            bool alertaDolor = evaluacion.DolorDoms.HasValue && evaluacion.DolorDoms.Value >= 7;
            bool alertaSueno = evaluacion.Sueño.HasValue && evaluacion.Sueño.Value <= 3;
            bool alertaEstres = evaluacion.Estres.HasValue && evaluacion.Estres.Value >= 7;

            if (alertaFatiga) puntaje += 25;
            if (alertaDolor) puntaje += 25;
            if (alertaSueno) puntaje += 25;
            if (alertaEstres) puntaje += 25;

            // Indicador heurístico experimental; no constituye evaluación clínica.
            string nivel = puntaje >= 75 ? "Alto" : puntaje >= 40 ? "Medio" : "Bajo";

            string motivo = "Sin alertas relevantes";

            if (alertaFatiga) motivo = "Fatiga elevada";
            else if (alertaDolor) motivo = "Dolor muscular elevado";
            else if (alertaSueno) motivo = "Sueño bajo";
            else if (alertaEstres) motivo = "Estrés elevado";

            return new RiesgoDeportivoDTO
            {
                NivelRiesgo = nivel,
                PuntajeRiesgo = puntaje,
                MotivoPrincipal = motivo,
                AlertaFatiga = alertaFatiga,
                AlertaDolor = alertaDolor,
                AlertaSueno = alertaSueno,
                AlertaEstres = alertaEstres
            };
        }

        private string CalcularDisponibilidad(
     RiesgoDeportivoDTO riesgo,
     List<HistorialLesion> lesiones)
        {
            var lesionAbierta = lesiones.Any(l => !l.FechaAltaReal.HasValue);

            if (lesionAbierta)
                return "No disponible - lesión activa";

            var enReadaptacion = lesiones.Any(l =>
                l.FechaAltaReal.HasValue &&
                l.EstadoReadaptacion < 100);

            if (enReadaptacion)
                return "Disponible con readaptación";

            if (riesgo.NivelRiesgo == "Alto")
                return "Revisión médica obligatoria";

            if (riesgo.NivelRiesgo == "Medio")
                return "Disponible con seguimiento";

            return "Disponible";
        }
    }

}