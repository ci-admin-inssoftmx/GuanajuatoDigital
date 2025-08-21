using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GuanajuatoAdminUsuarios.WebClientServices
{   
    public class RequestXMLDynamic<T> : IRequestXMLDynamic<T> where T : class
    {
        public string GetXMLRequest(T value)
        {
            if (value == null) return string.Empty;

            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    xmlSerializer.Serialize(xmlWriter, value);
                    return stringWriter.ToString();
                }
            }
        }

        //public string GetXMLResponse(T value)
        //{
        //    if (value == null) return string.Empty;

        //    var xmlSerializer = new XmlSerializer(typeof(T));

        //    using (var stringWriter = new StringWriter())
        //    {
        //        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        //        {
        //            xmlSerializer.Deserialize("s",);
        //            return stringWriter.ToString();
        //        }
        //    }
        //}
    }
}
