using System;

namespace HRApiLibrary.Models._10_Pis;

public class TrandisciplinaryModel
{
    public int          Id { get; set; }
    public int          IdEmpmas { get; set; }
    public string?      TranNumber { get; set; }
    public DateTime     PrepDate { get; set; }
    public string?      Mode { get; set; }
    public string?      Penalty_No { get; set; }
    public DateTime?    StartDate { get; set; }
    public DateTime?    EndDate { get; set; }
    public int          NoOfDays { get; set; }
    public int          EmpStatusId { get; set; }

    // ====================================
    public bool BlockPost { get; set; } = false;
     
}
