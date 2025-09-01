// --- Form1.cs - TRANSLATED VERSION ---

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json; // Make sure this NuGet package is installed

namespace BMS_Bridge_Launcher
{
    public partial class Form1 : Form
    {
        private Process serverProcess;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly Timer statusTimer = new Timer();
        private ServerHealthState lastServerState; // Stores the last known state of the server

        public Form1()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            // Configure the timer to poll the server's health endpoint
            statusTimer.Interval = 2000; // Poll every 2 seconds
            statusTimer.Tick += async (s, e) => await PollServerStatusAsync();

            // Set the initial UI state
            UpdateUI(new ServerHealthState { server_status = "STOPPED", bms_status = "NOT_AVAILABLE" });
        }

        #region --- Server Process Management ---

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (serverProcess != null && !serverProcess.HasExited)
            {
                StopServer();
            }
            else
            {
                StartServer();
            }
        }

        private void StartServer()
        {
            string launcherPath = AppDomain.CurrentDomain.BaseDirectory;
            string serverPath = Path.Combine(launcherPath, "Server");
            string serverExePath = Path.Combine(serverPath, "BMS_Bridge_Server.exe");

            if (!File.Exists(serverExePath))
            {
                MessageBox.Show($"Server executable not found:\n{serverExePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Log("--- Attempting to start server ---");
                UpdateUI(new ServerHealthState { server_status = "STARTING" });
                lastServerState = new ServerHealthState { server_status = "STARTING" };

                serverProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = serverExePath,
                        WorkingDirectory = serverPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                serverProcess.OutputDataReceived += (s, args) => Log(args.Data);
                serverProcess.ErrorDataReceived += (s, args) => Log(args.Data);
                serverProcess.Exited += ServerProcess_Exited;

                if (!serverProcess.Start())
                {
                    throw new InvalidOperationException("Process.Start() returned false.");
                }

                serverProcess.BeginOutputReadLine();
                serverProcess.BeginErrorReadLine();

                statusTimer.Start(); // Start the status poller
            }
            catch (Exception ex)
            {
                Log("--- FAILED TO START SERVER ---");
                Log(ex.ToString());
                MessageBox.Show("Failed to start server process: " + ex.Message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                serverProcess = null;
                UpdateUI(new ServerHealthState { server_status = "STOPPED" });
            }
        }

        private void StopServer()
        {
            statusTimer.Stop(); // Stop the status poller
            Log("--- Stopping server process tree... ---");
            try
            {
                Process taskkill = new Process
                {
                    StartInfo =
                    {
                        FileName = "taskkill.exe",
                        // /F = Force
                        // /IM = Image Name
                        // /T = Tree (Kill process and all of its children) - THIS IS THE KEY CHANGE!
                        Arguments = "/F /IM BMS_Bridge_Server.exe /T",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                taskkill.Start();
                taskkill.WaitForExit(); // Wait for the command to complete
                Log("Termination command sent to server process tree.");
            }
            catch (Exception ex)
            {
                Log("Error while stopping server: " + ex.Message);
            }
            finally
            {
                serverProcess = null;
                UpdateUI(new ServerHealthState { server_status = "STOPPED" });
            }
        }

        private void ServerProcess_Exited(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                statusTimer.Stop();
                Log("--- Server was stopped or has crashed. ---");
                serverProcess = null;
                UpdateUI(new ServerHealthState { server_status = "STOPPED" });
            });
        }

        #endregion

        #region --- UI Update and Logic ---

        private async Task PollServerStatusAsync()
        {
            // If the process object doesn't exist, don't poll
            if (serverProcess == null || serverProcess.HasExited)
            {
                StopServer(); // Just in case, to clean up the state
                return;
            }

            try
            {
                // This assumes the server is always on localhost:8000
                var response = await httpClient.GetAsync("http://localhost:8000/api/health");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var state = JsonConvert.DeserializeObject<ServerHealthState>(json);
                    UpdateUI(state);
                }
                else
                {
                    // The server process exists, but the API is not responding correctly - this is an error
                    UpdateUI(new ServerHealthState { server_status = "ERROR", server_message = "API is not responding." });
                }
            }
            catch (HttpRequestException)
            {
                // Cannot connect. This is expected when the server is starting up.
                // We don't change the status, just log and wait for the next poll.
                Log("Waiting for API response...");
            }
        }

        private void UpdateUI(ServerHealthState state)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateUI(state)));
                return;
            }

            lastServerState = state; // Cache the latest state

            // --- Update Server Status ---
            switch (state.server_status)
            {
                case "RUNNING":
                    lblStatusIndicatorServer.ForeColor = Color.Green;
                    lblStatusTextServer.Text = "Status: Running";
                    break;
                case "WARNING":
                    lblStatusIndicatorServer.ForeColor = Color.Orange;
                    lblStatusTextServer.Text = "Status: Warning";
                    break;
                case "ERROR":
                    lblStatusIndicatorServer.ForeColor = Color.Red;
                    lblStatusTextServer.Text = "Status: Error";
                    break;
                case "STARTING":
                    lblStatusIndicatorServer.ForeColor = Color.DodgerBlue;
                    lblStatusTextServer.Text = "Status: Starting...";
                    break;
                case "STOPPED":
                default:
                    lblStatusIndicatorServer.ForeColor = Color.Gray;
                    lblStatusTextServer.Text = "Status: Stopped";
                    break;
            }

            // --- Update BMS Status ---
            switch (state.bms_status)
            {
                case "CONNECTED":
                    lblStatusIndicatorBms.ForeColor = Color.Green;
                    lblStatusIndicatorBms.Text = "●";
                    lblStatusTextBms.Text = "BMS: Connected";
                    break;
                case "NOT_CONNECTED":
                    lblStatusIndicatorBms.ForeColor = Color.Gray;
                    lblStatusIndicatorBms.Text = "○";
                    lblStatusTextBms.Text = "BMS: Not Found";
                    break;
                default:
                    lblStatusIndicatorBms.ForeColor = Color.Gray;
                    lblStatusIndicatorBms.Text = "○";
                    lblStatusTextBms.Text = "BMS: -";
                    break;
            }

            // --- Update Address and Control Button ---
            if (state.IsRunning())
            {
                btnStartStop.Text = "Stop Server";
                lblServerAddress.Text = $"Address: {state.server_address}";
                lblServerAddress.Visible = true;
            }
            else
            {
                btnStartStop.Text = "Start Server";
                lblServerAddress.Visible = false;
            }
        }

        private void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            if (txtLogs.InvokeRequired)
            {
                txtLogs.Invoke(new Action<string>(Log), message);
            }
            else
            {
                txtLogs.AppendText(message + Environment.NewLine);
                txtLogs.ScrollToCaret();
            }
        }

        #endregion

        #region --- Window and Tray Logic ---

        // --- FINAL, WORKING VERSION ---
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // We only trust the state received from the API.
            // This is the single source of truth.
            if (lastServerState != null && lastServerState.IsRunning())
            {
                // If the server is running, cancel the close event and hide to tray.
                e.Cancel = true;
                this.Hide();
                notifyIcon.Visible = true;
            }
            // If IsRunning() returns false, do nothing.
            // The form will close normally, terminating the application.
        }
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // Show the window on double-click
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void CmsTrayMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Показать / Скрыть") // NOTE: This text will be translated in the Designer file
            {
                if (this.Visible)
                {
                    this.Hide();
                }
                else
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    notifyIcon.Visible = false;
                }
            }
            else if (e.ClickedItem.Text == "Выход") // NOTE: This text will be translated in the Designer file
            {
                StopServer();
                Application.Exit();
            }
        }

        #endregion

        #region --- Placeholder Buttons ---

        private void btnQRCode_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A QR code will be displayed here for quick access.", "Information");
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The settings window will open here.", "Information");
        }

        #endregion
    }

    // Helper class to deserialize the JSON response from the server
    public class ServerHealthState
    {
        public string server_status { get; set; } = "STOPPED";
        public string bms_status { get; set; } = "NOT_AVAILABLE";
        public string server_address { get; set; }
        public string server_message { get; set; }

        public bool IsRunning()
        {
            return server_status == "RUNNING" || server_status == "WARNING" || server_status == "ERROR";
        }
    }
}