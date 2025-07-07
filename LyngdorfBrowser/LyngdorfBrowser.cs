using System;
using System.Net;
using System.Timers;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using LyngdorfBrowser.Class;
using static LyngdorfBrowser.Class.FileLogger;

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
        private const double ConnectionCheckIntervalMs = 5000;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ConnectionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Call the CheckConnection() method
            CheckConnection();
        }

        private async void CheckConnection()
        {
            if (_chromeBrowser == null || !_chromeBrowser.IsBrowserInitialized) return;

            _connectionTimer.Stop();
            try
            {
                var response = await _chromeBrowser.EvaluateScriptAsync("navigator.onLine");
                if (response.Success && response.Result is bool isOnline && isOnline)
                {
                    StatusText = "Website is reachable";
#if DEBUG
            ShowStatusMessage($"Website '{TitleText}' is reachable", MessageBoxIcon.Information, EventType.Information, "Can connect");
#endif
                }
                else
                {
                    ShowStatusMessage($"Website '{TitleText}' is not reachable", MessageBoxIcon.Warning, EventType.Warning, "Can't connect");
                    StatusText = $"Website {TitleText} is not reachable";
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"An error occurred while checking the connection to '{TitleText}'.\nError: {ex.Message}", MessageBoxIcon.Warning, EventType.Error, "Can't connect");
                StatusText = $"An error occurred while checking the connection to '{TitleText}': {ex.Message}";
            }
            finally
            {
                _connectionTimer.Start();
            }
        }

        private void ShowStatusMessage(string message, MessageBoxIcon icon, EventType eventType, string title)
        {
            if (!_isTimerPaused)
            {
                _isTimerPaused = true;
                MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
                Message(message, eventType, 1001);
                _isTimerPaused = false;
            }
        }

        private bool TryInitializeChromiumWithArgument(string arg)
        {
            if (IPAddress.TryParse(arg, out _))
            {
                InitializeChromium(arg);
                return true;
            }
            else
            {
                Message("Invalid IP address argument - try again", EventType.Error, 1001);
                MessageBox.Show(@"Invalid IP address argument - try again", @"Error - not a valid IP address set in argument", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
                return false;
            }
        }

        private void InitializeConnectionTimer()
        {
            _connectionTimer = new System.Timers.Timer(ConnectionCheckIntervalMs);
            _connectionTimer.Elapsed += ConnectionTimerElapsed;
            Message("Calling CheckConnection() code to ensure device still online and run that", EventType.Information, 1000);
            _connectionTimer.Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OriginalText = Text;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (!TryInitializeChromiumWithArgument(args[1]))
                    return;
            }
            else
            {
                InitializeChromium();
            }

            InitializeConnectionTimer();
        }

        public void InitializeChromium(string ipAddress = null)
        {
            try
            {
                // If argument for IP address is null, find device on the network
                if (ipAddress == null)
                {
                    // Log the initialization message
                    Message($"Argument for IP address is null, will find device on the network", EventType.Information, 1000);

                    // Find the IP address for the device on the network
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

                // Log the found IP address message
                Message("Found IP address, will connect to the device on the network: " + ipAddress.Replace("\"", ""), EventType.Information, 1000);

                // Log the initialization message
                Message("Stating up ChromiumWebBrowser and setup", EventType.Information, 1000);

                _chromeBrowser = new ChromiumWebBrowser(_url);
                Controls.Add(_chromeBrowser);
                _chromeBrowser.TitleChanged += Browser_TitleChanged;
                _chromeBrowser.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                // If error somehow
                MessageBox.Show(@"Failed to get the IP address for the Lyngdorf device found on your network or the device is not supported for this tool. IP: "+ ipAddress + Environment.NewLine + @"Error: " + ex);

                Message("Failed to get the IP address for the Lyngdorf device found on your network or the device is not supported for this tool. Error: " + ex, EventType.Error, 1001);
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

            // Set the title text
            TitleText = e.Title;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Stop the connection checking timer
                _connectionTimer?.Stop();

                // Log the stopping timer message
                Message("Stopping CheckConnection() for connections checks...", EventType.Information, 1000);

                // Log the closing message
                Message("Stopping Cef...", EventType.Information, 1000);

                // Shutdown Cef
                Cef.Shutdown();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            // Log the closing message
            Message("Application " + Globals.ToolName.LyngdorfBrowser + " ended", EventType.Information, 1000);
        }
    }
}