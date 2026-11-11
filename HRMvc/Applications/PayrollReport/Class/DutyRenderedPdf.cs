using System.Data;
using HRApiLibrary.Modules._1303O;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace HRMvc.Applications.PayrollReport.Class;

public static class DutyRenderedPdf
{
    public static byte[] Build(M1303_Result res, string coverage, string group, string format = "PDF")
    {
        int nAcct = res.Accts.Count;
        int nCol = 5 + nAcct;
        var dt = new DataTable();
        for (int i = 0; i < nCol; i++) dt.Columns.Add("c" + i, typeof(string));
        dt.Columns.Add("kind", typeof(int));

        string[] Row(M1303_Row r)
        {
            var a = new string[nCol];
            a[0] = r.EmpName.Trim(); a[1] = r.Prd; a[2] = r.Rate.ToString("N2"); a[3] = r.Th.ToString("N2");
            for (int i = 0; i < nAcct; i++) a[4 + i] = Z(r.Amt.GetValueOrDefault(res.Accts[i].AcctNumber));
            a[4 + nAcct] = Z(r.Gross);
            return a;
        }
        void Add(string[] a, int kind) { var row = dt.NewRow(); for (int i = 0; i < nCol; i++) row[i] = a[i] ?? ""; row["kind"] = kind; dt.Rows.Add(row); }

        string prd = res.Rows[0].Prd;
        var sums = new decimal[nCol];
        void Total()
        {
            var a = new string[nCol];
            a[0] = "Period Total";
            a[2] = ""; a[3] = Z(sums[3]);
            for (int i = 0; i < nAcct; i++) a[4 + i] = Z(sums[4 + i]);
            a[4 + nAcct] = Z(sums[4 + nAcct]);
            Add(a, 1);
            Array.Clear(sums);
        }
        foreach (var r in res.Rows)
        {
            if (r.Prd != prd) { Total(); prd = r.Prd; }
            Add(Row(r), 0);
            sums[3] += r.Th;
            for (int i = 0; i < nAcct; i++) sums[4 + i] += r.Amt.GetValueOrDefault(res.Accts[i].AcctNumber);
            sums[4 + nAcct] += r.Gross ?? 0;
        }
        Total();

        var report = new Telerik.Reporting.Report();
        report.PageSettings.Landscape = true;
        report.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Legal;
        report.PageSettings.Margins = new MarginsU(Unit.Inch(0.4), Unit.Inch(0.4), Unit.Inch(0.4), Unit.Inch(0.4));
        var ods = new ObjectDataSource { DataSource = dt };

        double usable = 14 - 0.8;
        double[] w = new double[nCol];
        w[0] = 1.9; w[1] = 0.6; w[2] = 0.65; w[3] = 0.55;
        double rest = (usable - w[0] - w[1] - w[2] - w[3]) / (nAcct + 1);
        for (int i = 4; i < nCol; i++) w[i] = rest;

        var titles = new List<string> { "Emp. Name", "Period", "Rate", "TH" };
        titles.AddRange(res.Accts.Select(a => a.Caption));
        titles.Add("Gross Salary");

        var hdr = new PageHeaderSection { Height = Unit.Inch(1.15) };
        hdr.Items.Add(Tb("Duty rendered Report", 0, 0, usable, 0.3, 14, true));
        hdr.Items.Add(Tb("For the period covering " + coverage, 0, 0.3, usable, 0.25, 9, false));
        hdr.Items.Add(Tb("Payroll Group : " + group, 0, 0.55, usable, 0.25, 9, false));
        report.Items.Add(hdr);

        var det = new DetailSection { Height = Unit.Inch(0.3) };
        var tbl = new Telerik.Reporting.Table { Location = new PointU(Unit.Inch(0), Unit.Inch(0)), Size = new SizeU(Unit.Inch(usable), Unit.Inch(0.4)) };
        double x = 0;
        for (int i = 0; i < nCol; i++)
        {
            tbl.Body.Columns.Add(new TableBodyColumn(Unit.Inch(w[i])));
            var body = Tb("= Fields.c" + i, 0, 0, w[i], 0.2, 7, false, i >= 2);
            var fr = new FormattingRule { Style = { Font = { Bold = true } } };
            fr.Filters.Add(new Filter("= Fields.kind", FilterOperator.Equal, "1"));
            body.ConditionalFormatting.Add(fr);
            tbl.Body.SetCellContent(0, i, body);
            var h = Tb(titles[i], x, 0.85, w[i], 0.25, 7, true, i >= 2);
            h.Style.BackgroundColor = System.Drawing.Color.FromArgb(0xF4, 0xF6, 0xF9);
            hdr.Items.Add(h);
            x += w[i];
            tbl.ColumnGroups.Add(new TableGroup());
        }
        tbl.Body.Rows.Add(new TableBodyRow(Unit.Inch(0.2)));
        var rg = new TableGroup { Name = "detail" };
        rg.Groupings.Add(new Grouping());
        tbl.RowGroups.Add(rg);
        tbl.DataSource = ods;
        det.Items.Add(tbl);
        report.Items.Add(det);

        var pf = new PageFooterSection { Height = Unit.Inch(0.25) };
        pf.Items.Add(Tb("= 'Page ' + PageNumber + ' of ' + PageCount", 0, 0, usable, 0.2, 7, false, true));
        report.Items.Add(pf);

        var processor = new Telerik.Reporting.Processing.ReportProcessor();
        return processor.RenderReport(format, new InstanceReportSource { ReportDocument = report }, null).DocumentBytes;
    }

    static string Z(decimal? v) => v is null or 0 ? "" : v.Value.ToString("N2");

    static Telerik.Reporting.TextBox Tb(string value, double x, double y, double w, double h, double size, bool bold, bool right = false)
    {
        var t = new Telerik.Reporting.TextBox
        {
            Value = value,
            Location = new PointU(Unit.Inch(x), Unit.Inch(y)),
            Size = new SizeU(Unit.Inch(w), Unit.Inch(h)),
        };
        t.Style.Font.Size = Unit.Point(size);
        t.Style.Font.Bold = bold;
        t.Style.TextAlign = right ? HorizontalAlign.Right : HorizontalAlign.Left;
        t.Style.VerticalAlign = VerticalAlign.Middle;
        t.CanGrow = false;
        return t;
    }
}
