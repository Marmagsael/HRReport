using System;

namespace HRApiLibrary.Models._00_Main;

public class CityModel
{
    public int          Id              { get; set; }
    public int          CountryId       { get; set; }
    public string?      CountryCode     { get; set; }
    public int          RegionId        { get; set; }
    public string?      CityName        { get; set; }
}
