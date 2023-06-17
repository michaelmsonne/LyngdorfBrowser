using CefSharp.WinForms;
using CefSharp;
using System;
using System.IO;
using System.Windows.Forms;

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
            // Set the cache folder path to a location within the user's LocalApplicationData folder
            var cacheFolderPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Application.ProductName + "\\Cache");
            // Create the cache folder if it doesn't exist
            if (!Directory.Exists(cacheFolderPath))
            {
                Directory.CreateDirectory(cacheFolderPath);
            }

            // Create CefSettings object and configure it
            var settings = new CefSettings()
            {
                // Specify the cache folder path to persist data
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName + "\\Cache")
            };

            // Disable logging severity
            settings.LogSeverity = LogSeverity.Disable;

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}