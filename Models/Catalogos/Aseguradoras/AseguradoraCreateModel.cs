using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Aseguradoras
{
    public class AseguradoraCreateModel : AseguradoraBaseModel
    {
        public override CatAseguradoras ToEntity()
        {
            return new CatAseguradoras
            {
                IdAseguradora = default,
                NombreAseguradora = NombreAseguradora,
                Estatus = Estatus.GetValueOrDefault(),
            };
        }
    }
}
