using ClosedXML.Excel;
using HRApiLibrary.Models._20_Pay.OPay;
using Microsoft.AspNetCore.DataProtection;

namespace HRMvc.Applications.PayrollTransaction.Blazor._502;

// -- 13T/502 manhour Excel : validation key written by Create Excel and checked by Upload Excel.
//    The key is the payroll code, group and coverage, encrypted with the app's data-protection keys (HRMvc/keys),
//    in a very hidden "Key" sheet, so a file from another payroll, another server or an edited key is refused --
public static class _502_ExcelKey
{
    public const string Sheet = "Key";
    const string Purpose      = "HRSeries.13T.502.ManHourExcel.v1";

    static string Payload(OPayrollSession s) =>
        string.Join("|", s.Trn, s.Client.Code?.Trim(), s.AttStart.ToString("yyyy-MM-dd"), s.AttEnd.ToString("yyyy-MM-dd"));

    public static void Write(IDataProtectionProvider dp, XLWorkbook wb, OPayrollSession s)
    {
        var ws = wb.Worksheets.Add(Sheet);
        ws.Cell(1, 1).Value = dp.CreateProtector(Purpose).Protect(Payload(s));
        ws.Visibility = XLWorksheetVisibility.VeryHidden;
    }

    // -- null when the file belongs to the loaded payroll, otherwise the reason --
    public static string? Check(IDataProtectionProvider dp, XLWorkbook wb, OPayrollSession s)
    {
        if (!wb.TryGetWorksheet(Sheet, out var ws) || ws.Cell(1, 1).IsEmpty())
            return "This file has no validation key. Use a file made with Create Excel.";

        string payload;
        try
        {
            payload = dp.CreateProtector(Purpose).Unprotect(ws.Cell(1, 1).GetString());
        }
        catch (System.Security.Cryptography.CryptographicException)
        {
            return "Invalid validation key : the file was changed or made on another server.";
        }

        var p = payload.Split('|');
        if (p.Length != 4) return "Invalid validation key.";

        if (p[0] != s.Trn)
            return $"This file is for payroll {p[0]}; the loaded payroll is {s.Trn}.";
        if (p[1] != s.Client.Code?.Trim())
            return $"This file is for payroll group {p[1]}; the loaded group is {s.Client.Code?.Trim()}.";
        if (p[2] != s.AttStart.ToString("yyyy-MM-dd") || p[3] != s.AttEnd.ToString("yyyy-MM-dd"))
            return $"This file covers {p[2]} to {p[3]}; the loaded payroll covers {s.AttStart:yyyy-MM-dd} to {s.AttEnd:yyyy-MM-dd}.";

        return null;
    }
}
