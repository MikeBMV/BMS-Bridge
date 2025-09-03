using System; // Можно добавить для порядка

namespace BMS_Bridge_Launcher
{
    /// <summary>
    /// A helper class to deserialize the JSON response from the server's health endpoint.
    /// Property names must match the JSON keys exactly.
    /// </summary>
    public class ServerHealthState
    {
        public string server_status { get; set; } = "STOPPED";
        public string bms_status { get; set; }
        public string server_address { get; set; }
        public string server_message { get; set; }

        /// <summary>
        /// Determines if the server is in any running state (including warning or error states).
        /// </summary>
        public bool IsRunning()
        {
            return server_status == "RUNNING" || server_status == "WARNING" || server_status == "ERROR" || server_status == "STARTING";
        }
    }
}