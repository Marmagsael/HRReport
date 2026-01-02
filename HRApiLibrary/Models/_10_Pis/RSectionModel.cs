namespace HRApiLibrary.Models._10_Pis; 

public class RsectionModel
{
    public int          Id                  { get; set; } = 0;
    public int          Departmentid        { get; set; } = 0;
    public string?      Sname               { get; set; } = string.Empty;
    public string?      Name                { get; set; } = string.Empty;
    public string?      Level               { get; set; } = string.Empty;

    //----------------------------------------------------------
    public string?      Departmentname      { get; set; } = string.Empty; 
}