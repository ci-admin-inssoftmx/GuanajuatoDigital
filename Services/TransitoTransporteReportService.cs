using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services.CustomReportsService;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.IO;

namespace GuanajuatoAdminUsuarios.Services
{
	public class TransitoTransporteReportService : BaseCustomReportsService
	{
		public TransitoTransporteReportService() { }
		public TransitoTransporteReportService(string _FileName, string _TitleReport) : base(_FileName, _TitleReport) { }

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
				TransitoTransporteModel ModelTransitoTransporte = (TransitoTransporteModel)ModelData;
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
						doc.AddTitle("SERVICIO DE GRÚA");
						doc.AddCreator("SITTEG");

						// Abrimos el archivo
						doc.Open();

						// Escribimos el encabezamiento en el documento
						Paragraph tituloParagraph = new Paragraph("SERVICIO DE GRÚA", _titleFont);
						tituloParagraph.Alignment = Element.ALIGN_CENTER;
						doc.Add(tituloParagraph);

						doc = WritteHeader(doc, ModelTransitoTransporte);
						doc = WritteSolicita(doc, ModelTransitoTransporte);
						doc = WritteUbicacion(doc, ModelTransitoTransporte);
						doc = WritteVehiculoYPropietario(doc, ModelTransitoTransporte);
						doc = WritteTitulos(doc);
						doc = WritteGruaServiciosYTiempoServicio(doc, ModelTransitoTransporte);

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

		public Document WritteHeader(Document doc, TransitoTransporteModel ModelTransitoTransporte)
		{
			PdfPTable Invoicetable = new PdfPTable(3);
			Invoicetable.HorizontalAlignment = 0;
			Invoicetable.WidthPercentage = 100;
			Invoicetable.SetWidths(new float[] { 200f, 20f, 200f });  // then set the column's __relative__ widths
			Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;

			{
				PdfPTable nested = new PdfPTable(1);
				nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell(FieldCellBox("Folio: ", ModelTransitoTransporte.Folio));
				nested.AddCell(FieldCellBox("Fecha: ", ModelTransitoTransporte.FechaSolicitud != DateTime.MinValue ? ModelTransitoTransporte.FechaSolicitud.ToString("dd/MM/yyyy HH:mm") : "-"));
				nested.AddCell(FieldCellBox("Descripción del evento: ", ModelTransitoTransporte.evento));
				nested.AddCell(FieldCellBox("Tipo de usuario: ", ModelTransitoTransporte.tipoUsuario));
				nested.AddCell(FieldCellBox("Tipo de vehículo: ", ModelTransitoTransporte.tipoVehiculo));
				nested.AddCell(FieldCellBox("Propietario de grúa: ", ModelTransitoTransporte.Concesionario));
				nested.AddCell(FieldCellBox("Servicio que requiere: ", ModelTransitoTransporte.tipoServicio));
				nested.AddCell(FieldCellBox("Nombre del oficial: ", ModelTransitoTransporte.fullOficial));
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
				string nombreSolicitante = $"{ModelTransitoTransporte.solicitanteNombre} {ModelTransitoTransporte.solicitanteAp} {ModelTransitoTransporte.solicitanteAm}";
				nested.AddCell(FieldCellBox("Nombre del solicitante: ", (nombreSolicitante != ""? nombreSolicitante : "- - -")));
				nested.AddCell(FieldCellBox("", ""));
                string domicilio = "";

                if (!string.IsNullOrEmpty(ModelTransitoTransporte.solicitanteCalle))
                {
                    domicilio += ModelTransitoTransporte.solicitanteCalle;
                }

                if (!string.IsNullOrEmpty(ModelTransitoTransporte.solicitanteNumero))
                {
                    if (!string.IsNullOrEmpty(domicilio))
                    {
                        domicilio += ", ";
                    }
                    domicilio += ModelTransitoTransporte.solicitanteNumero;
                }

                if (!string.IsNullOrEmpty(ModelTransitoTransporte.solicitanteColonia))
                {
                    if (!string.IsNullOrEmpty(domicilio))
                    {
                        domicilio += ", ";
                    }
                    domicilio += ModelTransitoTransporte.solicitanteColonia;
                }

                if (!string.IsNullOrEmpty(ModelTransitoTransporte.solicitanteMunicipio))
                {
                    if (!string.IsNullOrEmpty(domicilio))
                    {
                        domicilio += ", ";
                    }
                    domicilio += ModelTransitoTransporte.solicitanteMunicipio;

                    if (!string.IsNullOrEmpty(ModelTransitoTransporte.solicitanteEntidad))
                    {
                        domicilio += ", " + ModelTransitoTransporte.solicitanteEntidad;
                    }
                }

                nested.AddCell(FieldCellBox("Domicilio: ", domicilio));
				nested.AddCell("");
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

		public Document WritteSolicita(Document doc, TransitoTransporteModel ModelTransitoTransporte)
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
				nested.AddCell(FieldCellBox("Solicita: ", string.IsNullOrEmpty(ModelTransitoTransporte.solicitanteNombre) ? "Oficial" : "Usuario"));

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

		public Document WritteUbicacion(Document doc, TransitoTransporteModel ModelTransitoTransporte)
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
				nested.AddCell(FieldCellTitleBox("Ubicación"));				
				nested.AddCell(FieldCellBox("Calle: ", ModelTransitoTransporte.calle));
				nested.AddCell(FieldCellBox("Número: ", ModelTransitoTransporte.numero));
				nested.AddCell(FieldCellBox("Colonia: ", ModelTransitoTransporte.colonia));
				nested.AddCell(FieldCellBox("Carretera: ", ModelTransitoTransporte.carretera));
				nested.AddCell(FieldCellBox("Tramo: ", ModelTransitoTransporte.tramo));
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
				nested.AddCell(FieldCellBox("Municipio: ", ModelTransitoTransporte.municipio));
				nested.AddCell(FieldCellBox("Pensión: ", ModelTransitoTransporte.pension));
				nested.AddCell(FieldCellBox("Intersección: ", ModelTransitoTransporte.interseccion));
				if (decimal.TryParse(ModelTransitoTransporte.Km, out decimal kmValue))
				{
					string formattedKm = kmValue.ToString("G29");
					nested.AddCell(FieldCellBox("Kilómetro: ", formattedKm));
				}
				nested.AddCell(FieldCellEmptyBox());
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

		public Document WritteVehiculoYPropietario(Document doc, TransitoTransporteModel ModelTransitoTransporte)
		{
			RoundRectangle roundRectangle = new RoundRectangle();

			PdfPTable TableMain = new PdfPTable(2);
			TableMain.HorizontalAlignment = 0;
			TableMain.WidthPercentage = 100;
			TableMain.SetWidths(new float[] { 45f,45f });
			TableMain.DefaultCell.Border = Rectangle.NO_BORDER;
			TableMain.SpacingBefore = 2f;

			PdfPTable Invoicetable = new PdfPTable(1);
			Invoicetable.HorizontalAlignment = 0;
			Invoicetable.WidthPercentage = 45;
			Invoicetable.SetWidths(new float[] { 45f });  // then set the column's __relative__ widths
			Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
			{
				PdfPTable nested = new PdfPTable(1);
				nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell(FieldCellTitleBox("Vehículo"));
				nested.AddCell(FieldCellBox("Placas: ", ModelTransitoTransporte.Placa));
				nested.AddCell(FieldCellBox("Serie: ", ModelTransitoTransporte.Serie));
				nested.AddCell(FieldCellBox("Marca: ", ModelTransitoTransporte.marcaVehiculo));
				nested.AddCell(FieldCellBox("Submarca: ", ModelTransitoTransporte.nombreSubmarca));
				nested.AddCell(FieldCellBox("Modelo: ", ModelTransitoTransporte.modelo));
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

			PdfPTable Invoicetable2 = new PdfPTable(1);
			Invoicetable2.HorizontalAlignment = 0;
			Invoicetable2.WidthPercentage = 45;
			Invoicetable2.SetWidths(new float[] { 45f });  // then set the column's __relative__ widths
			Invoicetable2.DefaultCell.Border = Rectangle.NO_BORDER;
			{
				PdfPTable nested = new PdfPTable(1);
				nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell(FieldCellTitleBox("Propietario"));
				nested.AddCell(FieldCellBox("Nombre: ", ModelTransitoTransporte.fullPropietario));
				nested.AddCell(FieldCellBox("CURP/RFC: ", ModelTransitoTransporte.CURP));
				nested.AddCell(FieldCellEmptyBox());
				nested.AddCell(FieldCellEmptyBox());
				nested.AddCell(FieldCellEmptyBox());
				nested.AddCell("");
				PdfPCell nesthousing = new PdfPCell(nested);
				nesthousing.Border = Rectangle.NO_BORDER;
				nesthousing.Rowspan = 5;
				nesthousing.PaddingBottom = 5f;
				Invoicetable2.AddCell(nesthousing);
			}

			Invoicetable2.PaddingTop = 10f;

			PdfPCell nesthousingMain2 = new PdfPCell(Invoicetable2);
			nesthousingMain2.Border = Rectangle.NO_BORDER;
			nesthousingMain2.Rowspan = 5;
			nesthousingMain2.PaddingBottom = 2f;
			nesthousingMain2.CellEvent = roundRectangle;
			TableMain.AddCell(nesthousingMain2);

			doc.Add(TableMain);
			return doc;
		}

		public Document WritteTitulos(Document doc)
		{
			RoundRectangle roundRectangle = new RoundRectangle();

			PdfPTable TableMain = new PdfPTable(1);
			TableMain.HorizontalAlignment = 0;
			TableMain.WidthPercentage = 100;
			TableMain.DefaultCell.Border = Rectangle.NO_BORDER;
			TableMain.SpacingBefore = 2f;

			PdfPTable Invoicetable = new PdfPTable(3);
			Invoicetable.HorizontalAlignment = 0;
			Invoicetable.WidthPercentage = 100;
			Invoicetable.SetWidths(new float[] { 200f, 200f, 200f });  // then set the column's __relative__ widths
			Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
			{
				PdfPTable nested = new PdfPTable(1);
				nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell(FieldCellTitleBox("Grúa"));
				PdfPCell nesthousing = new PdfPCell(nested);
				nesthousing.Border = Rectangle.NO_BORDER;
				nesthousing.Rowspan = 5;
				nesthousing.PaddingBottom = 5f;
				Invoicetable.AddCell(nesthousing);
			}
			{
				PdfPTable nested = new PdfPTable(1);
				nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell(FieldCellTitleBox("Servicios"));
				PdfPCell nesthousing = new PdfPCell(nested);
				nesthousing.Border = Rectangle.NO_BORDER;
				nesthousing.Rowspan = 5;
				nesthousing.PaddingBottom = 5f;
				Invoicetable.AddCell(nesthousing);
			}
			{
				PdfPTable nested = new PdfPTable(1);
				nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell(FieldCellTitleBox("Tiempo de servicio"));
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
			TableMain.AddCell(nesthousingMain);

			doc.Add(TableMain);
			return doc;
		}

		public Document WritteGruaServiciosYTiempoServicio(Document doc, TransitoTransporteModel ModelTransitoTransporte)
		{
			RoundRectangle roundRectangle = new RoundRectangle();

			PdfPTable TableMain = new PdfPTable(1);
			TableMain.HorizontalAlignment = 0;
			TableMain.WidthPercentage = 100;
			TableMain.DefaultCell.Border = Rectangle.NO_BORDER;
			TableMain.SpacingBefore = 2f;

			PdfPTable Invoicetable = new PdfPTable(3);
			Invoicetable.HorizontalAlignment = 0;
			Invoicetable.WidthPercentage = 100;
			Invoicetable.SetWidths(new float[] { 200f, 200f, 200f });  // then set the column's __relative__ widths
			Invoicetable.DefaultCell.Border = Rectangle.NO_BORDER;
			{
				PdfPTable nested = new PdfPTable(1);
				nested.DefaultCell.Border = Rectangle.NO_BORDER;
				nested.AddCell(FieldCellBox("No. económico: ", ModelTransitoTransporte.numeroEconomico));
				nested.AddCell(FieldCellBox("Tipo de grúa: ", ModelTransitoTransporte.tipoGrua));
				nested.AddCell(FieldCellBox("Placas: ", ModelTransitoTransporte.placasGrua));
				nested.AddCell(FieldCellBox("Operador: ", ModelTransitoTransporte.operador));
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
				nested.AddCell(FieldCellBox("", ModelTransitoTransporte.arrastre == 1 ? "Arrastre" : ""));
                nested.AddCell(FieldCellBox("", ModelTransitoTransporte.salvamento == 1 ? "Salvamento" : ""));
                nested.AddCell(FieldCellBox("", ModelTransitoTransporte.abanderamiento == 1 ? "Abanderamiento" : ""));

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
				nested.AddCell(FieldCellBox("Arribo: ", ModelTransitoTransporte.FechaArribo != DateTime.MinValue ? ModelTransitoTransporte.FechaArribo.ToString("dd/MM/yyyy HH:mm") : "-"));
				nested.AddCell(FieldCellBox("Inicio: ", ModelTransitoTransporte.FechaInicio != DateTime.MinValue ? ModelTransitoTransporte.FechaInicio.ToString("dd/MM/yyyy HH:mm") : "-"));
				nested.AddCell(FieldCellBox("Término: ", ModelTransitoTransporte.FechaFinal != DateTime.MinValue ? ModelTransitoTransporte.FechaFinal.ToString("dd/MM/yyyy HH:mm") : "-"));
				nested.AddCell(FieldCellBox("Tiempo: ", ModelTransitoTransporte.minutosManiobra));
				nested.AddCell(FieldCellEmptyBox());
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

	}
}
