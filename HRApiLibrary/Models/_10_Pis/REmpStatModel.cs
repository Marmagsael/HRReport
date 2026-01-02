namespace HRApiLibrary.Models._10_Pis;

public class RempstatModel
{
    public int      Id              { get; set; } = 0;
    public string?  Code            { get; set; } = string.Empty;
    public string?  Name            { get; set; } = string.Empty;
    public int      Isresigned      { get; set; } = 0;
    public int      Isonleaved      { get; set; } = 0;
    public int      Isfloating      { get; set; } = 0;
    public int      Issuspended     { get; set; } = 0;
    public int      Isfordeviation  { get; set; } = 0;
    //----------------------------------------------------------
    public int IsSelected { get; set; } = 0;
    public int isfordeviation{ get; set; } = 0;
    public int isfordisciplinary{ get; set; } = 0;
    public int isforexonerate{ get; set; } = 0;
    public int isforinvestigation { get; set; } = 0;

}

public class RempstatIsModel
{
    public int          Id              { get; set; } = 0;
    public string?      Code            { get; set; } = string.Empty;
    public string?      Name            { get; set; } = string.Empty;
    public bool         Isresigned      { get; set; } = false;
    public bool         Isonleaved      { get; set; } = false;
    public bool         Isfloating      { get; set; } = false;
    public bool         Issuspended     { get; set; } = false;
}