namespace HRApiLibrary.Models._00_MainPis;

public class EngagementModel
{
	public int          Id                  {get; set; } 
	public int          OwnerId             {get; set; } 
	public int          CompanyId           {get; set; } 
	public string?      Module              {get; set; } 
	public int          RoleId              {get; set; } 
	public DateTime     DateInvited         {get; set; } 
	public DateTime     DateApproved        {get; set; } 
	public string?      Status              {get; set; } 

	//-----------------------------------------------------------
	public string?      CompanyName 		{get; set; } = "";
	public string?      OwnerName 			{get; set; } = "";
	public string?      RoleName 			{get; set; } = "";


}