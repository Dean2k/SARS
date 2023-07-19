using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SARS.Models
{

    public class AvatarDetails
    {
        [JsonProperty("avatarId")]
        public string AvatarId { get; set; }

        [JsonProperty("avatarName")]
        public string AvatarName { get; set; }

        [JsonProperty("avatarDescription")]
        public string AvatarDescription { get; set; }

        [JsonProperty("authorId")]
        public string AuthorId { get; set; }

        [JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("questAssetUrl")]
        public string QuestAssetUrl { get; set; }

        [JsonProperty("pcAssetUrl")]
        public string PcAssetUrl { get; set; }

        [JsonProperty("releaseStatus")]
        public string ReleaseStatus { get; set; }

        [JsonProperty("unityVersion")]
        public string UnityVersion { get; set; }

        [JsonProperty("recordCreated")]
        public DateTime RecordCreated { get; set; }
    }

    public class AvatarDetailsSend
    {
        [JsonProperty("avatarId")]
        public string AvatarId { get; set; }

        [JsonProperty("avatarName")]
        public string AvatarName { get; set; }

        [JsonProperty("avatarDescription")]
        public string AvatarDescription { get; set; }

        [JsonProperty("authorId")]
        public string AuthorId { get; set; }

        [JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("questAssetUrl")]
        public string QuestAssetUrl { get; set; }

        [JsonProperty("pcAssetUrl")]
        public string PcAssetUrl { get; set; }

        [JsonProperty("releaseStatus")]
        public string ReleaseStatus { get; set; }

        [JsonProperty("unityVersion")]
        public string UnityVersion { get; set; }

        [JsonProperty("tags")]
        public string Tags{ get; set; }

        [JsonProperty("recordCreated")]
        public DateTime RecordCreated { get; set; }
    }


    public class AvatarModel
    {
        [JsonProperty("avatar")]
        public AvatarDetails Avatar { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
    }

    public class AvatarResponse
    {
        [JsonProperty("avatars")]
        public List<AvatarModel> Avatars { get; set; }

        [JsonProperty("authorized")]
        public bool Authorized { get; set; }

        [JsonProperty("banned")]
        public bool Banned { get; set; }
    }


    public class AvatarSearch
    {
        public string AvatarId { get; set; }
        public string AuthorId { get; set; }
        public string AvatarName { get; set; }
        public string AuthorName { get; set; }
        public List<Tag> Tags { get; set; }
        public int Amount { get; set; }
        public bool ContainsSearch { get; set; }
        public bool PublicAvatars { get; set; }
        public bool PrivateAvatars { get; set; }
        public bool PcAvatars { get; set; }
        public bool QuestAvatars { get; set; }
        public string Key { get; set; }
        public bool DebugMode { get; set; }
    }

    public class Tag
    {
        public string tag { get; set; }
    }

}