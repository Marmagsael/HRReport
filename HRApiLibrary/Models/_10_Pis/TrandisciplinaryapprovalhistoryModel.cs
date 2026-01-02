using System;

namespace HRApiLibrary.Models._10_Pis
{
    public class TrandisciplinaryapprovalhistoryModel
    {

        public int      Id                      { get; set; }
        public string   TranNumber              { get; set; } = string.Empty;
        public DateTime Date                    { get; set; }
        public int      UserId                  { get; set; }
        public string   Status                  { get; set; } = string.Empty;
        public int      ApproverId              { get; set; }
        public string   ApproverRemarks         { get; set; } = string.Empty;

        //-------------------------------------------------------------
        public string PreparedBy                { get; set; } = string.Empty;

    }
}
