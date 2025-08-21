using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.RESTModels
{
    public class CotejarDatosResponseModel
    {
        public class EsMensaje
        {
            public string TpMens { get; set; }
            public string V1Mens { get; set; }
        }

        public class EsPerMoral
        {
            public string name_org1 { get; set; }
        }
        public class EsPerFisica
        {
            public string sexo;

            public string Nro_curp { get; set; }
            public string Nombre { get; set; }
            public string Ape_paterno { get; set; }
            public string Ape_materno { get; set; }
            public DateTime? Fecha_nacimiento { get; set; } 





        }

        public class MTCotejarDatosRes
        {
            public int Nro_interlocutor { get; set; }
            public string Nro_rfc { get; set; }
            public int tp_interlocutor { get; set; }
            public EsPerMoral es_per_moral { get; set; }
            public EsPerFisica es_per_fisica { get; set; }
            
            public List<TbDireccion> tb_direccion { get; set; }
            public List<TbVehiculo> tb_vehiculo { get; set; }
            public EsMensaje Es_mensaje { get; set; }
        }

        public class RootCotejarDatosRes
        {
            public MTCotejarDatosRes MT_CotejarDatos_res { get; set; }
        }

        public class TbDireccion
        {
            public string tipo_direccion { get; set; }
            public string calle { get; set; }
            public string nro_exterior { get; set; }
            public string entre_calle { get; set; }
            public string otra_calle { get; set; }
            public string colonia { get; set; }
            public int codigo_postal { get; set; }
            public string localidad { get; set; }
            public string municipio { get; set; }
            public string estado { get; set; }
            public string telefono { get; set; }
            public string correo { get; set; }
            public string entidadreg { get; set; }
        }

        public class TbVehiculo
        {
            public string no_placa { get; set; }
            public string no_tarjeta { get; set; }
            public string tipo_vehiculo { get; set; }
            public string servicio { get; set; }
            public int uso { get; set; }
            public string categoria { get; set; }
            public string marca { get; set; }
            public string modelo { get; set; }
            public string linea { get; set; }
            public string color { get; set; }
            public string no_motor { get; set; }
            public string no_cilindros { get; set; }
            public string no_serie { get; set; }
            public string carga { get; set; }
            public string combustible { get; set; }
            public int? numpersona { get; set; }
            public string otros { get; set; }
            public string pesobruto { get; set; }
        }

    }
}
