using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtusGroup.API.Models
{
    public class POOmorfia
    {
        public POOmorfiaHeader Header { get; set; }
        public List<POOmorfiaBody> Body { get; set; }
    }
}