using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICortesiasNoAplicadas
    {
        List<CortesiasNoAplicadasModel> ObtInfraccionesCortesiasNoAplicadas(string FolioInfraccion,int corporacion);

        CortesiasNoAplicadasModel ObtenerDetalleCortesiasNoAplicada(string Id);


		CortesiasNoAplicadasModel GuardarObservacion(string FolioInfraccion, string observaciones);

		

	}
}
