using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string SendTo { get; set; }
        public string MessageText { get; set; }
    }
}
