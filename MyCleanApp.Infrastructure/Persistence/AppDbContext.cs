using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;

namespace MyCleanApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //Usuario como el nombre de la tabla
        public DbSet<Usuario> Usuario { get; set; }
    }
}
