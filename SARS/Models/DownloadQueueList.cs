using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARS.Models
{
    public class DownloadQueueList
    {
        public string AvatarId { get; set; }
        public bool Downloaded { get; set; }
        public bool Quest { get; set; }
        public bool Failed { get; set; }
    }
}
