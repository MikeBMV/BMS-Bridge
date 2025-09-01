// --- Form1.Designer.cs - TRANSLATED VERSION ---

namespace BMS_Bridge_Launcher
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            this.tabLogs = new System.Windows.Forms.TabPage();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsTrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.tabControlMain.SuspendLayout();
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
            this.tlpHeader.ColumnCount = 5;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHeader.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tlpHeader.Controls.Add(this.flowLayoutPanel2, 1, 0);
            this.tlpHeader.Controls.Add(this.lblServerAddress, 2, 0);
            this.tlpHeader.Controls.Add(this.flowLayoutPanel3, 4, 0);
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
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.lblStatusIndicatorServer);
            this.flowLayoutPanel1.Controls.Add(this.lblStatusTextServer);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(138, 20);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // lblStatusIndicatorServer
            // 
            this.lblStatusIndicatorServer.AutoSize = true;
            this.lblStatusIndicatorServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.lblStatusIndicatorServer.Location = new System.Drawing.Point(3, 0);
            this.lblStatusIndicatorServer.Name = "lblStatusIndicatorServer";
            this.lblStatusIndicatorServer.Size = new System.Drawing.Size(19, 20);
            this.lblStatusIndicatorServer.TabIndex = 0;
            this.lblStatusIndicatorServer.Text = "●";
            // 
            // lblStatusTextServer
            // 
            this.lblStatusTextServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatusTextServer.AutoSize = true;
            this.lblStatusTextServer.Location = new System.Drawing.Point(30, 6);
            this.lblStatusTextServer.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.lblStatusTextServer.Name = "lblStatusTextServer";
            this.lblStatusTextServer.Size = new System.Drawing.Size(84, 13);
            this.lblStatusTextServer.TabIndex = 1;
            this.lblStatusTextServer.Text = "Status: Stopped";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.lblStatusIndicatorBms);
            this.flowLayoutPanel2.Controls.Add(this.lblStatusTextBms);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(147, 16);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(67, 13);
            this.flowLayoutPanel2.TabIndex = 1;
            this.flowLayoutPanel2.WrapContents = false;
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
            this.lblStatusTextBms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatusTextBms.AutoSize = true;
            this.lblStatusTextBms.Location = new System.Drawing.Point(25, 0);
            this.lblStatusTextBms.Name = "lblStatusTextBms";
            this.lblStatusTextBms.Size = new System.Drawing.Size(39, 13);
            this.lblStatusTextBms.TabIndex = 1;
            this.lblStatusTextBms.Text = "BMS: -";
            // 
            // lblServerAddress
            // 
            this.lblServerAddress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblServerAddress.AutoSize = true;
            this.lblServerAddress.Location = new System.Drawing.Point(220, 16);
            this.lblServerAddress.Name = "lblServerAddress";
            this.lblServerAddress.Size = new System.Drawing.Size(54, 13);
            this.lblServerAddress.TabIndex = 2;
            this.lblServerAddress.Text = "Address: -";
            this.lblServerAddress.Visible = false;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.btnStartStop);
            this.flowLayoutPanel3.Controls.Add(this.btnSettings);
            this.flowLayoutPanel3.Controls.Add(this.btnQRCode);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(762, 4);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(153, 36);
            this.flowLayoutPanel3.TabIndex = 3;
            this.flowLayoutPanel3.WrapContents = false;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStartStop.Location = new System.Drawing.Point(75, 6);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(75, 23);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start Server";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSettings.Location = new System.Drawing.Point(39, 3);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(30, 30);
            this.btnSettings.TabIndex = 1;
            this.btnSettings.Text = "⚙️";
            this.btnSettings.UseVisualStyleBackColor = true;
            // 
            // btnQRCode
            // 
            this.btnQRCode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnQRCode.Location = new System.Drawing.Point(3, 3);
            this.btnQRCode.Name = "btnQRCode";
            this.btnQRCode.Size = new System.Drawing.Size(30, 30);
            this.btnQRCode.TabIndex = 2;
            this.btnQRCode.Text = "🌐";
            this.btnQRCode.UseVisualStyleBackColor = true;
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
            this.tabKneeboards.Location = new System.Drawing.Point(4, 22);
            this.tabKneeboards.Name = "tabKneeboards";
            this.tabKneeboards.Padding = new System.Windows.Forms.Padding(3);
            this.tabKneeboards.Size = new System.Drawing.Size(920, 474);
            this.tabKneeboards.TabIndex = 0;
            this.tabKneeboards.Text = "Kneeboard Management";
            this.tabKneeboards.UseVisualStyleBackColor = true;
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
            this.cmsTrayMenu.Size = new System.Drawing.Size(181, 70);
            this.cmsTrayMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.CmsTrayMenu_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem1.Text = "Show / Hide";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem2.Text = "Exit";
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
            this.tabLogs.ResumeLayout(false);
            this.tabLogs.PerformLayout();
            this.cmsTrayMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblStatusIndicatorServer;
        private System.Windows.Forms.Label lblStatusTextServer;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label lblStatusIndicatorBms;
        private System.Windows.Forms.Label lblStatusTextBms;
        private System.Windows.Forms.Label lblServerAddress;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnQRCode;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabKneeboards;
        private System.Windows.Forms.TabPage tabLogs;
        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip cmsTrayMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    }
}