namespace HRApiLibrary.Models._00_Main;

public class MenuModel
{
    public int?         Id          { get; set; }

    public string?      Type        { get; set; }

    public int?         IdParent    { get; set; }

    public int?         Indent      { get; set; }

    public string?      Icon        { get; set; }

    public string?      DispText    { get; set; }

    public string?      Action      { get; set; }

    public int?         Odr         { get; set; }
}