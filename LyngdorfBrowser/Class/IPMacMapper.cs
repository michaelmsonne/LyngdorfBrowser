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

        private static void InitializeGetIPsAndMac()
        {
            if (_list != null)
                return;

            // Log the initialization message
            Message("Initializing IP and MAC address data", EventType.Information, 1001);

            var arpStream = ExecuteCommandLine("arp", "-a");
            var result = new List<string>();
            while (!arpStream.EndOfStream)
            {
                var line = arpStream.ReadLine()?.Trim();
                result.Add(line);
            }
            _list = result.Where(x => !string.IsNullOrEmpty(x) && (x.Contains("dynamic") || x.Contains("static")))
                .Select(x =>
                {
                    var parts = x.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    return new IpAndMac { Ip = parts[0].Trim(), Mac = parts[1].Trim() };
                }).ToList();
        }

        public static string FindIpFromMacAddress(string macAddress)
        {
            // Part of MAC address
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

        private class IpAndMac
        {
            public string Ip { get; set; }
            public string Mac { get; set; }
        }
    }
}