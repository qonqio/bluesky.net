using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Qonq.BlueSky.Model
{
    public class DidResponse
    {
        [JsonPropertyName("did")]
        public string Did { get; set; }
    }
}
