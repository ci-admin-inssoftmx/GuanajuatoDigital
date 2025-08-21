using System.Xml.Serialization;

namespace GuanajuatoAdminUsuarios.SOAPModels
{
    [XmlRoot("reversaDePago")]
    public class RecibosPagoWSRequestModel
    {
        [XmlElement(ElementName = "UsuarioLog")]
        public string UsuarioLog { get; set; }

        [XmlElement(ElementName = "PasswordLog")]
        public string PasswordLog { get; set; }

        [XmlElement(ElementName = "ReciboControlInterno")]
        public string ReciboControlInterno { get; set; }

        [XmlElement(ElementName = "FechaReversa")]
        public string FechaReversa { get; set; }
    }
}
