using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Aseguradoras
{
    public class AseguradoraIndexViewModel
    {
        public AseguradoraFilterModel Filter { get; set; }
        public List<AseguradoraDetailsModel> Lista { get; set; } = new List<AseguradoraDetailsModel>();

    }
}
