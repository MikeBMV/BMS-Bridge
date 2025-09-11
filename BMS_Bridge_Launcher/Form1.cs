// Form1.cs - Refactored version following SRP
using System;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BMS_Bridge_Launcher
{
    public partial class Form1 : Form
    {
        private readonly ServerManager serverManager;
        private readonly HealthMonitor healthMonitor;
        private readonly UIController uiController;
        private ListView _lastFocusedListView;

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

            // Set event handlers
            this.btnDeleteLeft.Click += new System.EventHandler(this.btnDeleteLeft_Click);
            this.btnDeleteRight.Click += new System.EventHandler(this.btnDeleteRight_Click);
            this.btnAddLeft.Click += new System.EventHandler(this.btnAddLeft_Click);
            this.btnAddRight.Click += new System.EventHandler(this.btnAddRight_Click);
            this.lvKneeboardLeft.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.KneeboardItem_Checked);
            this.lvKneeboardRight.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.KneeboardItem_Checked);
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            this.btnMoveToLeft.Click += new System.EventHandler(this.btnMoveToLeft_Click);
            this.btnMoveToRight.Click += new System.EventHandler(this.btnMoveToRight_Click);
            this.lvKneeboardLeft.Enter += new System.EventHandler(this.listView_Enter);
            this.lvKneeboardRight.Enter += new System.EventHandler(this.listView_Enter);
            this.tsbStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            this.tsbSettings.Click += new System.EventHandler(this.btnSettings_Click);
            this.tsbQRCode.Click += new System.EventHandler(this.btnQRCode_Click);

            LoadKneeboardSettings();
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
                uiController.ClearLog();

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
            healthMonitor.StopMonitoring();
            serverManager.Stop();

            healthMonitor.ManuallySetState(new ServerHealthState { server_status = "STOPPED" });
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

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Program.WM_SHOWME)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
                this.Activate();
            }
            base.WndProc(ref m);
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
            string serverAddress = healthMonitor.LastKnownState?.server_address;

            if (serverManager.IsRunning && !string.IsNullOrEmpty(serverAddress))
            {
                using (var qrForm = new QrCodeForm(serverAddress))
                {
                    qrForm.ShowDialog(this);
                }
            }
            else
            {
                uiController.ShowInfo("Start the server first to get its address and generate a QR code.", "Server Not Running");
            }
        }

        #endregion

        private void LoadKneeboardSettings()
        {
            try
            {
                if (!File.Exists(serverManager.SettingsFilePath))
                {
                    return;
                }

                string json = File.ReadAllText(serverManager.SettingsFilePath);
                var settings = JsonConvert.DeserializeObject<AppSettings>(json);

                if (settings?.Kneeboards != null)
                {
                    PopulateKneeboardListView(lvKneeboardLeft, settings.Kneeboards.Left);
                    PopulateKneeboardListView(lvKneeboardRight, settings.Kneeboards.Right);
                }
            }
            catch (Exception ex)
            {
                uiController.ShowError($"Failed to load kneeboard settings: {ex.Message}", "Settings Error");
            }
        }

        private void PopulateKneeboardListView(ListView listView, List<KneeboardItem> items)
        {
            listView.Items.Clear();

            if (items == null) return;

            foreach (var item in items)
            {
                var listViewItem = new ListViewItem(item.Path)
                {
                    Checked = item.Enabled,
                    Tag = item
                };
                listView.Items.Add(listViewItem);
            }
        }

        private void SaveKneeboardSettings()
        {
            try
            {
                string json = File.Exists(serverManager.SettingsFilePath)
                    ? File.ReadAllText(serverManager.SettingsFilePath)
                    : "{}";

                var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                if (settings == null) settings = new AppSettings();

                var newKneeboardConfig = new KneeboardConfig();

                foreach (ListViewItem item in lvKneeboardLeft.Items)
                {
                    if (item.Tag is KneeboardItem kneeboardItem)
                    {
                        newKneeboardConfig.Left.Add(kneeboardItem);
                    }
                }

                foreach (ListViewItem item in lvKneeboardRight.Items)
                {
                    if (item.Tag is KneeboardItem kneeboardItem)
                    {
                        newKneeboardConfig.Right.Add(kneeboardItem);
                    }
                }

                settings.Kneeboards = newKneeboardConfig;

                string outputJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(serverManager.SettingsFilePath, outputJson);
            }
            catch (Exception ex)
            {
                uiController.ShowError($"Failed to save kneeboard settings: {ex.Message}", "Settings Error");
            }
        }

        private void btnDeleteLeft_Click(object sender, EventArgs e)
        {
            if (lvKneeboardLeft.SelectedItems.Count > 0)
            {
                lvKneeboardLeft.Items.Remove(lvKneeboardLeft.SelectedItems[0]);

                SaveKneeboardSettings();
            }
        }

        private void btnDeleteRight_Click(object sender, EventArgs e)
        {
            if (lvKneeboardRight.SelectedItems.Count > 0)
            {
                lvKneeboardRight.Items.Remove(lvKneeboardRight.SelectedItems[0]);

                SaveKneeboardSettings();
            }
        }

        private void btnAddLeft_Click(object sender, EventArgs e)
        {
            AddItemToKneeboard(lvKneeboardLeft);
        }

        private void btnAddRight_Click(object sender, EventArgs e)
        {
            AddItemToKneeboard(lvKneeboardRight);
        }

        private void AddItemToKneeboard(ListView targetListView)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Add Kneeboard File",
                Filter = "All Supported Files|*.png;*.jpg;*.jpeg;*.pdf|Image Files|*.png;*.jpg;*.jpeg|PDF Files|*.pdf",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFilePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(sourceFilePath);

                string destinationFolder = Path.Combine(serverManager.serverWorkingDirectory, "user_data", "kneeboards");

                Directory.CreateDirectory(destinationFolder);

                string destinationFilePath = Path.Combine(destinationFolder, fileName);

                if (File.Exists(destinationFilePath))
                {
                    var result = MessageBox.Show($"A file named '{fileName}' already exists. Do you want to overwrite it?",
                        "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                try
                {
                    File.Copy(sourceFilePath, destinationFilePath, true);

                    var newItem = new KneeboardItem { Path = fileName, Enabled = true };
                    var listViewItem = new ListViewItem(newItem.Path)
                    {
                        Tag = newItem,
                        Checked = true
                    };

                    targetListView.Items.Add(listViewItem);

                    SaveKneeboardSettings();
                }
                catch (Exception ex)
                {
                    uiController.ShowError($"Failed to add file: {ex.Message}", "File Error");
                }
            }
        }

        private void KneeboardItem_Checked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Tag is KneeboardItem kneeboardItem)
            {
                kneeboardItem.Enabled = e.Item.Checked;

                SaveKneeboardSettings();
            }
        }

        #region Kneeboard Movement Logic

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (_lastFocusedListView != null)
            {
                MoveSelectedItem(_lastFocusedListView, -1);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (_lastFocusedListView != null)
            {
                MoveSelectedItem(_lastFocusedListView, 1);
            }
        }

        private void btnMoveToRight_Click(object sender, EventArgs e)
        {
            TransferSelectedItem(lvKneeboardLeft, lvKneeboardRight);
        }

        private void btnMoveToLeft_Click(object sender, EventArgs e)
        {
            TransferSelectedItem(lvKneeboardRight, lvKneeboardLeft);
        }

        private void MoveSelectedItem(ListView listView, int direction)
        {
            if (listView.SelectedItems.Count == 0) return;

            var selectedItem = listView.SelectedItems[0];

            int oldIndex = selectedItem.Index;
            int newIndex = oldIndex + direction;

            if (newIndex < 0 || newIndex >= listView.Items.Count) return;

            var itemToMove = (ListViewItem)selectedItem.Clone();

            listView.Items.RemoveAt(oldIndex);
            listView.Items.Insert(newIndex, itemToMove);

            listView.Items[newIndex].Selected = true;
            listView.Focus();

            SaveKneeboardSettings();
        }

        private void TransferSelectedItem(ListView source, ListView destination)
        {
            if (source.SelectedItems.Count == 0) return;

            var selectedItem = source.SelectedItems[0];

            source.Items.Remove(selectedItem);
            destination.Items.Add(selectedItem);

            destination.Items[destination.Items.Count - 1].Selected = true;
            destination.Focus();

            SaveKneeboardSettings();
        }
        
        private void listView_Enter(object sender, EventArgs e)
        {
            _lastFocusedListView = sender as ListView;
        }

        #endregion
    }
}