using HRApiLibrary.Models._20_PayGeneric;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HRMvc.Applications.PayrollReport.Class;

//--- Payroll Excel sheets MBPay builds in code (no FRX): payroll_summary4 (Summary, one TRN: an Earnings and a Deductions sheet),
//--- payroll_register4V2 (Register, 2 liner: an earnings row and a deductions row per employee), payroll_summary_per_client4 /
//--- _perclient_permo4 (Summary per Payroll Group: one sheet, a row per client). Account columns: those used, in account order; the
//--- column title is the account's short description when it has one ---
public static class PayrollSheets
{
    private const string Money = "_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)";

    private static List<(string Acct, string Title)> Accounts(List<RSheetLineModel> lines, char type, bool shortDesc)
        => lines.Where(l => l.Acctnumber!.StartsWith(type.ToString(), StringComparison.OrdinalIgnoreCase))
                .GroupBy(l => l.Acctnumber!).OrderBy(g => g.Key)
                .Select(g => (g.Key, shortDesc && !string.IsNullOrWhiteSpace(g.First().Shortdesc) ? g.First().Shortdesc!.Trim() : (g.First().Acctname ?? g.Key))).ToList();

    private static List<(string Key, string Name)> Keys(List<RSheetLineModel> lines)
        => lines.GroupBy(l => l.Key!).Select(g => (g.Key, g.First().Name ?? "")).OrderBy(k => k.Item2).ToList();

    private static double Amt(List<RSheetLineModel> lines, string key, string acct)
        => lines.Where(l => l.Key == key && l.Acctnumber == acct).Sum(l => l.Amount);

    private static string Addr(ExcelWorksheet ws, int r, int c) => ws.Cells[r, c].Address;

    //--- Payroll Summary of one TRN: sheet 1 Earnings, sheet 2 Deductions ---
    public static byte[] Summary(List<RSheetLineModel> lines, string periodText)
    {
        using var pkg = new ExcelPackage();
        string group = lines.Select(l => l.Clname).FirstOrDefault(c => !string.IsNullOrWhiteSpace(c)) ?? "";
        foreach (var (type, sheet, total) in new[] { ('E', "Earnings", "Total Earnings"), ('D', "Deductions", "Total Deductions") })
        {
            var ws = pkg.Workbook.Worksheets.Add(sheet);
            var acc = Accounts(lines, type, true);
            var keys = Keys(lines);
            int totCol = acc.Count + 4;

            ws.Cells[1, 1].Value = "Payroll Summary Report";
            ws.Cells[2, 1].Value = periodText;
            ws.Cells[4, 1].Value = "Payroll Group : " + group;
            ws.Cells[4, 1].Style.Font.Bold = true;

            const int hdr = 6;
            ws.Cells[hdr, 1].Value = "No."; ws.Cells[hdr, 2].Value = "Emp. No"; ws.Cells[hdr, 3].Value = "Emp Name";
            for (int i = 0; i < acc.Count; i++) ws.Cells[hdr, 4 + i].Value = acc[i].Title;
            ws.Cells[hdr, totCol].Value = total;
            ws.Cells[hdr, 1, hdr, totCol].Style.Font.Bold = true;
            ws.Cells[hdr, 1, hdr, totCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            int r = hdr;
            foreach (var (key, name) in keys)
            {
                r++;
                ws.Cells[r, 1].Value = $"{r - hdr})";
                ws.Cells[r, 2].Value = key;
                ws.Cells[r, 3].Value = name;
                for (int i = 0; i < acc.Count; i++) ws.Cells[r, 4 + i].Value = Amt(lines, key, acc[i].Acct);
                ws.Cells[r, totCol].Formula = $"SUM({Addr(ws, r, 4)}:{Addr(ws, r, totCol - 1)})";
            }

            //--- Total Amount row ---
            int tr = r + 1;
            ws.Cells[tr, 3].Value = "Total Amount";
            for (int c = 4; c <= totCol; c++)
                ws.Cells[tr, c].Formula = $"SUM({Addr(ws, hdr + 1, c)}:{Addr(ws, r, c)})";
            ws.Cells[tr, 3, tr, totCol].Style.Font.Bold = true;
            ws.Cells[tr, 3, tr, totCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[tr, 3, tr, totCol].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            ws.Cells[hdr + 1, 4, tr, totCol].Style.Numberformat.Format = Money;
            ws.Column(totCol).Style.Font.Bold = true;
            ws.Calculate();
            ws.Cells[1, 1, tr, totCol].AutoFitColumns();
            ws.Column(1).Width = 5;
        }
        return pkg.GetAsByteArray();
    }

    //--- Payroll Register, 2 liner of one TRN: company, title, group, period, acknowledgement, then the header (earnings names, deductions
    //--- names), and two rows per employee: earnings, deductions; Tot. Earnings, Tot. Deduction and Net Pay (merged over the two rows) ---
    public static byte[] Register2Liner(List<RSheetLineModel> lines, string coName, string coAddress, string periodText)
    {
        using var pkg = new ExcelPackage();
        var ws = pkg.Workbook.Worksheets.Add("Payroll Register");
        var e = Accounts(lines, 'E', true);
        var d = Accounts(lines, 'D', true);
        var keys = Keys(lines);
        int max = Math.Max(e.Count, d.Count);
        int totCol = max + 5, netCol = max + 6, last = netCol;
        string group = lines.Select(l => l.Clname).FirstOrDefault(c => !string.IsNullOrWhiteSpace(c)) ?? "";

        void Merged(int row, string text, float size, bool bold)
        {
            ws.Cells[row, 1, row, last].Merge = true;
            ws.Cells[row, 1].Value = text;
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, 1].Style.Font.Size = size;
            ws.Cells[row, 1].Style.Font.Bold = bold;
        }
        Merged(1, coName, 14, true);
        Merged(2, coAddress, 12, false);
        Merged(4, "P A Y R O L L   R E G I S T E R", 13, false);
        ws.Cells[6, 1].Value = "Payroll Grp : ";
        ws.Cells[6, 3].Value = group;
        ws.Cells[6, 3].Style.Font.Bold = true;
        ws.Cells[7, 1].Value = "PERIOD COVERED : " + periodText;
        ws.Cells[9, 1].Value = $"We hereby acknowledge to have received from {coName}, the sum specified opposite our respective name as full compensation for our service rendered. ";

        //--- Header: row 11 earnings names, row 12 deductions names ---
        const int h1 = 11, h2 = 12;
        ws.Cells[h2, 1].Value = "No."; ws.Cells[h2, 2].Value = "Emp. No"; ws.Cells[h2, 3].Value = "Emp Name"; ws.Cells[h2, 4].Value = "Signature";
        for (int i = 0; i < e.Count; i++) ws.Cells[h1, 5 + i].Value = e[i].Title;
        for (int i = 0; i < d.Count; i++) ws.Cells[h2, 5 + i].Value = d[i].Title;
        ws.Cells[h1, totCol].Value = "Tot. Earnings";
        ws.Cells[h2, totCol].Value = "Tot. Deduction";
        ws.Cells[h1, netCol].Value = "Net Pay";
        ws.Cells[h1, 1, h2, last].Style.Font.Bold = true;
        ws.Cells[h1, 1, h2, last].Style.WrapText = true;
        ws.Cells[h1, 1, h2, last].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells[h1, 1, h2, last].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        ws.Cells[h1, 1, h2, last].Style.Fill.PatternType = ExcelFillStyle.Solid;
        ws.Cells[h1, 1, h2, last].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(210, 210, 210));
        ws.Cells[h2, 5, h2, last].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(100, 100, 255));
        ws.Cells[h1, netCol, h2, netCol].Merge = true;

        //--- Two rows per employee ---
        int er = h2;
        foreach (var (key, name) in keys)
        {
            int earn = er + 1, ded = er + 2;
            er = ded;
            ws.Cells[ded, 1].Value = $"{(ded - h2) / 2})";
            ws.Cells[ded, 2].Value = key;
            ws.Cells[ded, 3].Value = name;
            for (int i = 0; i < e.Count; i++) ws.Cells[earn, 5 + i].Value = Amt(lines, key, e[i].Acct);
            for (int i = 0; i < d.Count; i++) ws.Cells[ded, 5 + i].Value = Amt(lines, key, d[i].Acct);
            ws.Cells[earn, totCol].Formula = $"SUM({Addr(ws, earn, 5)}:{Addr(ws, earn, totCol - 1)})";
            ws.Cells[ded, totCol].Formula = $"SUM({Addr(ws, ded, 5)}:{Addr(ws, ded, totCol - 1)})";
            ws.Cells[earn, netCol, ded, netCol].Merge = true;
            ws.Cells[earn, netCol].Formula = $"{Addr(ws, earn, totCol)}-{Addr(ws, ded, totCol)}";
            ws.Cells[ded, 5, ded, totCol].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(100, 100, 255));
            ws.Cells[earn, 1, ded, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }
        int lastRow = Math.Max(er, h2);

        ws.Cells[h1, 4, lastRow, last].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        ws.Cells[h1, 4, lastRow, last].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        ws.Cells[h1, 4, lastRow, last].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        ws.Cells[h1, 4, lastRow, last].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        ws.Cells[h1, 5, lastRow, last].Style.Numberformat.Format = Money;
        ws.Cells[h1, netCol, lastRow, netCol].Style.Font.Bold = true;
        ws.Cells[h1, netCol, lastRow, netCol].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
        ws.Cells[h1, netCol, lastRow, netCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells[h1, netCol, lastRow, netCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        ws.Column(1).Width = 4;
        ws.Column(2).Width = 9;
        ws.Column(3).Width = 30;
        ws.Column(4).Width = 20;
        for (int c = 5; c <= last; c++) ws.Column(c).Width = 12;
        ws.PrinterSettings.FitToPage = true;
        ws.PrinterSettings.FitToWidth = 1;
        ws.PrinterSettings.FitToHeight = 0;
        return pkg.GetAsByteArray();
    }

    //--- Payroll Summary per Payroll Group (a period, or a month): a row per client, the earnings, Total Earnings, the deductions, Total
    //--- Deductions and Net Pay; account titles are the account names ---
    public static byte[] ClientSummary(List<RSheetLineModel> lines, string title, string subtitle)
    {
        using var pkg = new ExcelPackage();
        var ws = pkg.Workbook.Worksheets.Add("Payroll Summary");
        var e = Accounts(lines, 'E', false);
        var d = Accounts(lines, 'D', false);
        var keys = Keys(lines);
        int totE = e.Count + 4, totD = e.Count + d.Count + 5, net = totD + 1;

        ws.Cells[1, 1].Value = title;
        ws.Cells[2, 1].Value = subtitle;
        const int hdr = 4;
        ws.Cells[hdr, 1].Value = "No."; ws.Cells[hdr, 2].Value = "Grp Code"; ws.Cells[hdr, 3].Value = "Group Name";
        for (int i = 0; i < e.Count; i++) ws.Cells[hdr, 4 + i].Value = e[i].Title;
        ws.Cells[hdr, totE].Value = "Total Earnings";
        for (int i = 0; i < d.Count; i++) ws.Cells[hdr, totE + 1 + i].Value = d[i].Title;
        ws.Cells[hdr, totD].Value = "Total Deductions";
        ws.Cells[hdr, net].Value = "Net Pay";
        ws.Cells[hdr, 1, hdr, net].Style.Font.Bold = true;
        ws.Cells[hdr, 1, hdr, net].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        int r = hdr;
        foreach (var (key, name) in keys)
        {
            r++;
            ws.Cells[r, 1].Value = $"{r - hdr})";
            ws.Cells[r, 2].Value = key;
            ws.Cells[r, 3].Value = name;
            for (int i = 0; i < e.Count; i++) ws.Cells[r, 4 + i].Value = Amt(lines, key, e[i].Acct);
            for (int i = 0; i < d.Count; i++) ws.Cells[r, totE + 1 + i].Value = Amt(lines, key, d[i].Acct);
            ws.Cells[r, totE].Formula = $"SUM({Addr(ws, r, 4)}:{Addr(ws, r, totE - 1)})";
            ws.Cells[r, totD].Formula = $"SUM({Addr(ws, r, totE + 1)}:{Addr(ws, r, totD - 1)})";
            ws.Cells[r, net].Formula = $"{Addr(ws, r, totE)}-{Addr(ws, r, totD)}";
        }

        //--- Total row; the grid around the whole table ---
        int tr = r + 1;
        for (int c = 4; c <= net; c++) ws.Cells[tr, c].Formula = $"SUM({Addr(ws, hdr + 1, c)}:{Addr(ws, r, c)})";
        ws.Cells[tr, 1, tr, net].Style.Font.Bold = true;
        ws.Cells[tr, 1, tr, net].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
        ws.Cells[hdr, 1, r, net].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        ws.Cells[hdr, 1, r, net].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        ws.Cells[hdr, 1, r, net].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        ws.Cells[hdr, 1, r, net].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        ws.Cells[hdr + 1, 4, tr, net].Style.Numberformat.Format = Money;
        ws.Calculate();
        ws.Cells[1, 1, tr, net].AutoFitColumns();
        ws.Column(1).Width = 5;
        return pkg.GetAsByteArray();
    }
}
