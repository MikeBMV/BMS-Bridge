using BMS_Bridge_Launcher;
using QRCoder;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BMS_Bridge_Launcher
{
    public partial class QrCodeForm : Form
    {
        public QrCodeForm(string serverAddress)
        {
            InitializeComponent();
            GenerateAndDisplayQrCode(serverAddress);
        }

        private void GenerateAndDisplayQrCode(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                lblAddress.Text = "Server address not available";
                return;
            }

            lblAddress.Text = address;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            QRCodeData qrCodeData = qrGenerator.CreateQrCode(address, QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            picQrCode.Image = qrCodeImage;
        }
    }
}