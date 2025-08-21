using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatTipoBloc
    {
        public int TipoBlocId { get; set; }

        public string Abreviatura { get; set; } = null!;

        public string Tipo { get; set; } = null!;

        public virtual ICollection<RegistraBloc> RegistraBlocs { get; set; } = new List<RegistraBloc>();
    }
}
