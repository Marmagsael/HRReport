namespace HRApiLibrary.Models._10_Pis.OPis;

public class OLoansModel
{
	public string? 		Number 			{ get; set; } 
	public string? 		EmpNumber 		{ get; set; } 
	public DateTime? 	Date 			{ get; set; } 
	public string? 		DedNCode 		{ get; set; } 
	public string? 		DedNDesc 		{ get; set; } 
	public double? 		Amount 			{ get; set; } 
	public double? 		Amort 			{ get; set; } 
	public double? 		Balance 		{ get; set; } 
	public string? 		Status 			{ get; set; } 
	public string? 		EncodedBy 		{ get; set; } 
	public string? 		EncodedDT 		{ get; set; } 
	public string? 		ChangeBy 		{ get; set; } 
	public string? 		ChangeDT 		{ get; set; } 
	public string? 		Posted 			{ get; set; } 
	public string? 		PostFlag 		{ get; set; } 
	public string? 		Remarks 		{ get; set; } 
	public string? 		PayMode 		{ get; set; } 
	public DateTime? 	PayStart 		{ get; set; } 
	public DateTime? 	PayRes 			{ get; set; } 
	public string? 		CvNo 			{ get; set; } 
	public int? 		P1 				{ get; set; } 
	public int? 		P2 				{ get; set; } 
	public int? 		P3 				{ get; set; } 
	public int? 		P4 				{ get; set; } 
	public int? 		P5 				{ get; set; } 
	public string? 		TrnLastPosted 	{ get; set; } 

	//--- Other Fields ---------------------------------------
	public string? 		AcctName 		{ get; set; } = string.Empty; 
	public string? 		EmpName 		{ get; set; } = string.Empty; 

}

