using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BMS_Bridge_Launcher
{
    internal static class Program
    {
        private const string AppMutexName = "BMS_Bridge_Launcher_{E6FE03B1-276E-4829-87C7-2150E7E14EEE}";

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int RegisterWindowMessage(string lpString);
        
        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME_{E6FE03B1-276E-4829-87C7-2150E7E14EEE}");

        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (Mutex mutex = new Mutex(true, AppMutexName, out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else
                {
                    PostMessage(HWND_BROADCAST, WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }
    }
}