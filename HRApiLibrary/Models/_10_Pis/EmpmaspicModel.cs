using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.Models._10_Pis
{
    public class EmpmaspicModel
    {
        public int?         Id              { get; set; } = 0;

        public int?         EmpmasId        { get; set; } 

        public string?      PicName         { get; set; }

        public string?      PicAddress      { get; set; }

        public string?      Category        { get; set; }

        public string?      Validity        { get; set; }

        public int?         Mode            { get; set; }

        public DateTime?    Created         { get; set; }

        public string?      UploadedBy      { get; set; }
    }
}
