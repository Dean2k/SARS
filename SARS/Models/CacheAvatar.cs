using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARS.Models
{
    public class CacheAvatar
    {
        public string FileLocation { get; set; }
        public string AvatarId { get; set; }
        public DateTime AvatarDetected { get; set;}
    }
}
