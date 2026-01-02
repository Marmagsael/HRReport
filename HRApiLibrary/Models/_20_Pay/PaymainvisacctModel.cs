using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.Models._20_Pay;

public class PaymainvisacctModel
{
    public string?  Trn         { get; set; }
    public string?  AcctNumber  { get; set; }
    
    //--------------------------------------------------------------------------
    public string?    AcctName         { get; set; } = string.Empty; 
    public string?    ShortDesc        { get; set; } = string.Empty;
    
    
}
