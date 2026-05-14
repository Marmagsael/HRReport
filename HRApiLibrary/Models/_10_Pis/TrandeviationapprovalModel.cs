namespace HRApiLibrary.Models._10_Pis
{
    public class TrandeviationapprovalModel
    {

        public int          Id          { get; set; }
        public int          IdEmpmas    { get; set; }
        public string?      TranNumber  { get; set; } = string.Empty;
        public DateTime?    PrepDate    { get; set; } = DateTime.Now;
        public string       Mode        { get; set; } = string.Empty;
        public DateTime?    ReportDate  { get; set; }
        public DateTime?    OccurDate  { get; set; }
        public string?      Allegation  { get; set; } 
        public string?      Freq_No    { get; set; }
        public int          EmpStatusId { get; set; } = 1;
        public string?      Remarks     { get; set; }
        public int          IdApprover { get; set; } = 0;
        public int          MarkApprove { get; set; } = 0;


        ///-------------------------------------------------------------------------

        public int          TimesCommitted { get; set; } = 0;
        public string?      Link { get; set; }


    }
}
