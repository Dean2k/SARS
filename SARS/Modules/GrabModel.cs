namespace ARC.Modules
{
    public class GrabModel
    {
        public int? entry { get; set; }
        public string Id { get; set; }
        public string DateAdded { get; set; }
        public string LastChecked { get; set; }
        public string AvatarName { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Description { get; set; }
        public string AssetUrl { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public int? IsPrivate { get; set; }
        public int? SupportedPlatforms { get; set; }
        public int? Version { get; set; }
        public int? IsDeleted { get; set; }
        public string UnityVersion { get; set; }
    }
}