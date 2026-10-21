using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.Models._10_Pis.OPis
{
    public class OSecliclogsModel
    {
        public string?      EmpNumber       { get; set; }

        public string?      Sbrno           { get; set; }

        public string?      Opno            { get; set; }

        public DateTime?    Validated       { get; set; }

        public double?       Vfee            { get; set; } = 0.00;

        public DateTime?    Revalidated     { get; set; }

        public string?      ValStatus       { get; set; }

        public string?      User            { get; set; }

        public DateTime?    DateModified    { get; set; }

        public DateTime?    LicDateIssued   { get; set; }
    }
}
