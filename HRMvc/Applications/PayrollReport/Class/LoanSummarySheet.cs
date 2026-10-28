using HRApiLibrary.Models._20_PayGeneric;
using OfficeOpenXml;

namespace HRMvc.Applications.PayrollReport.Class;

//--- Loan Summary sheet (MBPay loans_management "Export to Excel"): title, loan name, header row, one row per employee ---
public static class LoanSummarySheet
{
    public static byte[] Build(List<RLoanSummaryModel> rows, string loanName)
    {
        using var pkg = new ExcelPackage();
        var ws = pkg.Workbook.Worksheets.Add("Loan Summary");
        ws.Cells[1, 1].Value = "Loan Report Summary";
        ws.Cells[2, 1].Value = "Loan Name : " + loanName;
        string[] head = ["Emp. Name", "Loan Date", "Loan Amount", "Payroll Deduction", "Payroll Balance", "Payment Made", "Unpaid Balance", "Status", "Client", "Amort"];
        for (int c = 0; c < head.Length; c++) ws.Cells[4, c + 1].Value = head[c];
        ws.Row(1).Style.Font.Bold = true;
        ws.Row(4).Style.Font.Bold = true;

        int i = 5;
        foreach (var r in rows)
        {
            ws.Cells[i, 1].Value = r.Empname;
            ws.Cells[i, 2].Value = r.Date?.ToString("MM/dd/yyyy");
            ws.Cells[i, 3].Value = r.Amount;
            ws.Cells[i, 4].Value = r.PayrollDeduction;
            ws.Cells[i, 5].Value = r.Balance;
            ws.Cells[i, 6].Value = r.Pymt;
            ws.Cells[i, 7].Value = r.Unpaid;
            ws.Cells[i, 8].Value = r.Status;
            ws.Cells[i, 9].Value = r.Clname;
            ws.Cells[i, 10].Value = r.Amort;
            i++;
        }
        ws.Cells[5, 3, i, 7].Style.Numberformat.Format = "#,##0.00";
        ws.Cells[5, 10, i, 10].Style.Numberformat.Format = "#,##0.00";
        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        return pkg.GetAsByteArray();
    }
}
