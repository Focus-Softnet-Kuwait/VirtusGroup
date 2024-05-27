using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtusGroup.API.Models
{
    public class JVOmorfia
    {
        public JVOmorfiaHeader Header { get; set; }
        public List<JVOmorfiaBody> Body { get; set; }   
    }
}