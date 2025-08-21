using GuanajuatoAdminUsuarios.DBConect;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.EntityFrameworkCore;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class GuanajuatoLicenciasContext : DbContext
    {

        DbConect dbconect;
        public GuanajuatoLicenciasContext()
        {

            dbconect = new DbConect();
        }

        public GuanajuatoLicenciasContext(DbContextOptions<GuanajuatoLicenciasContext> options) : base(options) { }
         
        public DbSet<LicenciaPersonaDatos> personaDatos { get; set; }

  //      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  //=> optionsBuilder.UseSqlServer(dbconect.DbConection3);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LicenciaPersonaDatos>().HasNoKey();
        }
    }
}
