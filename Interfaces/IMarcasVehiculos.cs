using GuanajuatoAdminUsuarios.Entity;
using System.Collections.Generic;


namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IMarcasVehiculos
    {
        List<MarcasVehiculo> GetMarcas();

        MarcasVehiculo GetMarcayById(int IdMarcaVehiculo);

        int SaveMarca(MarcasVehiculo marca);

        int UpdateMarca(MarcasVehiculo marca);

        int DeleteMarca(int marca);

    }
}