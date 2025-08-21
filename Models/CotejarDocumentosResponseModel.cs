using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CotejarDocumentosResponseModel
    {
        public long Nro_interlocutor { get; set; }
        public string Nro_rfc { get; set; }
        public int tp_interlocutor { get; set; }
        public EsPerMoral es_per_moral { get; set; }
        public List<TbDireccion> tb_direccion { get; set; }
        // Otros campos que faltan

        public class EsPerMoral
        {
            public string name_org1 { get; set; }
        }

        public class TbDireccion
        {
            public string tipo_direccion { get; set; }
            public string calle { get; set; }
            public int nro_exterior { get; set; }
            public string entre_calle { get; set; }
            public string otra_calle { get; set; }
            public string colonia { get; set; }
            public int codigo_postal { get; set; }
            public string localidad { get; set; }
            public string municipio { get; set; }
            public string estado { get; set; }
            public long telefono { get; set; }
            public string correo { get; set; }
            public string entidadreg { get; set; }
        }
    }

}
