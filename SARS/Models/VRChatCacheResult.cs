using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARS.Models
{

    public class VRChatCacheResult
    {
        [JsonProperty("assetUrl")]
        public string AssetUrl { get; set; }

        [JsonProperty("authorId")]
        public string AuthorId { get; set; }

        [JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("featured")]
        public bool Featured { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("releaseStatus")]
        public string ReleaseStatus { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; }

        [JsonProperty("unityPackageUrl")]
        public string UnityPackageUrl { get; set; }

        [JsonProperty("unityPackages")]
        public List<UnityPackage> UnityPackages { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }

    public class UnityPackage
    {
        [JsonProperty("assetUrl")]
        public string AssetUrl { get; set; }

        [JsonProperty("assetVersion")]
        public int AssetVersion { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("pluginUrl")]
        public string PluginUrl { get; set; }

        [JsonProperty("unitySortNumber")]
        public long UnitySortNumber { get; set; }

        [JsonProperty("unityVersion")]
        public string UnityVersion { get; set; }
    }


}
