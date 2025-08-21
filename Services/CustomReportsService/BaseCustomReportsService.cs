using GuanajuatoAdminUsuarios.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GuanajuatoAdminUsuarios.Services.CustomReportsService
{
    public abstract class BaseCustomReportsService 
    {
        public string FileName { get; set; }
        public string TitleReport { get; set; }
        public ReportModel Reporte { get; set; }
        public BaseCustomReportsService() { }
        public Font _titleFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.BLACK);
        public Font _standardFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);
        public Font _labelFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLACK);
        public BaseCustomReportsService(string _FileName, string _TitleReport)
        {
            FileName = _FileName + ".pdf";
            TitleReport = _TitleReport;
            Reporte = new ReportModel()
            {
                File = null,
                FileName = this.FileName
            };
        }

        public abstract ReportModel CreatePdf(EntityModel ModelData);
        public abstract ReportModel CreatePdf(EntityModel ModelData, string _FileName, string _TitleReport);
        public PdfPCell FieldCellBox(string Label, string Value) => FieldCellBox(Label, Value, Rectangle.NO_BORDER);        
        public PdfPCell FieldCellBox(string Label, string Value, int Border)
        {
            Paragraph p1 = new Paragraph();
            p1.Add(new Phrase(Label, _labelFont));
            p1.Add(new Phrase(Value, _standardFont));
            PdfPCell Cell = new PdfPCell(p1);
            Cell.Border = Border;
            return Cell;
        }
        public PdfPCell FieldCellBox(string Label, int Value, int Border) => FieldCellBox(Label, Value.ToString(), Border);
        public PdfPCell FieldCellBox(string Label, int? Value, int Border) => FieldCellBox(Label, Value.ToString(), Border);
        public PdfPCell FieldCellBox(string Label, decimal Value, int Border) => FieldCellBox(Label, Value.ToString(), Border);
        public PdfPCell FieldCellBox(string Label, int Value) => FieldCellBox(Label, Value.ToString());
        public PdfPCell FieldCellBox(string Label, int? Value) 
        {
            if (Value is null)
                return FieldCellBox(Label, "");
            else
                return FieldCellBox(Label, Value.Value.ToString());
        }
        public PdfPCell FieldCellBox(string Label, decimal Value) => FieldCellBox(Label, Value.ToString());
        public PdfPCell FieldCellTitleBox(string Title)
        {
            PdfPCell nextPostCell1 = new PdfPCell(new Phrase(Title, _titleFont));
            nextPostCell1.PaddingLeft = 10f;
            nextPostCell1.Border = Rectangle.NO_BORDER;
            return nextPostCell1;
        }
        public PdfPCell FieldCellEmptyBox()
        {
            PdfPCell nextPostCell1 = new PdfPCell(new Phrase(" ", _standardFont));
            nextPostCell1.Border = Rectangle.NO_BORDER;
            return nextPostCell1;
        }
        public PdfPTable GenericTable<T>(PdfPTable tableLayout, List<T> ModelData, Dictionary<string, string> ColumnsNames, int size) where T : class
        {
            if (ColumnsNames.Count == size)
            {

				//float[] headers = new float[(size)];
				//var porcentHeader = (float)100 / size;
				//for (int i = 0; i < size; i++)
				//{
				//    headers[i] = porcentHeader;
				//}

				tableLayout.SetWidths(new float[] { 70f, 240f, 200f, 90f });
				//tableLayout.SetWidths(headers);
                tableLayout.WidthPercentage = 100;
                tableLayout.HeaderRows = 1;
                tableLayout.SpacingBefore = 2f;
                var count = 1;

                foreach (var column in ColumnsNames)
                {
                    AddCellToTitleHeader(tableLayout, column.Value);
                }

                Type type = typeof(T);

                foreach (dynamic objectItem in ModelData)
                {
                    if (count >= 1)
                    {
                        foreach (var item in ColumnsNames)
                        {
                            bool isNumeric = false;
                            PropertyInfo property = type.GetProperty(item.Key);
                            if (item.Key.ToLower() == "calificacion")
                                isNumeric = true;

							var value = property.GetValue(objectItem);
                            AddCellToBody(isNumeric, tableLayout, count, Convert.ToString(value));
                        }
                        count++;
                    }
                }
                return tableLayout;
            }
            return null;
        }
        private static void AddCellToTitleHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5,
            });
        }

        private static void AddCellToBody(bool isNumeric, PdfPTable tableLayout, int count, string cellText = null )
        {
            var _value = "";
			if (isNumeric)
				_value = Convert.ToDecimal(cellText).ToString("###,###,###.00");
            else 
                _value = cellText;

			if (count % 2 == 0)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase(_value, new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK)))
                {
                    HorizontalAlignment = !isNumeric ?  Element.ALIGN_CENTER : Element.ALIGN_RIGHT,
                    Padding = 5,

                });
            }
            else
            {
                tableLayout.AddCell(new PdfPCell(new Phrase(_value, new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK)))
                {
					HorizontalAlignment = !isNumeric ? Element.ALIGN_CENTER : Element.ALIGN_RIGHT,
					Padding = 5,
                });
            }
        }
    }

    public class RoundRectangle : IPdfPCellEvent
    {
        public void CellLayout(
          PdfPCell cell, Rectangle rect, PdfContentByte[] canvas
        )
        {
            PdfContentByte cb = canvas[PdfPTable.LINECANVAS];
            cb.RoundRectangle(
              rect.Left,
              rect.Bottom,
              rect.Width,
              rect.Height,
              4 // change to adjust how "round" corner is displayed
            );
            cb.SetLineWidth(1f);
            cb.SetCMYKColorStrokeF(0f, 0f, 0f, 1f);
            cb.Stroke();
        }
    }
}