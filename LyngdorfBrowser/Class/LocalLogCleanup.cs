﻿using System;
using System.IO;
using static LyngdorfBrowser.Class.FileLogger;

namespace LyngdorfBrowser.Class
{
    internal class LocalLogCleanup
    {
        public static void CleanupLogs()
        {
            // Cleanup old log files
            string[] oldfiles = Directory.GetFiles(Files.LogFilePath);

            // Log
            Message("Checking for old log file(s) to cleanup...", EventType.Information, 1000);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
Checking for old log file(s) to cleanup...");
            Console.ResetColor();

            // Loop all files in folder
            foreach (string file in oldfiles)
            {
                FileInfo fi = new FileInfo(file);

                // Get all last access time back in time
                if (fi.LastAccessTime < DateTime.Now.AddDays(-30))
                {
                    try
                    {
                        // Do work
                        fi.Delete();

                        // Log
                        Message("> Deleted old log file: '" + fi + "'", EventType.Information, 1000);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(@"Deleted old log file: '" + fi + @"'");
                        Console.ResetColor();

                        // Set status
                        //ApplicationGlobals._oldLogfilesToDelete = true;
                        Globals.Logs._oldLogFilesToDeleteCount++;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Message("! Unable to delete old log file: '" + fi + "'. Make sure the account you use to run this tool has delete rights to this location.", EventType.Error, 1001);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(@"Unable to delete old log file: '" + fi + @"'. Make sure the account you use to run this tool has delete rights to this location.");
                        Console.ResetColor();

                        // Count errors
                        //ApplicationGlobals._errors++;
                    }
                    catch (Exception ex)
                    {
                        // Log
                        Message("Sorry, we are unable to delete old log file: '" + fi + "' - Error: " + ex, EventType.Error, 1001);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(@"Sorry, we are unable to delete old log file: '" + fi + @"' - Error: " + ex);
                        Console.ResetColor();

                        // Count errors
                        //ApplicationGlobals._errors++;
                    }
                }
            }

            // Check if there is old log files to delete
            if (Globals.Logs._oldLogfilesToDelete)
            {
                // Log
                Message($"There was '{Globals.Logs._oldLogFilesToDeleteCount}' old log files to delete (-30 days)", EventType.Information, 1000);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($@"There was '{Globals.Logs._oldLogFilesToDeleteCount}' old log files to delete (-30 days)");
                Console.ResetColor();
            }
            else
            {
                // Log
                Message("No old log files to delete (-30 days)", EventType.Information, 1000);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@"No old log files to delete (-30 days)");
                Console.ResetColor();
            }
        }
    }
}
