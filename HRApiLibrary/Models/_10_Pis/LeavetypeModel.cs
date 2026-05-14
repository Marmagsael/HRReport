namespace HRApiLibrary.Models._10_Pis;

public class LeavetypeModel
{
	public int          Id          {get; set; } 
	public string?      Code        {get; set; } 
	public string?      LeaveName   {get; set; } 
	public DateTime     AnivStart   {get; set; } 
	public DateTime     AnivEnd     {get; set; } 
	public int          DefValue    {get; set; } 

    // --- Other ------------------------------------------------------------------------------
    public bool         Enabled     { get; set; } = false; 
    public bool         IsEditable  { get; set; } = false; 

}
