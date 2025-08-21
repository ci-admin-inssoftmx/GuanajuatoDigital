using GuanajuatoAdminUsuarios.Interfaces;
using iTextSharp.text;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static iTextSharp.text.pdf.AcroFields;

namespace GuanajuatoAdminUsuarios.WebClientServices
{
    public class RequestDynamic<T, D> : IRequestDynamic<T, D> where T : class where D : class
    {
        private readonly IServiceAppSettingsService _serviceAppSettingsService;
        private readonly IRequestXMLDynamic<T> _RequestXMLDynamic;
        public RequestDynamic(IServiceAppSettingsService serviceAppSettingsService, IRequestXMLDynamic<T> requestXMLDynamic)
        {
            _serviceAppSettingsService = serviceAppSettingsService;
            _RequestXMLDynamic = requestXMLDynamic;
        }

        public async Task<D> EncryptionService(T model, D modelResponse, string urlName, string requestXMLName)
        {
            try
            {
                var UrlSetting = _serviceAppSettingsService.GetSettingbyName(urlName);
                var XMLSetting = _serviceAppSettingsService.GetSettingbyName(requestXMLName);
                string XMLRequestModel = _RequestXMLDynamic.GetXMLRequest(model);
                XDocument myxml = XDocument.Load(XMLSetting.SettingValue);
                string XMLRequest = myxml.ToString();
                var nodeName = ReflectionXMLNode(model);
                var values = ReflectionXMLNode(XMLRequestModel, nodeName);
                XMLRequest = string.Format(XMLRequest, values);
                var getEncryptionResponse = await PostSOAPRequestAsync(UrlSetting.SettingValue, XMLRequest);

                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(getEncryptionResponse);
                string encrypt = xmlDoc.InnerXml;
                DeserializeXMLFileToObject(encrypt,  modelResponse);

                return modelResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private D DeserializeXMLFileToObject(string XmlFile, D modelResponse)
        {
            D returnObject = default(D);
         
            //modelResponse = default(D);
            //if (string.IsNullOrEmpty(XmlFilename)) return default(D);
            var nodeName = ReflectionXMLNode(modelResponse);
            var values = ReflectionXMLNode2(XmlFile, nodeName);

            try
            {
                XDocument doc = XDocument.Parse(XmlFile);
                var keys = doc.Descendants(nodeName);

                var soapResponse = doc.Descendants().Where(x => x.Name.LocalName == nodeName);
                var result = string.Concat(soapResponse.Nodes());
                Type type = typeof(D);
                foreach (var val in soapResponse)
                {
                    var elements = val.Elements();
                    foreach (var element in elements)
                    {
                        var nameElement = element.Name;
                        var namerElement = nameElement.LocalName;
                        var valElement = element.Value;
                        PropertyInfo property = type.GetProperty(namerElement);
                        property.SetValue(modelResponse, Convert.ChangeType(valElement, property.PropertyType), null);

                    }
                }
                returnObject = modelResponse;
            }
            catch (Exception ex)
            {

            }
            return returnObject;
        }

        public static string ReflectionXMLNode(D target)
        {
            XmlRootAttribute attribute = target.GetType().GetCustomAttribute<XmlRootAttribute>();
            return attribute == null ? null : attribute.ElementName;
        }

        public static string ReflectionXMLNode(T target)
        {
            XmlRootAttribute attribute = target.GetType().GetCustomAttribute<XmlRootAttribute>();
            return attribute == null ? null : attribute.ElementName;
        }

        private string ReflectionXMLNode2(string XMLDoc, string NodeName)
        {

            XDocument xml = XDocument.Parse(XMLDoc);
            var soapResponse = xml.Descendants().Where(x => x.Name.LocalName == NodeName);
            var result = string.Concat(soapResponse.Nodes());
            return result;
        }


        private string ReflectionXMLNode(string XMLDoc, string NodeName)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(XMLDoc);
            XmlNodeList xnList = xml.SelectNodes(NodeName);
            var xmlNodes = xnList[0].InnerXml;
            return xmlNodes;
        }

        private async Task<string> PostSOAPRequestAsync(string requestUri, string text)
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                using (HttpContent content = new StringContent(text, System.Text.Encoding.UTF8, "text/xml"))
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri))
                {
                    request.Headers.Add("SOAPAction", "");
                    request.Content = content;
                    using (HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }

    }
}
