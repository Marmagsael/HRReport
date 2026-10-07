namespace HRApiLibrary.Models._10_Pis.OPis;

public class OClientModel
{
	public string? 		ClNumber      		{ get; set; } 
	public string? 		ClName        		{ get; set; } 
	public string? 		Addr1         		{ get; set; } 
	public string? 		Addr2         		{ get; set; } 
	public string? 		AreaCode      		{ get; set; } 
	public string? 		Tel1          		{ get; set; } 
	public string? 		FaxNo         		{ get; set; } 
	public string? 		Parent        		{ get; set; } 
	public Double? 		Rate          		{ get; set; } = 0.00;
	public Double? 		BillRate      		{ get; set; } = 0.00;
    public Double? 		Assist        		{ get; set; } = 0.00;
    public string? 		Status        		{ get; set; } = string.Empty;
	public Double? 		ColaRate      		{ get; set; } = 0.00;
    public Double? 		Nd_Rate       		{ get; set; } = 0.00;
    public Double? 		RetiRate      		{ get; set; } = 0.00;
    public Double? 		UniFRate       		{ get; set; } = 0.00;
    public Double? 		FdiRate       		{ get; set; } = 0.00;
    public Double? 		OTRate        		{ get; set; } = 0.00;
    public string? 		Tin           		{ get; set; } 
	public string? 		Cont          		{ get; set; } 
	public string? 		Used          		{ get; set; } 
	public string? 		Contact       		{ get; set; } 
	public string? 		PostPeriod    		{ get; set; } 
	public string? 		BatchX        		{ get; set; } 
	public Double? 		FsssEe        		{ get; set; } = 0.00;
    public Double? 		FsssEr        		{ get; set; } = 0.00;
    public Double? 		Fecc          		{ get; set; } = 0.00;
    public Double? 		Fmedee        		{ get; set; } = 0.00;
    public Double? 		Fmeder        		{ get; set; } = 0.00;
    public string? 		Remarks       		{ get; set; } 
	public DateTime? 	ContStart     		{ get; set; } 
	public DateTime? 	ContEnd       		{ get; set; } 
	public string? 		ParentCd      		{ get; set; } = string.Empty;
	public Double? 		MaxSss        		{ get; set; } = 0.00;
    public Double? 		MaxPhic       		{ get; set; } = 0.00;
    public DateTime? 	ContExp       		{ get; set; } 
	public Double? 		HavTax        		{ get; set; } = 0.00;
    public Double? 		MinRate       		{ get; set; } = 0.00;
    public Double? 		MealAllow     		{ get; set; } = 0.00;
    public int? 		WithUniform   		{ get; set; } = 0;
	public int? 		WithRetirement		{ get; set; } = 0;
	public string? 		Region        		{ get; set; } 
	public int? 		EcolaRevised  		{ get; set; } = 0;
	public Double? 		CtpaRate      		{ get; set; } = 0.00;
	public int? 		WithCtpa      		{ get; set; } = 0;
	public string? 		SeaRate       		{ get; set; } 
	public int? 		WithSea       		{ get; set; } = 0;
	public string? 		Payprd        		{ get; set; } = string.Empty;
	public string? 		SgCode        		{ get; set; } = string.Empty;
    public int? 		IsTrucking    		{ get; set; } = 0;
    public int? 		IsLumpsum     		{ get; set; } = 0;

    // --- Other -------------------------------------------------------
    public int? 	    IsSelected 			{ get; set; } = 0; 
	public bool     	IsSelectedB         { get => IsSelected == 1; set => IsSelected = value ? 1 : 0; }
    public string?		ParentName			{ get; set; }
    public string?		AreaName			{ get; set; }


    // DOLE REPORT ---------------------------------------------
    public string?		Job					{ get; set; }
    public string?		PersonnelPositions	{ get; set; }
    public int?			MaleCnt				{ get; set; } = 0;
    public int?			FemaleCnt			{ get; set; } = 0;
    public int?			OthersCnt			{ get; set; } = 0;



}
