using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace VRChatAPI_New.Models
{
    public class FileInfo
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("sizeInBytes")]
        public int SizeInBytes { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("uploadId")]
        public string UploadId { get; set; }
    }

    public class VersionList
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("file")]
        public FileInfo File { get; set; }

        [JsonProperty("delta")]
        public FileInfo Delta { get; set; }

        [JsonProperty("signature")]
        public FileInfo Signature { get; set; }
    }

    public class VRChatFileInformation
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ownerId")]
        public string OwnerId { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("tags")]
        public List<object> Tags { get; set; }

        [JsonProperty("versions")]
        public List<VersionList> Versions { get; set; }
    }
}