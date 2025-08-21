using GuanajuatoAdminUsuarios.Models.Files;
using System.Collections.Generic;
using System.IO;

namespace GuanajuatoAdminUsuarios.Interfaces.Files
{
    public interface IAccidenteFileManager
    {
		IEnumerable<FileData> GetAllLugarFiles(string path);

		string UploadLugarFileInDefaultUrl(int accidenteId, string filename, Stream fileContent);
        
        void UploadFile(string urlPath, string filename, Stream fileContent);

        void DeleteFile(string urlPath, string filename);
    }
}
