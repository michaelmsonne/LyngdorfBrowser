using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static LyngdorfBrowser.Class.FileLogger;

namespace LyngdorfBrowser.Class
{
    internal class IpMacMapper
    {
        private static List<IpAndMac> _list;
        private class IpAndMac
        {
            public string Ip { get; set; }
            public string Mac { get; set; }
        }

        private static StreamReader ExecuteCommandLine(string file, string arguments = "")
        {
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = file,
                Arguments = arguments
            };
            var process = Process.Start(startInfo);
            return process?.StandardOutput;
        }

        private static List<IpAndMac> FetchIpAndMacList()
        {
            var arpStream = ExecuteCommandLine("arp", "-a");
            if (arpStream != null)
            {
                var result = new List<IpAndMac>();
                try
                {
                    while (!arpStream.EndOfStream)
                    {
                        var line = arpStream.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(line) && (line.Contains("dynamic") || line.Contains("static")))
                        {
                            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 3)
                            {
                                result.Add(new IpAndMac { Ip = parts[0].Trim(), Mac = parts[1].Trim() });
                            }
                        }
                    }
                }
                finally
                {
                    arpStream.Dispose(); // Ensure proper disposal
                }

                return result;
            }

            throw new InvalidOperationException("Failed to retrieve ARP data.");
        }

        private static void InitializeGetIPsAndMac()
        {
            if (_list == null)
            {
                // Log the initialization message
                Message("Initializing IP and MAC address data", EventType.Information, 1001);
                _list = FetchIpAndMacList();
            }
        }

        public static string FindIpFromMacAddress(string macAddress)
        {
            InitializeGetIPsAndMac();

            var item = _list.SingleOrDefault(x => x.Mac.StartsWith(macAddress, StringComparison.OrdinalIgnoreCase));

            if (item != null)
            {
                // Log the IP found message
                Message($"IP address found for MAC address started with: {macAddress} - IP: {item.Ip}", EventType.Information, 1000);
            }
            else
            {
                // Log a message for MAC address not found
                Message($"IP address not found for MAC address started with: {macAddress}", EventType.Error, 1001);
            }

            return item?.Ip;
        }
    }
}