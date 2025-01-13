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
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        [JsonPropertyName("cid")]
        public string Cid { get; set; }
        [JsonPropertyName("value")]
        public RecordValue Value { get; set; }
    }
}
