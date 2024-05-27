using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtusGroup.API.Models
{
    public class PVOmorfia
    {
        public PVOmorfiaHeader Header { get; set; }
        public List<PVOmorfiaBody> Body { get; set; }
    }
}