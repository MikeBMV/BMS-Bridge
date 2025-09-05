namespace BMS_Bridge_Launcher
{
    partial class QrCodeForm
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
            this.picQrCode = new System.Windows.Forms.PictureBox();
            this.lblAddress = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picQrCode)).BeginInit();
            this.SuspendLayout();
            // 
            // picQrCode
            // 
            this.picQrCode.Location = new System.Drawing.Point(12, 12);
            this.picQrCode.Name = "picQrCode";
            this.picQrCode.Size = new System.Drawing.Size(300, 300);
            this.picQrCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQrCode.TabIndex = 0;
            this.picQrCode.TabStop = false;
            // 
            // lblAddress
            // 
            this.lblAddress.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddress.Location = new System.Drawing.Point(12, 324);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(300, 23);
            this.lblAddress.TabIndex = 1;
            this.lblAddress.Text = "http://127.0.0.1:8000";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // QrCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 356);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.picQrCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QrCodeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scan QR Code";
            ((System.ComponentModel.ISupportInitialize)(this.picQrCode)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox picQrCode;
        private System.Windows.Forms.Label lblAddress;
    }
}