using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Qonq.BlueSky.Model
{
    public class ListRecordsResponse
    {
        [JsonPropertyName("records")]
        public List<Record> Records { get; set; }
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }
}
