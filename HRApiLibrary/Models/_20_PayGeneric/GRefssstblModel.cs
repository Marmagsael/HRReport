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
	public DateTime?  	DateHired 		{ get; set; }
	public double?  	Ee 				{ get; set; }
	public double?  	Er 				{ get; set; }
	public double?  	Ec 				{ get; set; }
	public double?  	Compensation 	{ get; set; }
}



public class RPhicPremModel
{

    public string?      EmpNumber       { get; set; }
    public string?      EmpLastNm       { get; set; }
    public string?      EmpFirstNm      { get; set; }
    public string?      EmpMidNm        { get; set; }
    public string?      Suffix          { get; set; }
    public string?      Phic            { get; set; }
    public DateTime?    EmpBirth       { get; set; }
    public DateTime?    EffectiveDate   { get; set; } = DateTime.Now;
    public double?      Ee              { get; set; }
    public double?      Er              { get; set; }
    public double?      Salary          { get; set; } // to confirm what field to map here
    public string?      Status          { get; set; } 
    public string?      Gender          { get; set; } // M or F

}

public class RPagIbigPremModel
{

    public string?      EmpNumber { get; set; }
    public string?      EmpLastNm { get; set; }
    public string?      EmpFirstNm { get; set; }
    public string?      EmpMidNm { get; set; }
    public string?      PagibigNo { get; set; }
    public DateTime?    EmpBirth { get; set; }
    public double?      Ee { get; set; }
    public double?      Er { get; set; }
    public double?      Compensation { get; set; }
    public string?      Tin { get; set; }

}

