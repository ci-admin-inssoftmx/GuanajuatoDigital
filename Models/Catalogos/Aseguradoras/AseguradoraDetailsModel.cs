using GuanajuatoAdminUsuarios.Entity;
using System;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Aseguradoras
{
    public class AseguradoraDetailsModel
    {
        public long IdAseguradora { get; set; }
        public string NombreAseguradora { get; set; }

        public int Estatus { get; set; }

        public static AseguradoraDetailsModel FromEntity(CatAseguradoras entity)
        {
            return new AseguradoraDetailsModel
            {
                IdAseguradora = entity.IdAseguradora,
                NombreAseguradora = entity.NombreAseguradora,
                Estatus = entity.Estatus,
            };
        }
    }
}
