namespace HRApiLibrary.Models._10_Pis;

public class Attpunches1Model
{
    public int?          DayNo               { get; set; } = 0;
    public int?          EmpmasId            { get; set; } = 0;
    public DateTime?     PunchInDate         { get; set; } 
    public int?          SchedDuration       { get; set; } = 0;
    public int?          PunchT              { get; set; } = 0;
    public int?          DutyTypeId          { get; set; } = 0;
    public int?          TimeZoneIdIn        { get; set; } = 0;


    public string?      IpAddressIn         { get; set; } = string.Empty; 
    public string?      MacAddressIn        { get; set; } = string.Empty;

    public int?         UserIdIn            { get; set; } = 0;

    public DateTime?    PunchOutDate        { get; set; }
    public int?         TimeZoneIdOut       { get; set; } = 0;

    public string?      IpAddressOut        { get; set; } = string.Empty;
    public string?      MacAddressOut       { get; set; } = string.Empty;

    public int?         UserIdOut           { get; set; } = 0;

    public string?      Status              { get; set; } = string.Empty;

    //--- Extra Fields -----------------//
	public int? 		TotalHrs      	 	{ get; set; } = 0;
	public int? 		Hour      	 		{ get; set; } = 0;
	public int? 		Minute      	 	{ get; set; } = 0;
	public string? 		Remarks 	 		{ get; set; } = string.Empty;

    //-----------------------------------------------------------
    public string?      DutyTypeCode         { get; set; } = string.Empty;
    public string?      DutyTypeName        { get; set; } = string.Empty;
    public double?      TotalHrsWorked      { get; set; } = 0.0;
    public List<Attpunches1Model> Logs      { get; set; } = new();



}
