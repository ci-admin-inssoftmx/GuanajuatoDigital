using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace GuanajuatoAdminUsuarios.FileUtil
{
    public class FileUtil
    {
        public static object syncObj = new object();
        private static string _pathExec;
       
        public static void WriteInLogFile(string linetoWrite)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();


            string localLogPath = configuration.GetSection("AppSettings").GetSection("LocalLogPath").Value;
            string nameSettPathToSave = configuration.GetSection("AppSettings").GetSection("NameSettPathToSave").Value;
            lock (syncObj)
            {
                CheckLogSize();

                string fechaLog = "(" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ")"; //fecha en que se escribio la incidencia

                string logPath = localLogPath;

                if (!localLogPath.Contains(":") && Assembly.GetEntryAssembly() != null)
                    logPath = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),localLogPath); //ruta obtenida desde el app config

                string logFilePath = Path.Combine(logPath, nameSettPathToSave);

                if (!System.IO.Directory.Exists(logPath))
                    System.IO.Directory.CreateDirectory(logPath);

                CreateOrWriteFile(logFilePath, fechaLog + "-" + linetoWrite);
            }
        }
        public static void CheckLogSize()
        {
            var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();

            string localLogPath = configuration.GetSection("AppSettings").GetSection("LocalLogPath").Value;
            long logRotateSize = Convert.ToInt64(configuration.GetSection("AppSettings").GetSection("LogRotateSize").Value);
            string nameSettPathToSave = configuration.GetSection("AppSettings").GetSection("NameSettPathToSave").Value;


            string logPath = localLogPath;

            if (!localLogPath.Contains(":") && Assembly.GetEntryAssembly() != null)
                logPath = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), localLogPath); //ruta obtenida desde el app config

            string logFilePath = Path.Combine(logPath, nameSettPathToSave);

            long logSize;
            long configSize;

            System.IO.FileInfo logFile = null;
            try
            {
                logFile = new System.IO.FileInfo(logFilePath);
                logSize = logFile.Length;
                configSize = logRotateSize;
                if (logSize >= configSize)
                {
                    //renombramos el archivo
                    String newLogName = logFilePath.Replace(".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(' ', '_').Replace(':', '-'));
                    System.IO.File.Move(logFilePath, newLogName + ".txt");
                }

            }
            catch (System.IO.FileNotFoundException)
            {
                if (!System.IO.Directory.Exists(logPath))
                    System.IO.Directory.CreateDirectory(logPath);

                CreateOrWriteFile(logFilePath, "Creación del Log->" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            catch (System.IO.IOException e)
            {
                throw new Exception("Error al rotar el log:" + e.Message);
            }
        }

        public static string ReadFile(string file)
        {


            var response = string.Empty;
            if (System.IO.File.Exists(file))
            {
                string[] lines = System.IO.File.ReadAllLines(file);

                if (lines.Count() > 0)
                    response = lines[0];
            }

            //Logger.Log("File reader size {0}.", response.Length);
            return response;

        }

       
        private static void CreateOrWriteFile(string FileName, string text)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName, true))
            {
                file.WriteLine(text);
            }
        }

        public static bool SaveFile(string idFolderName, string fileName, byte[] fileData, bool overWrite, ref string error)
        {
            var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();

            string localLogPath = configuration.GetSection("AppSettings").GetSection("LocalLogPath").Value;
            string nameSettPathToSave = configuration.GetSection("AppSettings").GetSection("NameSettPathToSave").Value;

            string logPath = localLogPath;

            if (!localLogPath.Contains(":"))
                logPath = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), localLogPath); //ruta obtenida desde el app config

            string logFilePath = Path.Combine(logPath, nameSettPathToSave);


            string pathToSave = nameSettPathToSave.Trim();



            bool result = false;
            string filePath;
            //Validate Root directory from webconfig and add slash if is missing. 
            pathToSave = ValidateDirectory(pathToSave);
            //Validate directory with idFolderName
            pathToSave = pathToSave + idFolderName;
            pathToSave = ValidateDirectory(pathToSave);
            //concat. Path and Filename
            filePath = pathToSave + fileName.Trim();


            try
            {
                FileStream fs;
                BinaryWriter bw;
                if (overWrite)
                {
                    fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
                }
                bw = new BinaryWriter(fs);
                bw.Write(fileData);
                bw.Flush();
                bw.Close();
                fs.Close();
                bw = null;
                fs.Dispose();

                result = true;
            }
            catch (IOException ex)
            {
                error = ex.Message;
                result = false;
            }

            return result;
        }

        public static bool SaveFileGral(string directory, string fileName, byte[] fileData, bool overWrite, ref string error)
        {
            //get configuration from web.config


            bool result = false;
            string filePath;
            ValidateDirectory(directory);
            //concat. Path and Filename


            filePath = Path.Combine(directory, fileName);


            try
            {
                FileStream fs;
                BinaryWriter bw;
                if (overWrite)
                {
                    fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
                }
                bw = new BinaryWriter(fs);
                bw.Write(fileData);
                bw.Flush();
                bw.Close();
                fs.Close();
                bw = null;
                fs.Dispose();

                result = true;
            }
            catch (IOException ex)
            {
                error = ex.Message;
                result = false;
            }

            return result;
        }

        public static string ValidateDirectory(string pathToSave)
        {
            char lastchar = pathToSave[pathToSave.Length - 1];
            if (!lastchar.Equals("\\"))
            {
                pathToSave = pathToSave + "\\";
            }
            //Check if the directory exists
            if (!System.IO.Directory.Exists(pathToSave))
            {
                System.IO.Directory.CreateDirectory(pathToSave);
            }
            return pathToSave;
        }

    }
}