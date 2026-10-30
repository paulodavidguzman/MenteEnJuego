using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MenteEnJuego.Dominio.Entidades
{
    [Table("metricas_gps_avanzadas")]
    public class MetricasGpsAvanzadas
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("id_evaluacion")]
        public int IdEvaluacion { get; set; }

        // CATEGORÍA 1: Desplazamiento y Velocidad
        [Required]
        [Column("distancia_total_km")]
        public decimal DistanciaTotalKm { get; set; }

        [Required]
        [Column("velocidad_maxima_kmh")]
        public decimal VelocidadMaximaKmh { get; set; }

        [Column("distancia_zona_trote_metros")]
        public decimal? DistanciaZonaTroteMetros { get; set; }

        [Column("distancia_zona_carrera_media_metros")]
        public decimal? DistanciaZonaCarreraMediaMetros { get; set; }

        [Column("distancia_zona_alta_velocidad_metros")]
        public decimal? DistanciaZonaAltaVelocidadMetros { get; set; }

        [Column("distancia_zona_sprint_metros")]
        public decimal? DistanciaZonaSprintMetros { get; set; }

        // CATEGORÍA 2: Aceleración y Desaceleración
        [Column("cantidad_aceleraciones_intensas")]
        public int? CantidadAceleracionesIntensas { get; set; }

        [Column("cantidad_desaceleraciones_intensas")]
        public int? CantidadDesaceleracionesIntensas { get; set; }

        [Column("cantidad_cambios_direction")]
        public int? CantidadCambiosDireccion { get; set; }

        [Required]
        [Column("player_load_indice")]
        public decimal PlayerLoadIndice { get; set; }

        // CATEGORÍA 3: Datos Fisiológicos
        [Column("fc_promedio_lpm")]
        public int? FcPromedioLpm { get; set; }

        [Column("fc_maxima_lpm")]
        public int? FcMaximaLpm { get; set; }

        [Column("tiempo_zona_aerobica_minutos")]
        public decimal? TiempoZonaAerobicaMinutos { get; set; }

        [Column("tiempo_zona_anaerobica_minutos")]
        public decimal? TiempoZonaAnaerobicaMinutos { get; set; }

        [Column("indice_recuperacion_fc")]
        public int? IndiceRecuperacionFc { get; set; }

        // CATEGORÍA 4: Análisis Táctico
        [Column("heatmap_json_url")]
        [StringLength(500)]
        public string? HeatmapJsonUrl { get; set; }

        [Column("posicion_media_x")]
        public decimal? PosicionMediaX { get; set; }

        [Column("posicion_media_y")]
        public decimal? PosicionMediaY { get; set; }

        // CATEGORÍA 5: Prevención de Lesiones
        [Column("simetria_zancada_porcentaje")]
        public decimal? SimetriaZancadaPorcentaje { get; set; }

        [Column("indice_fatiga_gps")]
        public decimal? IndiceFatigaGps { get; set; }

        [Column("cantidad_saltos_totales")]
        public int? CantidadSaltosTotales { get; set; }

        [Column("altura_salto_maxima_cm")]
        public decimal? AlturaSaltoMaximaCm { get; set; }

        [Column("tiempo_vuelo_promedio_ms")]
        public int? TiempoVueloPromedioMs { get; set; }

        [Column("fuerza_impacto_g_maxima")]
        public decimal? FuerzaImpactoGMaxima { get; set; }

        [ForeignKey(nameof(IdEvaluacion))]
        public virtual EvaluacionDiaria EvaluacionDiaria { get; set; } = null!;
    }
}
