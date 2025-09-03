// ServerManager.cs - Extracted server management logic
using System;
using System.Diagnostics;
using System.IO;

namespace BMS_Bridge_Launcher
{
    public class ServerManager : IDisposable
    {
        private Process serverProcess;
        private readonly string serverExecutablePath;
        private readonly string serverWorkingDirectory;

        private readonly string pidFilePath;
        private FileSystemWatcher logWatcher;
        private long lastLogFileSize = 0;

        public event EventHandler<string> OutputReceived;
        public event EventHandler<string> ErrorReceived;
        public event EventHandler ServerExited;

        public bool IsRunning => serverProcess != null && !serverProcess.HasExited;

        public ServerManager()
        {
            string launcherPath = AppDomain.CurrentDomain.BaseDirectory;
            serverWorkingDirectory = Path.Combine(launcherPath, "Server");
            serverExecutablePath = Path.Combine(serverWorkingDirectory, "BMS_Bridge_Server.exe");
            pidFilePath = Path.Combine(serverWorkingDirectory, "server.pid");
        }

        public void Start()
        {
            if (IsRunning)
                throw new InvalidOperationException("Server is already running");

            if (!File.Exists(serverExecutablePath))
                throw new FileNotFoundException($"Server executable not found: {serverExecutablePath}");

            try
            {
                OnOutputReceived("--- Starting server as an independent process ---");

                serverProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = serverExecutablePath,
                        WorkingDirectory = serverWorkingDirectory,
                        UseShellExecute = true,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                if (!serverProcess.Start())
                    throw new InvalidOperationException("Failed to start server process");

                File.WriteAllText(pidFilePath, serverProcess.Id.ToString());
                InitializeLogWatcher();

                OnOutputReceived("Server process started successfully. PID: " + serverProcess.Id);
            }
            catch (Exception ex)
            {
                OnErrorReceived($"Failed to start server: {ex.Message}");
                serverProcess = null;
                throw;
            }
        }

        public void Stop()
        {
            logWatcher?.Dispose();
            logWatcher = null;

            OnOutputReceived("--- Stopping server process via PID file ---");

            try
            {
                if (File.Exists(pidFilePath))
                {
                    string pidText = File.ReadAllText(pidFilePath);
                    if (int.TryParse(pidText, out int pid))
                    {
                        Process serverToKill = Process.GetProcessById(pid);
                        
                        KillProcessTree(serverToKill);
                        OnOutputReceived($"Termination command sent to process with PID {pid}.");
                    }
                    File.Delete(pidFilePath);
                }
                else
                {
                    KillProcessByName();
                }
            }
            catch (ArgumentException)
            {
                OnOutputReceived("Server process with stored PID not found. Already stopped?");
                if (File.Exists(pidFilePath)) File.Delete(pidFilePath);
            }
            catch (Exception ex)
            {
                OnErrorReceived($"Error stopping server: {ex.Message}");
            }
            finally
            {
                serverProcess = null;
            }
        }

        protected virtual void OnOutputReceived(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
                OutputReceived?.Invoke(this, data);
        }

        protected virtual void OnErrorReceived(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
                ErrorReceived?.Invoke(this, data);
        }

        protected virtual void OnServerExited()
        {
            OnOutputReceived("Server process has exited");
            serverProcess = null;
            ServerExited?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeLogWatcher()
        {
            string logFilePath = Path.Combine(serverWorkingDirectory, "bms_bridge.log");

            // Сначала читаем все, что уже есть в логе
            ReadNewLogEntries(logFilePath);

            logWatcher = new FileSystemWatcher
            {
                Path = serverWorkingDirectory,
                Filter = "bms_bridge.log",
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            logWatcher.Changed += (s, e) => ReadNewLogEntries(e.FullPath);
            logWatcher.EnableRaisingEvents = true;
        }

        private void ReadNewLogEntries(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return;

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    if (fs.Length < lastLogFileSize) lastLogFileSize = 0;

                    sr.BaseStream.Seek(lastLogFileSize, SeekOrigin.Begin);
                    string newLines = sr.ReadToEnd();
                    if (!string.IsNullOrEmpty(newLines))
                    {
                        OnOutputReceived(newLines.Trim());
                    }
                    lastLogFileSize = fs.Position;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error reading log file: " + ex.Message);
            }
        }

        private void KillProcessTree(Process root)
        {
            var taskkill = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "taskkill.exe",
                    Arguments = $"/F /T /PID {root.Id}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            taskkill.Start();
            taskkill.WaitForExit();
        }

        private void KillProcessByName()
        {
            OnOutputReceived("PID file not found. Attempting to stop by image name...");
            var taskkill = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "taskkill.exe",
                    Arguments = "/F /IM BMS_Bridge_Server.exe /T",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            taskkill.Start();
            taskkill.WaitForExit();
        }

        public void Dispose()
        {
            Stop();
            serverProcess?.Dispose();
        }
    }
}