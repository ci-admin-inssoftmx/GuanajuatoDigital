using GuanajuatoAdminUsuarios.Interfaces.Files;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services.Files
{
	public class FileManageFileSystem : IFileManager
    {
		public IEnumerable<string> GetAllFilesFromDirectory(string path)
		{
			if (!Directory.Exists(path))
				return Array.Empty<string>();

			string[] files = Directory.GetFiles(path);
			return files;
		}

		public void Upload(string path, string filename, Stream fileContent)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var filePath = Path.Combine(path, filename);

            // sobreescribe el archivo si ya existe
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fileContent.CopyTo(fileStream);
        }

		public void Delete(string path, string filename)
		{
			try
			{
				var filePath = Path.Combine(path, filename);

				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}
			catch (DirectoryNotFoundException)
			{
				// no se hace nada, ya que el archivo no existe
				// o ya fue eliminado previamente por otro proceso
				return;
			}				
		}

        public void UploadUniqueFile(string directoryPath, string filename, Stream fileContent)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, filename);
            var backupPath = Path.Combine(directoryPath, $"backup_{filename}");

            try
            {
                if (File.Exists(filePath))
                {
                    File.Copy(filePath, backupPath, true);
                }

                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    if (Path.GetFileName(file) != filename && Path.GetFileName(file) != $"backup_{filename}")
                    {
                        File.Delete(file);
                    }
                }

                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                fileContent.CopyTo(fileStream);

                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(backupPath))
                {
                    File.Copy(backupPath, filePath, true);
                    File.Delete(backupPath);
                }

                throw new Exception("Error al guardar el archivo", ex);
            }
        }

    }

}
