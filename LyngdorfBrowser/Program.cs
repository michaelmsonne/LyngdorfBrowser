using CefSharp.WinForms;
using CefSharp;
using System;
using System.IO;
using System.Windows.Forms;
using static LyngdorfBrowser.Class.FileLogger;

namespace LyngdorfBrowser
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Set Global Logfile properties for log
            DateFormat = "dd-MM-yyyy";
            DateTimeFormat = "dd-MM-yyyy HH:mm:ss";
            WriteOnlyErrorsToEventLog = false;
            WriteToEventLog = false;
            WriteToFile = true;

            Message("Initialized log config", EventType.Information, 1000);

            // Set the cache folder path to a location within the user's LocalApplicationData folder
            var cacheFolderPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Application.ProductName + "\\Cache");

            // Create the cache folder if it doesn't exist
            if (!Directory.Exists(cacheFolderPath))
            {
                // Try to create folder folder and perform error correction
                try
                {
                    Directory.CreateDirectory(cacheFolderPath);
                    Message("Created cache folder '" + cacheFolderPath + "'.", EventType.Information, 1000);
                }
                catch (UnauthorizedAccessException)
                {
                    // Show error message in gui and console and exit application
                    MessageBox.Show(@"Unable to create downloaded cache folder: '" + cacheFolderPath + @"'. Make sure the account you use to run this tool has write rights to this location - exiting...", @"Error when creating cache folder", MessageBoxButtons.OK);
                    
                    // Write error message to console and log file
                    Console.WriteLine(@"Unable to create downloaded cache folder: '" + cacheFolderPath + @"'. Make sure the account you use to run this tool has write rights to this location.");
                    Message("Unable to create downloaded cache folder: '" + cacheFolderPath + @"'. Make sure the account you use to run this tool has write rights to this location.", EventType.Error, 1001);

                    // Exit application
                    Application.Exit();
                }
                catch (Exception e)
                {
                    // Log unknown error message to console and log file
                    Console.WriteLine(e);

                    Message("Error when trying to create downloaded cache folder: '" + cacheFolderPath + @"' - Error: " + e, EventType.Error, 1001);

                    throw;
                }
            }
            else
            {
                Message("Cache folder '" + cacheFolderPath + "' exists.", EventType.Information, 1000);
            }

            // Create CefSettings object and configure it
            var settings = new CefSettings()
            {
                // Specify the cache folder path to persist data
                // By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName + "\\Cache")
            };

            // Disable logging severity
            settings.LogSeverity = LogSeverity.Disable;

            // Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            Message("Initialized Cef with settings: settings, performDependencyCheck: true, browserProcessHandler: null", EventType.Information, 1000);

            Message("Initializing GUI", EventType.Information, 1000);

            // Create a browser component
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}