using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ARC.Models.ExternalModels
{
    public class VRChatCacheResultWorld
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("featured")]
        public bool Featured { get; set; }

        [JsonProperty("authorId")]
        public string AuthorId { get; set; }

        [JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [JsonProperty("capacity")]
        public int Capacity { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("releaseStatus")]
        public string ReleaseStatus { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("unityPackages")]
        public List<UnityPackageWorld> UnityPackages { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("publicationDate")]
        public DateTime? PublicationDate { get; set; }

        [JsonProperty("labsPublicationDate")]
        public DateTime? LabsPublicationDate { get; set; }

    }

    public class UnityPackageWorld
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("assetUrl")]
        public string AssetUrl { get; set; }

        [JsonProperty("unityVersion")]
        public string UnityVersion { get; set; }

        [JsonProperty("unitySortNumber")]
        public object UnitySortNumber { get; set; }

        [JsonProperty("assetVersion")]
        public int AssetVersion { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}