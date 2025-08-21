using GuanajuatoAdminUsuarios.Models;
using iTextSharp.text;
using System.Collections.Generic;
using System.IO;

namespace GuanajuatoAdminUsuarios.Utils
{
	public interface IPdfGenerator
    {
        (MemoryStream, string) CreatePdfReporteAsignacion(string NamePdf, string Title, Dictionary<string, string> ColumnsNames, List<ReporteAsignacionModel> ModelData, float[] columnWidth);
        (MemoryStream, string) CreatePdf<T>(string NamePdf, string Title, Dictionary<string, string> ColumnsNames, List<T> ModelData,float[] columnWidth);
        (MemoryStream, string) CreatePdf<T>(string NamePdf, string Title, Dictionary<string, string> ColumnsNames, T ModelData,float[] columnWidth);
        byte[] CreatePDFByHTML(string html, string cssText, Rectangle pageSize);
		//Byte[] PdfSharpConvert(String html);
	}
}
