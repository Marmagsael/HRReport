namespace HRApiLibrary.Models._10_Pis;

public class PissettingsModel
{
	public int          Id                              {get; set; }
	public int 			LeaveYrImplementation 			{get; set; }	= DateTime.Now.Year;
	public DateTime		LeaveAnniversaryStart 			{get; set; }	= DateTime.Now;
	public DateTime		LeaveAnniversaryEnd 			{ get; set; }	= DateTime.Now; 

	//----------------------------------------------------------------------------------------
	public int	 		UserId 							{ get; set; } =0; 
}
