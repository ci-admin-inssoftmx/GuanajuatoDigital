using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;


namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IDependencias

    {
        List<DependenciasModel> GetDependencias(int corp);

        DependenciasModel GetDependenciaById(int IdDependencia);

        int SaveDependencia(DependenciasModel dependencia,int corp);

        int UpdateDependencia(DependenciasModel dependencia);

        int DeleteDependencia(int dependencia);




    }
}