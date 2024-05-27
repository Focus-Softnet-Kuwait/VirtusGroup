using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtusGroup.API.Models
{
    public class POOmorfiaHeader
    {
        public string DocNo { get; set; }
        public string Date { get; set; }
        public string VendorACName { get; set; }
        public string CurrencyID { get; set; }
        public string ExchangeRate { get; set; }
        public int EntityID { get; set; }
        public string BusinessUnitID { get; set; }
        public string Narration { get; set; }
    }
}