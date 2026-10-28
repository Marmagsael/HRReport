namespace HRApiLibrary.Models._20_PayGeneric;

//--- Payroll Register (MBPay frmpayrollregister per Client / per Batch -> rptPYR_04b) ------------------------------------
//--- One row per employee per TRN. E1..E48 / D1..D48 are column slots: the accounts used in the run, earnings by
//--- Chartofacct.Sort and deductions by DedSort. HE*/HD* hold each slot's caption (MBPay cursor rh) on every row.
public class RPayrollRegisterModel
{
	public string?  	Trn 			{ get; set; }
	public string?  	Batch 			{ get; set; }
	public string?  	EmpNumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	Clname 			{ get; set; }
	public string?  	Dstart 			{ get; set; }
	public string?  	Dend 			{ get; set; }
	public double   	Netpay 			{ get; set; }
	public double   	GrossPay 		{ get; set; }
	public double   	GrossDed 		{ get; set; }
	public string?  	HDtl 			{ get; set; }

	 public double E1 { get; set; } public double E2 { get; set; } public double E3 { get; set; } public double E4 { get; set; } public double E5 { get; set; } public double E6 { get; set; }
	 public double E7 { get; set; } public double E8 { get; set; } public double E9 { get; set; } public double E10 { get; set; } public double E11 { get; set; } public double E12 { get; set; }
	 public double E13 { get; set; } public double E14 { get; set; } public double E15 { get; set; } public double E16 { get; set; } public double E17 { get; set; } public double E18 { get; set; }
	 public double E19 { get; set; } public double E20 { get; set; } public double E21 { get; set; } public double E22 { get; set; } public double E23 { get; set; } public double E24 { get; set; }
	 public double E25 { get; set; } public double E26 { get; set; } public double E27 { get; set; } public double E28 { get; set; } public double E29 { get; set; } public double E30 { get; set; }
	 public double E31 { get; set; } public double E32 { get; set; } public double E33 { get; set; } public double E34 { get; set; } public double E35 { get; set; } public double E36 { get; set; }
	 public double E37 { get; set; } public double E38 { get; set; } public double E39 { get; set; } public double E40 { get; set; } public double E41 { get; set; } public double E42 { get; set; }
	 public double E43 { get; set; } public double E44 { get; set; } public double E45 { get; set; } public double E46 { get; set; } public double E47 { get; set; } public double E48 { get; set; }

	 public double D1 { get; set; } public double D2 { get; set; } public double D3 { get; set; } public double D4 { get; set; } public double D5 { get; set; } public double D6 { get; set; }
	 public double D7 { get; set; } public double D8 { get; set; } public double D9 { get; set; } public double D10 { get; set; } public double D11 { get; set; } public double D12 { get; set; }
	 public double D13 { get; set; } public double D14 { get; set; } public double D15 { get; set; } public double D16 { get; set; } public double D17 { get; set; } public double D18 { get; set; }
	 public double D19 { get; set; } public double D20 { get; set; } public double D21 { get; set; } public double D22 { get; set; } public double D23 { get; set; } public double D24 { get; set; }
	 public double D25 { get; set; } public double D26 { get; set; } public double D27 { get; set; } public double D28 { get; set; } public double D29 { get; set; } public double D30 { get; set; }
	 public double D31 { get; set; } public double D32 { get; set; } public double D33 { get; set; } public double D34 { get; set; } public double D35 { get; set; } public double D36 { get; set; }
	 public double D37 { get; set; } public double D38 { get; set; } public double D39 { get; set; } public double D40 { get; set; } public double D41 { get; set; } public double D42 { get; set; }
	 public double D43 { get; set; } public double D44 { get; set; } public double D45 { get; set; } public double D46 { get; set; } public double D47 { get; set; } public double D48 { get; set; }

	 public string HE1 { get; set; } = ""; public string HE2 { get; set; } = ""; public string HE3 { get; set; } = ""; public string HE4 { get; set; } = ""; public string HE5 { get; set; } = ""; public string HE6 { get; set; } = "";
	 public string HE7 { get; set; } = ""; public string HE8 { get; set; } = ""; public string HE9 { get; set; } = ""; public string HE10 { get; set; } = ""; public string HE11 { get; set; } = ""; public string HE12 { get; set; } = "";
	 public string HE13 { get; set; } = ""; public string HE14 { get; set; } = ""; public string HE15 { get; set; } = ""; public string HE16 { get; set; } = ""; public string HE17 { get; set; } = ""; public string HE18 { get; set; } = "";
	 public string HE19 { get; set; } = ""; public string HE20 { get; set; } = ""; public string HE21 { get; set; } = ""; public string HE22 { get; set; } = ""; public string HE23 { get; set; } = ""; public string HE24 { get; set; } = "";
	 public string HE25 { get; set; } = ""; public string HE26 { get; set; } = ""; public string HE27 { get; set; } = ""; public string HE28 { get; set; } = ""; public string HE29 { get; set; } = ""; public string HE30 { get; set; } = "";
	 public string HE31 { get; set; } = ""; public string HE32 { get; set; } = ""; public string HE33 { get; set; } = ""; public string HE34 { get; set; } = ""; public string HE35 { get; set; } = ""; public string HE36 { get; set; } = "";
	 public string HE37 { get; set; } = ""; public string HE38 { get; set; } = ""; public string HE39 { get; set; } = ""; public string HE40 { get; set; } = ""; public string HE41 { get; set; } = ""; public string HE42 { get; set; } = "";
	 public string HE43 { get; set; } = ""; public string HE44 { get; set; } = ""; public string HE45 { get; set; } = ""; public string HE46 { get; set; } = ""; public string HE47 { get; set; } = ""; public string HE48 { get; set; } = "";

	 public string HD1 { get; set; } = ""; public string HD2 { get; set; } = ""; public string HD3 { get; set; } = ""; public string HD4 { get; set; } = ""; public string HD5 { get; set; } = ""; public string HD6 { get; set; } = "";
	 public string HD7 { get; set; } = ""; public string HD8 { get; set; } = ""; public string HD9 { get; set; } = ""; public string HD10 { get; set; } = ""; public string HD11 { get; set; } = ""; public string HD12 { get; set; } = "";
	 public string HD13 { get; set; } = ""; public string HD14 { get; set; } = ""; public string HD15 { get; set; } = ""; public string HD16 { get; set; } = ""; public string HD17 { get; set; } = ""; public string HD18 { get; set; } = "";
	 public string HD19 { get; set; } = ""; public string HD20 { get; set; } = ""; public string HD21 { get; set; } = ""; public string HD22 { get; set; } = ""; public string HD23 { get; set; } = ""; public string HD24 { get; set; } = "";
	 public string HD25 { get; set; } = ""; public string HD26 { get; set; } = ""; public string HD27 { get; set; } = ""; public string HD28 { get; set; } = ""; public string HD29 { get; set; } = ""; public string HD30 { get; set; } = "";
	 public string HD31 { get; set; } = ""; public string HD32 { get; set; } = ""; public string HD33 { get; set; } = ""; public string HD34 { get; set; } = ""; public string HD35 { get; set; } = ""; public string HD36 { get; set; } = "";
	 public string HD37 { get; set; } = ""; public string HD38 { get; set; } = ""; public string HD39 { get; set; } = ""; public string HD40 { get; set; } = ""; public string HD41 { get; set; } = ""; public string HD42 { get; set; } = "";
	 public string HD43 { get; set; } = ""; public string HD44 { get; set; } = ""; public string HD45 { get; set; } = ""; public string HD46 { get; set; } = ""; public string HD47 { get; set; } = ""; public string HD48 { get; set; } = "";

	public const int Slots = 48;

	private static readonly System.Reflection.PropertyInfo[] _e = Enumerable.Range(1, Slots).Select(i => typeof(RPayrollRegisterModel).GetProperty($"E{i}")!).ToArray();
	private static readonly System.Reflection.PropertyInfo[] _d = Enumerable.Range(1, Slots).Select(i => typeof(RPayrollRegisterModel).GetProperty($"D{i}")!).ToArray();
	private static readonly System.Reflection.PropertyInfo[] _he = Enumerable.Range(1, Slots).Select(i => typeof(RPayrollRegisterModel).GetProperty($"HE{i}")!).ToArray();
	private static readonly System.Reflection.PropertyInfo[] _hd = Enumerable.Range(1, Slots).Select(i => typeof(RPayrollRegisterModel).GetProperty($"HD{i}")!).ToArray();

	//--- slot is 1-based; isEarning picks E/D ---
	public double GetSlot(bool isEarning, int slot) => (double)(isEarning ? _e : _d)[slot - 1].GetValue(this)!;
	public void SetSlot(bool isEarning, int slot, double amount) => (isEarning ? _e : _d)[slot - 1].SetValue(this, amount);
	public void AddSlot(bool isEarning, int slot, double amount) => SetSlot(isEarning, slot, GetSlot(isEarning, slot) + amount);
	public void SetCaption(bool isEarning, int slot, string caption) => (isEarning ? _he : _hd)[slot - 1].SetValue(this, caption);
}

public class RPayrollRegisterTrnModel
{
	public string?  	Trn 			{ get; set; }
	public string?  	Batch 			{ get; set; }
	public string?  	ClName 			{ get; set; }
}

public class RPayrollRegisterTranModel
{
	public string?  	Trn 			{ get; set; }
	public string?  	EmpNumber 		{ get; set; }
	public string?  	AcctNumber 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public string?  	Batch 			{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	Clname 			{ get; set; }
	public string?  	Dstart 			{ get; set; }
	public string?  	Dend 			{ get; set; }
}

public class RPayrollRegisterAcctModel
{
	public string?  	AcctNumber 		{ get; set; }
	public string?  	AcctName 		{ get; set; }
	public string?  	ShortDesc 		{ get; set; }
	public int?     	Sort 			{ get; set; }
	public int?     	DedSort 		{ get; set; }
}
