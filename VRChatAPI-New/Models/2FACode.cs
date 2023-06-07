using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChatAPI_New.Models
{
    public class _2FACode
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        public _2FACode(string code)
        {
            Code = code;
        }
    }
}
