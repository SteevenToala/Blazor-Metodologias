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

                entity.HasOne(e => e.Rol)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(e => e.RolId);

                entity.HasOne(e => e.Persona)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(e => e.PersonaId);
            });

            // Configuración de tabla Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Rol");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.RolNombre)
                    .HasColumnName("rol") // Mapea al nombre real de la columna
                    .IsRequired()
                    .HasMaxLength(20);
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
