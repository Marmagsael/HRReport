using Microsoft.VisualBasic;

namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasEducateModel
{
    public int Id { get; set; }

    public int EmpmasId { get; set; }

    public string? Code { get; set; }

    public string? School { get; set; }

    public DateTime FROM_ { get; set; }

    public DateTime TO_ { get; set; }

    public string? COURSE { get; set; }

    public string? LEVEL { get; set; }
    public string? LEVELNAME { get; set; }


}
