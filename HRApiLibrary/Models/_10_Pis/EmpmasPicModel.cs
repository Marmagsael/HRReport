namespace HRApiLibrary.Models._10_Pis;

// -- {Code}_Pis.EmpmasPic : one row per uploaded picture; Mode 1 = the current one of its category --
public class EmpmasPicModel
{
    public int?         Id          { get; set; }
    public int?         EmpmasId    { get; set; }
    public string?      PicName     { get; set; }   // -- uploaded file name --
    public string?      PicAddress  { get; set; }   // -- Backblaze file : {OldPis}/employees/{EmpNumber}/photo/{yyyyMMdd-HHmmss}.{ext} --
    public string?      Category    { get; set; } = "Profile";
    public DateTime?    Validity    { get; set; }
    public int?         Mode        { get; set; } = 1;
    public DateTime?    Created     { get; set; }
    public int?         UploadedBy  { get; set; }
}
