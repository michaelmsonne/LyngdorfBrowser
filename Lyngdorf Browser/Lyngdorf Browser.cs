using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace LyngdorfBrowser
{
    public partial class MainForm : Form
    {
        private String _url = string.Empty;
        private ChromiumWebBrowser ChromeBrowser;
        public MainForm()
        {
            InitializeComponent();
            InitializeChromium();
        }

        public void InitializeChromium()
        {
            try
            {
                string ipAddress = IPMacMapper.FindIPFromMacAddress("50-1e-2d-1e-15-2a");
                _url = ipAddress;
            }
            catch
            {
                MessageBox.Show(@"WiFI ik fundet");
            }
            try
            {
                string ipAddress = IPMacMapper.FindIPFromMacAddress("50-1e-2d-1e-15-28");
                _url = ipAddress;
            }
            catch
            {
                MessageBox.Show(@"LAN ik fundet");
            }

            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            ChromeBrowser = new ChromiumWebBrowser("http://" + _url);
            Controls.Add(ChromeBrowser);
            ChromeBrowser.Dock = DockStyle.Fill;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}