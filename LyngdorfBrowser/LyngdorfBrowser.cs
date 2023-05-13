using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using LyngdorfBrowser.Class;

namespace LyngdorfBrowser
{
    public partial class MainForm : Form
    {
        private String _url = string.Empty;
        private ChromiumWebBrowser _chromeBrowser;
        public string OriginalText;

        public MainForm()
        {
            InitializeComponent();
            InitializeChromium();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OriginalText = Text; // Store the original form text in a variable
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

        public void InitializeChromium()
        {
            try
            {
                string ipAddress = IpMacMapper.FindIpFromMacAddress("50-1e-2d");
                if (ipAddress == null)
                {
                    MessageBox.Show(@"No Lyngdorf devices found on your network");
                }
                else
                {
                    _url = "http://" + ipAddress.Replace("\"", "");
                    _chromeBrowser = new ChromiumWebBrowser(_url);
                    Controls.Add(_chromeBrowser);
                    _chromeBrowser.TitleChanged += Browser_TitleChanged;
                    _chromeBrowser.Dock = DockStyle.Fill;
                }
            }
            catch
            {
                MessageBox.Show(@"Failed to get the IP address for the Lyngdorf device found on your network or the device is not supported for this tool");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}