using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Qonq.BlueSky.Model
{
    public class CreateRecordRequest
    {
        [JsonPropertyName("repo")]
        public string Repo { get; set; }
        [JsonPropertyName("collection")]
        public string Collection { get; set; }
        [JsonPropertyName("record")]
        public Record Record { get; set; }
    }
}
