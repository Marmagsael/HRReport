namespace HRApiLibrary.Models._20_Pay;

public class PaymaindtlModel
{
	public string?		Trn 			{ get; set; }
	public string?		Empnumber 		{ get; set; }
	public int		    EmpmasId 		{ get; set; }
	public double		Sss 			{ get; set; }
	public double		SssEr 			{ get; set; }
	public double		SssEc 			{ get; set; }
	public double		Pagibig 		{ get; set; }
	public double		PagibigEr 		{ get; set; }
	public double		Phic 			{ get; set; }
	public double		PhicEr 			{ get; set; }
	public string?		PayStatus 		{ get; set; }
	public string?		PayrollGrpId	{ get; set; }
	public int			CompanyId 		{ get; set; }
	public int			BranchId 		{ get; set; }
	public double		DayWrk 			{ get; set; }
	public double		Absent 			{ get; set; }
	public double		Late 			{ get; set; }
	public double		UTime 			{ get; set; }
	public double		Rn 				{ get; set; }
	public double		RnOt 			{ get; set; }
	public double		Rot 			{ get; set; }
	public double		Rd 				{ get; set; }
	public double		RdOt 			{ get; set; }
	public double		Lh 				{ get; set; }
	public double		LhOt 			{ get; set; }
	public double		RdLh 			{ get; set; }
	public double		RdLhOt 			{ get; set; }
	public double		Sh 				{ get; set; }
	public double		ShOt 			{ get; set; }
	public double		RdSh 			{ get; set; }
	public double		RdShOt 			{ get; set; }
	public double		Custom1 		{ get; set; }
	public double		Custom2 		{ get; set; }
	public double		Custom3 		{ get; set; }
	public double		NdRd 			{ get; set; }
	public double		NdRdOt 			{ get; set; }
	public double		NdRdLh 			{ get; set; }
	public double		NdRdLhOt 		{ get; set; }
	public double		NdRdSh 			{ get; set; }
	public double		NdRdShOt 		{ get; set; }
	public double		Nd 				{ get; set; }
	public double		NdOt 			{ get; set; }
	public double		NdLh 			{ get; set; }
	public double		NdLhOt 			{ get; set; }
	public double		NdSh 			{ get; set; }
	public double		NdShOt 			{ get; set; }
	public double		Dh 				{ get; set; }
	public double		DhOt 			{ get; set; }
	public double		RdDh 			{ get; set; }
	public double		RdDhOt 			{ get; set; }
	
	//----------------------------------------------------
	public string?      EmpName         { get; set; } = string.Empty;
}