using Newtonsoft.Json;

namespace ARC.Models.InternalModels
{
    public class DatabaseStats
    {
        [JsonProperty("totalDatabase")]
        public int TotalDatabase { get; set; }

        [JsonProperty("totalAlive")]
        public int TotalAlive { get; set; }

        [JsonProperty("totalPc")]
        public int TotalPc { get; set; }

        [JsonProperty("totalQuest")]
        public int TotalQuest { get; set; }

        [JsonProperty("totalPublic")]
        public int TotalPublic { get; set; }

        [JsonProperty("totalPrivate")]
        public int TotalPrivate { get; set; }

        [JsonProperty("totalLastDay")]
        public int TotalLastDay { get; set; }

        [JsonProperty("totalWeek")]
        public int TotalWeek { get; set; }

        [JsonProperty("totalMonth")]
        public int TotalMonth { get; set; }
    }
}