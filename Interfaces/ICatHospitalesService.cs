using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatHospitalesService
    {
        List<CatHospitalesModel> GetHospitales(int corp);

    }
}
