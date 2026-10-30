using Microsoft.EntityFrameworkCore;
using MenteEnJuego.Dominio.Entidades;

namespace MenteEnJuego.Datos
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor necesario para que la API le inyecte la cadena de conexión de SQL Server
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Colecciones de Entidades (Mapeo directo a las tablas de SSMS)
        public DbSet<Domicilio> Domicilios { get; set; } = null!;
        public DbSet<Jugadora> Jugadoras { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<EvaluacionDiaria> EvaluacionesDiarias { get; set; } = null!;
        public DbSet<EvaluacionZonaDolor> EvaluacionesZonasDolor { get; set; } = null!;
        public DbSet<MetricasGpsAvanzadas> MetricasGpsAvanzadas { get; set; } = null!;
        public DbSet<StaffProfesional> StaffProfesionales { get; set; } = null!;
        public DbSet<HistorialLesion> HistorialLesiones { get; set; } = null!;


        public DbSet<EvaluacionKinesiologica> EvaluacionesKinesiologicas { get; set; }

        public DbSet<EvaluacionNutricional> EvaluacionesNutricionales { get; set; } = null!;
        public DbSet<NotaPsicologica> NotasPsicologicas { get; set; } = null!;
        public DbSet<ActividadClub> ActividadesClub { get; set; } = null!;
        public DbSet<AuditoriaSistema> AuditoriasSistema { get; set; } = null!;
        public DbSet<SesionDeportiva> SesionesDeportivas { get; set; } = null!;
        public DbSet<ConvocatoriaSesion> ConvocatoriasSesion { get; set; } = null!;
        public DbSet<FormularioSesion> FormulariosSesion { get; set; } = null!;
        public DbSet<Notificacion> Notificaciones { get; set; } = null!;
        public DbSet<Alerta> Alertas { get; set; } = null!;
        public DbSet<PizarraTecnica> PizarrasTecnicas { get; set; } = null!;
        public DbSet<Microciclo> Microciclos { get; set; } = null!;
        public DbSet<PlanificacionTecnica> PlanificacionesTecnicas { get; set; } = null!;
        public DbSet<ObjetivoEspecifico> ObjetivosEspecificos  { get; set; } = null!;
        public DbSet<ContenidoPlanificado> ContenidosPlanificados { get;  set; } = null!;
        public DbSet<BloqueSesion> BloquesSesion { get; set; } = null!;
        public DbSet<IndicadorPlanificado> IndicadoresPlanificados { get; set; } = null!;
        public DbSet<StaffPlanificacion> StaffPlanificaciones { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ===============================
            // Sesiones Deportivas
            // ===============================
            modelBuilder.Entity<SesionDeportiva>(entity =>
            {
                entity.HasOne(s => s.Microciclo)
    .WithMany(m => m.SesionesDeportivas)
    .HasForeignKey(s => s.MicrocicloId)
    .OnDelete(DeleteBehavior.NoAction)
    .HasConstraintName(
        "FK_SesionesDeportivas_Microciclos");

                entity.HasIndex(s => s.MicrocicloId)
                    .HasDatabaseName(
                        "IX_SesionesDeportivas_MicrocicloId");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.TipoSesion)
                      .HasMaxLength(30)
                      .IsRequired();

                entity.Property(e => e.Titulo)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Fecha)
                      .HasColumnType("date");

                entity.Property(e => e.HoraInicio)
                      .HasColumnType("time");

                entity.Property(e => e.HoraFin)
                      .HasColumnType("time");

                entity.Property(e => e.Lugar)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Objetivo)
                      .HasMaxLength(300);

                entity.Property(e => e.Estado)
                      .HasMaxLength(30)
                      .HasDefaultValue("Programada")
                      .IsRequired();

                entity.Property(e => e.FechaCreacion)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.FechaActualizacion)
                      .HasColumnType("datetime");

                entity.HasOne(e => e.UsuarioCreador)
                      .WithMany()
                      .HasForeignKey(e => e.CreadoPorUsuarioId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_SesionesDeportivas_Usuarios");

                entity.HasMany(e => e.Convocatorias)
                      .WithOne(e => e.SesionDeportiva)
                      .HasForeignKey(e => e.SesionDeportivaId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_ConvocatoriasSesion_SesionesDeportivas");

               /* entity.HasMany(e => e.Formularios)
                      .WithOne(e => e.SesionDeportiva)
                      .HasForeignKey(e => e.SesionDeportivaId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_FormulariosSesion_SesionesDeportivas");*/

              /*  entity.HasMany(e => e.Notificaciones)
                      .WithOne(e => e.SesionDeportiva)
                      .HasForeignKey(e => e.SesionDeportivaId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_Notificaciones_SesionesDeportivas");*/

              /*  entity.HasMany(e => e.Alertas)
                      .WithOne(e => e.SesionDeportiva)
                      .HasForeignKey(e => e.SesionDeportivaId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_Alertas_SesionesDeportivas");*/
            });


            // ===============================
            // Convocatorias por Sesión
            // ===============================
            modelBuilder.Entity<ConvocatoriaSesion>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EstadoConvocatoria)
                      .HasMaxLength(30)
                      .HasDefaultValue("Convocada")
                      .IsRequired();

                entity.Property(e => e.EstadoAsistencia)
                      .HasMaxLength(30);

                entity.Property(e => e.Observaciones)
                      .HasMaxLength(500);

                entity.Property(e => e.FechaCreacion)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Jugadora)
                      .WithMany()
                      .HasForeignKey(e => e.JugadoraId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_ConvocatoriasSesion_Jugadoras");

                entity.HasIndex(e => new
                {
                    e.SesionDeportivaId,
                    e.JugadoraId
                })
                .IsUnique()
                .HasDatabaseName("UQ_Convocatoria_Sesion_Jugadora");
            });


            // ===============================
            // Formularios por sesión
            // ===============================
            modelBuilder.Entity<FormularioSesion>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TipoFormulario)
                      .HasMaxLength(30)
                      .IsRequired();

                entity.Property(e => e.Estado)
                      .HasMaxLength(30)
                      .HasDefaultValue("Pendiente")
                      .IsRequired();

                entity.Property(e => e.FechaHabilitacion)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.FechaRespuesta)
                      .HasColumnType("datetime");

                entity.Property(e => e.Observaciones)
                      .HasMaxLength(500);

                entity.HasOne(e => e.SesionDeportiva)
                      .WithMany(s => s.Formularios)
                      .HasForeignKey(e => e.SesionDeportivaId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_FormulariosSesion_SesionesDeportivas");

                entity.HasOne(e => e.Jugadora)
                      .WithMany()
                      .HasForeignKey(e => e.JugadoraId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_FormulariosSesion_Jugadoras");

                entity.HasIndex(e => new
                {
                    e.SesionDeportivaId,
                    e.JugadoraId,
                    e.TipoFormulario
                })
                .IsUnique()
                .HasDatabaseName("UQ_Formulario_Sesion_Jugadora_Tipo");
            });
            // ===============================
            // Notificaciones
            // ===============================
            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Canal)
                      .HasMaxLength(30)
                      .IsRequired();

                entity.Property(e => e.TipoNotificacion)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Titulo)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Mensaje)
                      .IsRequired();

                entity.Property(e => e.FechaProgramada)
                      .HasColumnType("datetime");

                entity.Property(e => e.FechaEnvio)
                      .HasColumnType("datetime");

                entity.Property(e => e.Estado)
                      .HasMaxLength(30)
                      .HasDefaultValue("Pendiente")
                      .IsRequired();

                entity.Property(e => e.Intentos)
                      .HasDefaultValue(0);

                entity.Property(e => e.ErrorEnvio)
                      .HasMaxLength(1000);

                entity.Property(e => e.FechaCreacion)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_Notificaciones_Usuarios");

                entity.HasOne(e => e.Jugadora)
                      .WithMany()
                      .HasForeignKey(e => e.JugadoraId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_Notificaciones_Jugadoras");

                entity.HasOne(e => e.SesionDeportiva)
                      .WithMany(s => s.Notificaciones)
                      .HasForeignKey(e => e.SesionDeportivaId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_Notificaciones_SesionesDeportivas");
            });
            // ===============================
            // Alertas
            // ===============================
            modelBuilder.Entity<Alerta>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TipoAlerta)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Nivel)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Titulo)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Mensaje)
                      .IsRequired();

                entity.Property(e => e.Estado)
                      .HasMaxLength(30)
                      .HasDefaultValue("Activa")
                      .IsRequired();

                entity.Property(e => e.FechaCreacion)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.FechaResolucion)
                      .HasColumnType("datetime");

                entity.Property(e => e.ObservacionResolucion)
                      .HasMaxLength(500);

                entity.HasOne(e => e.Jugadora)
                      .WithMany()
                      .HasForeignKey(e => e.JugadoraId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_Alertas_Jugadoras");

                entity.HasOne(e => e.SesionDeportiva)
                      .WithMany(s => s.Alertas)
                      .HasForeignKey(e => e.SesionDeportivaId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_Alertas_SesionesDeportivas");

                entity.HasOne(e => e.UsuarioResolucion)
                      .WithMany()
                      .HasForeignKey(e => e.ResueltaPorUsuarioId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_Alertas_Usuarios");
            });

            // ===============================
            // Alertas
            // ===============================

            /* modelBuilder.Entity<Alerta>(entity =>
             {
                 entity.HasKey(e => e.Id);
             });

             // 1. Configuración de restricciones únicas y longitudes de campos
             modelBuilder.Entity<Jugadora>(entity =>
             {
                 entity.HasIndex(e => e.Dni).IsUnique();
                 entity.HasIndex(e => e.Matricula).IsUnique();
             });

             modelBuilder.Entity<Usuario>(entity =>
             {
                 entity.HasIndex(e => e.NombreUsuario).IsUnique();
                 entity.HasIndex(e => e.Email).IsUnique();
             });

             // 2. Configuración de la restricción única diaria para evitar duplicados en el cuestionario de Talleres
             modelBuilder.Entity<EvaluacionDiaria>(entity =>
             {
                 entity.HasIndex(e => new { e.IdJugadora, e.Fecha, e.TipoSesion })
                       .IsUnique()
                       .HasDatabaseName("UQ_Jugadora_Fecha_Tipo");

                 // Indicar a EF Core que el campo "unidades_arbitrarias" es un campo calculado por SSMS
                 entity.Property(e => e.UnidadesArbitrarias)
                       .HasComputedColumnSql("[esfuerzo_rpe] * [minutos_sesion]");
             });

             // 3. Configuración de la relación estricta 1 a 1 entre Evaluación Diaria y el Módulo GPS Élite
             modelBuilder.Entity<MetricasGpsAvanzadas>()
                 .HasOne(g => g.EvaluacionDiaria)
                 .WithOne(e => e.MetricasGps)
                 .HasForeignKey<MetricasGpsAvanzadas>(g => g.IdEvaluacion)
                 .OnDelete(DeleteBehavior.Cascade);*/

            // ===============================
            // Auditoría del sistema
            // ===============================
            modelBuilder.Entity<AuditoriaSistema>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FechaHora)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_AuditoriaSistema_Usuarios");
            });

            // ===============================
            // Evaluaciones diarias - Wellness
            // ===============================
            modelBuilder.Entity<EvaluacionDiaria>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Fecha)
                      .HasColumnType("datetime2");

                entity.Property(e => e.TipoSesion)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.EstadoAsistencia)
                      .HasMaxLength(30)
                      .IsRequired();

                entity.Property(e => e.UnidadesArbitrarias)
                      .HasComputedColumnSql(
                          "[esfuerzo_rpe] * [minutos_sesion]",
                          stored: false);

                entity.HasOne(e => e.Jugadora)
                      .WithMany()
                      .HasForeignKey(e => e.IdJugadora)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName(
                          "FK_evaluaciones_diarias_jugadoras_id_jugadora");

                entity.HasOne(e => e.SesionDeportiva)
                      .WithMany()
                      .HasForeignKey(e => e.SesionDeportivaId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName(
                          "FK_evaluaciones_diarias_SesionesDeportivas");

                entity.HasIndex(e => new
                {
                    e.IdJugadora,
                    e.Fecha,
                    e.TipoSesion
                })
                .IsUnique()
                .HasDatabaseName("UQ_Jugadora_Fecha_Tipo");

                entity.HasIndex(e => e.SesionDeportivaId)
                      .HasDatabaseName("IX_Evaluaciones_Sesion");
            });

            // ===============================
            // Zonas de dolor de evaluaciones
            // ===============================
            modelBuilder.Entity<EvaluacionZonaDolor>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ZonaMuscular)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ObservacionesOtro)
                      .HasMaxLength(255);

                entity.HasOne(e => e.EvaluacionDiaria)
                      .WithMany(e => e.ZonasDeDolor)
                      .HasForeignKey(e => e.IdEvaluacion)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ===============================
            // Métricas GPS avanzadas
            // ===============================
            modelBuilder.Entity<MetricasGpsAvanzadas>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DistanciaTotalKm)
                      .HasPrecision(18, 2);

                entity.Property(e => e.VelocidadMaximaKmh)
                      .HasPrecision(18, 2);

                entity.Property(e => e.DistanciaZonaTroteMetros)
                      .HasPrecision(18, 2);

                entity.Property(e => e.DistanciaZonaCarreraMediaMetros)
                      .HasPrecision(18, 2);

                entity.Property(e => e.DistanciaZonaAltaVelocidadMetros)
                      .HasPrecision(18, 2);

                entity.Property(e => e.DistanciaZonaSprintMetros)
                      .HasPrecision(18, 2);

                entity.Property(e => e.PlayerLoadIndice)
                      .HasPrecision(18, 2);

                entity.Property(e => e.TiempoZonaAerobicaMinutos)
                      .HasPrecision(18, 2);

                entity.Property(e => e.TiempoZonaAnaerobicaMinutos)
                      .HasPrecision(18, 2);

                entity.Property(e => e.PosicionMediaX)
                      .HasPrecision(18, 2);

                entity.Property(e => e.PosicionMediaY)
                      .HasPrecision(18, 2);

                entity.Property(e => e.SimetriaZancadaPorcentaje)
                      .HasPrecision(18, 2);

                entity.Property(e => e.IndiceFatigaGps)
                      .HasPrecision(18, 2);

                entity.Property(e => e.AlturaSaltoMaximaCm)
                      .HasPrecision(18, 2);

                entity.Property(e => e.FuerzaImpactoGMaxima)
                      .HasPrecision(18, 2);

                entity.Property(e => e.HeatmapJsonUrl)
                      .HasMaxLength(500);

                entity.HasOne(e => e.EvaluacionDiaria)
                      .WithOne(e => e.MetricasGps)
                      .HasForeignKey<MetricasGpsAvanzadas>(e => e.IdEvaluacion)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName(
                          "FK_metricas_gps_avanzadas_evaluaciones_diarias_id_evaluacion");

                entity.HasIndex(e => e.IdEvaluacion)
                      .IsUnique()
                      .HasDatabaseName("IX_metricas_gps_avanzadas_id_evaluacion");
            });

            // ===============================
            // Jugadoras
            // ===============================
            modelBuilder.Entity<Jugadora>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AlturaCm)
                      .HasPrecision(5, 2);

                entity.HasIndex(e => e.Dni)
                      .IsUnique()
                      .HasDatabaseName("IX_jugadoras_dni");

                entity.HasIndex(e => e.Matricula)
                      .IsUnique()
                      .HasFilter("[matricula] IS NOT NULL")
                      .HasDatabaseName("IX_jugadoras_matricula");

                entity.HasOne(e => e.DomicilioActual)
                      .WithMany()
                      .HasForeignKey(e => e.IdDomicilio)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // ===============================
            // Usuarios
            // ===============================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.NombreUsuario)
                      .HasColumnType("varchar(100)");

                entity.Property(e => e.Email)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.PasswordHash)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.Rol)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.FechaCreacion)
                      .HasColumnType("datetime2");

                entity.HasIndex(e => e.Email)
                      .IsUnique()
                      .HasDatabaseName("IX_usuarios_email");

                entity.HasOne(e => e.Jugadora)
                      .WithMany()
                      .HasForeignKey(e => e.IdJugadora)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // ===============================
            // Historial de lesiones
            // ===============================
            modelBuilder.Entity<HistorialLesion>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TipoLesion)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Gravedad)
                      .HasMaxLength(20);

                entity.Property(e => e.FechaInicio)
                      .HasColumnType("datetime2");

                entity.Property(e => e.FechaAltaEstimada)
                      .HasColumnType("datetime2");

                entity.Property(e => e.FechaAltaReal)
                      .HasColumnType("datetime2");

                entity.HasOne(e => e.Jugadora)
                      .WithMany()
                      .HasForeignKey(e => e.IdJugadora)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.StaffProfesional)
                      .WithMany()
                      .HasForeignKey(e => e.IdMedico)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // ===============================
            // Evaluaciones nutricionales
            // ===============================
            modelBuilder.Entity<EvaluacionNutricional>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Fecha)
                      .HasColumnType("datetime2");

                entity.Property(e => e.PesoKg)
                      .HasPrecision(6, 2);

                entity.Property(e => e.PorcentajeGrasa)
                      .HasPrecision(5, 2);

                entity.Property(e => e.PorcentajeMusculo)
                      .HasPrecision(5, 2);

                entity.Property(e => e.NotasNutricion)
                      .HasMaxLength(500);

                entity.HasOne(e => e.Jugadora)
                      .WithMany()
                      .HasForeignKey(e => e.IdJugadora)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.StaffProfesional)
                      .WithMany()
                      .HasForeignKey(e => e.IdMedico)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // ===============================
            // Notas psicológicas
            // ===============================
            modelBuilder.Entity<NotaPsicologica>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Fecha)
                      .HasColumnType("datetime2");

                entity.Property(e => e.SesionConfidencialNotas)
                      .IsRequired();

                entity.HasOne(e => e.Jugadora)
                      .WithMany()
                      .HasForeignKey(e => e.IdJugadora)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.StaffProfesional)
                      .WithMany()
                      .HasForeignKey(e => e.IdMedico)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // ===============================
            // Pizarra técnica
            // ===============================
            modelBuilder.Entity<PizarraTecnica>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Titulo)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.TipoPizarra)
                      .HasMaxLength(40)
                      .IsRequired();

                entity.Property(e => e.ContenidoJson)
                      .IsRequired();

                entity.Property(e => e.Observaciones)
                      .HasMaxLength(500);

                entity.Property(e => e.FechaCreacion)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.FechaActualizacion)
                      .HasColumnType("datetime");

                entity.Property(e => e.Estado)
                      .HasMaxLength(30)
                      .HasDefaultValue("Activa")
                      .IsRequired();

                entity.HasOne(e => e.SesionDeportiva)
                      .WithMany(s => s.PizarrasTecnicas)
                      .HasForeignKey(e => e.SesionDeportivaId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_PizarraTecnica_SesionesDeportivas");

                entity.HasOne(e => e.UsuarioCreador)
                      .WithMany()
                      .HasForeignKey(e => e.CreadoPorUsuarioId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_PizarraTecnica_Usuarios");
            });

            // MICROCICLO

            modelBuilder.Entity<Microciclo>(entity =>
            {
                entity.ToTable("Microciclos");

                entity.HasKey(m => m.Id)
                    .HasName("PK_Microciclos");

                entity.Property(m => m.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(m => m.NumeroSemana)
                    .IsRequired();

                entity.Property(m => m.FechaInicio)
                    .HasColumnType("date");

                entity.Property(m => m.FechaFin)
                    .HasColumnType("date");

                entity.Property(m => m.ObjetivoGeneral)
                    .HasMaxLength(1000);

                entity.Property(m => m.TipoMicrociclo)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(m => m.Estado)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Planificado");

                entity.Property(m => m.Observaciones)
                    .HasMaxLength(2000);

                entity.Property(m => m.FechaCreacion)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("SYSDATETIME()");

                entity.Property(m => m.FechaModificacion)
                    .HasColumnType("datetime2(0)");

                entity.HasCheckConstraint(
                    "CK_Microciclos_NumeroSemana",
                    "[NumeroSemana] > 0");

                entity.HasCheckConstraint(
                    "CK_Microciclos_Fechas",
                    "[FechaFin] >= [FechaInicio]");

                entity.HasCheckConstraint(
                    "CK_Microciclos_Estado",
                    "[Estado] IN " +
                    "('Planificado','EnEjecucion'," +
                    "'Finalizado','Cancelado')");

                entity.HasCheckConstraint(
                    "CK_Microciclos_Tipo",
                    "[TipoMicrociclo] IN " +
                    "('Carga','Competitivo'," +
                    "'Precompetitivo','Recuperacion'," +
                    "'Regenerativo','Evaluacion'," +
                    "'Descarga')");

                entity.HasIndex(m => new
                {
                    m.FechaInicio,
                    m.FechaFin
                })
                .HasDatabaseName(
                    "IX_Microciclos_Fechas");

                entity.HasIndex(m => m.Estado)
                    .HasDatabaseName(
                        "IX_Microciclos_Estado");
            });

            modelBuilder.Entity<PlanificacionTecnica>(entity =>
            {
                entity.ToTable("PlanificacionesTecnicas");

                entity.HasKey(p => p.Id)
                    .HasName("PK_PlanificacionesTecnicas");

                entity.Property(p => p.ObjetivoGeneral)
                    .HasMaxLength(1000);

                entity.Property(p =>
                        p.DuracionPlanificadaMinutos);

                entity.Property(p =>
                        p.IntensidadPlanificada)
                    .HasMaxLength(20);

                entity.Property(p => p.RpeObjetivo)
                    .HasColumnType("tinyint");

                entity.Property(p => p.CargaPlanificada)
                    .HasComputedColumnSql(
                        "[DuracionPlanificadaMinutos] * " +
                        "CONVERT(int, [RpeObjetivo])",
                        stored: true);

                entity.Property(p =>
                        p.MaterialesNecesarios)
                    .HasMaxLength(1500);

                entity.Property(p => p.Precauciones)
                    .HasMaxLength(1500);

                entity.Property(p => p.Observaciones)
                    .HasMaxLength(2000);

                entity.Property(p => p.Estado)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Borrador");

                entity.Property(p => p.FechaCreacion)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("SYSDATETIME()");

                entity.Property(p => p.FechaModificacion)
                    .HasColumnType("datetime2(0)");

                entity.HasOne(p =>
                        p.SesionDeportiva)
                    .WithOne(s =>
                        s.PlanificacionTecnica)
                    .HasForeignKey<PlanificacionTecnica>(
                        p => p.SesionDeportivaId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName(
                        "FK_PlanificacionesTecnicas_Sesion");

                entity.HasOne(p =>
                        p.UsuarioResponsable)
                    .WithMany()
                    .HasForeignKey(p =>
                        p.UsuarioResponsableId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName(
                        "FK_PlanificacionesTecnicas_Usuario");

                entity.HasIndex(p =>
                        p.SesionDeportivaId)
                    .IsUnique()
                    .HasDatabaseName(
                        "UX_PlanificacionesTecnicas_SesionDeportivaId");

                entity.HasIndex(p =>
                        p.UsuarioResponsableId)
                    .HasDatabaseName(
                        "IX_PlanificacionesTecnicas_UsuarioResponsable");

                entity.HasIndex(p => p.Estado)
                    .HasDatabaseName(
                        "IX_PlanificacionesTecnicas_Estado");

                entity.HasCheckConstraint(
                    "CK_PlanificacionesTecnicas_Duracion",
                    "[DuracionPlanificadaMinutos] IS NULL " +
                    "OR [DuracionPlanificadaMinutos] > 0");

                entity.HasCheckConstraint(
                    "CK_PlanificacionesTecnicas_Rpe",
                    "[RpeObjetivo] IS NULL " +
                    "OR [RpeObjetivo] BETWEEN 1 AND 10");

                entity.HasCheckConstraint(
                    "CK_PlanificacionesTecnicas_Intensidad",
                    "[IntensidadPlanificada] IS NULL " +
                    "OR [IntensidadPlanificada] IN " +
                    "('MuyBaja','Baja','Moderada'," +
                    "'Alta','MuyAlta')");

                entity.HasCheckConstraint(
                    "CK_PlanificacionesTecnicas_Estado",
                    "[Estado] IN " +
                    "('Borrador','Pendiente','Aprobada'," +
                    "'EnEjecucion','Ejecutada','Cancelada')");
            });

            modelBuilder.Entity<ObjetivoEspecifico>(entity =>
            {
                entity.ToTable("ObjetivosEspecificos");

                entity.HasKey(o => o.Id)
                    .HasName("PK_ObjetivosEspecificos");

                entity.Property(o => o.Descripcion)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(o => o.Prioridad)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValue("Media");

                entity.Property(o => o.Orden)
                    .IsRequired();

                entity.Property(o =>
                        o.ObservacionResultado)
                    .HasMaxLength(1500);

                entity.HasOne(o =>
                        o.PlanificacionTecnica)
                    .WithMany(p =>
                        p.ObjetivosEspecificos)
                    .HasForeignKey(o =>
                        o.PlanificacionTecnicaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName(
                        "FK_ObjetivosEspecificos_Planificacion");

                entity.HasIndex(o => new
                {
                    o.PlanificacionTecnicaId,
                    o.Orden
                })
                .IsUnique()
                .HasDatabaseName(
                    "UX_ObjetivosEspecificos_Planificacion_Orden");

                entity.HasCheckConstraint(
                    "CK_ObjetivosEspecificos_Prioridad",
                    "[Prioridad] IN ('Alta','Media','Baja')");

                entity.HasCheckConstraint(
                    "CK_ObjetivosEspecificos_Orden",
                    "[Orden] > 0");
            });

            modelBuilder.Entity<ContenidoPlanificado>(entity =>
            {
                entity.ToTable("ContenidosPlanificados");

                entity.HasKey(c => c.Id)
                    .HasName("PK_ContenidosPlanificados");

                entity.Property(c => c.Categoria)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(c => c.Descripcion)
                    .HasMaxLength(1000);

                entity.Property(c => c.Prioridad)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValue("Media");

                entity.Property(c => c.Orden)
                    .IsRequired();

                entity.HasOne(c =>
                        c.PlanificacionTecnica)
                    .WithMany(p =>
                        p.ContenidosPlanificados)
                    .HasForeignKey(c =>
                        c.PlanificacionTecnicaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName(
                        "FK_ContenidosPlanificados_Planificacion");

                entity.HasIndex(c => new
                {
                    c.PlanificacionTecnicaId,
                    c.Categoria,
                    c.Nombre
                })
                .IsUnique()
                .HasDatabaseName(
                    "UX_ContenidosPlanificados_Planificacion_Categoria_Nombre");

                entity.HasCheckConstraint(
                    "CK_ContenidosPlanificados_Categoria",
                    "[Categoria] IN "
                    + "('Tecnico','Tactico','Fisico',"
                    + "'Preventivo','Psicologico',"
                    + "'Regenerativo','Arqueras')");

                entity.HasCheckConstraint(
                    "CK_ContenidosPlanificados_Prioridad",
                    "[Prioridad] IN ('Alta','Media','Baja')");

                entity.HasCheckConstraint(
                    "CK_ContenidosPlanificados_Orden",
                    "[Orden] > 0");
            });

            modelBuilder.Entity<BloqueSesion>(entity =>
            {
                entity.ToTable("BloquesSesion");

                entity.HasKey(b => b.Id)
                    .HasName("PK_BloquesSesion");

                entity.Property(b => b.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(b => b.TipoBloque)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(b => b.Orden)
                    .IsRequired();

                entity.Property(b =>
                        b.DuracionPlanificadaMinutos)
                    .IsRequired();

                entity.Property(b =>
                        b.IntensidadPlanificada)
                    .HasMaxLength(20);

                entity.Property(b => b.Objetivo)
                    .HasMaxLength(1000);

                entity.Property(b => b.Descripcion)
                    .HasMaxLength(2000);

                entity.Property(b =>
                        b.PausaPlanificadaMinutos)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.HasOne(b =>
                        b.PlanificacionTecnica)
                    .WithMany(p =>
                        p.BloquesSesion)
                    .HasForeignKey(b =>
                        b.PlanificacionTecnicaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName(
                        "FK_BloquesSesion_Planificacion");

                entity.HasIndex(b => new
                {
                    b.PlanificacionTecnicaId,
                    b.Orden
                })
                .IsUnique()
                .HasDatabaseName(
                    "UX_BloquesSesion_Planificacion_Orden");

                entity.HasCheckConstraint(
                    "CK_BloquesSesion_Orden",
                    "[Orden] > 0");

                entity.HasCheckConstraint(
                    "CK_BloquesSesion_Duracion",
                    "[DuracionPlanificadaMinutos] > 0");

                entity.HasCheckConstraint(
                    "CK_BloquesSesion_Pausa",
                    "[PausaPlanificadaMinutos] >= 0");

                entity.HasCheckConstraint(
                    "CK_BloquesSesion_Intensidad",
                    "[IntensidadPlanificada] IS NULL " +
                    "OR [IntensidadPlanificada] IN " +
                    "('MuyBaja','Baja','Moderada'," +
                    "'Alta','MuyAlta')");

                entity.HasCheckConstraint(
                    "CK_BloquesSesion_Tipo",
                    "[TipoBloque] IN " +
                    "('Activacion'," +
                    "'EntradaEnCalor'," +
                    "'Tecnico'," +
                    "'Tactico'," +
                    "'Fisico'," +
                    "'PartePrincipal'," +
                    "'JuegoAplicado'," +
                    "'VueltaALaCalma'," +
                    "'Recuperacion'," +
                    "'Evaluacion')");
            });

            modelBuilder.Entity<IndicadorPlanificado>(entity =>
            {
                entity.ToTable("IndicadoresPlanificados");

                entity.HasKey(i => i.Id)
                    .HasName("PK_IndicadoresPlanificados");

                entity.Property(i => i.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(i => i.Categoria)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(i => i.UnidadMedida)
                    .HasMaxLength(50);

                entity.Property(i => i.ValorObjetivo)
                    .HasColumnType("decimal(12,3)");

                entity.Property(i => i.ValorMinimo)
                    .HasColumnType("decimal(12,3)");

                entity.Property(i => i.ValorMaximo)
                    .HasColumnType("decimal(12,3)");

                entity.Property(i => i.FuenteDatos)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(i => i.EsObligatorio)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(i => i.Orden)
                    .IsRequired();

                entity.HasOne(i =>
                        i.PlanificacionTecnica)
                    .WithMany(p =>
                        p.IndicadoresPlanificados)
                    .HasForeignKey(i =>
                        i.PlanificacionTecnicaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName(
                        "FK_IndicadoresPlanificados_Planificacion");

                entity.HasIndex(i => new
                {
                    i.PlanificacionTecnicaId,
                    i.Categoria
                })
                .HasDatabaseName(
                    "IX_IndicadoresPlanificados_Planificacion_Categoria");

                entity.HasIndex(i => new
                {
                    i.PlanificacionTecnicaId,
                    i.Orden
                })
                .IsUnique()
                .HasDatabaseName(
                    "UX_IndicadoresPlanificados_Planificacion_Orden");

                entity.HasCheckConstraint(
                    "CK_IndicadoresPlanificados_Categoria",
                    "[Categoria] IN " +
                    "('GPS','Wellness','Tecnico'," +
                    "'Tactico','Fisico','Medico'," +
                    "'Asistencia','CargaInterna')");

                entity.HasCheckConstraint(
                    "CK_IndicadoresPlanificados_FuenteDatos",
                    "[FuenteDatos] IN " +
                    "('GPS','FormularioPre'," +
                    "'FormularioPost','RegistroManual'," +
                    "'EvaluacionMedica','AnalisisTecnico')");

                entity.HasCheckConstraint(
                    "CK_IndicadoresPlanificados_Rango",
                    "[ValorMinimo] IS NULL " +
                    "OR [ValorMaximo] IS NULL " +
                    "OR [ValorMinimo] <= [ValorMaximo]");

                entity.HasCheckConstraint(
                    "CK_IndicadoresPlanificados_Orden",
                    "[Orden] > 0");
            });

            modelBuilder.Entity<StaffPlanificacion>(entity =>
            {
                entity.ToTable("StaffPlanificaciones");

                entity.HasKey(s => s.Id)
                    .HasName("PK_StaffPlanificaciones");

                entity.Property(s => s.FuncionEnSesion)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(s =>
                        s.EsResponsablePrincipal)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(s => s.Observaciones)
                    .HasMaxLength(1000);

                entity.HasOne(s =>
                        s.PlanificacionTecnica)
                    .WithMany(p =>
                        p.StaffPlanificaciones)
                    .HasForeignKey(s =>
                        s.PlanificacionTecnicaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName(
                        "FK_StaffPlanificaciones_Planificacion");

                entity.HasOne(s =>
                        s.StaffProfesional)
                    .WithMany(p =>
                        p.StaffPlanificaciones)
                    .HasForeignKey(s =>
                        s.StaffProfesionalId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName(
                        "FK_StaffPlanificaciones_Staff");

                entity.HasIndex(s => new
                {
                    s.PlanificacionTecnicaId,
                    s.StaffProfesionalId
                })
                .IsUnique()
                .HasDatabaseName(
                    "UX_StaffPlanificaciones_Planificacion_Staff");

                entity.HasIndex(s =>
                        s.PlanificacionTecnicaId)
                    .HasFilter(
                        "[EsResponsablePrincipal] = 1")
                    .IsUnique()
                    .HasDatabaseName(
                        "UX_StaffPlanificaciones_ResponsablePrincipal");
            });


            // =====================================
            // EVALUACIONES KINESIOLOGICAS
            // =====================================
            modelBuilder.Entity<EvaluacionKinesiologica>(entity =>
            {
                entity.ToTable("evaluaciones_kinesiologicas");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.FechaEvaluacion)
                    .HasColumnType("datetime2")
                    .IsRequired();

                entity.Property(e => e.RegionCorporal)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Lateralidad)
                    .HasMaxLength(30);

                entity.Property(e => e.Movilidad)
                    .HasMaxLength(500);

                entity.Property(e => e.LimitacionFuncional)
                    .HasMaxLength(100);

                entity.Property(e => e.FaseRehabilitacion)
                    .HasMaxLength(100);

                entity.HasOne(e => e.Jugadora)
                    .WithMany()
                    .HasForeignKey(e => e.IdJugadora)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Profesional)
                    .WithMany()
                    .HasForeignKey(e => e.IdProfesional)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.IdJugadora);
                entity.HasIndex(e => e.FechaEvaluacion);
            });

            base.OnModelCreating(modelBuilder);


        }
    }
}
