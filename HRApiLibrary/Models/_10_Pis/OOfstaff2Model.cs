using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.Models._10_Pis.OPis
{
    public class OOfstaff2Model
    {
        public int? Staff_Id        { get; set; }

        public string? Staff_Nm     { get; set; }

        public string? Staff_Pos    { get; set; }
            
        public int? Defa            { get; set; } = 0;
    }
}
