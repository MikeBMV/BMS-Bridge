// Form1.cs - Refactored version following SRP
using System;
using System.Windows.Forms;

namespace BMS_Bridge_Launcher
{
    public partial class Form1 : Form
    {
        private readonly ServerManager serverManager;
        private readonly HealthMonitor healthMonitor;
        private readonly UIController uiController;
                
        public Form1()
        {
            InitializeComponent();
            
            // Initialize managers
            serverManager = new ServerManager();
            healthMonitor = new HealthMonitor();
            uiController = new UIController(this);
            
            InitializeApp();
        }
        
        private void InitializeApp()
        {
            // Subscribe to server manager events
            serverManager.OutputReceived += (s, data) => uiController.AppendLog(data);
            serverManager.ErrorReceived += (s, data) => uiController.AppendLog($"ERROR: {data}");
            serverManager.ServerExited += OnServerExited;
            
            // Subscribe to health monitor events
            healthMonitor.HealthUpdated += (s, state) => uiController.UpdateServerStatus(state);
            healthMonitor.ErrorOccurred += (s, error) => uiController.AppendLog($"Health check: {error}");
            
            // Set initial UI state
            uiController.UpdateServerStatus(new ServerHealthState { server_status = "STOPPED" });
        }
        
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (serverManager.IsRunning)
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
            try
            {
                // Update UI to starting state
                uiController.UpdateServerStatus(new ServerHealthState { server_status = "STARTING" });
                
                // Start server
                serverManager.Start();
                
                // Start monitoring
                healthMonitor.StartMonitoring();
            }
            catch (Exception ex)
            {
                uiController.ShowError($"Failed to start server: {ex.Message}", "Startup Error");
                uiController.UpdateServerStatus(new ServerHealthState { server_status = "STOPPED" });
            }
        }
        
        private void StopServer()
        {
            // Stop monitoring first
            healthMonitor.StopMonitoring();
            
            // Stop server
            serverManager.Stop();
            
            // Update UI
            uiController.UpdateServerStatus(new ServerHealthState { server_status = "STOPPED" });
        }
        
        private void OnServerExited(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                healthMonitor.StopMonitoring();
                uiController.UpdateServerStatus(new ServerHealthState { server_status = "STOPPED" });
            });
        }
        
        #region Window and Tray Management
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Use the health monitor's last known state as single source of truth
            if (healthMonitor.LastKnownState.IsRunning())
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon.Visible = true;
            }
        }
        
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
        
        private void CmsTrayMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Show / Hide")
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
            else if (e.ClickedItem.Text == "Exit")
            {
                StopServer();
                Application.Exit();
            }
        }
        
        #endregion
        
        #region Placeholder Feature Buttons
        
        private void btnSettings_Click(object sender, EventArgs e)
        {
            uiController.ShowInfo("Settings window will be implemented here.", "Coming Soon");
        }
        
        private void btnQRCode_Click(object sender, EventArgs e)
        {
            uiController.ShowInfo("QR code generator will be implemented here.", "Coming Soon");
        }
        
        #endregion       
    }
}