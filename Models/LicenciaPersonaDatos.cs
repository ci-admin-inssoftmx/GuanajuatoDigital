using Newtonsoft.Json;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class LicenciaPersonaDatos
    {
        [JsonProperty("id_persona")]
        public decimal ID_PERSONA { get; set; }

        [JsonProperty("nombre_completo")]
        public string? NOMBRE_COMPLETO { get; set; }

        [JsonProperty("nombre")]
        public string? NOMBRE { get; set; }

        [JsonProperty("estado")]
        public string? ESTADO { get; set; }

        [JsonProperty("primer_apellido")]
        public string? PRIMER_APELLIDO { get; set; }

        [JsonProperty("segundo_apellido")]
        public string? SEGUNDO_APELLIDO { get; set; }

        [JsonProperty("fecha_nacimiento")]
        public DateTime? FECHA_NACIMIENTO { get; set; }

        [JsonProperty("curp")]
        public string? CURP { get; set; }

        [JsonProperty("rfc")]
        public string? RFC { get; set; }

        [JsonProperty("id_genero")]
        public int? ID_GENERO { get; set; }

        [JsonProperty("genero")]
        public string? GENERO { get; set; }

        [JsonProperty("num_licencia")]
        public string? NUM_LICENCIA { get; set; }

        [JsonProperty("id_licencia")]
        public decimal? ID_LICENCIA { get; set; }

        [JsonProperty("estado_nacimiento")]
        public string? ESTADO_NACIMIENTO { get; set; }

        [JsonProperty("nacionalidad")]
        public string? NACIONALIDAD { get; set; }

        [JsonProperty("id_tipo_licencia")]
        public int? ID_TIPO_LICENCIA { get; set; }

        [JsonProperty("tipolicencia")]
        public string? TIPOLICENCIA { get; set; }

        [JsonProperty("fecha_inicio_vigencia")]
        public DateTime? FECHA_INICIO_VIGENCIA { get; set; }

        [JsonProperty("fecha_termino_vigencia")]
        public DateTime? FECHA_TERMINO_VIGENCIA { get; set; }

        [JsonProperty("id_municipio")]
        public int? ID_MUNICIPIO { get; set; }

        [JsonProperty("municipio")]
        public string? MUNICIPIO { get; set; }

        [JsonProperty("cp")]
        public string? CP { get; set; }

        [JsonProperty("colonia")]
        public string? COLONIA { get; set; }

        [JsonProperty("calle")]
        public string? CALLE { get; set; }

        [JsonProperty("num_ext")]
        public string? NUM_EXT { get; set; }

        [JsonProperty("num_int")]
        public string? NUM_INT { get; set; }

        [JsonProperty("telefono1")]
        public string? TELEFONO1 { get; set; }

        [JsonProperty("email")]
        public string? EMAIL { get; set; }

        [JsonProperty("Vigencia")]
        public Decimal Vigencia { get; set; }



    }
}
