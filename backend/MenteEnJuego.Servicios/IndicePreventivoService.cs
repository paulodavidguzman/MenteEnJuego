using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Datos;
using MenteEnJuego.Dominio.DTOs;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios
{
    public class IndicePreventivoService : IIndicePreventivoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICicloMenstrualService _cicloMenstrualService;

        public IndicePreventivoService(
            ApplicationDbContext context,
            ICicloMenstrualService cicloMenstrualService)
        {
            _context = context;
            _cicloMenstrualService = cicloMenstrualService;
        }

        public async Task<IndicePreventivoResultadoDTO> CalcularIndicePreventivoAsync(
            int idJugadora,
            DateTime fecha)
        {
            var fechaDesde = fecha.Date;
            var fechaHasta = fechaDesde.AddDays(1);

            var evaluacionesDelDia = await _context.EvaluacionesDiarias
                .Where(e =>
                    e.IdJugadora == idJugadora &&
                    e.Fecha >= fechaDesde &&
                    e.Fecha < fechaHasta)
                .OrderBy(e => e.Fecha)
                .Select(e => new
                {
                    e.Id,
                    e.Fecha,
                    e.TipoSesion,
                    e.Estres,
                    e.Humor,
                    e.Sueño,
                    e.DolorDoms,
                    e.EsfuerzoRpe,
                    e.MinutosSesion,
                    e.UnidadesArbitrarias,
                    e.DiaCicloMenstrual,

                    ZonasDolor = e.ZonasDeDolor
                        .Select(z => new
                        {
                            z.ZonaMuscular,
                            z.IntensidadDolor,
                            z.ObservacionesOtro
                        })
                        .ToList(),

                    Gps = e.MetricasGps == null
                        ? null
                        : new
                        {
                            e.MetricasGps.Id,
                            e.MetricasGps.DistanciaTotalKm,
                            e.MetricasGps.VelocidadMaximaKmh,
                            e.MetricasGps.DistanciaZonaAltaVelocidadMetros,
                            e.MetricasGps.DistanciaZonaSprintMetros,
                            e.MetricasGps.CantidadAceleracionesIntensas,
                            e.MetricasGps.CantidadDesaceleracionesIntensas,
                            e.MetricasGps.PlayerLoadIndice,
                            e.MetricasGps.FcPromedioLpm,
                            e.MetricasGps.FcMaximaLpm,
                            e.MetricasGps.IndiceRecuperacionFc,
                            e.MetricasGps.SimetriaZancadaPorcentaje,
                            e.MetricasGps.IndiceFatigaGps
                        }
                })
                .ToListAsync();

            if (evaluacionesDelDia.Count == 0)
            {
                return new IndicePreventivoResultadoDTO
                {
                    Disponible = false,
                    IdJugadora = idJugadora,
                    Fecha = fechaDesde.ToString("yyyy-MM-dd"),
                    IndicePreventivo = null,
                    NivelPreventivo = "SIN_DATOS",
                    GpsDisponible = false
                };
            }

            // =========================================================
            // WELLNESS DEL DÍA
            // =========================================================

            var evaluacionWellness =
                evaluacionesDelDia.FirstOrDefault(e =>
                    e.TipoSesion != null &&
                    e.TipoSesion.ToLower().Contains("pre"))
                ?? evaluacionesDelDia.First();

            int? estres = evaluacionWellness.Estres;
            int? humor = evaluacionWellness.Humor;
            int? sueno = evaluacionWellness.Sueño;
            int? dolorDoms = evaluacionWellness.DolorDoms;

            int cantidadVariablesWellness = 0;
            int sumaWellness = 0;

            if (estres.HasValue)
            {
                sumaWellness += estres.Value;
                cantidadVariablesWellness++;
            }

            if (humor.HasValue)
            {
                sumaWellness += humor.Value;
                cantidadVariablesWellness++;
            }

            if (sueno.HasValue)
            {
                sumaWellness += sueno.Value;
                cantidadVariablesWellness++;
            }

            if (dolorDoms.HasValue)
            {
                sumaWellness += dolorDoms.Value;
                cantidadVariablesWellness++;
            }

            int? wellness =
                cantidadVariablesWellness > 0
                    ? sumaWellness
                    : null;


            // =========================================================
            // RPE Y CARGA INTERNA DEL DÍA
            // =========================================================

            var evaluacionCarga =
                evaluacionesDelDia
                    .Where(e =>
                        e.EsfuerzoRpe > 0 ||
                        e.MinutosSesion > 0 ||
                        e.UnidadesArbitrarias > 0)
                    .OrderByDescending(e => e.Fecha)
                    .FirstOrDefault();

            int? rpe = null;
            int? minutosSesion = null;
            int? cargaInterna = null;

            if (evaluacionCarga != null)
            {
                if (evaluacionCarga.EsfuerzoRpe > 0)
                    rpe = evaluacionCarga.EsfuerzoRpe;

                if (evaluacionCarga.MinutosSesion > 0)
                    minutosSesion = evaluacionCarga.MinutosSesion;

                if (evaluacionCarga.UnidadesArbitrarias > 0)
                    cargaInterna = evaluacionCarga.UnidadesArbitrarias;
            }


            // =========================================================
            // DOLOR DEL DÍA
            // =========================================================

            var zonasDolorDelDia = evaluacionesDelDia
                .SelectMany(e => e.ZonasDolor)
                .Where(z =>
                    z.IntensidadDolor.HasValue &&
                    z.IntensidadDolor.Value > 0)
                .ToList();

            int cantidadZonasDolor = zonasDolorDelDia.Count;

            int? dolorMaximo = cantidadZonasDolor > 0
                ? zonasDolorDelDia.Max(z => z.IntensidadDolor)
                : null;

            int? domsGeneral = evaluacionWellness.DolorDoms;


            // =========================================================
            // CICLO MENSTRUAL DEL DÍA
            // =========================================================

            var evaluacionCiclo = evaluacionesDelDia
                .Where(e => e.DiaCicloMenstrual > 0)
                .OrderByDescending(e => e.Fecha)
                .FirstOrDefault();

            int? diaCiclo = null;

            if (evaluacionCiclo != null &&
                evaluacionCiclo.DiaCicloMenstrual > 0)
            {
                diaCiclo = evaluacionCiclo.DiaCicloMenstrual;
            }

            FaseCicloMenstrualDTO? analisisCiclo = null;

            if (diaCiclo.HasValue)
            {
                analisisCiclo =
                    _cicloMenstrualService.EvaluarFaseFisiologica(
                        diaCiclo.Value);
            }


            // =========================================================
            // GPS DEL MISMO DÍA
            // =========================================================

            var evaluacionGps = evaluacionesDelDia
                .Where(e => e.Gps != null)
                .OrderByDescending(e => e.Fecha)
                .FirstOrDefault();

            var gpsDelDia = evaluacionGps?.Gps;


            // =========================================================
            // CÁLCULO DEL ÍNDICE PREVENTIVO
            // =========================================================

            double puntajeWellness = 0;
            double puntajeCarga = 0;
            double puntajeDolor = 0;
            double puntajeCiclo = 0;
            double puntajeGps = 0;

            bool usaWellness = wellness.HasValue;

            bool usaCarga =
                rpe.HasValue ||
                cargaInterna.HasValue;

            bool usaDolor =
                dolorMaximo.HasValue ||
                domsGeneral.HasValue;

            bool usaCiclo = diaCiclo.HasValue;

            bool usaGps = gpsDelDia != null;


            // =========================================================
            // COMPONENTE WELLNESS
            // =========================================================

            if (usaWellness)
            {
                int wellnessLimitado =
                    Math.Clamp(wellness!.Value, 4, 20);

                puntajeWellness =
                    ((20.0 - wellnessLimitado) / 16.0) * 100.0;
            }


            // =========================================================
            // COMPONENTE CARGA INTERNA
            // =========================================================

            if (usaCarga)
            {
                double señalRpe = 0;
                double señalCargaInterna = 0;

                if (rpe.HasValue)
                {
                    int rpeLimitado =
                        Math.Clamp(rpe.Value, 0, 10);

                    señalRpe =
                        (rpeLimitado / 10.0) * 100.0;
                }

                if (cargaInterna.HasValue)
                {
                    int cargaLimitada =
                        Math.Clamp(cargaInterna.Value, 0, 1000);

                    señalCargaInterna =
                        (cargaLimitada / 1000.0) * 100.0;
                }

                if (rpe.HasValue && cargaInterna.HasValue)
                {
                    puntajeCarga =
                        (señalRpe * 0.60) +
                        (señalCargaInterna * 0.40);
                }
                else if (rpe.HasValue)
                {
                    puntajeCarga = señalRpe;
                }
                else
                {
                    puntajeCarga = señalCargaInterna;
                }
            }


            // =========================================================
            // COMPONENTE DOLOR
            // =========================================================

            if (usaDolor)
            {
                double señalDoms = 0;
                double señalDolorMaximo = 0;
                double señalCantidadZonas = 0;

                if (domsGeneral.HasValue)
                {
                    int domsLimitado =
                        Math.Clamp(domsGeneral.Value, 1, 5);

                    señalDoms =
                        ((5.0 - domsLimitado) / 4.0) * 100.0;
                }

                if (dolorMaximo.HasValue)
                {
                    int dolorLimitado =
                        Math.Clamp(dolorMaximo.Value, 0, 10);

                    señalDolorMaximo =
                        (dolorLimitado / 10.0) * 100.0;
                }

                int zonasLimitadas =
                    Math.Clamp(cantidadZonasDolor, 0, 5);

                señalCantidadZonas =
                    (zonasLimitadas / 5.0) * 100.0;

                puntajeDolor =
                    (señalDolorMaximo * 0.60) +
                    (señalCantidadZonas * 0.25) +
                    (señalDoms * 0.15);
            }


            // =========================================================
            // COMPONENTE CICLO MENSTRUAL
            // =========================================================

            // El ciclo se mantiene como contexto fisiológico.
            // Actualmente no incrementa automáticamente el índice.
            if (usaCiclo)
            {
                puntajeCiclo = 0;
            }


            // =========================================================
            // COMPONENTE GPS
            // =========================================================

            if (usaGps && gpsDelDia != null)
            {
                if (gpsDelDia.IndiceFatigaGps.HasValue)
                {
                    double fatigaGps =
                        (double)gpsDelDia.IndiceFatigaGps.Value;

                    puntajeGps =
                        Math.Clamp(fatigaGps, 0.0, 100.0);
                }
                else
                {
                    usaGps = false;
                }
            }


            // =========================================================
            // PESOS DEL ÍNDICE PREVENTIVO
            // =========================================================

            double sumaPonderada = 0;
            double sumaPesos = 0;

            double pesoWellness;
            double pesoCarga;
            double pesoDolor;
            double pesoGps;

            if (usaGps)
            {
                pesoWellness = 0.25;
                pesoCarga = 0.20;
                pesoDolor = 0.35;
                pesoGps = 0.20;
            }
            else
            {
                pesoWellness = 0.30;
                pesoCarga = 0.25;
                pesoDolor = 0.45;
                pesoGps = 0;
            }

            if (usaWellness)
            {
                sumaPonderada +=
                    puntajeWellness * pesoWellness;

                sumaPesos += pesoWellness;
            }

            if (usaCarga)
            {
                sumaPonderada +=
                    puntajeCarga * pesoCarga;

                sumaPesos += pesoCarga;
            }

            if (usaDolor)
            {
                sumaPonderada +=
                    puntajeDolor * pesoDolor;

                sumaPesos += pesoDolor;
            }

            if (usaGps)
            {
                sumaPonderada +=
                    puntajeGps * pesoGps;

                sumaPesos += pesoGps;
            }


            // =========================================================
            // ÍNDICE FINAL
            // =========================================================

            double? indicePreventivo = null;

            if (sumaPesos > 0)
            {
                indicePreventivo =
                    Math.Round(
                        sumaPonderada / sumaPesos,
                        1);
            }


            // =========================================================
            // NIVEL PREVENTIVO
            // =========================================================

            string nivelPreventivo = "SIN_DATOS";

            if (indicePreventivo.HasValue)
            {
                if (indicePreventivo.Value < 35)
                {
                    nivelPreventivo = "BAJO";
                }
                else if (indicePreventivo.Value < 65)
                {
                    nivelPreventivo = "MODERADO";
                }
                else
                {
                    nivelPreventivo = "ALTO";
                }
            }


            // =========================================================
            // FACTORES EXPLICATIVOS
            // =========================================================

            var factoresPrincipales = new List<string>();
            var contextoPreventivo = new List<string>();


            // DOLOR
            if (dolorMaximo.HasValue)
            {
                if (dolorMaximo.Value >= 7)
                {
                    factoresPrincipales.Add(
                        $"Dolor localizado elevado: {dolorMaximo.Value}/10");
                }
                else if (dolorMaximo.Value >= 4)
                {
                    factoresPrincipales.Add(
                        $"Dolor localizado moderado: {dolorMaximo.Value}/10");
                }
            }

            if (cantidadZonasDolor >= 3)
            {
                factoresPrincipales.Add(
                    $"Dolor registrado en {cantidadZonasDolor} zonas corporales");
            }


            // CARGA / RPE
            if (rpe.HasValue)
            {
                if (rpe.Value >= 8)
                {
                    factoresPrincipales.Add(
                        $"Percepción de esfuerzo elevada: RPE {rpe.Value}/10");
                }
                else if (rpe.Value >= 6)
                {
                    factoresPrincipales.Add(
                        $"Percepción de esfuerzo moderada-alta: RPE {rpe.Value}/10");
                }
            }

            if (cargaInterna.HasValue)
            {
                contextoPreventivo.Add(
                    $"Carga interna registrada: {cargaInterna.Value} UA");
            }


            // WELLNESS
            if (wellness.HasValue)
            {
                contextoPreventivo.Add(
                    $"Wellness registrado: {wellness.Value}/20");
            }


            // CICLO MENSTRUAL
            if (analisisCiclo != null && diaCiclo.HasValue)
            {
                contextoPreventivo.Add(
                    $"Día {diaCiclo.Value} del ciclo - {analisisCiclo.FaseNombre}");
            }


            // GPS
            if (!usaGps)
            {
                contextoPreventivo.Add(
                    "GPS no disponible para la fecha analizada");
            }
            else
            {
                contextoPreventivo.Add(
                    "GPS disponible e incorporado al Índice Preventivo");
            }


            // =========================================================
            // RESULTADO
            // =========================================================

            return new IndicePreventivoResultadoDTO
            {
                Disponible = indicePreventivo.HasValue,
                IdJugadora = idJugadora,
                Fecha = fechaDesde.ToString("yyyy-MM-dd"),
                IndicePreventivo = indicePreventivo,
                NivelPreventivo = nivelPreventivo,
                FactoresPrincipales = factoresPrincipales,
                ContextoPreventivo = contextoPreventivo,
                GpsDisponible = usaGps
            };
        }
    }
}