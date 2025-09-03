// UIController.cs - Extracted UI management logic
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BMS_Bridge_Launcher
{
    public class UIController
    {
        private readonly Form1 form;
        
        public UIController(Form1 form)
        {
            this.form = form ?? throw new ArgumentNullException(nameof(form));
        }
        
        public void UpdateServerStatus(ServerHealthState state)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new Action(() => UpdateServerStatus(state)));
                return;
            }
            
            // Update server status indicator and text
            switch (state.server_status)
            {
                case "RUNNING":
                    form.lblStatusIndicatorServer.ForeColor = Color.Green;
                    form.lblStatusTextServer.Text = "Status: Running";
                    break;
                case "WARNING":
                    form.lblStatusIndicatorServer.ForeColor = Color.Orange;
                    form.lblStatusTextServer.Text = "Status: Warning";
                    break;
                case "ERROR":
                    form.lblStatusIndicatorServer.ForeColor = Color.Red;
                    form.lblStatusTextServer.Text = "Status: Error";
                    break;
                case "STARTING":
                    form.lblStatusIndicatorServer.ForeColor = Color.DodgerBlue;
                    form.lblStatusTextServer.Text = "Status: Starting...";
                    break;
                case "STOPPED":
                default:
                    form.lblStatusIndicatorServer.ForeColor = Color.Gray;
                    form.lblStatusTextServer.Text = "Status: Stopped";
                    break;
            }
            
            // Update BMS status
            UpdateBmsStatus(state.bms_status);
            
            // Update server address and control button
            UpdateControlsForServerState(state);
        }
        
        private void UpdateBmsStatus(string bmsStatus)
        {
            switch (bmsStatus)
            {
                case "CONNECTED":
                    form.lblStatusIndicatorBms.ForeColor = Color.Green;
                    form.lblStatusIndicatorBms.Text = "●";
                    form.lblStatusTextBms.Text = "BMS: Connected";
                    break;
                case "NOT_CONNECTED":
                    form.lblStatusIndicatorBms.ForeColor = Color.Gray;
                    form.lblStatusIndicatorBms.Text = "○";
                    form.lblStatusTextBms.Text = "BMS: Not Found";
                    break;
                default:
                    form.lblStatusIndicatorBms.ForeColor = Color.Gray;
                    form.lblStatusIndicatorBms.Text = "○";
                    form.lblStatusTextBms.Text = "BMS: -";
                    break;
            }
        }
        
        private void UpdateControlsForServerState(ServerHealthState state)
        {
            if (state.IsRunning())
            {
                form.btnStartStop.Text = "Stop Server";
                
                if (!string.IsNullOrEmpty(state.server_address))
                {
                    form.lblServerAddress.Text = $"Address: {state.server_address}";
                    form.lblServerAddress.Visible = true;
                }
            }
            else
            {
                form.btnStartStop.Text = "Start Server";
                form.lblServerAddress.Visible = false;
            }
        }
        
        public void AppendLog(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
                
            if (form.txtLogs.InvokeRequired)
            {
                form.txtLogs.Invoke(new Action<string>(AppendLog), message);
                return;
            }
            
            form.txtLogs.AppendText(message + Environment.NewLine);
            form.txtLogs.ScrollToCaret();
        }
        
        public void ShowError(string message, string title = "Error")
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new Action(() => ShowError(message, title)));
                return;
            }
            
            MessageBox.Show(form, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        public void ShowInfo(string message, string title = "Information")
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new Action(() => ShowInfo(message, title)));
                return;
            }
            
            MessageBox.Show(form, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}