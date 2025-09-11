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
                    form.tslStatusServer.ForeColor = Color.Green;
                    form.tslStatusServer.Text = "Server: Running";
                    break;
                case "WARNING":
                    form.tslStatusServer.ForeColor = Color.Orange;
                    form.tslStatusServer.Text = "Server: Warning";
                    break;
                case "ERROR":
                    form.tslStatusServer.ForeColor = Color.Red;
                    form.tslStatusServer.Text = "Server: Error";
                    break;
                case "STARTING":
                    form.tslStatusServer.ForeColor = Color.DodgerBlue;
                    form.tslStatusServer.Text = "Server: Starting...";
                    break;
                case "STOPPED":
                default:
                    form.tslStatusServer.ForeColor = SystemColors.ControlText;
                    form.tslStatusServer.Text = "Server: Stopped";
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
                    form.tslStatusBms.ForeColor = Color.Green;
                    form.tslStatusBms.Text = "BMS: Connected";
                    break;
                case "NOT_CONNECTED":
                    form.tslStatusBms.ForeColor = SystemColors.ControlText;
                    form.tslStatusBms.Text = "BMS: Not Found";
                    break;
                default:
                    form.tslStatusBms.ForeColor = SystemColors.GrayText;
                    form.tslStatusBms.Text = "BMS: -";
                    break;
            }
        }

        private void UpdateControlsForServerState(ServerHealthState state)
        {
            if (state.IsRunning())
            {
                form.tsbStartStop.Text = "■ Stop";
                form.tsbStartStop.ForeColor = Color.Red;

                if (!string.IsNullOrEmpty(state.server_address))
                {
                    form.tslServerAddress.Text = state.server_address;
                    form.tslServerAddress.Visible = true;
                }
            }
            else
            {
                form.tsbStartStop.Text = "▶ Start";
                form.tsbStartStop.ForeColor = Color.Green;
                form.tslServerAddress.Visible = false;
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
        
        public void ClearLog()
        {
            if (form.txtLogs.InvokeRequired)
            {
                form.txtLogs.Invoke(new Action(ClearLog));
                return;
            }
            form.txtLogs.Clear();
        }
    }
}