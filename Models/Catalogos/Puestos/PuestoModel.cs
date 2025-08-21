using GuanajuatoAdminUsuarios.Entity;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Puestos
{
    public class PuestoModel
    {
        [Required(ErrorMessage = PuestoErrors.RequiredMessage)]        
        public int Id { get; set; }
        
        [Required(ErrorMessage = PuestoErrors.RequiredMessage)]
        public string Puesto { get; set; }
        
        public string Descripcion { get; set; }        
        
        public string Delegacion { get; set; }

        [Required(ErrorMessage = PuestoErrors.RequiredMessage)]
        public int? IdDelegacion { get; set; }

        public CatPuesto ToEntity(int corporacion)
        {
            return new CatPuesto
            {
                IdPuesto = Id,
                NombrePuesto = Puesto,
                Descripcion = Descripcion,
                IdDelegacion = IdDelegacion,
                Transito = corporacion
            };
        }

        public static PuestoModel FromEntity(CatPuesto entity)
        {
            return new PuestoModel
            {
                Id = entity.IdPuesto,
                Puesto = entity.NombrePuesto,
                Descripcion = entity.Descripcion,
                IdDelegacion = entity.IdDelegacion,
                Delegacion = entity.Delegacion?.Delegacion
            };
        }
    }
}
