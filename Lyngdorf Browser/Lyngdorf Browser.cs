﻿using System;
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

        public MainForm()
        {
            InitializeComponent();
            InitializeChromium();
        }

        private void Browser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            Invoke(new Action(() => { Text = e.Title; }));
        }

        public void InitializeChromium()
        {
            try
            {
                string ipAddress = IpMacMapper.FindIpFromMacAddress("50-1e-2d");
                if (ipAddress == null)
                {
                    MessageBox.Show(@"No Lyngdorf devices found on network");
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
                MessageBox.Show(@"Failed to get IP address for Lyngdorf device found on network");
            }

            // FUll MAC address
            /*try
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
            ChromeBrowser.Dock = DockStyle.Fill;*/
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}