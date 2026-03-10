namespace HRApiLibrary.Models._20_PayGeneric;

public class GRefssstblModel
{
	public int?				YrStart 		{ get; set; } 
	public int?				YrEnd 			{ get; set; } 
	public double?			FStart 			{ get; set; } 
	public double?			FEnd 			{ get; set; } 
	public double?			Ee 				{ get; set; } 
	public double?			Er 				{ get; set; } 
	public string?			Ecc 			{ get; set; } 
	public double?			Compensation	{ get; set; } 
}

public class RSssPremModel
{
	
	public string?  	EmpNumber 		{ get; set; }
	public string?  	EmpLastNm 		{ get; set; }
	public string?  	EmpFirstNm 		{ get; set; }
	public string?  	EmpMidNm 		{ get; set; }
	public string?  	Payrollgrp 		{ get; set; }
	public string?  	DateHired 		{ get; set; }
	public double?  	Ee 				{ get; set; }
	public double?  	Er 				{ get; set; }
	public double?  	Ec 				{ get; set; }
	public double?  	Compensation 	{ get; set; }
}