using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARS.Models
{

    public class VRChatCacheResult
    {
        public string assetUrl { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public DateTime created_at { get; set; }
        public string description { get; set; }
        public bool featured { get; set; }
        public string id { get; set; }
        public string imageUrl { get; set; }
        public string name { get; set; }
        public string releaseStatus { get; set; }
        public List<string> tags { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string unityPackageUrl { get; set; }
        public List<UnityPackage> unityPackages { get; set; }
        public DateTime updated_at { get; set; }
        public int version { get; set; }
    }

    public class UnityPackage
    {
        public string assetUrl { get; set; }
        public int assetVersion { get; set; }
        public DateTime created_at { get; set; }
        public string id { get; set; }
        public string platform { get; set; }
        public string pluginUrl { get; set; }
        public long unitySortNumber { get; set; }
        public string unityVersion { get; set; }
    }


}
