using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Qonq.BlueSky.Model
{
    public class CreateSessionRequest
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}