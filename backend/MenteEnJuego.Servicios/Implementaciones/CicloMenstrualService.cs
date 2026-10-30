using MenteEnJuego.Dominio.DTOs;
using MenteEnJuego.Servicios.Interfaces;

namespace MenteEnJuego.Servicios.Implementaciones
{
    public class CicloMenstrualService : ICicloMenstrualService
    {
        public FaseCicloMenstrualDTO EvaluarFaseFisiologica(int diaCiclo)
        {
            var dto = new FaseCicloMenstrualDTO
            {
                DiaCiclo = diaCiclo
            };

            // =====================================================
            // INTERPRETACIÓN CONTEXTUAL DEL CICLO MENSTRUAL
            // =====================================================
            //
            // La fase se estima a partir del día del ciclo informado.
            // No constituye confirmación hormonal de la fase ni
            // diagnóstico médico.
            //
            // La información debe interpretarse junto con síntomas
            // individuales, Wellness, dolor, recuperación, carga
            // interna y métricas GPS cuando estén disponibles.
            //
            // El color se utiliza únicamente para representación
            // visual de la fase y NO representa nivel de riesgo.

            if (diaCiclo >= 1 && diaCiclo <= 8)
            {
                dto.FaseNombre =
                    "Fase Menstrual / Folicular Temprana Estimada";

                dto.ColorSemaforo =
                    "Rosa";

                dto.ImpactoBiologico =
                    "Etapa estimada en la que pueden presentarse variaciones individuales en síntomas, percepción de fatiga y recuperación.";

                dto.RecomendacionPlanificacion =
                    "Interpretar la respuesta individual de la jugadora junto con Wellness, dolor, recuperación y carga de entrenamiento.";

                dto.AlertaMedicaPreventiva =
                    "Observar síntomas individuales y cambios respecto del patrón habitual de la jugadora.";
            }
            else if (diaCiclo >= 9 && diaCiclo <= 12)
            {
                dto.FaseNombre =
                    "Fase Folicular Tardía Estimada";

                dto.ColorSemaforo =
                    "Verde";

                dto.ImpactoBiologico =
                    "Etapa estimada asociada a cambios hormonales propios de la fase folicular. La respuesta al entrenamiento puede variar entre jugadoras.";

                dto.RecomendacionPlanificacion =
                    "Mantener la planificación basada en la respuesta individual, la carga prevista y los indicadores de recuperación.";

                dto.AlertaMedicaPreventiva =
                    "Sin alerta específica por fase. Valorar los restantes indicadores preventivos.";
            }
            else if (diaCiclo >= 13 && diaCiclo <= 14)
            {
                dto.FaseNombre =
                    "Fase Ovulatoria Estimada";

                dto.ColorSemaforo =
                    "Amarillo";

                dto.ImpactoBiologico =
                    "Etapa ovulatoria estimada. Se han estudiado variaciones hormonales, neuromusculares y biomecánicas durante este período, con resultados que requieren interpretación individual.";

                dto.RecomendacionPlanificacion =
                    "Considerar esta fase como información contextual y valorar conjuntamente dolor, Wellness, carga, fatiga y antecedentes individuales.";

                dto.AlertaMedicaPreventiva =
                    "No determina por sí sola un aumento individual del riesgo de lesión. Mantener seguimiento preventivo habitual.";
            }
            else if (diaCiclo >= 15 && diaCiclo <= 17)
            {
                dto.FaseNombre =
                    "Fase Lútea Temprana Estimada";

                dto.ColorSemaforo =
                    "Naranja";

                dto.ImpactoBiologico =
                    "Etapa estimada de transición hacia la fase lútea, con posibles variaciones individuales en percepción del esfuerzo, recuperación y respuesta fisiológica.";

                dto.RecomendacionPlanificacion =
                    "Interpretar RPE, Wellness, recuperación y carga en relación con el patrón habitual de la jugadora.";

                dto.AlertaMedicaPreventiva =
                    "Observar cambios individuales relevantes respecto de registros previos.";
            }
            else if (diaCiclo >= 18 && diaCiclo <= 24)
            {
                dto.FaseNombre =
                    "Fase Lútea Media Estimada";

                dto.ColorSemaforo =
                    "Azul";

                dto.ImpactoBiologico =
                    "Etapa lútea media estimada. La respuesta fisiológica y la percepción del esfuerzo pueden presentar variabilidad individual.";

                dto.RecomendacionPlanificacion =
                    "Mantener la planificación según carga prevista, recuperación y respuesta individual de la jugadora.";

                dto.AlertaMedicaPreventiva =
                    "Sin alerta específica por fase. Continuar seguimiento de los indicadores habituales.";
            }
            else if (diaCiclo >= 25 && diaCiclo <= 28)
            {
                dto.FaseNombre =
                    "Fase Lútea Tardía / Premenstrual Estimada";

                dto.ColorSemaforo =
                    "Violeta";

                dto.ImpactoBiologico =
                    "Etapa premenstrual estimada en la que algunas jugadoras pueden presentar síntomas físicos o emocionales que afecten su percepción de bienestar y recuperación.";

                dto.RecomendacionPlanificacion =
                    "Valorar especialmente síntomas individuales, Wellness, dolor, recuperación y percepción del esfuerzo antes de modificar la carga.";

                dto.AlertaMedicaPreventiva =
                    "Observar cambios relevantes en síntomas, humor, estrés, dolor o recuperación respecto del patrón habitual.";
            }
            else
            {
                // Días 29 a 45.
                //
                // El sistema acepta estos valores para contemplar ciclos
                // de mayor duración, pero no asigna automáticamente una
                // fase fisiológica únicamente a partir del calendario.

                dto.FaseNombre =
                  "Fase no determinada por calendario";

                dto.ColorSemaforo =
                    "Gris";

                dto.ImpactoBiologico =
                    "El día registrado supera el ciclo de referencia utilizado para la estimación automática de fases.";

                dto.RecomendacionPlanificacion =
                    "No modificar la planificación únicamente por este dato. Interpretar junto con el patrón individual de la jugadora, síntomas, Wellness, dolor, recuperación y carga.";

                dto.AlertaMedicaPreventiva =
                    "Revisar el registro y considerar seguimiento profesional si existen cambios relevantes respecto del patrón menstrual habitual de la jugadora.";
            }

            return dto;
        }
    }
}
