namespace HRApiLibrary.Models._20_Pay;

public class UserpayinprocessModel
{
	public int			UserId			{get; set; } 

	public string?		Trn				{get; set; } 

	public int			PayrollgrpId	{get; set; } 

	public DateTime		AttStart		{get; set; } 

	public DateTime		AttEnd			{get; set; } 

	public int			Yr				{get; set; } 

	public string?		Month			{get; set; } 

	public string?		Period			{get; set; } 
}
