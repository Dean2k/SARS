using System;
using System.Collections.Generic;

namespace SARS.Models
{

    public class AvatarDetails
    {
        public string avatarId { get; set; }
        public string avatarName { get; set; }
        public string avatarDescription { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string imageUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string questAssetUrl { get; set; }
        public string pcAssetUrl { get; set; }
        public string releaseStatus { get; set; }
        public string unityVersion { get; set; }
        public DateTime recordCreated { get; set; }
    }

    public class AvatarDetailsSend
    {
        public string avatarId { get; set; }
        public string avatarName { get; set; }
        public string avatarDescription { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string imageUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string questAssetUrl { get; set; }
        public string pcAssetUrl { get; set; }
        public string releaseStatus { get; set; }
        public string unityVersion { get; set; }
        public string tags { get; set; }
        public DateTime recordCreated { get; set; }
    }


    public class Avatar
    {
        public AvatarDetails avatar { get; set; }
        public List<string> tags { get; set; }
    }

    public class AvatarResponse
    {
        public List<Avatar> avatars { get; set; }
        public bool authorized { get; set; }
        public bool banned { get; set; }
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