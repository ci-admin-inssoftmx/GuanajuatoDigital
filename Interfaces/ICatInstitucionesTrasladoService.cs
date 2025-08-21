using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatInstitucionesTrasladoService
    {
        List<CatInstitucionesTrasladoModel> ObtenerInstitucionesActivas(int corp);
        List<CatInstitucionesTrasladoModel> ObtenerInstituciones(int corp);

    }
}
