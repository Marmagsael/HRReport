using System;

namespace HRApiLibrary.Models._10_Pis;

public class TrandisciplinaryapprovalModel
{
    public int              Id          { get; set; }
    public int              IdEmpmas    { get; set; }
    public string?          TranNumber  { get; set; }
    public DateTime         PrepDate    { get; set; } = DateTime.Now;
    public string?          Mode        { get; set; }
    public string?          Penalty_No  { get; set; }
    public DateTime?        StartDate   { get; set; } = DateTime.Now;
    public DateTime?        EndDate     { get; set; } = DateTime.Now;
    public int              NoOfDays    { get; set; }
    public int              EmpStatusId { get; set; } = 0;

    // ====================================
    public bool             BlockPost   { get; set; } = false;
     
}
