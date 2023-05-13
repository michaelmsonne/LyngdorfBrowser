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

        private static StreamReader ExecuteCommandLine(String file, String arguments = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = file;
            startInfo.Arguments = arguments;
            Process process = Process.Start(startInfo);
            return process?.StandardOutput;
        }

        private static void InitializeGetIPsAndMac()
        {
            if (_list != null)
                return;
            var arpStream = ExecuteCommandLine("arp", "-a");
            List<string> result = new List<string>();
            while (!arpStream.EndOfStream)
            {
                var line = arpStream.ReadLine()?.Trim();
                result.Add(line);
            }
            _list = result.Where(x => !string.IsNullOrEmpty(x) && (x.Contains("dynamic") || x.Contains("static")))
                .Select(x =>
                {
                    string[] parts = x.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    return new IpAndMac { Ip = parts[0].Trim(), Mac = parts[1].Trim() };
                }).ToList();
        }

        public static string FindIpFromMacAddress(string macAddress)
        {
            // Part of MAC address
            InitializeGetIPsAndMac();
            IpAndMac item = _list.SingleOrDefault(x => x.Mac.StartsWith(macAddress, StringComparison.OrdinalIgnoreCase));
            if (item == null)
                return null;
            return item.Ip;

            // Full MAC address
            // InitializeGetIPsAndMac();
            // IPAndMac item = list.SingleOrDefault(x => x.MAC == macAddress);
            // if (item == null)
            //     return null;
            // return item.IP;
        }

        public static string FindMacFromIpAddress(string ip)
        {
            InitializeGetIPsAndMac();
            IpAndMac item = _list.SingleOrDefault(x => x.Ip == ip);
            if (item == null)
                return null;
            return item.Mac;
        }

        private class IpAndMac
        {
            public string Ip { get; set; }
            public string Mac { get; set; }
        }
    }
}