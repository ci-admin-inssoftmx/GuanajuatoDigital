using GuanajuatoAdminUsuarios.DBConect;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.EntityFrameworkCore;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class GuanajuatoIncidenciasMigContext : DbContext
    {

        DbConect dbconect;
        public GuanajuatoIncidenciasMigContext()
        {

            dbconect = new DbConect();
        }

        public GuanajuatoIncidenciasMigContext(DbContextOptions<GuanajuatoIncidenciasMigContext> options) : base(options) { }

        public DbSet<LicenciaPersonaDatos> personaDatos { get; set; }

  //      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  //=> optionsBuilder.UseSqlServer(dbconect.DbConection3);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LicenciaPersonaDatos>().HasNoKey();
        }
    }
}
