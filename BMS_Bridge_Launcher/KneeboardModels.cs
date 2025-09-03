using System.Collections.Generic;
using Newtonsoft.Json;

namespace BMS_Bridge_Launcher
{
    /// <summary>
    /// Represents a single item in one of the kneeboard lists.
    /// This will be stored in the ListView items' Tag property.
    /// </summary>
    public class KneeboardItem
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        // We won't use 'type' for now as the launcher will manage files,
        // but it's good to have for future server-side logic.
        [JsonProperty("type")]
        public string Type { get; set; } = "user_file";
    }

    /// <summary>
    /// Represents the 'kneeboards' section in the settings.json file.
    /// </summary>
    public class KneeboardConfig
    {
        [JsonProperty("left")]
        public List<KneeboardItem> Left { get; set; } = new List<KneeboardItem>();

        [JsonProperty("right")]
        public List<KneeboardItem> Right { get; set; } = new List<KneeboardItem>();
    }

    /// <summary>
    /// Represents the entire settings.json file structure.
    /// We only define the parts we need to interact with.
    /// </summary>
    public class AppSettings
    {
        [JsonProperty("kneeboards")]
        public KneeboardConfig Kneeboards { get; set; } = new KneeboardConfig();

        // This allows Newtonsoft.Json to preserve other settings in the file
        // that we are not directly using in the launcher.
        [JsonExtensionData]
        public IDictionary<string, object> OtherSettings { get; set; }
    }
}