using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._20_Pay.Report;

namespace HRApiLibrary.DataAccess._20_Pay.Report;

public interface IReportDataAccess
{
    Task<PayslipModel?> _02Payslip(string trn, string paySchema, string pisSchema, string conn); 
}

public class ReportDataAccess : IReportDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public ReportDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PayslipModel?> _02Payslip(string trn, string paydb, string pisdb, string conn)
    {
        var payslip = new PayslipModel(); 
        var sql = ""; 
        
        await _02Settings();                                            //-- CoCode, CoName and Address ---      
        await _02Paymain();                                             //-- AttStart, AttEnd, Date Covered
        var tbltrans = await _02Tbltran();       //-- Details ----------------------
        
        var empList = _02EmpList();
        FillPayslipDtl(); 
        
        /*Console.WriteLine($"|---> {payslip.PayslipDtls.Count} : payslip records found.");
        foreach (var p in payslip.PayslipDtls)
        {
            Console.WriteLine($"|---> Emp. Name: {p?.EmpName} * {p?.EAcctName} - {p?.EAmount} ** {p?.DAcctName} - {p?.DAmount} ** ");
        }*/
       

        return payslip;

        
        
        void FillPayslipDtl()
        {
            payslip.PayslipDtls = [];

            foreach (var emp in empList)
            {
                var empmasId = emp?.EmpmasId ?? 0;

                var empEarnings = tbltrans?
                    .Where(t => t?.EmpmasId == empmasId && 
                                (t!.AcctNumber ?? "").StartsWith("E")  )
                    .ToList() ?? [];

                var empDeductions = tbltrans?
                    .Where(t => t?.EmpmasId == empmasId && 
                                (t!.AcctNumber ?? "").StartsWith("D") )
                    .ToList() ?? [];

                var eCnt   = empEarnings?.Count ?? 0;    // <--- fixed
                var dCnt   = empDeductions?.Count ?? 0;  // <--- fixed
                var maxCnt = Math.Max(eCnt, dCnt);       // clearer

                //Console.WriteLine($" |++++> Loops  -----Total Count : {maxCnt}---------> ");
                for (int i = 0; i < maxCnt; i++)       // start at 0
                {
                    /* 1) ----- Initiliaze PayslipdtlQuery -------------------*/
                    PayslipdtlModel? pd = 
                        new() {Trn = trn, 
                               EmpmasId = emp?.EmpmasId??0,
                               EmpNumber = emp?.EmpNumber??"",
                               EmpName = emp?.EmpName??"", 
                               SssNo = emp?.SssNo??"",
                               TaxNo = emp?.TaxNo??"",
                               EmpRate = emp?.EmpRate??0,
                               RateTypeId = emp?.RateTypeId??0,
                        };
                    
                    // kunin ang item lang kung valid ang index; otherwise null
                    var earning = (i < eCnt) ? empEarnings?[i] : null;
                    var deduction = (i < dCnt) ? empDeductions?[i] : null;

                    if (i < eCnt) //--- Earnings Account -----------------------
                    {
                        pd.EAcctNumber = earning?.AcctNumber;               
                        pd.EAcctName   = earning?.AcctName;                 
                        pd.EQty        = earning?.Qty??0;              
                        pd.ERate       = earning?.Rate??0;                 
                        pd.EAmount     = earning?.Amount??0;               
                        pd.ESource     = earning?.Source;               
                        pd.ERefId      = earning?.RefId??0;                
                    }

                    if ((i < dCnt)) // Deductions Account -----------------------------
                    {
                        pd.DAcctNumber = deduction?.AcctNumber;               
                        pd.DAcctName   = deduction?.AcctName;                 
                        pd.DQty        = deduction?.Qty??0;              
                        pd.DRate       = deduction?.Rate??0;                 
                        pd.DAmount     = deduction?.Amount??0;               
                        pd.DSource     = deduction?.Source;               
                        pd.DRefId      = deduction?.RefId??0;
                        
                        if(deduction?.RefId>0) pd.DAcctName   = $"{deduction?.AcctName?.Trim()} [Ref # - {deduction?.RefId}]";                 

                    }
                    
                    payslip.PayslipDtls.Add(pd);
                    //Console.WriteLine($" --> {i} ) Earnings : {eacctNumber} * Deductions : {dacctNumber}");
                    
                }
            }
   
        }

        async Task _02Settings()
        {
            /* 1) --- Settings - ( CoName and Address )  ---------------------------------------------*/
            sql = $"select * from {paydb}.Settings where Id = 1 "; 
            var settings = await _sql.FetchData<SettingsModel?,dynamic>(sql, new {  }, conn);
            var setting = settings.FirstOrDefault();
            payslip.CoCode = setting?.CoShortName;
            payslip.CoName = setting?.CoFullName; 
            payslip.CoAddress = setting?.CoAddress;
            
        }
        async Task _02Paymain() 
        {
            /* 2) --- Paymain header - (AttStart, AttEnd, Date Covered) --------------------------------*/
            sql = @$"select * from {paydb}.Paymainhdr where Trn = @Trn "; 
            var paymainhdr = await _sql.FetchData<PaymainhdrModel?,dynamic>(sql, new { Trn = trn }, conn);

            if (paymainhdr.Count < 1) return;
            var hdr    = paymainhdr.First();
                
            payslip.AttStart          = hdr?.AttStart; 
            payslip.AttEnd            = hdr?.AttEnd;
            payslip.PaymainhdrStatus  = hdr?.Status;
            
            if (hdr is { AttStart: not null, AttEnd: not null })
            {
                var dateCov = $"{((DateTime)hdr.AttStart):MMM dd, yyyy} - {((DateTime)hdr.AttEnd):MMM dd, yyyy}";
                payslip.DateCovered = dateCov;
                     
            }
        }
        async Task<List<PayslipdtlQueryModel?>?> _02Tbltran() 
        {
            var tblName = "Tbltran"; 
            if(payslip.PaymainhdrStatus=="Editing") tblName="Tmptbltran";
            
            sql = $@"SELECT CONCAT(TRIM(e.EmpLastNm),', ', TRIM(e.EmpFirstNm), ' ', IFNULL(TRIM(e.EmpMidNm),'')) EmpName, c.AcctName, 
                          IFNULL(g.SSS,'') SssNo, IFNULL(g.Tin,'') TaxNo, 
                           t.* FROM {paydb}.{tblName} t 
                       LEFT JOIN {paydb}.Coa c ON c.acctNumber = t.AcctNumber 
                       LEFT JOIN {pisdb}.Empmas       e ON e.Id = t.EmpmasId
                       LEFT JOIN {pisdb}.Empmasgovph  g ON g.Id = t.EmpmasId
                     WHERE t.Trn = @Trn  and t.Amount > 0
                     ORDER BY EmpName, AcctNumber ";
            var data    = await _sql.FetchData<PayslipdtlQueryModel?, dynamic>(sql, new { Trn = trn}, conn);
            
            return data??[]; 

        }

        List<PayslipdtlQueryModel?> _02EmpList()
        {

            var res = tbltrans?
                .Where(p => p != null)                  // <-- added
                .GroupBy(p => p!.EmpmasId)              // <-- NEW: unique by EmpmasId
                .Select(g => g.First())      // <-- NEW: get single representative
                .OrderBy(p => p!.EmpmasId)              // <-- same order but safer
                .ToList();
 
            List<PayslipdtlQueryModel?>? payslipQueryList = [];

            foreach (var r in res ?? [])
            {
                var pq = 
                    new PayslipdtlQueryModel 
                    { 
                        EmpmasId  = r?.EmpmasId??0,
                        EmpNumber = r?.EmpNumber??"", 
                        EmpName   = r?.EmpName??"", 
                        SssNo     = r?.SssNo??"", 
                        TaxNo     = r?.TaxNo,
                    };
                
                var r1 = tbltrans?.FirstOrDefault(t => t?.EmpmasId == r.EmpmasId && t.AcctNumber == "E001");
                pq.EmpRate       = r1?.EmpRate??0;
                pq.RateTypeId    = r1?.RateTypeId??0;
                payslipQueryList.Add(pq);
            }
            
            return payslipQueryList; 
        }
        
        
        
        
    } 

}


