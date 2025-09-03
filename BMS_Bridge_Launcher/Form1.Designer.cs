// --- Form1.Designer.cs - WITH KNEBOARDS UI ---

namespace BMS_Bridge_Launcher
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsTrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblStatusIndicatorServer = new System.Windows.Forms.Label();
            this.lblStatusTextServer = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblStatusIndicatorBms = new System.Windows.Forms.Label();
            this.lblStatusTextBms = new System.Windows.Forms.Label();
            this.lblServerAddress = new System.Windows.Forms.Label();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnQRCode = new System.Windows.Forms.Button();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabKneeboards = new System.Windows.Forms.TabPage();
            this.tlpKneeboardsMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblLeftKneeboard = new System.Windows.Forms.Label();
            this.lblRightKneeboard = new System.Windows.Forms.Label();
            this.lvKneeboardLeft = new System.Windows.Forms.ListView();
            this.chLeftPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvKneeboardRight = new System.Windows.Forms.ListView();
            this.chRightPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlLeftButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddLeft = new System.Windows.Forms.Button();
            this.btnDeleteLeft = new System.Windows.Forms.Button();
            this.pnlRightButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddRight = new System.Windows.Forms.Button();
            this.btnDeleteRight = new System.Windows.Forms.Button();
            this.pnlCenterButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveToRight = new System.Windows.Forms.Button();
            this.btnMoveToLeft = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.tabLogs = new System.Windows.Forms.TabPage();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.tlpMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabKneeboards.SuspendLayout();
            this.tlpKneeboardsMain.SuspendLayout();
            this.pnlLeftButtons.SuspendLayout();
            this.pnlRightButtons.SuspendLayout();
            this.pnlCenterButtons.SuspendLayout();
            this.tabLogs.SuspendLayout();
            this.cmsTrayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.pnlHeader, 0, 0);
            this.tlpMain.Controls.Add(this.tabControlMain, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(934, 561);
            this.tlpMain.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.tlpHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(3, 3);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(5);
            this.pnlHeader.Size = new System.Drawing.Size(928, 49);
            this.pnlHeader.TabIndex = 0;
            // 
            // tlpHeader
            // 
            this.tlpHeader.ColumnCount = 4;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHeader.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tlpHeader.Controls.Add(this.flowLayoutPanel2, 1, 0);
            this.tlpHeader.Controls.Add(this.lblServerAddress, 2, 0);
            this.tlpHeader.Controls.Add(this.flowLayoutPanel3, 3, 0);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Location = new System.Drawing.Point(5, 5);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 1;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tlpHeader.Size = new System.Drawing.Size(918, 39);
            this.tlpHeader.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.lblStatusIndicatorServer);
            this.flowLayoutPanel1.Controls.Add(this.lblStatusTextServer);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 9);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(138, 20);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // lblStatusIndicatorServer
            // 
            this.lblStatusIndicatorServer.AutoSize = true;
            this.lblStatusIndicatorServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblStatusIndicatorServer.Location = new System.Drawing.Point(3, 0);
            this.lblStatusIndicatorServer.Name = "lblStatusIndicatorServer";
            this.lblStatusIndicatorServer.Size = new System.Drawing.Size(19, 20);
            this.lblStatusIndicatorServer.TabIndex = 0;
            this.lblStatusIndicatorServer.Text = "●";
            // 
            // lblStatusTextServer
            // 
            this.lblStatusTextServer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatusTextServer.AutoSize = true;
            this.lblStatusTextServer.Location = new System.Drawing.Point(28, 3);
            this.lblStatusTextServer.Name = "lblStatusTextServer";
            this.lblStatusTextServer.Size = new System.Drawing.Size(84, 13);
            this.lblStatusTextServer.TabIndex = 1;
            this.lblStatusTextServer.Text = "Status: Stopped";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.lblStatusIndicatorBms);
            this.flowLayoutPanel2.Controls.Add(this.lblStatusTextBms);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(157, 13);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(13, 3, 3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(67, 13);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // lblStatusIndicatorBms
            // 
            this.lblStatusIndicatorBms.AutoSize = true;
            this.lblStatusIndicatorBms.Location = new System.Drawing.Point(3, 0);
            this.lblStatusIndicatorBms.Name = "lblStatusIndicatorBms";
            this.lblStatusIndicatorBms.Size = new System.Drawing.Size(16, 13);
            this.lblStatusIndicatorBms.TabIndex = 0;
            this.lblStatusIndicatorBms.Text = "○";
            // 
            // lblStatusTextBms
            // 
            this.lblStatusTextBms.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatusTextBms.AutoSize = true;
            this.lblStatusTextBms.Location = new System.Drawing.Point(25, 0);
            this.lblStatusTextBms.Name = "lblStatusTextBms";
            this.lblStatusTextBms.Size = new System.Drawing.Size(39, 13);
            this.lblStatusTextBms.TabIndex = 1;
            this.lblStatusTextBms.Text = "BMS: -";
            // 
            // lblServerAddress
            // 
            this.lblServerAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblServerAddress.AutoSize = true;
            this.lblServerAddress.Location = new System.Drawing.Point(240, 13);
            this.lblServerAddress.Margin = new System.Windows.Forms.Padding(13, 0, 3, 0);
            this.lblServerAddress.Name = "lblServerAddress";
            this.lblServerAddress.Size = new System.Drawing.Size(54, 13);
            this.lblServerAddress.TabIndex = 2;
            this.lblServerAddress.Text = "Address: -";
            this.lblServerAddress.Visible = false;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.btnStartStop);
            this.flowLayoutPanel3.Controls.Add(this.btnSettings);
            this.flowLayoutPanel3.Controls.Add(this.btnQRCode);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(741, 4);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(174, 30);
            this.flowLayoutPanel3.TabIndex = 3;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(3, 3);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(95, 23);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start Server";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(104, 3);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(30, 23);
            this.btnSettings.TabIndex = 1;
            this.btnSettings.Text = "⚙️";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnQRCode
            // 
            this.btnQRCode.Location = new System.Drawing.Point(140, 3);
            this.btnQRCode.Name = "btnQRCode";
            this.btnQRCode.Size = new System.Drawing.Size(31, 23);
            this.btnQRCode.TabIndex = 2;
            this.btnQRCode.Text = "🌐";
            this.btnQRCode.UseVisualStyleBackColor = true;
            this.btnQRCode.Click += new System.EventHandler(this.btnQRCode_Click);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabKneeboards);
            this.tabControlMain.Controls.Add(this.tabLogs);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(3, 58);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(928, 500);
            this.tabControlMain.TabIndex = 1;
            // 
            // tabKneeboards
            // 
            this.tabKneeboards.Controls.Add(this.tlpKneeboardsMain);
            this.tabKneeboards.Location = new System.Drawing.Point(4, 22);
            this.tabKneeboards.Name = "tabKneeboards";
            this.tabKneeboards.Padding = new System.Windows.Forms.Padding(3);
            this.tabKneeboards.Size = new System.Drawing.Size(920, 474);
            this.tabKneeboards.TabIndex = 0;
            this.tabKneeboards.Text = "Kneeboard Management";
            this.tabKneeboards.UseVisualStyleBackColor = true;
            // 
            // tlpKneeboardsMain
            // 
            this.tlpKneeboardsMain.ColumnCount = 3;
            this.tlpKneeboardsMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpKneeboardsMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpKneeboardsMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpKneeboardsMain.Controls.Add(this.lblLeftKneeboard, 0, 0);
            this.tlpKneeboardsMain.Controls.Add(this.lblRightKneeboard, 2, 0);
            this.tlpKneeboardsMain.Controls.Add(this.lvKneeboardLeft, 0, 1);
            this.tlpKneeboardsMain.Controls.Add(this.lvKneeboardRight, 2, 1);
            this.tlpKneeboardsMain.Controls.Add(this.pnlLeftButtons, 0, 2);
            this.tlpKneeboardsMain.Controls.Add(this.pnlRightButtons, 2, 2);
            this.tlpKneeboardsMain.Controls.Add(this.pnlCenterButtons, 1, 1);
            this.tlpKneeboardsMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpKneeboardsMain.Location = new System.Drawing.Point(3, 3);
            this.tlpKneeboardsMain.Name = "tlpKneeboardsMain";
            this.tlpKneeboardsMain.RowCount = 3;
            this.tlpKneeboardsMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpKneeboardsMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpKneeboardsMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpKneeboardsMain.Size = new System.Drawing.Size(914, 468);
            this.tlpKneeboardsMain.TabIndex = 0;
            // 
            // lblLeftKneeboard
            // 
            this.lblLeftKneeboard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblLeftKneeboard.AutoSize = true;
            this.lblLeftKneeboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeftKneeboard.Location = new System.Drawing.Point(171, 6);
            this.lblLeftKneeboard.Name = "lblLeftKneeboard";
            this.lblLeftKneeboard.Size = new System.Drawing.Size(85, 13);
            this.lblLeftKneeboard.TabIndex = 0;
            this.lblLeftKneeboard.Text = "Left Kneeboard";
            // 
            // lblRightKneeboard
            // 
            this.lblRightKneeboard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblRightKneeboard.AutoSize = true;
            this.lblRightKneeboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRightKneeboard.Location = new System.Drawing.Point(649, 6);
            this.lblRightKneeboard.Name = "lblRightKneeboard";
            this.lblRightKneeboard.Size = new System.Drawing.Size(94, 13);
            this.lblRightKneeboard.TabIndex = 1;
            this.lblRightKneeboard.Text = "Right Kneeboard";
            // 
            // lvKneeboardLeft
            // 
            this.lvKneeboardLeft.CheckBoxes = true;
            this.lvKneeboardLeft.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chLeftPath});
            this.lvKneeboardLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvKneeboardLeft.FullRowSelect = true;
            this.lvKneeboardLeft.HideSelection = false;
            this.lvKneeboardLeft.Location = new System.Drawing.Point(3, 28);
            this.lvKneeboardLeft.MultiSelect = false;
            this.lvKneeboardLeft.Name = "lvKneeboardLeft";
            this.lvKneeboardLeft.Size = new System.Drawing.Size(421, 397);
            this.lvKneeboardLeft.TabIndex = 2;
            this.lvKneeboardLeft.UseCompatibleStateImageBehavior = false;
            this.lvKneeboardLeft.View = System.Windows.Forms.View.Details;
            // 
            // chLeftPath
            // 
            this.chLeftPath.Text = "File Path";
            this.chLeftPath.Width = 390;
            // 
            // lvKneeboardRight
            // 
            this.lvKneeboardRight.CheckBoxes = true;
            this.lvKneeboardRight.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chRightPath});
            this.lvKneeboardRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvKneeboardRight.FullRowSelect = true;
            this.lvKneeboardRight.HideSelection = false;
            this.lvKneeboardRight.Location = new System.Drawing.Point(490, 28);
            this.lvKneeboardRight.MultiSelect = false;
            this.lvKneeboardRight.Name = "lvKneeboardRight";
            this.lvKneeboardRight.Size = new System.Drawing.Size(421, 397);
            this.lvKneeboardRight.TabIndex = 3;
            this.lvKneeboardRight.UseCompatibleStateImageBehavior = false;
            this.lvKneeboardRight.View = System.Windows.Forms.View.Details;
            // 
            // chRightPath
            // 
            this.chRightPath.Text = "File Path";
            this.chRightPath.Width = 390;
            // 
            // pnlLeftButtons
            // 
            this.pnlLeftButtons.Controls.Add(this.btnAddLeft);
            this.pnlLeftButtons.Controls.Add(this.btnDeleteLeft);
            this.pnlLeftButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeftButtons.Location = new System.Drawing.Point(3, 431);
            this.pnlLeftButtons.Name = "pnlLeftButtons";
            this.pnlLeftButtons.Size = new System.Drawing.Size(421, 34);
            this.pnlLeftButtons.TabIndex = 4;
            // 
            // btnAddLeft
            // 
            this.btnAddLeft.Location = new System.Drawing.Point(3, 3);
            this.btnAddLeft.Name = "btnAddLeft";
            this.btnAddLeft.Size = new System.Drawing.Size(75, 23);
            this.btnAddLeft.TabIndex = 0;
            this.btnAddLeft.Text = "Add...";
            this.btnAddLeft.UseVisualStyleBackColor = true;
            // 
            // btnDeleteLeft
            // 
            this.btnDeleteLeft.Location = new System.Drawing.Point(84, 3);
            this.btnDeleteLeft.Name = "btnDeleteLeft";
            this.btnDeleteLeft.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteLeft.TabIndex = 1;
            this.btnDeleteLeft.Text = "Delete";
            this.btnDeleteLeft.UseVisualStyleBackColor = true;
            // 
            // pnlRightButtons
            // 
            this.pnlRightButtons.Controls.Add(this.btnAddRight);
            this.pnlRightButtons.Controls.Add(this.btnDeleteRight);
            this.pnlRightButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRightButtons.Location = new System.Drawing.Point(490, 431);
            this.pnlRightButtons.Name = "pnlRightButtons";
            this.pnlRightButtons.Size = new System.Drawing.Size(421, 34);
            this.pnlRightButtons.TabIndex = 5;
            // 
            // btnAddRight
            // 
            this.btnAddRight.Location = new System.Drawing.Point(3, 3);
            this.btnAddRight.Name = "btnAddRight";
            this.btnAddRight.Size = new System.Drawing.Size(75, 23);
            this.btnAddRight.TabIndex = 0;
            this.btnAddRight.Text = "Add...";
            this.btnAddRight.UseVisualStyleBackColor = true;
            // 
            // btnDeleteRight
            // 
            this.btnDeleteRight.Location = new System.Drawing.Point(84, 3);
            this.btnDeleteRight.Name = "btnDeleteRight";
            this.btnDeleteRight.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteRight.TabIndex = 1;
            this.btnDeleteRight.Text = "Delete";
            this.btnDeleteRight.UseVisualStyleBackColor = true;
            // 
            // pnlCenterButtons
            // 
            this.pnlCenterButtons.ColumnCount = 1;
            this.pnlCenterButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlCenterButtons.Controls.Add(this.btnMoveUp, 0, 0);
            this.pnlCenterButtons.Controls.Add(this.btnMoveToRight, 0, 1);
            this.pnlCenterButtons.Controls.Add(this.btnMoveToLeft, 0, 2);
            this.pnlCenterButtons.Controls.Add(this.btnMoveDown, 0, 3);
            this.pnlCenterButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenterButtons.Location = new System.Drawing.Point(430, 28);
            this.pnlCenterButtons.Name = "pnlCenterButtons";
            this.pnlCenterButtons.RowCount = 4;
            this.pnlCenterButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlCenterButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlCenterButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlCenterButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnlCenterButtons.Size = new System.Drawing.Size(54, 397);
            this.pnlCenterButtons.TabIndex = 6;
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnMoveUp.Location = new System.Drawing.Point(12, 65);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(30, 30);
            this.btnMoveUp.TabIndex = 0;
            this.btnMoveUp.Text = "▲";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            // 
            // btnMoveToRight
            // 
            this.btnMoveToRight.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnMoveToRight.Location = new System.Drawing.Point(12, 164);
            this.btnMoveToRight.Name = "btnMoveToRight";
            this.btnMoveToRight.Size = new System.Drawing.Size(30, 30);
            this.btnMoveToRight.TabIndex = 1;
            this.btnMoveToRight.Text = "▶";
            this.btnMoveToRight.UseVisualStyleBackColor = true;
            // 
            // btnMoveToLeft
            // 
            this.btnMoveToLeft.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnMoveToLeft.Location = new System.Drawing.Point(12, 201);
            this.btnMoveToLeft.Name = "btnMoveToLeft";
            this.btnMoveToLeft.Size = new System.Drawing.Size(30, 30);
            this.btnMoveToLeft.TabIndex = 2;
            this.btnMoveToLeft.Text = "◀";
            this.btnMoveToLeft.UseVisualStyleBackColor = true;
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnMoveDown.Location = new System.Drawing.Point(12, 300);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(30, 30);
            this.btnMoveDown.TabIndex = 3;
            this.btnMoveDown.Text = "▼";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            // 
            // tabLogs
            // 
            this.tabLogs.Controls.Add(this.txtLogs);
            this.tabLogs.Location = new System.Drawing.Point(4, 22);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.Padding = new System.Windows.Forms.Padding(3);
            this.tabLogs.Size = new System.Drawing.Size(920, 474);
            this.tabLogs.TabIndex = 1;
            this.tabLogs.Text = "Logs";
            this.tabLogs.UseVisualStyleBackColor = true;
            // 
            // txtLogs
            // 
            this.txtLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogs.Location = new System.Drawing.Point(3, 3);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogs.Size = new System.Drawing.Size(914, 468);
            this.txtLogs.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 561);
            this.Controls.Add(this.tlpMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "Form1";
            this.Text = "BMS Bridge Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tlpMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.tabKneeboards.ResumeLayout(false);
            this.tlpKneeboardsMain.ResumeLayout(false);
            this.tlpKneeboardsMain.PerformLayout();
            this.pnlLeftButtons.ResumeLayout(false);
            this.pnlRightButtons.ResumeLayout(false);
            this.pnlCenterButtons.ResumeLayout(false);
            this.tabLogs.ResumeLayout(false);
            this.tabLogs.PerformLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.cmsTrayMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "BMS Bridge";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // cmsTrayMenu
            // 
            this.cmsTrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.cmsTrayMenu.Name = "cmsTrayMenu";
            this.cmsTrayMenu.Size = new System.Drawing.Size(139, 48);
            this.cmsTrayMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.CmsTrayMenu_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem1.Text = "Show / Hide";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem2.Text = "Exit";
            this.cmsTrayMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        public System.Windows.Forms.Label lblStatusIndicatorServer;
        public System.Windows.Forms.Label lblStatusTextServer;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        public System.Windows.Forms.Label lblStatusIndicatorBms;
        public System.Windows.Forms.Label lblStatusTextBms;
        public System.Windows.Forms.Label lblServerAddress;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        public System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnQRCode;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabKneeboards;
        private System.Windows.Forms.TabPage tabLogs;
        public System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.TableLayoutPanel tlpKneeboardsMain;
        private System.Windows.Forms.Label lblLeftKneeboard;
        private System.Windows.Forms.Label lblRightKneeboard;
        private System.Windows.Forms.ListView lvKneeboardLeft;
        private System.Windows.Forms.ListView lvKneeboardRight;
        private System.Windows.Forms.FlowLayoutPanel pnlLeftButtons;
        private System.Windows.Forms.Button btnAddLeft;
        private System.Windows.Forms.Button btnDeleteLeft;
        private System.Windows.Forms.FlowLayoutPanel pnlRightButtons;
        private System.Windows.Forms.Button btnAddRight;
        private System.Windows.Forms.Button btnDeleteRight;
        private System.Windows.Forms.TableLayoutPanel pnlCenterButtons;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveToRight;
        private System.Windows.Forms.Button btnMoveToLeft;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.ColumnHeader chLeftPath;
        private System.Windows.Forms.ColumnHeader chRightPath;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip cmsTrayMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    }
}