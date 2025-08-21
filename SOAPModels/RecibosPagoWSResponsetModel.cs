using System.Xml.Serialization;

namespace GuanajuatoAdminUsuarios.SOAPModels
{
    [XmlRoot("return")]
    public class RecibosPagoWSResponsetModel
    {
        //[XmlElement("codigoRespuesta")]
        public int codigoRespuesta { get; set; }

        //[XmlElement("mensaje")]
        public string mensaje { get; set; }
    }
}
