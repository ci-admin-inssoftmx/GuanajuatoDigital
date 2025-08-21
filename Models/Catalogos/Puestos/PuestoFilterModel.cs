using System;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Puestos
{
    public class PuestoFilterModel
    {
        public string Nombre { get; set; }

        public int? IdDelegacion { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Nombre) && IdDelegacion is null;
        }

    }
}
