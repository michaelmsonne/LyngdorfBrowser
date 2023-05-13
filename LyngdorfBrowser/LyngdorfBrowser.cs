using System;
using System.Net;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using LyngdorfBrowser.Class;

namespace LyngdorfBrowser
{
    public partial class MainForm : Form
    {
        private string _url = string.Empty;
        private ChromiumWebBrowser _chromeBrowser;
        public string OriginalText;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OriginalText = Text; // Store the original form text in a variable

            // Check arguments
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // If an argument is used, try to use it as ip address for the device
                string ipAddress = args[1];

                // Validate the IP address
                if (IPAddress.TryParse(ipAddress, out _))
                {
                    InitializeChromium(ipAddress);
                }
                else
                {
                    // If the IP address parsed it not in IP address format show it to the user and exit application
                    MessageBox.Show(@"Invalid IP address argument - try again", @"Error - not a valid IP address set in argument", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
            else
            {
                // If no arguments is set, find device on the network from MAC address (vendor)
                InitializeChromium();
            }
        }

        private void Browser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                switch (Text.StartsWith(OriginalText))
                {
                    // Check if the original text has already been appended
                    case false:
                        Text = $@"{OriginalText}: {e.Title}";
                        break;
                    default:
                        Text = $@"{OriginalText}: {e.Title}";
                        break;
                }
            }));
        }

        public void InitializeChromium(string ipAddress = null)
        {
            try
            {
                // If argument for IP address is null, find device on the network
                if (ipAddress == null)
                {
                    ipAddress = IpMacMapper.FindIpFromMacAddress("50-1e-2d");
                    if (ipAddress == null)
                    {
                        MessageBox.Show(@"No Lyngdorf devices found on your network.
Try again or connect to your network with a cable.

If this not works, use the commandline to connect to a specific IP address in this format:

.\LyngdorfBrowser.exe 192.168.1.200", @"Can´s connect to device", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Environment.Exit(1);
                    }
                }

                // Set IP address for the browser for the device to use
                _url = "http://" + ipAddress.Replace("\"", "");
                _chromeBrowser = new ChromiumWebBrowser(_url);
                Controls.Add(_chromeBrowser);
                _chromeBrowser.TitleChanged += Browser_TitleChanged;
                _chromeBrowser.Dock = DockStyle.Fill;
            }
            catch
            {
                // If error somehow
                MessageBox.Show(@"Failed to get the IP address for the Lyngdorf device found on your network or the device is not supported for this tool");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Cef.Shutdown();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}