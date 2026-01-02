namespace HRApiLibrary.Models._10_Pis;

public class EmpmasgrpModel
{
    public int          Empmasid            { get; set; } = 0;
    public int          Secid               { get; set; } = 0;
    public int          Depid               { get; set; } = 0;
    public int          Divid               { get; set; } = 0;
    public int          Leavegrpid          { get; set; } = 0;
    public int          Payrollgrpid        { get; set; } = 0;

    //---------------------------------------------------------------------------
    public string?      Empmasname              { get; set; } = string.Empty;
    public string?      Secname                 { get; set; } = string.Empty;
    public string?      Depsname                { get; set; } = string.Empty;
    public string?      Divname                 { get; set; } = string.Empty;
    public string?      Leavegrpname            { get; set; } = string.Empty;
    public string?      Payrollgrpname          { get; set; } = string.Empty;
}
