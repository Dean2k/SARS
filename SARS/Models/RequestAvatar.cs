using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARS.Models
{
    public class RequestAvatar
    {
        public string AvatarId { get; set; }
        public Guid Key { get; set; }
        public bool Quest { get; set; }
    }
}
