using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.Models._20_Pay.OPay
{
    public class ODomainusrModel
    {

        public string? Empnumber          { get; set; }

        public string? Status             { get; set; }
         
        public int? CreatedBy             { get; set; } //System Id

        public DateTime? DateCreated      { get; set; }


        // Full Name ---------------------------------------
        public string? FullName           { get; set; }
        public string StatusName => Status == "A" ? "Active" : "Disabled";
    }
}
