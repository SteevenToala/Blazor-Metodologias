using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;

namespace MyCleanApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Persona> Persona { get; set; }
        public DbSet<Docente> Docente { get; set; }
        public DbSet<CumplimientoRequisito> CumplimientoRequisito { get; set; }
        public DbSet<CursoCapacitacion> CursoCapacitacion { get; set; }
        public DbSet<EvaluacionDocente> EvaluacionDocente { get; set; }
        public DbSet<NivelAcademico> NivelAcademico { get; set; }
        public DbSet<ProyectoInvestigacion> ProyectoInvestigacion { get; set; }
        public DbSet<PublicacionAcademica> PublicacionAcademica { get; set; }
        public DbSet<ReporteAvance> ReporteAvance { get; set; }
        public DbSet<RequisitoNivelAcademico> RequisitoNivelAcademico { get; set; }
        public DbSet<RequisitoPromocion> RequisitoPromocion { get; set; }
        public DbSet<SolicitudAvanceRango> SolicitudAvanceRango { get; set; }
        public DbSet<TipoRequisito> TipoRequisito { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de tabla Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.Rol)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(e => e.Persona)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(e => e.PersonaId);
            });

            // Configuración de tabla Persona
            modelBuilder.Entity<Persona>(entity =>
            {
                entity.ToTable("Persona");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Direccion).HasMaxLength(200);
                entity.Property(e => e.FechaNacimiento).IsRequired();
            });

            // Configuración de tabla TipoRequisito
            modelBuilder.Entity<TipoRequisito>(entity =>
            {
                entity.ToTable("TipoRequisito");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Configuración de tabla NivelAcademico
            modelBuilder.Entity<NivelAcademico>(entity =>
            {
                entity.ToTable("NivelAcademico");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.descripcion)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            // Configuración de tabla RequisitoNivelAcademico
            modelBuilder.Entity<RequisitoNivelAcademico>(entity =>
            {
                entity.ToTable("RequisitoNivelAcademico");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.ValorRequerido)
                    .IsRequired();

                entity.HasOne(e => e.NivelAcademico)
                    .WithMany()
                    .HasForeignKey(e => e.NivelAcademicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TipoRequisito)
                    .WithMany()
                    .HasForeignKey(e => e.TipoRequisitoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // Configuración de tabla Docente
            modelBuilder.Entity<Docente>(entity =>
            {
                entity.ToTable("Docente");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.FechaInicioNivel)
                    .IsRequired();

                entity.HasOne(e => e.NivelAcademico)
                    .WithMany()
                    .HasForeignKey(e => e.NivelAcademicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de tabla SolicitudAvanceRango
            modelBuilder.Entity<SolicitudAvanceRango>(entity =>
            {
                entity.ToTable("SolicitudAvanceRango");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.FechaSolicitud)
                    .IsRequired();

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Observaciones)
                    .HasMaxLength(300);

                entity.HasOne(e => e.Docente)
                    .WithMany()
                    .HasForeignKey(e => e.DocenteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NuevoNivelAcademico)
                    .WithMany()
                    .HasForeignKey(e => e.NuevoNivelAcademicoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de tabla ReporteAvance
            modelBuilder.Entity<ReporteAvance>(entity =>
            {
                entity.ToTable("ReporteAvance");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.FechaGeneracion)
                    .IsRequired();

                entity.HasOne(e => e.Docente)
                    .WithMany()
                    .HasForeignKey(e => e.DocenteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de tabla PublicacionAcademica
            modelBuilder.Entity<PublicacionAcademica>(entity =>
            {
                entity.ToTable("PublicacionAcademica");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Revista)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Volumen)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Anio)
                    .IsRequired();

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Archivo)
                    .HasColumnType("varbinary(max)");

                entity.HasOne(e => e.Docente)
                    .WithMany()
                    .HasForeignKey(e => e.DocenteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de tabla ProyectoInvestigacion
            modelBuilder.Entity<ProyectoInvestigacion>(entity =>
            {
                entity.ToTable("ProyectoInvestigacion");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.FechaInicio)
                    .IsRequired();

                entity.Property(e => e.FechaFin)
                    .IsRequired();

                entity.Property(e => e.RolEnProyecto)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Documento)
                    .HasColumnType("varbinary(max)");

                entity.HasOne(e => e.Docente)
                    .WithMany()
                    .HasForeignKey(e => e.DocenteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de tabla EvaluacionDocente
            modelBuilder.Entity<EvaluacionDocente>(entity =>
            {
                entity.ToTable("EvaluacionDocente");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Periodo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Puntaje)
                    .IsRequired();

                entity.HasOne(e => e.Docente)
                    .WithMany()
                    .HasForeignKey(e => e.DocenteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de tabla CursoCapacitacion
            modelBuilder.Entity<CursoCapacitacion>(entity =>
            {
                entity.ToTable("CursoCapacitacion");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Horas)
                    .IsRequired();

                entity.Property(e => e.FechaInicio)
                    .IsRequired();

                entity.Property(e => e.FechaFin)
                    .IsRequired();

                entity.Property(e => e.Certificado)
                    .HasColumnType("varbinary(max)");

                entity.HasOne(e => e.Docente)
                    .WithMany()
                    .HasForeignKey(e => e.DocenteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de tabla RequisitoPromocion
            modelBuilder.Entity<RequisitoPromocion>(entity =>
            {
                entity.ToTable("RequisitoPromocion");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PorcentajeAsignado)
                    .IsRequired();
            });


            // Configuración de tabla CumplimientoRequisito
            modelBuilder.Entity<CumplimientoRequisito>(entity =>
            {
                entity.ToTable("CumplimientoRequisito");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Cumplido)
                    .IsRequired();

                entity.Property(e => e.FechaCumplimiento)
                    .IsRequired();

                entity.HasOne(e => e.Docente)
                    .WithMany()
                    .HasForeignKey(e => e.DocenteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RequisitoPromocion)
                    .WithMany()
                    .HasForeignKey(e => e.RequisitoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });




            base.OnModelCreating(modelBuilder);
        }
    }
}
