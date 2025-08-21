using iTextSharp.text;
using iTextSharp.tool.xml.pipeline.html;
using System;
using System.IO;

namespace GuanajuatoAdminUsuarios.Utils
{
	public class Base64ImageProvider : AbstractImageProvider
	{
		public override string GetImageRootPath()
		{
			return "";
		}

		public override Image Retrieve(string src)
		{
			if (src.StartsWith("data:image"))
			{
				try
				{
					var base64Data = src.Substring(src.IndexOf("base64,") + 7);
					byte[] imageBytes = Convert.FromBase64String(base64Data);
					using (var ms = new MemoryStream(imageBytes))
					{
						return Image.GetInstance(ms);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error al procesar imagen base64: {ex.Message}");
					return null;
				}
			}
			else
			{
				return base.Retrieve(src);
			}
		}
	}
}
