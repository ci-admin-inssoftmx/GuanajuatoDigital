using GuanajuatoAdminUsuarios.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PdfSharp.Charting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using Telerik.SvgIcons;
using iText = iTextSharp.text;

namespace GuanajuatoAdminUsuarios.Services.CustomReportsService
{
    public class InfraccionReportService : BaseCustomReportsService
    {
        public InfraccionReportService() { }
        public InfraccionReportService(string _FileName, string _TitleReport) : base(_FileName, _TitleReport) { }

        public override ReportModel CreatePdf(EntityModel ModelData, string _FileName, string _TitleReport)
        {
            FileName = _FileName + ".pdf";
            TitleReport = _TitleReport;
            Reporte = new ReportModel()
            {
                File = null,
                FileName = this.FileName
            };
            return CreatePdf(ModelData);
        }
        public override ReportModel CreatePdf(EntityModel ModelData)
        {
            try
            {
                InfraccionesReportModel ModelDataInfracciones = (InfraccionesReportModel)ModelData;
                DateTime dTime = DateTime.Now;
                string customPDFFileName = string.Format(FileName + "-" + dTime.ToString("ddMMyyyy") + ".pdf");

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Creamos el documento con el tamaño de página tradicional
                        Document doc = new Document(PageSize.LETTER);
                        // Indicamos donde vamos a guardar el documento


                        PdfWriter.GetInstance(doc, ms).CloseStream = false;

                        // Le colocamos el título y el autor
                        // **Nota: Esto no será visible en el documento
                        doc.AddTitle("Infracciones");
                        doc.AddCreator("SITTEG");

                        // Abrimos el archivo
                        doc.Open();

                        // Escribimos el encabezamiento en el documento
                        Paragraph tituloParagraph = new Paragraph("INFRACCIÓN", _titleFont);
                        tituloParagraph.Alignment = Element.ALIGN_CENTER;
                        doc.Add(tituloParagraph);

                        doc = WritteHeader(doc, ModelDataInfracciones);
                        doc = BodyLugar(doc, ModelDataInfracciones);
                        doc = BodyCoductor(doc, ModelDataInfracciones);
						doc = BodyVehiculo(doc, ModelDataInfracciones);
                        doc = BodyAplicacion(doc, ModelDataInfracciones);
                        doc = BodyMotivosInfraccion(doc, ModelDataInfracciones);
                        doc = BodyGarantía(doc, ModelDataInfracciones);
                        doc = BodyDatosPago(doc, ModelDataInfracciones);
                        doc = BodyConceptoInfraccion(doc, ModelDataInfracciones);

                        doc.Close();

                        byte[] byteInfo = ms.ToArray();
                        ms.Write(byteInfo, 0, byteInfo.Length);
                        ms.Position = 0;

                        Reporte.FileName = customPDFFileName;
                        Reporte.File = ms;
                    }
                }
                catch (Exception e)
                {

                }

                return Reporte;
            }
            catch (Exception ex)
            {
                return new ReportModel();
            }
        }

        public Document WritteHeader(Document doc, InfraccionesReportModel ModelDataInfracciones)
        {
            PdfPTable Invoicetable = new PdfPTable(3);
            Invoicetable.HorizontalAlignment = 0;
            Invoicetable.WidthPercentage = 100;
            Invoicetable.SetWidths(new float[] { 200f, 20f, 200f });  // then set the column's __relative__ widths
            Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellBox("Folio: ", ModelDataInfracciones.folioInfraccion));
                nested.AddCell(FieldCellBox("Fecha: ", ModelDataInfracciones.fechaInfraccion.ToString("dd-MM-yyyy")));
                nested.AddCell(FieldCellBox("Hora: ", ModelDataInfracciones.horaInfraccion ));
                nested.AddCell(FieldCellBox("Fecha de vencimiento: ", ModelDataInfracciones.fechaVencimiento.ToString("dd-MM-yyyy")));
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 10f;
                Invoicetable.AddCell(nesthousing);
            }

            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 10f;
                Invoicetable.AddCell(nesthousing);
            }


            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;                
                nested.AddCell(FieldCellEmptyBox());
                nested.AddCell(FieldCellBox("Estatus: ", ModelDataInfracciones.estatusInfraccion == null?"": ModelDataInfracciones.estatusInfraccion.ToUpper()));             
                nested.AddCell(FieldCellBox("Oficial: ", (ModelDataInfracciones.nombreOficial == null) ? "" : ModelDataInfracciones.nombreOficial.ToString()));
                nested.AddCell(FieldCellBox("Municipio: ", (ModelDataInfracciones.municipio == null) ? "" : ModelDataInfracciones.municipio));
                nested.AddCell(FieldCellBox("",""));
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 10f;
                Invoicetable.AddCell(nesthousing);
            }


            Invoicetable.PaddingTop = 3f;

            doc.Add(Invoicetable);
            return doc;
        }
        public Document BodyLugar(Document doc, InfraccionesReportModel ModelDataInfracciones)
        {
            RoundRectangle roundRectangle = new RoundRectangle();

            PdfPTable TableMain = new PdfPTable(1);
            TableMain.HorizontalAlignment = 0;
            TableMain.WidthPercentage = 100;
            TableMain.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPTable Invoicetable = new PdfPTable(2);
            Invoicetable.HorizontalAlignment = 0;
            Invoicetable.WidthPercentage = 100;
            Invoicetable.SetWidths(new float[] { 200f, 200f });  // then set the column's __relative__ widths
            Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellTitleBox("Lugar"));
                nested.AddCell(FieldCellBox("Carretera: ", (ModelDataInfracciones.carretera==null) ? "" : ModelDataInfracciones.carretera.ToString()));                
                nested.AddCell(FieldCellBox("Tramo: ", (ModelDataInfracciones.tramo == null) ? "" : ModelDataInfracciones.tramo.ToString()));
                nested.AddCell(FieldCellBox("Kilómetro: ", (ModelDataInfracciones.kmCarretera == null) ? "" : ModelDataInfracciones.kmCarretera));

                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellEmptyBox());
                nested.AddCell(FieldCellEmptyBox());
                nested.AddCell(FieldCellBox("Colonia: ", (ModelDataInfracciones.colonia == null) ? "" : ModelDataInfracciones.colonia.ToString()));
                nested.AddCell(FieldCellBox("Calle: ", (ModelDataInfracciones.calle == null) ? "" : ModelDataInfracciones.calle.ToString()));
                nested.AddCell(FieldCellBox("Número: ", (ModelDataInfracciones.numero == null) ? "" : ModelDataInfracciones.numero.ToString()));
                nested.AddCell(FieldCellBox("Entre Calle: ", (ModelDataInfracciones.entreCalle == null) ? "" : ModelDataInfracciones.entreCalle.ToString()));

                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }


            Invoicetable.PaddingTop = 10f;

            PdfPCell nesthousingMain = new PdfPCell(Invoicetable);
            nesthousingMain.Border = Rectangle.NO_BORDER;
            nesthousingMain.Rowspan = 5;
            nesthousingMain.PaddingBottom = 1f;
            nesthousingMain.CellEvent = roundRectangle;
            TableMain.AddCell(nesthousingMain);

            doc.Add(TableMain);
            return doc;
        }
        public Document BodyCoductor(Document doc, InfraccionesReportModel ModelDataInfracciones)
        {
            RoundRectangle roundRectangle = new RoundRectangle();

            PdfPTable TableMain = new PdfPTable(1);
            TableMain.HorizontalAlignment = 0;
            TableMain.WidthPercentage = 100;
            TableMain.DefaultCell.Border = Rectangle.NO_BORDER;
            TableMain.SpacingBefore = 2f;

            PdfPTable Invoicetable = new PdfPTable(2);
            Invoicetable.HorizontalAlignment = 0;
            Invoicetable.WidthPercentage = 100;
            Invoicetable.SetWidths(new float[] { 200f, 200f });  // then set the column's __relative__ widths
            Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellTitleBox("Conductor"));
                nested.AddCell(FieldCellBox("Nombre: ", (ModelDataInfracciones.nombreConductor == null) ? "" : ModelDataInfracciones.nombreConductor));
                if (ModelDataInfracciones.fechaNacimientoConductor == null || ModelDataInfracciones.fechaNacimientoConductor.Value == DateTime.MinValue)                
                    nested.AddCell(FieldCellBox("Fecha de nacimiento: ", ""));
                else
                    nested.AddCell(FieldCellBox("Fecha de nacimiento: ", ModelDataInfracciones.fechaNacimientoConductor?.ToString("dd-MM-yyyy")));

                nested.AddCell(FieldCellBox("Edad: ", (ModelDataInfracciones.edadConductor == null) ? "" : ModelDataInfracciones.edadConductor.ToString()));
                nested.AddCell(FieldCellBox("Sexo: ", (ModelDataInfracciones.generoConductor == null) ? "" : ModelDataInfracciones.generoConductor.ToString()));

                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 4;
                nesthousing.PaddingBottom = 4f;
                Invoicetable.AddCell(nesthousing);
            }
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellEmptyBox());
                nested.AddCell(FieldCellBox("Télefono: ", (ModelDataInfracciones.telefonoConductor == null) ? "" : ModelDataInfracciones.telefonoConductor));
                nested.AddCell(FieldCellBox("No. Licencia: ", (ModelDataInfracciones.numLicenciaConductor == null) ? "" : ModelDataInfracciones.numLicenciaConductor));
                nested.AddCell(FieldCellBox("Tipo de licencia: ", (ModelDataInfracciones.tipoLicenciaConductor == null) ? "" : ModelDataInfracciones.tipoLicenciaConductor));
                if (ModelDataInfracciones.vencimientoLicConductor == null || ModelDataInfracciones.vencimientoLicConductor.Value == DateTime.MinValue  || ModelDataInfracciones.vencimientoLicConductor.Value.Year == 1900)
                    nested.AddCell(FieldCellBox("Vencimiento: ", ""));
                else 
                    nested.AddCell(FieldCellBox("Vencimiento: ", ModelDataInfracciones.vencimientoLicConductor.Value.ToString("dd-MM-yyyy")));

				PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 4;
                nesthousing.PaddingBottom = 4f;
                Invoicetable.AddCell(nesthousing);
            }

			PdfPCell domicilio = new PdfPCell(FieldCellBox("Domicilio: ", (ModelDataInfracciones.domicilioConductor == null) ? "" : ModelDataInfracciones.domicilioConductor.ToString()));
			domicilio.Colspan = 2;
			Invoicetable.AddCell(domicilio);


			Invoicetable.PaddingTop = 10f;

            PdfPCell nesthousingMain = new PdfPCell(Invoicetable);
            nesthousingMain.Border = Rectangle.NO_BORDER;
            nesthousingMain.Rowspan = 5;
            nesthousingMain.PaddingBottom = 2f;
            nesthousingMain.CellEvent = roundRectangle;
            TableMain.AddCell(nesthousingMain);

            doc.Add(TableMain);
            return doc;
        }

		public Document BodyVehiculo(Document doc, InfraccionesReportModel ModelDataInfracciones)
        {
            RoundRectangle roundRectangle = new RoundRectangle();

            PdfPTable TableMain = new PdfPTable(1);
            TableMain.HorizontalAlignment = 0;
            TableMain.WidthPercentage = 100;
            TableMain.DefaultCell.Border = Rectangle.NO_BORDER;
            TableMain.SpacingBefore = 2f;

            PdfPTable Invoicetable = new PdfPTable(2);
            Invoicetable.HorizontalAlignment = 0;
            Invoicetable.WidthPercentage = 100;
            Invoicetable.SetWidths(new float[] { 200f, 200f });  // then set the column's __relative__ widths
            Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellTitleBox("Vehículo"));
                nested.AddCell(FieldCellBox("No. de placa: ", ModelDataInfracciones.placas));
                nested.AddCell(FieldCellBox("Tipo: ", ModelDataInfracciones.tipoVehiculo));
                nested.AddCell(FieldCellBox("Marca: ", ModelDataInfracciones.marcaVehiculo));
                nested.AddCell(FieldCellBox("Submarca: ", ModelDataInfracciones.nombreSubmarca));
                nested.AddCell(FieldCellBox("Modelo: ", ModelDataInfracciones.modelo));
                nested.AddCell(FieldCellBox("Color: ", ModelDataInfracciones.color));
                nested.AddCell(FieldCellTitleBox("Propietario"));
                nested.AddCell(FieldCellBox("Nombre: ", ModelDataInfracciones.nombrePropietario));
                //     nested.AddCell(FieldCellBox("Domicilio: ", ModelDataInfracciones.domicilioPropietario));
                          
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellEmptyBox());
                nested.AddCell(FieldCellBox("No. de serie: ", ModelDataInfracciones.serie));
                nested.AddCell(FieldCellBox("Tarjeta de circulación: ", ModelDataInfracciones.NumTarjetaCirculacion));
                nested.AddCell(FieldCellBox("Entidad de registro: ", ModelDataInfracciones.nombreEntidad));
                nested.AddCell(FieldCellBox("Tipo de servicio: ", ModelDataInfracciones.tipoServicio));
                nested.AddCell(FieldCellBox("No. Económico: ", ModelDataInfracciones.numeroEconomico));
                nested.AddCell("");
               


                


                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }
            PdfPCell domicilioProp = new PdfPCell(FieldCellBox("Domicilio: ", (ModelDataInfracciones.domicilioPropietario == null) ? "" : ModelDataInfracciones.domicilioPropietario.ToString()));
            domicilioProp.Colspan = 2;
            Invoicetable.AddCell(domicilioProp);

            Invoicetable.PaddingTop = 10f;

            PdfPCell nesthousingMain = new PdfPCell(Invoicetable);
            nesthousingMain.Border = Rectangle.NO_BORDER;
            nesthousingMain.Rowspan = 5;
            nesthousingMain.PaddingBottom = 2f;
            nesthousingMain.CellEvent = roundRectangle;
            TableMain.AddCell(nesthousingMain);

            doc.Add(TableMain);
            return doc;
        }
        public Document BodyAplicacion(Document doc, InfraccionesReportModel ModelDataInfracciones)
        {
            RoundRectangle roundRectangle = new RoundRectangle();

            PdfPTable TableMain = new PdfPTable(1);
            TableMain.HorizontalAlignment = 0;
            TableMain.WidthPercentage = 100;
            TableMain.DefaultCell.Border = Rectangle.NO_BORDER;
            TableMain.SpacingBefore = 2f;

            PdfPTable Invoicetable = new PdfPTable(2);
            Invoicetable.HorizontalAlignment = 0;
            Invoicetable.WidthPercentage = 100;
            Invoicetable.SetWidths(new float[] { 200f, 200f });  // then set the column's __relative__ widths
            Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellTitleBox("Aplicación"));
                nested.AddCell(FieldCellBox("Infracción aplicada a: ", ModelDataInfracciones.AplicadaA));//PENDIENTE!!!
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellBox("Cortesía: ", ModelDataInfracciones.cortesia ));
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }


            Invoicetable.PaddingTop = 10f;

            PdfPCell nesthousingMain = new PdfPCell(Invoicetable);
            nesthousingMain.Border = Rectangle.NO_BORDER;
            nesthousingMain.Rowspan = 5;
            nesthousingMain.PaddingBottom = 2f;
            nesthousingMain.CellEvent = roundRectangle;
            TableMain.AddCell(nesthousingMain);

            doc.Add(TableMain);
            return doc;
        }
        public Document BodyMotivosInfraccion(Document doc, InfraccionesReportModel ModelDataInfracciones)
        {
            if (ModelDataInfracciones.MotivosInfraccion != null)
            {
                if (ModelDataInfracciones.MotivosInfraccion.Any())
                {
                    Paragraph tituloMotivoInfParagraph = new Paragraph("MOTIVOS DE INFRACCIÓN", _titleFont);
                    tituloMotivoInfParagraph.Alignment = Element.ALIGN_CENTER;
                    doc.Add(tituloMotivoInfParagraph);
                    var uma = ModelDataInfracciones.Uma;
                    int i = 1;
                    foreach (var item in ModelDataInfracciones.MotivosInfraccion)
                    {
                        item.Fundamento = item.Fundamento?.ToUpper();
                        item.Motivo = item.Motivo?.ToUpper();
                        item.prioridad = i;
                        i++;
                    }

                    Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
                {
                    {"prioridad","PRIORIDAD"},
                    {"Motivo","MOTIVO"},
                    {"Fundamento","FUNDAMENTO LEGAL"},
                    {"calificacion","CALIFICACIÓN"},
                };
                    PdfPTable tableLayout = new PdfPTable(ColumnsNames.Count);

                    doc.Add(GenericTable(tableLayout, ModelDataInfracciones.MotivosInfraccion, ColumnsNames, ColumnsNames.Count));
                    doc = WritteTotalesMotivosInfraccion(doc, ModelDataInfracciones, uma);
                }
            }
            
            return doc;
        }


        public Document WritteTotalesMotivosInfraccion(Document doc, InfraccionesReportModel ModelDataInfracciones,decimal uma)
        {
            PdfPTable Invoicetable = new PdfPTable(3);
            Invoicetable.HorizontalAlignment = 1;
            Invoicetable.WidthPercentage = 100;
            Invoicetable.SetWidths(new float[] { 410f, 100f, 90f });  // then set the column's __relative__ widths
            Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;                
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 10f;
                Invoicetable.AddCell(nesthousing);
            }

            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell(new PdfPCell(new Phrase("TOTAL (UMAS):", new iText.Font(iText.Font.FontFamily.HELVETICA, 8, iText.Font.BOLD, BaseColor.BLACK)))
				{
					Border = Rectangle.RECTANGLE,
					HorizontalAlignment = Element.ALIGN_RIGHT,
					Padding = 5,

				});
				nested.AddCell(new PdfPCell(new Phrase("UMA:", new iText.Font(iText.Font.FontFamily.HELVETICA, 8, iText.Font.BOLD, BaseColor.BLACK)))
				{
					Border = Rectangle.RECTANGLE,
					HorizontalAlignment = Element.ALIGN_RIGHT,
					Padding = 5,

				});
				nested.AddCell(new PdfPCell(new Phrase("MONTO TOTAL:", new iText.Font(iText.Font.FontFamily.HELVETICA, 8, iText.Font.BOLD, BaseColor.BLACK)))
				{   
                    Border = Rectangle.RECTANGLE,
					HorizontalAlignment = Element.ALIGN_RIGHT,
					Padding = 5,

				});

				//nested.AddCell("");
				PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 10f;
				Invoicetable.AddCell(nesthousing);
            }
                        

            {
                int totalSalarios = 0;
                if (ModelDataInfracciones.MotivosInfraccion.Any())                
                    totalSalarios = ModelDataInfracciones.MotivosInfraccion.Sum(x => x.calificacion.Value);
                
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell(new PdfPCell(new Phrase(totalSalarios.ToString("###,###,###.00").ToString(), new iText.Font(iText.Font.FontFamily.HELVETICA, 8, iText.Font.BOLD, BaseColor.BLACK)))
				{
					Border = Rectangle.RECTANGLE,
				    HorizontalAlignment = Element.ALIGN_RIGHT,
					Padding = 5,

				});
				nested.AddCell(new PdfPCell(new Phrase("$ " + uma.ToString("###,###,###.00").ToString(), new iText.Font(iText.Font.FontFamily.HELVETICA, 8, iText.Font.BOLD, BaseColor.BLACK)))
				{
					Border = Rectangle.RECTANGLE,
					HorizontalAlignment = Element.ALIGN_RIGHT,
					Padding = 5,

				});
				nested.AddCell(new PdfPCell(new Phrase("$ " + (uma * totalSalarios).ToString("###,###,###.00").ToString(), new iText.Font(iText.Font.FontFamily.HELVETICA, 8, iText.Font.BOLD, BaseColor.BLACK)))
				{
					Border = Rectangle.RECTANGLE,
					HorizontalAlignment = Element.ALIGN_RIGHT,
					Padding = 5,

				});

				//nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 10f;
				nested.HorizontalAlignment = Element.ALIGN_RIGHT;
				Invoicetable.AddCell(nesthousing);
            }


            Invoicetable.PaddingTop = 0f;

            doc.Add(Invoicetable);
            return doc;
        }
        public Document BodyGarantía(Document doc, InfraccionesReportModel ModelDataInfracciones)
        {

            var tipoGar = ModelDataInfracciones.Garantia?.garantia;

            RoundRectangle roundRectangle = new RoundRectangle();

            PdfPTable TableMain = new PdfPTable(1);
            TableMain.HorizontalAlignment = 0;
            TableMain.WidthPercentage = 100;
            TableMain.DefaultCell.Border = Rectangle.NO_BORDER;
            TableMain.SpacingBefore = 2f;

            PdfPTable Invoicetable = new PdfPTable(2);
            Invoicetable.HorizontalAlignment = 0;
            Invoicetable.WidthPercentage = 100;
            Invoicetable.SetWidths(new float[] { 200f, 200f });  // then set the column's __relative__ widths
            Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellTitleBox("Garantía"));
                nested.AddCell(FieldCellBox("Tipo de garantía: ", ModelDataInfracciones.Garantia?.garantia?.ToUpper()));
                nested.AddCell(FieldCellBox("Tipo placa: ", tipoGar == "Placa"? ModelDataInfracciones.Garantia?.tipoPlaca?.ToUpper() : "-"));
                nested.AddCell(FieldCellBox("No. de placa: ", tipoGar== "Placa"? ModelDataInfracciones.placas?.ToUpper() : "-"));
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell("   ");
				nested.AddCell(FieldCellBox("Tipo licencia: ", tipoGar == "Licencia"? ModelDataInfracciones.Garantia?.tipoLicencia?.ToUpper() : "-"));
                nested.AddCell(FieldCellBox("No. de licencia: ", tipoGar == "Licencia"? ModelDataInfracciones.Garantia?.numLicencia?.ToUpper() : "-"));
                nested.AddCell(FieldCellBox("No. de tarjeta: ", tipoGar== "Tarjeta"? ModelDataInfracciones.NumTarjetaCirculacion?.ToUpper() : "-"));//VALIDAR
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }


            Invoicetable.PaddingTop = 10f;

            PdfPCell nesthousingMain = new PdfPCell(Invoicetable);
            nesthousingMain.Border = Rectangle.NO_BORDER;
            nesthousingMain.Rowspan = 5;
            nesthousingMain.PaddingBottom = 2f;
            nesthousingMain.CellEvent = roundRectangle;
            TableMain.AddCell(nesthousingMain);

            doc.Add(TableMain);
            return doc;
        }
        public Document BodyDatosPago(Document doc, InfraccionesReportModel ModelDataInfracciones)
        {
            RoundRectangle roundRectangle = new RoundRectangle();

            var total = ModelDataInfracciones.Uma;
            var mot = ModelDataInfracciones.MotivosInfraccion?.Sum(s => s.calificacion)??0;
            var result = total * mot;
            ModelDataInfracciones.montoCalificacion = result;

            PdfPTable TableMain = new PdfPTable(1);
            TableMain.HorizontalAlignment = 0;
            TableMain.WidthPercentage = 100;
            TableMain.DefaultCell.Border = Rectangle.NO_BORDER;
            TableMain.SpacingBefore = 2f;

            PdfPTable Invoicetable = new PdfPTable(2);
            Invoicetable.HorizontalAlignment = 0;
            Invoicetable.WidthPercentage = 100;
            Invoicetable.SetWidths(new float[] { 200f, 200f });  // then set the column's __relative__ widths
            Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellTitleBox("Datos pago"));
                nested.AddCell(FieldCellBox("Monto calificación: ","$ "+ ModelDataInfracciones.montoCalificacion.ToString("###,###,###.00")));
                nested.AddCell(FieldCellBox("Monto pagado: ", ModelDataInfracciones.montoPagado));
                nested.AddCell(FieldCellBox("Recibo: ", ModelDataInfracciones.reciboPago?.ToUpper()));
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellBox("Oficio de condonación: ", ModelDataInfracciones.oficioCondonacion?.ToUpper()));
                nested.AddCell(FieldCellEmptyBox());
                if (ModelDataInfracciones.fechaPago == null || ModelDataInfracciones.fechaPago.Value == DateTime.MinValue)
                    nested.AddCell(FieldCellBox("Fecha de pago: ", ""));
                else
                    nested.AddCell(FieldCellBox("Fecha de pago: ", ModelDataInfracciones.fechaPago.Value.ToString("dd-MM-yyyy")));

                nested.AddCell(FieldCellBox("Lugar de pago: ", ModelDataInfracciones.lugarPago?.ToUpper()));//VALIDAR
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }


            Invoicetable.PaddingTop = 10f;

            PdfPCell nesthousingMain = new PdfPCell(Invoicetable);
            nesthousingMain.Border = Rectangle.NO_BORDER;
            nesthousingMain.Rowspan = 5;
            nesthousingMain.PaddingBottom = 2f;
            nesthousingMain.CellEvent = roundRectangle;
            TableMain.AddCell(nesthousingMain);

            doc.Add(TableMain);
            return doc;
        }
        public Document BodyConceptoInfraccion(Document doc, InfraccionesReportModel ModelDataInfracciones)
        {
            PdfPTable TableMain = new PdfPTable(1);
            TableMain.HorizontalAlignment = 0;
            TableMain.WidthPercentage = 100;
            TableMain.DefaultCell.Border = Rectangle.BOX;
            TableMain.SpacingBefore = 2f;

            PdfPTable Invoicetable = new PdfPTable(1);
            Invoicetable.HorizontalAlignment = 0;
            Invoicetable.WidthPercentage = 100;
            Invoicetable.SetWidths(new float[] { 200f });  // then set the column's __relative__ widths
            Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
            {
                PdfPTable nested = new PdfPTable(1);
                nested.DefaultCell.Border = Rectangle.NO_BORDER;
                nested.AddCell(FieldCellBox("Concepto de infracción: ", ModelDataInfracciones.observaciones));
                nested.AddCell("");
                PdfPCell nesthousing = new PdfPCell(nested);
                nesthousing.Border = Rectangle.NO_BORDER;
                nesthousing.Rowspan = 5;
                nesthousing.PaddingBottom = 5f;
                Invoicetable.AddCell(nesthousing);
            }

            Invoicetable.PaddingTop = 10f;

            PdfPCell nesthousingMain = new PdfPCell(Invoicetable);
            nesthousingMain.Border = Rectangle.BOX;
            nesthousingMain.Rowspan = 5;
            nesthousingMain.PaddingBottom = 2f;
            TableMain.AddCell(nesthousingMain);

            doc.Add(TableMain);
            return doc;
        }
    }
}
