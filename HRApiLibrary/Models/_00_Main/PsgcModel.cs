namespace HRApiLibrary.Models._00_Main;

// -- Main.Psgc : PSA Philippine Standard Geographic Code (regions down to barangays) --
public class PsgcModel
{
    public string?      PsgcCode        { get; set; }   // Reg 2, Prov 4, City/Mun 6, Bgy 9 digits
    public string?      Name            { get; set; }
    public string?      Level           { get; set; }   // Reg / Prov / City / Mun / Bgy
    public string?      RegCode         { get; set; }
    public string?      ProvCode        { get; set; }   // empty for HUC / NCR cities
    public string?      CityMunCode     { get; set; }   // barangays only
    public string?      Version         { get; set; }
}
