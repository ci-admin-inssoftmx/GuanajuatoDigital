using System;
using System.IO;

namespace GuanajuatoAdminUsuarios.Utils
{
	
	public static class FileToBase64Parser
	{
		public static string Parse(string path)
		{
			if (string.IsNullOrEmpty(path))			
				throw new ArgumentException("ruta del archivo inválida", nameof(path));
			

			if (!File.Exists(path))			
				throw new FileNotFoundException("archivo no encontrado", path);
			

			byte[] fileBytes = File.ReadAllBytes(path);
			return Convert.ToBase64String(fileBytes);
		}

		public static bool TryParse(string path, out string base64)
		{
			base64 = null;

			try
			{
				base64 = Parse(path);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}

}
