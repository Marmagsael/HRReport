using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace HRMvc.Applications.PisModules.Views.Blazor;

public class GroupRecallData
{
    public string CompName = "", CompAdd = "", Client = "", ControlNo = "", PrepDate = "", ReportOnD = "", ReportOnT = "", Reason = "";
    public string Staff1 = "", Pos1 = "", Staff2 = "", Pos2 = "";
    public bool ShowTime, Show1 = true, Show2 = true;
    public byte[]? Logo;
    public List<(string EmpNo, string EmpName)> Employees = new();
}

public static class GroupRecallPdf
{
    // -- The Group Recall Order of the FoxPro grpro.frx : page header, one detail row per employee, page footer with the signatories (inches) --
    public static byte[] Build(GroupRecallData d)
    {
        var report = new Telerik.Reporting.Report();
        report.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
        report.PageSettings.Margins = new MarginsU(Unit.Inch(0), Unit.Inch(0), Unit.Inch(0), Unit.Inch(0));
        report.DataSource = new ObjectDataSource
        {
            DataSource = d.Employees.Select(e => new { EmpNo = e.EmpNo, EmpName = e.EmpName }).ToList()
        };

        var head = new PageHeaderSection { Height = Unit.Inch(2.677) };
        var det = new DetailSection { Height = Unit.Inch(0.188) };
        var foot = new PageFooterSection { Height = Unit.Inch(1.9) };
        report.Items.Add(head);
        report.Items.Add(det);
        report.Items.Add(foot);

        void T(ReportItemBase.ItemCollection into, string v, double top, double left, double w, double h, double size = 8, bool bold = false, string font = "Tahoma", bool center = false, bool italic = false)
        {
            var t = new Telerik.Reporting.TextBox
            {
                Value = v,
                Location = new PointU(Unit.Inch(left), Unit.Inch(top)),
                Size = new SizeU(Unit.Inch(w), Unit.Inch(h)),
                CanGrow = false
            };
            t.Style.Font.Name = font;
            t.Style.Font.Size = Unit.Point(size);
            t.Style.Font.Bold = bold;
            t.Style.Font.Italic = italic;
            t.Style.TextAlign = center ? HorizontalAlign.Center : HorizontalAlign.Left;
            t.Style.VerticalAlign = VerticalAlign.Top;
            into.Add(t);
        }
        void L(ReportItemBase.ItemCollection into, double top, double left, double w, double thick)
        {
            var t = new Telerik.Reporting.TextBox
            {
                Value = "",
                Location = new PointU(Unit.Inch(left), Unit.Inch(top)),
                Size = new SizeU(Unit.Inch(w), Unit.Inch(thick))
            };
            t.Style.BackgroundColor = System.Drawing.Color.Black;
            into.Add(t);
        }

        if (d.Logo != null)
        {
            System.Drawing.Image? img = null;
            try { img = System.Drawing.Image.FromStream(new MemoryStream(d.Logo)); } catch { }
            if (img != null)
                head.Items.Add(new Telerik.Reporting.PictureBox
                {
                    Value = img,
                    Sizing = ImageSizeMode.ScaleProportional,
                    Location = new PointU(Unit.Inch(0.698), Unit.Inch(0.198)),
                    Size = new SizeU(Unit.Inch(0.79), Unit.Inch(0.75))
                });
        }

        var h = head.Items;
        T(h, d.CompName, 0.344, 1.719, 5.062, 0.25, 12, true, "Trebuchet MS");
        T(h, d.CompAdd, 0.615, 1.688, 5.125, 0.156);
        T(h, "Run Date:", 0.875, 5.771, 0.51, 0.135);
        T(h, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), 0.875, 6.375, 1.385, 0.156);
        T(h, "Group Recall Order", 0.792, 3.458, 1.583, 0.24, 13, true, "Trebuchet MS");
        T(h, "Client Name:", 1.302, 0.458, 0.635, 0.135);
        T(h, d.Client, 1.292, 1.208, 4.552, 0.156);
        T(h, "Control Number:", 1.302, 5.875, 0.823, 0.135);
        T(h, d.ControlNo, 1.292, 6.75, 1.01, 0.156);
        T(h, "Date Prepared:", 1.5, 5.875, 0.771, 0.135);
        T(h, d.PrepDate, 1.49, 6.75, 1.01, 0.156);
        T(h, "Report On", 1.531, 0.458, 0.521, 0.135);
        T(h, "Date", 1.677, 0.458, 0.24, 0.135);
        if (d.ShowTime) T(h, "Time", 1.677, 1.375, 0.229, 0.135);
        T(h, d.ReportOnD, 1.844, 0.458, 0.802, 0.156);
        if (d.ShowTime) T(h, d.ReportOnT, 1.844, 1.375, 0.708, 0.156);
        T(h, "Reason For Recall /", 1.708, 4.125, 0.969, 0.135);
        T(h, "Instruction", 1.833, 4.125, 0.542, 0.135);
        T(h, d.Reason, 1.698, 5.135, 2.72, 0.4);
        L(h, 2.271, 0.458, 7.396, 0.0104);
        T(h, "To Officially Recall", 2.312, 0.458, 0.906, 0.135);
        T(h, "Emp No.", 2.448, 0.875, 0.417, 0.135);
        T(h, "Name", 2.448, 1.5, 0.281, 0.135);
        L(h, 2.615, 0.458, 7.396, 0.0104);
        L(h, 2.656, 0.458, 7.396, 0.0208);

        T(det.Items, "=Fields.EmpNo", 0.0, 0.875, 0.51, 0.156);
        T(det.Items, "=Fields.EmpName", 0.0, 1.5, 2.385, 0.156);

        var f = foot.Items;
        L(f, 0.15, 0.458, 7.396, 0.0104);
        L(f, 0.192, 0.458, 7.396, 0.0208);
        T(f, "Received by:", 0.629, 0.5, 0.656, 0.135);
        T(f, "signature over printed name", 0.775, 1.188, 1.417, 0.135);
        L(f, 0.754, 1.156, 1.51, 0.0104);
        T(f, "Prepared by:", 0.629, 2.708, 0.656, 0.135);
        T(f, "Approved by:", 0.629, 5.125, 0.688, 0.135);
        if (d.Show1)
        {
            T(f, d.Staff1, 0.567, 3.0, 2.4, 0.156, 8, true, "Tahoma", true);
            T(f, d.Pos1, 0.796, 3.0, 2.4, 0.156, 8, false, "Tahoma", true);
            L(f, 0.744, 3.396, 1.635, 0.0104);
        }
        if (d.Show2)
        {
            T(f, d.Staff2, 0.567, 5.43, 2.4, 0.156, 8, true, "Tahoma", true);
            T(f, d.Pos2, 0.796, 5.43, 2.4, 0.156, 8, false, "Tahoma", true);
            L(f, 0.744, 5.823, 1.635, 0.0104);
        }
        T(f, "=\"Page \" + PageNumber + \" of \" + PageCount", 1.18, 0.5, 1.635, 0.156, 8, false, "Tahoma", false, true);
        T(f, "D-OD-GRO-F-004", 1.442, 0.5, 0.875, 0.135);

        return new Telerik.Reporting.Processing.ReportProcessor()
            .RenderReport("PDF", new InstanceReportSource { ReportDocument = report }, null).DocumentBytes;
    }
}
