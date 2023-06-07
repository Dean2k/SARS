using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChatAPI_New.Models
{
    public class TwoFactorAuth
    {
        [JsonProperty("requiresTwoFactorAuth")]
        public List<string> RequiresTwoFactorAuth { get; set; }
    }
}
