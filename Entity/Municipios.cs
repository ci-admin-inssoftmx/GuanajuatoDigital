using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class Municipios
    {
        public int idMunicipio { get; set; }

        public string Municipio { get; set; }

        public virtual ICollection<DiasInhabiles> DiasInhabiles { get; } = new List<DiasInhabiles>();

    }
}

