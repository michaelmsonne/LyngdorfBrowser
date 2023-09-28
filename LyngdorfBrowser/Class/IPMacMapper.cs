using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            return item?.Ip;
        }

        private class IpAndMac
        {
            public string Ip { get; set; }
            public string Mac { get; set; }
        }
    }
}