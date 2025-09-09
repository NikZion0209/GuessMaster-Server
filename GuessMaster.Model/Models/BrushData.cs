using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GuessMaster.Model.Models
{
    public class BrushData
    {
        [JsonPropertyName("fromX")]
        public float FromX { get; set; }
        [JsonPropertyName("fromY")]
        public float FromY { get; set; }
        [JsonPropertyName("toX")]
        public float ToX { get; set; }
        [JsonPropertyName("toY")]
        public float ToY { get; set; }
        [JsonPropertyName("colour")]
        public string Colour { get; set; } = string.Empty;
        [JsonPropertyName("size")]
        public float Size { get; set; }
        [JsonPropertyName("tool")]
        public int Tool { get; set; }
        [JsonPropertyName("orientationLandscape")]
        public bool OrientationLandscape { get; set; }
    }
}
