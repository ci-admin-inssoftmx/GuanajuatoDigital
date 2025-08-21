using System.Collections.Generic;
using System.IO;

namespace GuanajuatoAdminUsuarios.Interfaces.Files
{
    public interface IFileManager
    {
		IEnumerable<string> GetAllFilesFromDirectory(string path);
		
        void Upload(string path, string filename, Stream fileContent);        

        void Delete(string path, string filename);

        void UploadUniqueFile(string path, string filename, Stream fileContent);
    }
}
