using HRApiLibrary.Models._20_PayGeneric;
using OfficeOpenXml;

namespace HRMvc.Applications.PayrollReport.Class;

//--- Bank upload sheet (MBPay frmbanktextfile "Bank Text File"): header, a total row, then one row per employee ---------
//--- currency, pay code and account length come from reportsettings (BANKCURRENCY, BANKPAYCODE01/02, BANKACCTLEN) ---
public static class BankTextFileSheet
{
    //--- Dashes removed, cut to the bank's account length ---
    public static string BankAcct(string? acct, int acctLen)
    {
        var a = (acct ?? "").Replace("-", "").Trim();
        return a.Length > acctLen ? a[..acctLen] : a;
    }

    public static byte[] Build(List<RBankAdviseModel> rows, string motherAcct, string currency, string payCode, int acctLen)
    {
        //--- MBPay wrote the pay code as a number ---
        object code = int.TryParse(payCode, out var n) ? n : payCode;
        using var pkg = new ExcelPackage();
        var ws = pkg.Workbook.Worksheets.Add("Sheet1");
        string[] head = ["CURRENCY", "EMPLOYER ACCT. #", "PAY CODE", "EMP.  ACCT. #", "AMOUNT", "SURNAME", "FIRSTNAME"];
        for (int c = 0; c < head.Length; c++) ws.Cells[1, c + 1].Value = head[c];
        ws.Row(1).Style.Font.Bold = true;

        ws.Cells[2, 1].Value = currency;
        ws.Cells[2, 2].Value = motherAcct.Trim();
        ws.Cells[2, 3].Value = code;
        ws.Cells[2, 5].Formula = $"SUM(E3:E{rows.Count + 2})";
        ws.Cells[2, 5].Style.Font.Bold = true;

        int i = 3;
        foreach (var r in rows)
        {
            ws.Cells[i, 1].Value = currency;
            ws.Cells[i, 2].Value = motherAcct.Trim();
            ws.Cells[i, 3].Value = code;
            ws.Cells[i, 4].Value = BankAcct(r.Acctcode, acctLen);
            ws.Cells[i, 5].Value = r.Amt;
            ws.Cells[i, 6].Value = r.EmpLastNm;
            ws.Cells[i, 7].Value = r.EmpFirstNm;
            i++;
        }
        ws.Column(5).Style.Numberformat.Format = "0.00";
        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        return pkg.GetAsByteArray();
    }
}
