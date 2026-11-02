namespace HRApiLibrary.Models._20_Pay;

public class HolidayGlobalModel
{
    public int?      Id         { get; set; }
    public DateTime  HDate      { get; set; } = DateTime.Today;
    public string?   Name       { get; set; }
    public string?   TypeCode   { get; set; } = "LH";
}

public class HolidayLocalModel
{
    public int?         Id              { get; set; }
    public DateTime     HDate           { get; set; } = DateTime.Today;
    public string?      Name            { get; set; }
    public string?      TypeCode        { get; set; } = "LH";
    public string?      GroupIds        { get; set; }
    public string?      GroupNames      { get; set; }
    public int          GroupCount      { get; set; }
}
