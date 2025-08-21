using System;
using GuanajuatoAdminUsuarios.JsonConverters;
using Newtonsoft.Json;

namespace GuanajuatoAdminUsuarios.RESTModels
{
	public class RepuveRoboModel
	{
		[JsonProperty(PropertyName = "estatus")]
		[JsonConverter(typeof(BooleanJsonConverter))]
		public bool EsRobado { get; set; }
		[JsonProperty(PropertyName = "fecha")]
		public string Fecha { get; set; }
		[JsonProperty(PropertyName = "placa")]
		public string Placa { get; set; }
		[JsonProperty(PropertyName = "niv")]
		public string Niv { get; set; }
		[JsonProperty(PropertyName = "fuente_robo")]
		public string FuenteRobo { get; set; }
		[JsonProperty(PropertyName = "averiguacion")]
		public string Averiguacion { get; set; }
		[JsonProperty(PropertyName = "agente_mp")]
		public string AgenteMinisterioPublico { get; set; }
		[JsonProperty(PropertyName = "fecha_averiguacion")]
		public string FechaAveriguacion { get; set; }
		[JsonProperty(PropertyName = "_3")]
		public string EstatusConsulta { get; set; }
		[JsonProperty(PropertyName = "ip")]
		public string Ip { get; set; }
		public override string ToString()
		{
			return "[PLACA: " + Placa + ",NIV:" + Niv + "FECHA:" + Fecha + ",FUENTE DE ROBO:" + FuenteRobo + ",AVERIGUACION:" + Averiguacion + ",FECHA DE AVERIGUACION:" + FechaAveriguacion + ",AGENTE DEL MINISTERIO PUBLICO:" + AgenteMinisterioPublico+"]";
		}

	}
}
