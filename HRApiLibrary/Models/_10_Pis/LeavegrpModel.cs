namespace HRApiLibrary.Models._10_Pis;

public class LeavegrpModel
{
    public int          Id          { get; set; }
    public string?      Name        { get; set; }

    // --- FK Link ------------------------------------------------------------------------------
    public string?      Approver1Name       { get; set; }
    public string?      FinalApproverName   { get; set; }

    // --- Other ------------------------------------------------------------------------------
    public bool         Enabled             { get; set; } = false; 
    public bool         IsEditable          { get; set; } = false; 
}
