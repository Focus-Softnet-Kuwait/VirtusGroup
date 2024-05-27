using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtusGroup.API.Models
{
    public class HashData
    {
        public string url { get; set; }
        public List<Hashtable> data { get; set; }
        public int result { get; set; }
        public string message { get; set; }
    }
    public class LstDetails
    {
        public int FieldId { get; set; }
        public double Input { get; set; }
        public string FieldName { get; set; }
    }
}