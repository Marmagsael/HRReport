using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace HRMvc.Applications.PisModules.Views.Blazor;

public class RtuData
{
    public string CompName = "", CompAdd = "", Client = "", ControlNo = "", PrepDate = "", AssDate = "", EmpNo = "", EmpName = "", ReportOnD = "", ReportOnT = "", Reason = "";
    public string Staff1 = "", Pos1 = "", Staff2 = "", Pos2 = "";
    public bool ShowTime, Show1 = true, Show2 = true;
    public byte[]? Logo;
}

public static class RtuPdf
{
    const double U = 0.096;

    public static byte[] Build(RtuData d)
    {
        var report = new Telerik.Reporting.Report();
        report.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
        report.PageSettings.Margins = new MarginsU(Unit.Inch(0), Unit.Inch(0), Unit.Inch(0), Unit.Inch(0));
        var det = new DetailSection { Height = Unit.Inch(10.4) };
        report.Items.Add(det);

        foreach (double dy in new[] { 0.0, 3.958 })
        {
            void T(string v, double top, double left, double w, double h, double size = 8, bool bold = false, string font = "Tahoma", bool center = false)
            {
                var t = new Telerik.Reporting.TextBox
                {
                    Value = v,
                    Location = new PointU(Unit.Inch(left * U), Unit.Inch(top * U + dy)),
                    Size = new SizeU(Unit.Inch(w * U), Unit.Inch(h * U)),
                    CanGrow = false
                };
                t.Style.Font.Name = font;
                t.Style.Font.Size = Unit.Point(size);
                t.Style.Font.Bold = bold;
                t.Style.TextAlign = center ? HorizontalAlign.Center : HorizontalAlign.Left;
                t.Style.VerticalAlign = VerticalAlign.Top;
                det.Items.Add(t);
            }
            void L(double top, double left, double w, double thick)
            {
                var t = new Telerik.Reporting.TextBox
                {
                    Value = "",
                    Location = new PointU(Unit.Inch(left), Unit.Inch(top + dy)),
                    Size = new SizeU(Unit.Inch(w), Unit.Inch(thick))
                };
                t.Style.BackgroundColor = System.Drawing.Color.Black;
                det.Items.Add(t);
            }

            if (d.Logo != null)
            {
                System.Drawing.Image? img = null;
                try { img = System.Drawing.Image.FromStream(new MemoryStream(d.Logo)); } catch { }
                if (img != null)
                    det.Items.Add(new Telerik.Reporting.PictureBox
                    {
                        Value = img,
                        Sizing = ImageSizeMode.ScaleProportional,
                        Location = new PointU(Unit.Inch(0.698), Unit.Inch(0.427 + dy)),
                        Size = new SizeU(Unit.Inch(0.79), Unit.Inch(0.75))
                    });
            }

            T(d.CompName, 5.97, 15.73, 52.73, 2.6, 12, true, "Trebuchet MS");
            T(d.CompAdd, 8.79, 15.73, 53.39, 1.63);
            T("Run Date:", 11.39, 58.59, 5.32, 1.41);
            T(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), 11.28, 65.1, 14.43, 1.63);
            T("Return To Unit", 10.63, 37.87, 12.8, 2.5, 13, true, "Trebuchet MS");

            T("Client Name:", 15.95, 4.77, 6.62, 1.41);
            T(d.Client, 15.84, 12.59, 44.81, 1.63);
            T("Control Number:", 15.95, 59.24, 8.57, 1.41);
            T(d.ControlNo, 15.84, 68.36, 10.53, 1.63);
            T("DateAssigned:", 18.01, 4.77, 7.6, 1.41);
            T(d.AssDate, 18.01, 12.59, 10.53, 1.63);
            T("Date Prepared:", 18.01, 59.24, 8.03, 1.41);
            T(d.PrepDate, 17.9, 68.36, 10.53, 1.63);

            T("Emp No.", 22.24, 4.77, 4.34, 1.41);
            T("Name", 22.24, 11.28, 2.93, 1.41);
            T("Report On", 20.83, 27.67, 5.43, 1.41);
            T("Date", 22.24, 27.67, 2.5, 1.41);
            if (d.ShowTime) T("Time", 22.24, 34.94, 2.39, 1.41);
            T("Reason / Instruction", 22.35, 42.43, 10.63, 1.41);
            T(d.EmpNo, 25.07, 4.77, 5.32, 1.63);
            T(d.EmpName, 25.07, 11.28, 15.73, 1.63);
            T(d.ReportOnD, 25.07, 27.67, 6.62, 1.63);
            if (d.ShowTime) T(d.ReportOnT, 25.07, 34.94, 6.62, 1.63);
            T(d.Reason, 25.07, 42.43, 37.11, 4.45);

            T("Received by:", 34.18, 5.21, 6.84, 1.41);
            T("signature over printed name", 35.7, 12.37, 14.76, 1.41);
            T("Prepared by:", 34.18, 28.21, 6.84, 1.41);
            T("Approved by:", 34.18, 53.39, 7.16, 1.41);
            if (d.Show1) { T(d.Staff1, 33.53, 31.4, 25, 1.63, 8, true, "Tahoma", true); T(d.Pos1, 35.92, 31.4, 25, 1.63, 8, false, "Tahoma", true); }
            if (d.Show2) { T(d.Staff2, 33.53, 56.7, 25, 1.63, 8, true, "Tahoma", true); T(d.Pos2, 35.92, 56.7, 25, 1.63, 8, false, "Tahoma", true); }
            T("D-OD-RTU-F-017", 38.85, 5.1, 9.22, 1.41);

            L(1.9583, 0.4583, 7.2396, 0.0104);
            L(2.3021, 0.4583, 7.2396, 0.0104);
            L(2.34375, 0.4583, 7.2396, 0.0208);
            L(2.875, 0.4583, 7.2396, 0.0104);
            L(2.9167, 0.4583, 7.2396, 0.0208);
            L(3.40625, 1.15625, 1.5104, 0.0104);
            if (d.Show1) L(3.3958, 3.3958, 1.6354, 0.0104);
            if (d.Show2) L(3.3958, 5.8229, 1.6354, 0.0104);
            if (dy == 0.0) L(4.0625, 0.4583, 7.2396, 0.0104);
        }

        return new Telerik.Reporting.Processing.ReportProcessor()
            .RenderReport("PDF", new InstanceReportSource { ReportDocument = report }, null).DocumentBytes;
    }
}
