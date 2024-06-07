using System;
using System.Net;

namespace ARC.Models.InternalModels
{
    public class Config
    {
        public string UserId { get; set; }
        public string AuthKey { get; set; }
        public string TwoFactor { get; set; }
        public string ApiKey { get; set; }
        public string[] CookiesValues { get; set; }
        public string PreSelectedAvatarLocation { get; set; }
        public bool PreSelectedAvatarLocationChecked { get; set; }
        public string PreSelectedWorldLocation { get; set; }
        public bool PreSelectedWorldLocationChecked { get; set; }
        public string UnityLocation2019 { get; set; }
        public string UnityLocation2022L { get; set; }
        public string ClientVersion { get; set; }
        public string UnityVersion { get; set; }
        public DateTime ClientVersionLastUpdated { get; set; }
        public bool LightMode { get; set; }
        public string ThemeColor { get; set; }
        public string HotSwapName2022L { get; set; }
        public string HotSwapName2019 { get; set; }
        public string MacAddress { get; set; }
        public int ViewerVersion { get; set; }
        public string UserAgent { get; set; }
        public bool ReplaceUnityVersion { get; set; }
        public bool IndexAdded { get; set; }
        public int AvatarsInLocalDatabase { get; set; }
        public int AvatarsLoggedToApi { get; set; }
        public string DocumentLocation { get; set; }
    }
}