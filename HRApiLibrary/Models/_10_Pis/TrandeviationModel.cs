namespace HRApiLibrary.Models._10_Pis
{
    public class TrandeviationModel
    {

        public int          Id          { get; set; }
        public int          IdEmpmas    { get; set; }
        public string?      TranNumber  { get; set; } = string.Empty;
        public DateTime?    PrepDate    { get; set; } = DateTime.Now;
        public string       Mode        { get; set; } = string.Empty;
        public DateTime?    ReportDate  { get; set; }
        public DateTime?    OccurDate   { get; set; }
        public string?      Allegation  { get; set; }
        public string?      Freq_No     { get; set; }
        public int          EmpStatusId { get; set; }
        public int          IdApprover  { get; set; } = 0;
        public int          MarkApprove { get; set; } = 0;


        ///-------------------------------------------------------------------------
        public string?      Empnumber           { get; set; }
        public string?      Empname             { get; set; }
        public string?      Deployment          { get; set; }
        public int          DeploymentId        { get; set; }
        public int          DeviationId         { get; set; }
        public DateTime     DeploymentDate      { get; set; }
        public string?      PreparedBy          { get; set; }
        public int          PrepId              { get; set; }
        public int          TimesCommitted      { get; set; }
        public string?      Remarks             { get; set; }
        public string?      Link                { get; set; }
        public DateTime?    DateReported        { get; set; }






    }
}
