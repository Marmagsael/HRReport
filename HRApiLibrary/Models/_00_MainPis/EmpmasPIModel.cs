namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasPIModel
{
    public int          Id             { get; set; }
    public DateTime     EmpBirth       { get; set; } 
    public string?      BirthPlace     { get; set; }
    public string?      Sex_           { get; set; }
    public string?      CivStat_       { get; set; }
    public string?      Citizen        { get; set; }
    public string?      Religion       { get; set; }
    public int          Height         { get; set; }
    public int          HeightInch     { get; set; }
    public double?      Weight         { get; set; }
    public string?      Hair           { get; set; }
    public string?      Eyes           { get; set; }
    public string?      Complexion     { get; set; }
    public string?      Marks          { get; set; }
    public string?      BloodType      { get; set; }
    public string?      Spouse         { get; set; }
    public string?      Occupation     { get; set; }
    public int          NoChildren     { get; set; }
}
