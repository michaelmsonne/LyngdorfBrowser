using System;
using System.IO;

namespace LyngdorfBrowser.Class
{
    internal class Files
    {
        public static string LogFilePath
        {
            get
            {
                // Root folder for log files
                var logfilePathvar = LocalAppDataFilePath + @"\Lyngdorf Browser\Log";
                return logfilePathvar;
            }
        }

        public static string ProgramDataFilePath
        {
            get
            {
                // Root path for program data
                var currentDirectory = Directory.GetCurrentDirectory();
                var programDataFilePathvar = currentDirectory;
                return programDataFilePathvar;
            }
        }

        public static string LocalAppDataFilePath
        {
            get
            {
                // Root path for local appdata
                var LocalAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var LocalAppDataFilePathvar = LocalAppDataDirectory;
                return LocalAppDataFilePathvar;
            }
        }
    }
}
