namespace ARC.Models.InternalModels
{
    public class VrcCacheScannerModel
    {
        public string ScanDate { get; set; }
        public string FileCreationDate { get; set; }
        public string FilePath { get; set; }
        public string FileHash { get; set; }
        public string ContentType { get; set; }
        public string BundleName { get; set; }
        public string Id { get; set; }
    }
}