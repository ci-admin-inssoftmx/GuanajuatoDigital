using GuanajuatoAdminUsuarios.Entity;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ISubmarcasVehiculos
    {
        List<SubmarcasVehiculo> GetSubmarcas();

        SubmarcasVehiculo GetSubmarcaById(int IdSubmarca);

        int SaveSubmarca(SubmarcasVehiculo submarca);

        int UpdateSubmarca(SubmarcasVehiculo submarca);

        int DeleteSubmarca(int submarca);


    }
}
