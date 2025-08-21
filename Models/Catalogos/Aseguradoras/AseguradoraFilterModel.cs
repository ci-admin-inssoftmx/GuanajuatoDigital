using System;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Aseguradoras
{
    public class AseguradoraFilterModel
    {
        public string NombreAseguradora { get; set; }

        public int? Estatus { get; set; }

        public bool IsEmpty()
        {
            return 
                string.IsNullOrWhiteSpace(NombreAseguradora) &&
                Estatus is null;
        }

    }
}
