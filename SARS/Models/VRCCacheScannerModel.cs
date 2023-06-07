using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARS.Models
{
    public class VRCCacheScannerModel
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
