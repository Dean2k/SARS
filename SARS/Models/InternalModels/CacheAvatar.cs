using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARC.Models.InternalModels
{
    public class CacheAvatar
    {
        public string FileLocation { get; set; }
        public string AvatarId { get; set; }
        public long FileSize { get; set; }
        public DateTime AvatarDetected { get; set; }
    }
}