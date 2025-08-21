using GuanajuatoAdminUsuarios.Interfaces.Files;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Files;
using GuanajuatoAdminUsuarios.Models.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace GuanajuatoAdminUsuarios.Services.Files
{
    public class AccidenteFileManager : IAccidenteFileManager
    {
        private readonly IFileManager _fileManager;
        private readonly AccidenteSettings _settings;
        private readonly string _baseDirectory;

        public AccidenteFileManager(IFileManager fileManager, IOptions<AccidenteSettings> accidenteSettings, IOptions<AppSettings> appSettings)
        {
            _fileManager = fileManager;
            _settings = accidenteSettings.Value;
            _baseDirectory = appSettings.Value.RutaArchivosAccidente;
        }

		public IEnumerable<FileData> GetAllLugarFiles(string path)
		{
			IEnumerable<string> filesPath = _fileManager.GetAllFilesFromDirectory(path);

			var result = new List<FileData>();
			foreach (var file in filesPath)
			{
				; result.Add(new FileData { FilePath = file, FileName = Path.GetFileName(file), Content = File.ReadAllBytes(file) });
			}

			return result;
		}

		public string UploadLugarFileInDefaultUrl(int accidenteId, string filename, Stream fileContent)
        {
            var evidenciasPath = _settings.RutaArchivosEvidenciasLugar
                .Replace("{{accidenteId}}", $"{accidenteId}");

            var path = Path.Combine(_baseDirectory, evidenciasPath);

            UploadFile(path, filename, fileContent);

            return path;
        }

        public void UploadFile(string urlPath, string filename, Stream fileContent) => _fileManager.Upload(urlPath, filename, fileContent);

        public void DeleteFile(string urlPath, string filename) => _fileManager.Delete(urlPath, filename);
		
	}
}
