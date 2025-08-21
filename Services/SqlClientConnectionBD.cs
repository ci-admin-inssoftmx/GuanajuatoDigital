using GuanajuatoAdminUsuarios.DBConect;
using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GuanajuatoAdminUsuarios.Services
{
    
        public class SqlClientConnectionBD : ISqlClientConnectionBD
        {
            private readonly IConfiguration _configuration;
            public string CadenaConexion { get; set; } = null;
            DbConect dbconect;
            public SqlClientConnectionBD(IConfiguration configuration)
            {
                _configuration = configuration;
                dbconect = new DbConect();
            }

            public string GetConnection()
            {
            CadenaConexion = dbconect.DbConection; //_configuration.GetConnectionString("DefaultConnection");
                return CadenaConexion;
            }

        public string GetConnection2()
        {
            CadenaConexion = dbconect.DbConection2; //_configuration.GetConnectionString("DefaultConnectionServices");
            return CadenaConexion;
        }


    }

    }
