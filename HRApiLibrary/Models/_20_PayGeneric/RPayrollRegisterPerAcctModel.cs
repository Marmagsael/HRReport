namespace HRApiLibrary.Models._20_PayGeneric;

//--- Payroll Register (Per Account) (MBPay frmpayrollregisterperacct -> rptPYR.frx) -----------------------------------
//--- One row per employee per TRN with a fixed column per account: Ennn / Dnnn amounts and EnnnH hours (MBPay's
//--- Ennn_<timedmode> fields). Only the accounts the report prints are here; Gross and Net use every account.
public class RPayrollRegisterPerAcctModel
{
	public string?  	Trn 			{ get; set; }
	public string?  	Batch 			{ get; set; }
	public string?  	EmpNumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	Clname 			{ get; set; }
	public string?  	PeriodCovered 	{ get; set; }
	public string?  	HDtl 			{ get; set; }
	public double   	Gross 			{ get; set; }
	public double   	Net 			{ get; set; }

	 public double E001 { get; set; } public double E002 { get; set; } public double E003 { get; set; } public double E004 { get; set; } public double E005 { get; set; } public double E006 { get; set; }
	 public double E007 { get; set; } public double E008 { get; set; } public double E009 { get; set; } public double E010 { get; set; } public double E011 { get; set; } public double E012 { get; set; }
	 public double E013 { get; set; } public double E014 { get; set; } public double E015 { get; set; } public double E016 { get; set; } public double E017 { get; set; } public double E018 { get; set; }
	 public double E019 { get; set; } public double E020 { get; set; } public double E021 { get; set; } public double E022 { get; set; } public double E023 { get; set; } public double E024 { get; set; }
	 public double E025 { get; set; }

	 public double E001H { get; set; } public double E002H { get; set; } public double E003H { get; set; } public double E004H { get; set; } public double E005H { get; set; } public double E006H { get; set; }
	 public double E007H { get; set; } public double E008H { get; set; } public double E009H { get; set; } public double E010H { get; set; } public double E011H { get; set; } public double E012H { get; set; }
	 public double E013H { get; set; } public double E014H { get; set; }

	 public double D001 { get; set; } public double D002 { get; set; } public double D003 { get; set; } public double D004 { get; set; } public double D005 { get; set; } public double D006 { get; set; }
	 public double D007 { get; set; } public double D008 { get; set; } public double D009 { get; set; } public double D010 { get; set; } public double D011 { get; set; } public double D012 { get; set; }
	 public double D013 { get; set; } public double D014 { get; set; } public double D015 { get; set; } public double D016 { get; set; } public double D017 { get; set; } public double D018 { get; set; }
	 public double D019 { get; set; } public double D020 { get; set; } public double D021 { get; set; } public double D022 { get; set; } public double D023 { get; set; } public double D024 { get; set; }
	 public double D025 { get; set; }
	 public double D048 { get; set; }

	private static readonly Dictionary<string, System.Reflection.PropertyInfo> _props =
		typeof(RPayrollRegisterPerAcctModel).GetProperties().Where(p => p.PropertyType == typeof(double)).ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

	//--- Adds to the Ennn/Dnnn (or EnnnH) column; accounts the report does not print are ignored ---
	public void Add(string column, double amount)
	{
		if (_props.TryGetValue(column, out var p)) p.SetValue(this, (double)p.GetValue(this)! + amount);
	}
	public void Round(Func<double, double> round)
	{
		foreach (var p in _props.Values) p.SetValue(this, round((double)p.GetValue(this)!));
	}
}
