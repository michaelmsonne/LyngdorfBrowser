﻿using System;
using System.Net;
using System.Timers;
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
        public string TitleText;
        public string StatusText;
        private System.Timers.Timer _connectionTimer;
        private bool _isTimerPaused;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ConnectionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            CheckConnection();
        }

        private void CheckConnection()
        {
            try
            {
                if (_chromeBrowser != null && _chromeBrowser.IsBrowserInitialized)
                {
                    // Pause the timer
                    _connectionTimer.Stop();

                    // Execute JavaScript code to check if the website is still reachable
                    _chromeBrowser.EvaluateScriptAsync("navigator.onLine").ContinueWith(task =>
                    {
                        if (task.IsCompleted && !task.IsFaulted)
                        {
                            var response = task.Result;
                            if (response.Success && response.Result is bool isOnline && isOnline)
                            {
                                // Website is reachable
                                StatusText = "Website is reachable";
#if DEBUG
                                _isTimerPaused = true;
                                MessageBox.Show(@"Website '" + TitleText + @"' is reachable", @"Can connect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                _isTimerPaused = false;
#endif
                            }
                            else
                            {
                                // Website is not reachable
                                if (!_isTimerPaused)
                                {
                                    _isTimerPaused = true;
                                    MessageBox.Show(@"Website '" + TitleText + @"' is not reachable", @"Can't connect right now", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    _isTimerPaused = false;
                                }
                                StatusText = "Website " + TitleText + @" is not reachable";
                            }
                        }
                        else
                        {
                            // Error occurred while checking the connection
                            if (!_isTimerPaused)
                            {
                                _isTimerPaused = true;
                                MessageBox.Show(@"An error occurred while checking the connection to '" + TitleText + @"'.", @"Can't connect right now", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                _isTimerPaused = false;
                            }
                            StatusText = "An error occurred while checking the connection to '" + TitleText + @"'";
                        }

                        // Resume the timer
                        _connectionTimer.Start();
                    });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                if (!_isTimerPaused)
                {
                    _isTimerPaused = true;
                    MessageBox.Show(@"An error occurred while checking the connection to: '" + TitleText + @"'." + Environment.NewLine + @"Error: " + ex.Message, @"Can't connect right now", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _isTimerPaused = false;
                }
#endif
                StatusText = "An error occurred while checking the connection to '" + TitleText + @"': " + ex.Message;

                // Resume the timer
                _connectionTimer.Start();
            }

            // Simulate the title change event
            //Text = $@"{OriginalText}: {TitleText} - Status: " + StatusText;

            // string newTitle = $@"{OriginalText}: {TitleText} - Status: " + StatusText;
            // TitleChangedEventArgs args = new TitleChangedEventArgs(_chromeBrowser.GetBrowser(), newTitle);
            // Browser_TitleChanged(_chromeBrowser, args);
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

            // Start the connection checking timer
            _connectionTimer = new System.Timers.Timer(5000); // Set the interval to 5 seconds (adjust as needed)
            _connectionTimer.Elapsed += ConnectionTimerElapsed;
            _connectionTimer.Start();
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

        private void Browser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                switch (Text.StartsWith(OriginalText))
                {
                    // Check if the original text has already been appended
                    case false:
                        //TODO
                        //Text = $@"{OriginalText}: {e.Title} - Status: " + StatusText;
                        Text = $@"{OriginalText}: {e.Title}";
                        break;
                    default:
                        Text = $@"{OriginalText}: {e.Title}";
                        break;
                }
            }));

            TitleText = e.Title;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Stop the connection checking timer
                _connectionTimer?.Stop();

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