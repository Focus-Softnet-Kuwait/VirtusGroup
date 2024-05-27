using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtusGroup.API.Models
{
    public class JVOmorfiaBody
    {
        //public string BusinessUnitId { get; set; }
        public string PaymentType { get; set; }
        //public string CrAccountId {  get; set; }
        public double Amount { get; set; }
        public string Reference { get; set; }
        public string Remark { get; set; }


    }
}