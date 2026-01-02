
namespace HRApiLibrary.Models._10_Pis;

public class TranexonerateModel
{

    public  int Id { get; set; }
    public  int IdEmpmas { get; set; }
    public  string TranNumber { get; set; } = string.Empty;
    public  DateTime PrepDate { get; set; }
    public  int Prep_ById { get; set; }
    public string Mode { get; set; } = string.Empty;
    public  int EmpStatusId { get; set; }
    public  int IdApprover { get; set; }
    public  int MarkApprove { get; set; }
    //--------------------------------
    public  string? Remarks { get; set; }
}
