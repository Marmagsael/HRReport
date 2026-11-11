using System.Data;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace HRMvc.Applications.PayrollReport.Class;

public class PdfCol { public string Title { get; set; } = ""; public double Width { get; set; } public bool Right { get; set; } }

public static class TablePdf
{
    // widths in inches; rows are pre-formatted strings; kind 1 = bold row
    public static byte[] Build(string title, List<string> subtitles, List<PdfCol> cols, List<(string[] Cells, int Kind)> rows, bool landscape = true, System.Drawing.Printing.PaperKind paper = System.Drawing.Printing.PaperKind.Legal)
    {
        int n = cols.Count;
        var dt = new DataTable();
        for (int i = 0; i < n; i++) dt.Columns.Add("c" + i, typeof(string));
        dt.Columns.Add("kind", typeof(int));
        foreach (var (cells, kind) in rows)
        {
            var r = dt.NewRow();
            for (int i = 0; i < n; i++) r[i] = i < cells.Length ? cells[i] ?? "" : "";
            r["kind"] = kind;
            dt.Rows.Add(r);
        }

        var report = new Telerik.Reporting.Report();
        report.PageSettings.Landscape = landscape;
        report.PageSettings.PaperKind = paper;
        report.PageSettings.Margins = new MarginsU(Unit.Inch(0.4), Unit.Inch(0.4), Unit.Inch(0.4), Unit.Inch(0.4));
        double usable = (landscape ? 14 : 8.5) - 0.8;
        double sum = cols.Sum(c => c.Width);
        double k = usable / sum;

        double hy = 0.3 + 0.25 * subtitles.Count;
        var hdr = new PageHeaderSection { Height = Unit.Inch(hy + 0.3) };
        hdr.Items.Add(Tb(title, 0, 0, usable, 0.3, 14, true));
        for (int i = 0; i < subtitles.Count; i++) hdr.Items.Add(Tb(subtitles[i], 0, 0.3 + 0.25 * i, usable, 0.25, 9, false));
        report.Items.Add(hdr);

        var ods = new ObjectDataSource { DataSource = dt };
        var det = new DetailSection { Height = Unit.Inch(0.3) };
        var tbl = new Telerik.Reporting.Table { Location = new PointU(Unit.Inch(0), Unit.Inch(0)), Size = new SizeU(Unit.Inch(usable), Unit.Inch(0.4)) };
        double x = 0;
        for (int i = 0; i < n; i++)
        {
            double w = cols[i].Width * k;
            tbl.Body.Columns.Add(new TableBodyColumn(Unit.Inch(w)));
            var body = Tb("= Fields.c" + i, 0, 0, w, 0.2, 8, false, cols[i].Right);
            var fr = new FormattingRule { Style = { Font = { Bold = true } } };
            fr.Filters.Add(new Filter("= Fields.kind", FilterOperator.Equal, "1"));
            body.ConditionalFormatting.Add(fr);
            tbl.Body.SetCellContent(0, i, body);
            var h = Tb(cols[i].Title, x, hy, w, 0.25, 8, true, cols[i].Right);
            h.Style.BackgroundColor = System.Drawing.Color.FromArgb(0xF4, 0xF6, 0xF9);
            hdr.Items.Add(h);
            x += w;
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

        return new Telerik.Reporting.Processing.ReportProcessor()
            .RenderReport("PDF", new InstanceReportSource { ReportDocument = report }, null).DocumentBytes;
    }

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
