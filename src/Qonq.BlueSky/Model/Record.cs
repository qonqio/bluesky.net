using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Qonq.BlueSky.Model
{
    public class Record
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("$type")]
        public string Type { get; set; }
    }
}