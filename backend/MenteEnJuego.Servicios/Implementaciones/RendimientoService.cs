using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class RendimientoService : IRendimientoService
    {
        private readonly ApplicationDbContext _context;

        public RendimientoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReporteSemanalDTO>> ObtenerReporteAcumuladoSemanalAsync(DateTime fechaLunes)
        {
            // Definir el rango estricto de la semana (Lunes a Domingo)
            var inicioSemana = fechaLunes.Date;
            var finSemana = inicioSemana.AddDays(7);

            // Traer todas las jugadoras activas con sus evaluaciones de esa semana específica
            var jugadorasConDatos = await _context.Jugadoras
                .Select(j => new
                {
                    j.Id,
                    j.Apellido,
                    j.Nombre,
                    Evaluaciones = _context.EvaluacionesDiarias
                        .Where(e => e.IdJugadora == j.Id && e.Fecha >= inicioSemana && e.Fecha < finSemana)
                        .Select(e => new { e.Fecha, e.UnidadesArbitrarias })
                        .ToList()
                })
                .ToListAsync();

            var listadoReportes = new List<ReporteSemanalDTO>();

            foreach (var item in jugadorasConDatos)
            {
                var dto = new ReporteSemanalDTO
                {
                    JugadoraId = item.Id,
                    ApellidoYNombre = $"{item.Apellido}, {item.Nombre}"
                };

                // Estructurar las cargas del Lunes (Día 0) al Domingo (Día 6)
                var cargasSieteDias = new double[7];
                double sumaTotalCarga = 0;
                int diasEntrenados = 0;

                for (int i = 0; i < 7; i++)
                {
                    var fechaDia = inicioSemana.AddDays(i);
                    var evalDia = item.Evaluaciones.FirstOrDefault(e => e.Fecha.Date == fechaDia.Date);

                    int carga = evalDia?.UnidadesArbitrarias ?? 0;
                    cargasSieteDias[i] = carga;
                    dto.CargasDiariasLunesADomingo.Add(carga);

                    if (carga > 0)
                    {
                        sumaTotalCarga += carga;
                        diasEntrenados++;
                    }
                }

                // Si la jugadora no entrenó ningún día de la semana, se omiten cálculos avanzados
                if (diasEntrenados == 0)
                {
                    dto.AlertaMonotonia = "Sin datos";
                    dto.AlertaFatiga = "Sin datos";
                    listadoReportes.Add(dto);
                    continue;
                }

                // 1. CARGA MEDIA SEMANAL (Fórmula del Excel)
                dto.CargaMediaSemanal = Math.Round(sumaTotalCarga / 7, 2);

                // 2. DESVIACIÓN ESTÁNDAR (Fórmula estadística SD de la planilla)
                double sumaDiferenciasAlCuadrado = cargasSieteDias.Sum(c => Math.Pow(c - dto.CargaMediaSemanal, 2));
                // Usamos división por 7 días totales para reflejar la variabilidad del microciclo completo
                dto.DesviacionEstandar = Math.Round(Math.Sqrt(sumaDiferenciasAlCuadrado / 7), 2);

                // 3. ÍNDICE DE MONOTONÍA (IM = Media / SD)
                if (dto.DesviacionEstandar > 0)
                {
                    dto.IndiceMonotonia = Math.Round(dto.CargaMediaSemanal / dto.DesviacionEstandar, 2);
                }
                else
                {
                    dto.IndiceMonotonia = 0; // Si no hay variación (ej. todos los días hizo 300 UA), la SD es 0
                }

                // 4. ÍNDICE DE FATIGA (IF = Suma Total * IM)
                dto.IndiceFatiga = Math.Round(sumaTotalCarga * dto.IndiceMonotonia, 2);

                // 5. EVALUACIÓN DE ALERTAS DE SEMÁFORO (Pintar las celdas del Excel en el frontend)
                if (dto.IndiceMonotonia >= 2.0)
                    dto.AlertaMonotonia = "Peligro (Riesgo alto de sobrecarga/enfermedad - Celda Roja)";
                else if (dto.IndiceMonotonia >= 1.5)
                    dto.AlertaMonotonia = "Alerta (Falta variación de estímulos - Celda Naranja)";
                else
                    dto.AlertaMonotonia = "Seguro (Excelente variación de cargas - Celda Verde)";

                if (dto.IndiceFatiga >= 4000)
                    dto.AlertaFatiga = "Peligro (Agotamiento agudo residual crítico)";
                else
                    dto.AlertaFatiga = "Normal (Fatiga controlada)";

                listadoReportes.Add(dto);
            }

            return listadoReportes;
        }
    }
}
