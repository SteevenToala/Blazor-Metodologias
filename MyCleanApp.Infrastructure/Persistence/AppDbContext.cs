using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;

namespace MyCleanApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Persona> Persona { get; set; }
        public DbSet<Administrador> Administrador { get; set; }
        public DbSet<NivelAcademico> NivelAcademico { get; set; }
        public DbSet<Docente> Docente { get; set; }
        public DbSet<EvaluacionDocente> EvaluacionDocente { get; set; }
        public DbSet<CursoCapacitacion> CursoCapacitacion { get; set; }
        public DbSet<ProyectoInvestigacion> ProyectoInvestigacion { get; set; }
        public DbSet<PublicacionAcademica> PublicacionAcademica { get; set; }
        public DbSet<RequisitoPromocion> RequisitoPromocion { get; set; }
        public DbSet<CumplimientoRequisito> CumplimientoRequisito { get; set; }
        public DbSet<ReporteAvance> ReporteAvance { get; set; }
        public DbSet<SolicitudAvanceRango> SolicitudAvanceRango { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Correo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Activo).IsRequired();
                entity.HasOne(e => e.Rol).WithMany(r => r.Usuarios).HasForeignKey(e => e.RolId);
                entity.HasOne(e => e.Persona).WithMany(p => p.Usuarios).HasForeignKey(e => e.PersonaId);
            });

            // Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Rol");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RolNombre).HasColumnName("rol").IsRequired().HasMaxLength(20);
            });

            // Persona
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

            // Administrador
            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.ToTable("Administrador");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario).WithOne().HasForeignKey<Administrador>(e => e.UsuarioId);
            });

            // NivelAcademico
            modelBuilder.Entity<NivelAcademico>(entity =>
            {
                entity.ToTable("NivelAcademico");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(200);
            });

            // Docente
            modelBuilder.Entity<Docente>(entity =>
            {
                entity.ToTable("Docente");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario).WithOne().HasForeignKey<Docente>(e => e.UsuarioId);
                entity.HasOne(e => e.NivelAcademico).WithMany().HasForeignKey(e => e.NivelAcademicoId);
                entity.Property(e => e.FechaInicioNivel).IsRequired();
            });

            // EvaluacionDocente
            modelBuilder.Entity<EvaluacionDocente>(entity =>
            {
                entity.ToTable("EvaluacionDocente");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Periodo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Puntaje).IsRequired();
                entity.HasOne(e => e.Docente).WithOne().HasForeignKey<EvaluacionDocente>(e => e.DocenteId);
            });

            // CursoCapacitacion
            modelBuilder.Entity<CursoCapacitacion>(entity =>
            {
                entity.ToTable("CursoCapacitacion");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Horas).IsRequired();
                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFin).IsRequired();
                entity.HasOne(e => e.Docente).WithMany().HasForeignKey(e => e.DocenteId);
            });

            // ProyectoInvestigacion
            modelBuilder.Entity<ProyectoInvestigacion>(entity =>
            {
                entity.ToTable("ProyectoInvestigacion");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFin).IsRequired();
                entity.Property(e => e.RolEnProyecto).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Docente).WithMany().HasForeignKey(e => e.DocenteId);
            });

            // PublicacionAcademica
            modelBuilder.Entity<PublicacionAcademica>(entity =>
            {
                entity.ToTable("PublicacionAcademica");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Revista).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Volumen).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Anio).IsRequired();
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.Docente).WithMany().HasForeignKey(e => e.DocenteId);
            });

            // RequisitoPromocion
            modelBuilder.Entity<RequisitoPromocion>(entity =>
            {
                entity.ToTable("RequisitoPromocion");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PorcentajeAsignado).IsRequired();
            });

            // CumplimientoRequisito
            modelBuilder.Entity<CumplimientoRequisito>(entity =>
            {
                entity.ToTable("CumplimientoRequisito");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cumplido).IsRequired();
                entity.Property(e => e.FechaCumplimiento).IsRequired();
                entity.HasOne(e => e.Docente).WithMany().HasForeignKey(e => e.DocenteId);
                entity.HasOne(e => e.Requisito).WithMany().HasForeignKey(e => e.RequisitoId);
            });

            // ReporteAvance
            modelBuilder.Entity<ReporteAvance>(entity =>
            {
                entity.ToTable("ReporteAvance");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaGeneracion).IsRequired();
                entity.HasOne(e => e.Docente).WithMany().HasForeignKey(e => e.DocenteId);
            });

            // SolicitudAvanceRango
            modelBuilder.Entity<SolicitudAvanceRango>(entity =>
            {
                entity.ToTable("SolicitudAvanceRango");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaSolicitud).IsRequired();
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Observaciones).HasMaxLength(300);
                entity.HasOne(e => e.Docente).WithMany().HasForeignKey(e => e.DocenteId);
                entity.HasOne(e => e.Administrador).WithMany().HasForeignKey(e => e.AdministradorId);
                entity.HasOne(e => e.NuevoNivelAcademico).WithMany().HasForeignKey(e => e.NuevoNivelAcademicoId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
