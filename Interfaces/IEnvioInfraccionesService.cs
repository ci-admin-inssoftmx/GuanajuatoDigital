using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IEnvioInfraccionesService
    {
        List<EnvioInfraccionesModel> ObtenerInfracciones(EnvioInfraccionesModel model);

        int GuardarEnvioInfracciones(ModalEnvioModel model);
    }
}
