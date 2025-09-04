// HealthMonitor.cs - Extracted health monitoring logic
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BMS_Bridge_Launcher
{
    public class HealthMonitor : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly Timer pollTimer;
        private readonly string healthEndpoint;
        
        public event EventHandler<ServerHealthState> HealthUpdated;
        public event EventHandler<string> ErrorOccurred;
        
        public ServerHealthState LastKnownState { get; private set; }
        public bool IsMonitoring { get; private set; }
        
        public HealthMonitor(string baseUrl = "http://localhost:8000", int pollIntervalMs = 2000)
        {
            httpClient = new HttpClient();
            healthEndpoint = $"{baseUrl}/api/health";
            
            pollTimer = new Timer();
            pollTimer.Interval = pollIntervalMs;
            pollTimer.Tick += async (s, e) => await PollHealthAsync();
            
            LastKnownState = new ServerHealthState { server_status = "STOPPED" };
        }
        
        public void StartMonitoring()
        {
            if (IsMonitoring)
                return;
                
            IsMonitoring = true;
            pollTimer.Start();
        }
        
        public void StopMonitoring()
        {
            if (!IsMonitoring)
                return;
                
            IsMonitoring = false;
            pollTimer.Stop();
        }
        
        private async Task PollHealthAsync()
        {
            try
            {
                var response = await httpClient.GetAsync(healthEndpoint);
                
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var healthState = JsonConvert.DeserializeObject<ServerHealthState>(json);
                    
                    LastKnownState = healthState;
                    OnHealthUpdated(healthState);
                }
                else
                {
                    var errorState = new ServerHealthState 
                    { 
                        server_status = "ERROR", 
                        server_message = $"API returned status {response.StatusCode}" 
                    };
                    
                    LastKnownState = errorState;
                    OnHealthUpdated(errorState);
                }
            }
            catch (HttpRequestException ex)
            {
                // Connection failed - this is expected during startup
                OnErrorOccurred($"Health check failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Unexpected error during health check: {ex.Message}");
            }
        }
        
        protected virtual void OnHealthUpdated(ServerHealthState state)
        {
            HealthUpdated?.Invoke(this, state);
        }
        
        protected virtual void OnErrorOccurred(string error)
        {
            ErrorOccurred?.Invoke(this, error);
        }
        /// <summary>
        /// Allows to manually set the health state from the outside.
        /// This is useful for immediately reflecting a state change (like a manual shutdown)
        /// without waiting for a poll cycle.
        /// </summary>
        public void ManuallySetState(ServerHealthState newState)
        {
            LastKnownState = newState;
            // We also fire the event to ensure the UI updates through the standard pipeline.
            OnHealthUpdated(newState);
        }
        public void Dispose()
        {
            StopMonitoring();
            pollTimer?.Dispose();
            httpClient?.Dispose();
        }
    }
}