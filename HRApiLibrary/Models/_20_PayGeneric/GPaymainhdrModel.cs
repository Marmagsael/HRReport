namespace HRApiLibrary.Models._20_PayGeneric;

public class GPaymainhdrModel
{
	public string? 		Trn 			{ get; set; } 
	public double? 		ClRate 			{ get; set; } 
	public double? 		Minrate 		{ get; set; } 
	public int? 		WithSea 		{ get; set; } 
	public double? 		SeaRate 		{ get; set; } 
	public int? 		WithCtpa 		{ get; set; } 
	public double? 		CtpaRate 		{ get; set; } 
	public double? 		EcolaRevised	{ get; set; } 
	public double? 		BillRate 		{ get; set; } 
	public string? 		User 			{ get; set; } 
	public string? 		Status 			{ get; set; } 
	public DateTime? 	DateCreated 	{ get; set; } 
	public DateTime? 	DatePosted 		{ get; set; } 
	public DateTime? 	AttStart 		{ get; set; } 
	public DateTime? 	AttEnd			{ get; set; } 

	// -- Other Fields ------------------------------------------
	public string? ClientName 			{ get; set; }

}