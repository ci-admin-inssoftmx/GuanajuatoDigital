using System.ComponentModel.DataAnnotations;
using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Aseguradoras
{
    public class AseguradoraModel : AseguradoraBaseModel
    {
        [Required(ErrorMessage = AseguradoraErrors.RequiredMessage)]
        public long IdAseguradora { get; set; }

        public override CatAseguradoras ToEntity()
        {
            return new CatAseguradoras
            {
                IdAseguradora = (int)IdAseguradora,
                NombreAseguradora = NombreAseguradora,
                Estatus = Estatus.GetValueOrDefault(),
            };
        }

        public static AseguradoraModel FromEntity(CatAseguradoras entity)
        {
            return new AseguradoraModel
            {
                IdAseguradora = entity.IdAseguradora,
                NombreAseguradora = entity.NombreAseguradora,
                Estatus = entity.Estatus,
            };
        }
    }
}
