using GuanajuatoAdminUsuarios.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using iText = iTextSharp.text;
//using Microsoft.Extensions.Hosting;

namespace GuanajuatoAdminUsuarios.Utils
{
	public class PdfGenerator : IPdfGenerator
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PdfGenerator(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

        }

        public (MemoryStream, string) CreatePdfReporteAsignacion(string NamePdf, string Title, Dictionary<string, string> ColumnsNames, List<ReporteAsignacionModel> ModelData, float[] columnWidth)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                StringBuilder status = new StringBuilder("");
                DateTime dTime = DateTime.Now;

                string strPDFFileName = string.Format(NamePdf + "-" + dTime.ToString("ddMMyyyy") + ".pdf");
                float left = 10;
                float right = 10;
                float top = 20;
                float bottom = 0;
                iText.Document doc = new Document(PageSize.LETTER.Rotate(), left,right,top,bottom);

                PdfWriter.GetInstance(doc, workStream).CloseStream = false;
                doc.Open();

                #region Header
                PdfPTable tableHeader = new PdfPTable(2);
                tableHeader.DefaultCell.Border = iText.Rectangle.NO_BORDER;
                float[] headers = new float[] { 30, 60 };
                tableHeader.SetWidths(headers);
                tableHeader.WidthPercentage = 100;


                var path = Path.Combine(_hostingEnvironment.WebRootPath, "img", "logo-gto.png");
                iText.Image image = iText.Image.GetInstance(path);
                image.ScaleToFit(150f, 150f);
                image.ScaleToFitHeight = false;
                tableHeader.AddCell(new PdfPCell(image)).Border = 0;

                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iText.Font fontInvoice = new iText.Font(bf, 20, iText.Font.BOLD);
                tableHeader.AddCell(new PdfPCell(new iText.Phrase(Title, fontInvoice))).Border = 0;

                doc.Add(tableHeader);

                #endregion

                doc.Add(new Paragraph("\n"));
                PdfPTable tableLayout = new PdfPTable(ColumnsNames.Count);
                doc.Add(Add_Content_To_PDFReporteAsignacion(tableLayout, ModelData, ColumnsNames, columnWidth));

                doc.Close();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;
                return (workStream, strPDFFileName);
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        public (MemoryStream, string) CreatePdf<T>(string NamePdf, string Title, Dictionary<string, string> ColumnsNames, List<T> ModelData, float[] columnWidth)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                StringBuilder status = new StringBuilder("");
                DateTime dTime = DateTime.Now;

                string strPDFFileName = string.Format(NamePdf + "-" + dTime.ToString("ddMMyyyy") + ".pdf");

                iText.Document doc = new Document(PageSize.LEGAL);
                doc.SetMargins(10, 10, 20, 10);

                PdfWriter.GetInstance(doc, workStream).CloseStream = false;
                doc.Open();

                #region Header
                PdfPTable tableHeader = new PdfPTable(2);
                tableHeader.DefaultCell.Border = iText.Rectangle.NO_BORDER;
                float[] headers = new float[] { 30, 60 };
                tableHeader.SetWidths(headers);
                tableHeader.WidthPercentage = 100;


                var path = Path.Combine(_hostingEnvironment.WebRootPath, "img", "logo-gto.png");
                iText.Image image = iText.Image.GetInstance(path);
                image.ScaleToFit(150f, 150f);
                image.ScaleToFitHeight = false;
                tableHeader.AddCell(new PdfPCell(image)).Border = 0;

                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iText.Font fontInvoice = new iText.Font(bf, 20, iText.Font.NORMAL);
                tableHeader.AddCell(new PdfPCell(new iText.Phrase(Title, fontInvoice))).Border = 0;

                doc.Add(tableHeader);

                #endregion

                doc.Add(new Paragraph("\n"));

                //iText.Paragraph p3 = new iText.Paragraph();
                //p3.SpacingAfter = 6;
                //doc.Add(p3);

                PdfPTable tableLayout = new PdfPTable(ColumnsNames.Count);
                doc.Add(Add_Content_To_PDF(tableLayout, ModelData, ColumnsNames, columnWidth));

                doc.Close();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;
                return (workStream, strPDFFileName);
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        public (MemoryStream, string) CreatePdf<T>(string NamePdf, string Title, Dictionary<string, string> ColumnsNames, T ModelData, float[] columnWidth)
        {
            try
            {
                MemoryStream workStream = new MemoryStream();
                StringBuilder status = new StringBuilder("");
                DateTime dTime = DateTime.Now;

                string strPDFFileName = string.Format(NamePdf + "-" + dTime.ToString("ddMMyyyy") + ".pdf");

                iText.Document doc = new iText.Document();
                doc.SetMargins(10, 10, 20, 10);

                PdfWriter.GetInstance(doc, workStream).CloseStream = false;
                doc.Open();

                #region Header
                PdfPTable tableHeader = new PdfPTable(2);
                tableHeader.DefaultCell.Border = iText.Rectangle.NO_BORDER;
                float[] headers = new float[] { 30, 60 };
                tableHeader.SetWidths(headers);
                tableHeader.WidthPercentage = 100;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "img", "logo-gto.png");
                iText.Image image = iText.Image.GetInstance(path);
                image.ScaleToFit(150f, 150f);
                image.ScaleToFitHeight = false;
                tableHeader.AddCell(new PdfPCell(image)).Border = 0;

                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iText.Font fontInvoice = new iText.Font(bf, 20, iText.Font.NORMAL);
                tableHeader.AddCell(new PdfPCell(new iText.Phrase(Title, fontInvoice))).Border = 0;

                doc.Add(tableHeader);

                #endregion

                doc.Add(new Paragraph("\n"));

                //iText.Paragraph p3 = new iText.Paragraph();
                //p3.SpacingAfter = 6;
                //doc.Add(p3);

                PdfPTable tableLayout = new PdfPTable(ColumnsNames.Count);
                doc.Add(Add_Content_To_PDF(tableLayout, ModelData, ColumnsNames, columnWidth));

                doc.Close();
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;
                return (workStream, strPDFFileName);
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        private PdfPTable Add_Content_To_PDF<T>(PdfPTable tableLayout, T ModelData, Dictionary<string, string> ColumnsNames, float[] columnWidth)
        {
            /* if (ColumnsNames.Count == size)
             {*/

            float[] headers = columnWidth.Length == 0 ? new float[ColumnsNames.Count] : columnWidth;

            if (headers.Length != ColumnsNames.Count)
                throw new Exception("El número de columnas no coincide con el arreglo de ancho de columnas");

            //Se calcula el tamaño de cada columna en caso de no proporcionar ancho por cada columna
            if (columnWidth.Length == 0)
            {
                var porcentHeader = (float)100 / ColumnsNames.Count;
                for (int i = 0; i < ColumnsNames.Count; i++)
                {
                    headers[i] = porcentHeader;
                }
            }

            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;
            var count = 1;

            //Agrega nombre de columnas
            foreach (var column in ColumnsNames)
            {
                AddCellToTitleHeader(tableLayout, column.Value);
            }

            Type type = typeof(T);


            if (count >= 1)
            {
                foreach (var item in ColumnsNames)
                {
                    PropertyInfo property = type.GetProperty(item.Key);
                    var value = property.GetValue(ModelData);
                    AddCellToBody(tableLayout, count, Convert.ToString(value));
                }
                count++;
            }
            return tableLayout;
            /* }
             return null;*/
        }

        private PdfPTable Add_Content_To_PDF<T>(PdfPTable tableLayout, List<T> ModelData, Dictionary<string, string> ColumnsNames, float[] columnWidth)
        {
            /* if (ColumnsNames.Count == size)
             {*/

            float[] headers = columnWidth.Length == 0 ? new float[ColumnsNames.Count] : columnWidth;

            //Se calcula el tamaño de cada columna en caso de no proporcionar ancho por cada columna
            if (columnWidth.Length == 0)
            {
                var porcentHeader = (float)100 / ColumnsNames.Count - 1;
                for (int i = 0; i < ColumnsNames.Count; i++)
                {
                    headers[i] = porcentHeader;
                }
            }

            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;
            var count = 1;

            //Agrega nombre de columnas
            foreach (var column in ColumnsNames)
            {
                AddCellToTitleHeader(tableLayout, column.Value);
            }

            Type type = typeof(T);

            #region Comment
            //Busqueda de 
            //List<string> NameHeaders = new List<string>();
            //PropertyInfo[] propertyInfos = type.GetProperties();
            //foreach (PropertyInfo item in propertyInfos)
            //{
            //    var name = item.Name;
            //    var findPropertyBool = ColumnsNames.ContainsKey(name);
            //    //var findPropertyBool = Array.Find(ColumnsNames, n => n.Equals(name)) != null ? true : false;
            //    if (findPropertyBool)
            //    {
            //        NameHeaders.Add(name);
            //    }
            //}
            //si implemento la busqueda de propiedades tendria que ordenar la lista como viene la de ColumnsNames
            //NameHeaders.OrderBy(x => x.Equals(ColumnsNames.Keys));
            #endregion

            foreach (dynamic objectItem in ModelData)
            {
                if (count >= 1)
                {
                    foreach (var item in ColumnsNames)
                    {
                        PropertyInfo pi = type.GetProperty(item.Key);
                        var value = pi.GetValue(objectItem);
                        AddCellToBody(tableLayout, count, Convert.ToString(
                            pi.PropertyType == typeof(DateTime?) ? string.Empty :
                            pi.PropertyType == typeof(DateTime) ? (((DateTime)value) == DateTime.MinValue ? string.Empty : ((DateTime)value).ToString("dd-MM-yyyy")) :
                            value ?? string.Empty));
                    }
                    count++;
                }
            }
            return tableLayout;
            /*}
            return null;*/
        }


        private PdfPTable Add_Content_To_PDFReporteAsignacion(PdfPTable tableLayout, List<ReporteAsignacionModel> ModelData, Dictionary<string, string> ColumnsNames, float[] columnWidth)
        {
            float[] headers = columnWidth.Length == 0 ? new float[ColumnsNames.Count] : columnWidth;

            //Se calcula el tamaño de cada columna en caso de no proporcionar ancho por cada columna
            if (columnWidth.Length == 0)
            {
                var porcentHeader = (float)100 / ColumnsNames.Count - 1;
                for (int i = 0; i < ColumnsNames.Count; i++)
                {
                    if (i == 0)
                        headers[i] = 4;
                    else if (i == 1)
                        headers[i] = 14;
                    else if (i == 2)
                        headers[i] = 22;
                    else if (i == 3)
                        headers[i] = 20;
                    else
                        headers[i] = porcentHeader;
                }
            }

            tableLayout.SetWidths(headers);
            tableLayout.WidthPercentage = 100;
            tableLayout.HeaderRows = 1;
            var count = 1;

            //Agrega nombre de columnas
            foreach (var column in ColumnsNames)
            {
                AddCellToTitleHeader(tableLayout, column.Value);
            }

            //Type type = typeof(T);
            int row = 1;
            foreach (var item in ModelData)
            {
               
                //if (count >= 1)
                //{
                    for (int x=0; x<=ColumnsNames.Count-1; x++)
                    {
                        if (x == 0)
                        {
                            AddCellToBodyReporteAsignacion(tableLayout,
                                                             x,
                                                            "\n" + row.ToString() + "\n");
                        row++;
                    } else if (x== 1)
                        {
                            AddCellToBodyReporteAsignacion(tableLayout,
                                                           x,
                                                           "Folio: " + item.folio + "\n" + 
                                                           "Fecha: " + item.fechaSolicitud.ToString("dd/MM/yyyy") + "\n" + 
                                                           "Evento: " + item.evento);
                           
                        }
                        else if (x == 2)
                        {
                            var km = (item.vehiculoKm.Trim() == "") ? "" : Convert.ToDecimal(item.vehiculoKm).ToString("G29");
                            AddCellToBodyReporteAsignacion(tableLayout,
                                                           x,
                                                           "Carretera: " + item.carretera.Substring(0, item.carretera.Length > 30 ? 30 : item.carretera.Length) + "\n" +
                                                           "Tramo: " + item.tramo.Substring(0, item.tramo.Length>30 ? 30 : item.tramo.Length) + "\n" +
                                                           "Km: " + km);

                        }
                        else if (x == 3)
                        {
                            AddCellToBodyReporteAsignacion(tableLayout,
                                                           x,
                                                           "Nombre: " + item.fullName.Substring(0, item.fullName.Length>30 ? 30 : item.fullName.Length) + "\n" +
                                                           "Dirección:" + item.fullcolonia + "\n" +
                                                           "Motivo: " + item.motivoAsignacion);

                        }
                        else if (x == 4)
                        {
                            AddCellToBodyReporteAsignacion(tableLayout,
                                                           x,
                                                           "Delegación: " + item.Delegacion + "\n" +
                                                           "Oficial: " + item.oficial + "\n" +
                                                           "Tipo vehículo: " + item.tipoVehiculo);

                        }
                        else if (x == 5)
                        {
                            AddCellToBodyReporteAsignacion(tableLayout,
                                                           x,
                                                           "Grúa: " + item.propietarioGrua + "\n" +
                                                           "Pensión: " + item.vehiculoPension + "\n" +
                                                           "Inventario: " + item.numeroIventario);

                        }
                   
                    //count++;
                }
            }
            return tableLayout;
        }


        private static void AddCellToTitleHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new iText.Phrase(cellText, new iText.Font(iText.Font.FontFamily.HELVETICA, 8, 1, iText.BaseColor.BLACK)))
            {
                HorizontalAlignment = iText.Element.ALIGN_CENTER,
                Padding = 8,
                BackgroundColor = new iText.BaseColor(185, 200, 230)
            });
        }

        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new iText.Phrase(cellText, new iText.Font(iText.Font.FontFamily.HELVETICA, 8, 1, iText.BaseColor.BLACK)))
            {
                HorizontalAlignment = iText.Element.ALIGN_LEFT,
                Padding = 8,
                BackgroundColor = new iText.BaseColor(255, 255, 255)
            });
        }

        private static void AddCellToBody(PdfPTable tableLayout, int count, string cellText = null)
        {
            if (count % 2 == 0)
            {
                tableLayout.AddCell(new PdfPCell(new iText.Phrase(cellText, new iText.Font(iText.Font.FontFamily.HELVETICA, 8, 1, iText.BaseColor.BLACK)))
                {
                    HorizontalAlignment = iText.Element.ALIGN_LEFT,
                    Padding = 8,
                    BackgroundColor = new iText.BaseColor(211, 211, 211)

                });
            }
            else
            {
                tableLayout.AddCell(new PdfPCell(new iText.Phrase(cellText, new iText.Font(iText.Font.FontFamily.HELVETICA, 8, 1, iText.BaseColor.BLACK)))
                {
                    HorizontalAlignment = iText.Element.ALIGN_LEFT,
                    Padding = 8,
                    BackgroundColor = new iText.BaseColor(255, 255, 255)
                });
            }
        }

        private static void AddCellToBodyReporteAsignacion(PdfPTable tableLayout, int count, string cellText = null)
        {
            tableLayout.AddCell(new PdfPCell(new iText.Phrase(cellText, new iText.Font(iText.Font.NORMAL, 7, 1, iText.BaseColor.BLACK)))
            {
                HorizontalAlignment = (count == 0) ? iText.Element.ALIGN_CENTER : iText.Element.ALIGN_LEFT,
                VerticalAlignment = (count == 0) ? iText.Element.ALIGN_CENTER : iText.Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iText.BaseColor(255, 255, 255),
            });
        }

		public static Byte[] PdfSharpConvert(String html)
		{
			Byte[] res = null;
			using (MemoryStream ms = new MemoryStream())
			{
				var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
				pdf.Save(ms);
				res = ms.ToArray();
			}
			return res;
		}

		public byte[] CreatePDFByHTML(string html, string cssText, Rectangle pageSize)
        {
			using MemoryStream ms = new MemoryStream();
			GeneratePdfStream(html, cssText, () => new Document(pageSize, 20, 20, 20, 20), ms);
			return ms.ToArray();
        }

		
		private void GeneratePdfStream(string html, string css, Func<Document> funcNewDocument, Stream outputStream)
		{
			using (Document document = funcNewDocument())
			{
				PdfWriter writer = PdfWriter.GetInstance(document, outputStream);
				document.Open();

				// Configurar CSS
				var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
				cssResolver.AddCss(css, true);

				// Configurar el HTML context con nuestro ImageProvider personalizado
				var htmlContext = new HtmlPipelineContext(null);
				htmlContext.SetImageProvider(new Base64ImageProvider());
				htmlContext.SetTagFactory(iTextSharp.tool.xml.html.Tags.GetHtmlTagProcessorFactory());

				// Crear las pipelines
				var cssPipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
				var xmlWorker = new XMLWorker(cssPipeline, true);
				var xmlParser = new XMLParser(xmlWorker);

				// Parsear el HTML
				using (var htmlReader = new StringReader(html))
				{
					xmlParser.Parse(htmlReader);
				}

				document.Close();
			}
		}
	}
}
