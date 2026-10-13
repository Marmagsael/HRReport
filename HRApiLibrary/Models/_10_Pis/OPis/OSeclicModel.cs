using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.Models._10_Pis.OPis
{
    public class OSeclicModel
    {
        public string? Empnumber            { get; set; }

        public string? Sbrno                { get; set; }

        public string? Opno                 { get; set; }

        public DateOnly? Validated          { get; set; }

        public double? Vfee                 { get; set; }

        public DateOnly? Revalidated        { get; set; }

        public string? ValStatus            { get; set; }

        public string? User                 { get; set; }

        public DateTime? DateModified       { get; set; }

        public DateOnly? LicDateIssued      { get; set; }

    }
}
