using CefSharp.WinForms;
using CefSharp;
using System;
using System.IO;
using System.Windows.Forms;
using LyngdorfBrowser.Class;
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
            // Set global logfile properties for log
            DateFormat = "dd-MM-yyyy";
            DateTimeFormat = "dd-MM-yyyy HH:mm:ss";
            WriteOnlyErrorsToEventLog = false;
            WriteToEventLog = false;
            WriteToFile = true;

            Message("Initialized log config", EventType.Information, 1000);

            // Set and reuse the cache folder path
            var cacheFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Globals.ToolName.LyngdorfBrowser + "\\Cache"
            );

            // Ensure cache folder exists, handle errors and exit early if needed
            if (!EnsureCacheFolderExists(cacheFolderPath))
                return;

            // Create and configure CefSettings
            var settings = new CefSettings
            {
                CachePath = cacheFolderPath,
                LogSeverity = LogSeverity.Default
            };

            try
            {
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
                Message("Initialized Cef with settings: settings, performDependencyCheck: true, browserProcessHandler: null", EventType.Information, 1000);
            }
            catch (FileLoadException ex)
            {
                Message("CefSharp FileLoadException: " + ex.Message + " - " + ex.FusionLog, EventType.Error, 1002);
                MessageBox.Show(@"CefSharp failed to load: " + ex.Message, @"CefSharp Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Message("Initializing GUI", EventType.Information, 1000);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /// <summary>
        /// Ensures the cache folder exists. Handles errors and logs as needed.
        /// Returns false if the application should exit.
        /// </summary>
        private static bool EnsureCacheFolderExists(string cacheFolderPath)
        {
            if (!Directory.Exists(cacheFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(cacheFolderPath);
                    Message($"Created cache folder '{cacheFolderPath}'.", EventType.Information, 1000);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $@"Unable to create downloaded cache folder: '{cacheFolderPath}'. Make sure the account you use to run this tool has write rights to this location - exiting...",
                        @"Error when creating cache folder", MessageBoxButtons.OK
                    );
                    Console.WriteLine($@"Unable to create downloaded cache folder: '{cacheFolderPath}'. Make sure the account you use to run this tool has write rights to this location.");
                    Message($@"Unable to create downloaded cache folder: '{cacheFolderPath}'. Make sure the account you use to run this tool has write rights to this location.", EventType.Error, 1001);
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Message($@"Error when trying to create downloaded cache folder: '{cacheFolderPath}' - Error: {e}", EventType.Error, 1001);
                    throw;
                }
            }
            else
            {
                Message($"Cache folder '{cacheFolderPath}' exists.", EventType.Information, 1000);
            }
            return true;
        }
    }
}