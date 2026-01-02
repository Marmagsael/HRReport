using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.Models._20_Pay;

public class PaymainhistoryModel
{
    public int          Id      { get; set; }

    public string?      Trn     { get; set; }

    public int          UserId  { get; set; }

    public DateTime     Posted  { get; set; }

    public string?      Action  { get; set; }
}


