using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace HRMvc.Applications.PisModules.Views.Blazor;

public static class RecallPdf
{
    // -- The Recall Order of the FoxPro ro.frx : two copies on one letter page (positions in inches) --
    public static byte[] Build(RtuData d)
    {
        var report = new Telerik.Reporting.Report();
        report.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
        report.PageSettings.Margins = new MarginsU(Unit.Inch(0), Unit.Inch(0), Unit.Inch(0), Unit.Inch(0));
        var det = new DetailSection { Height = Unit.Inch(10.4) };
        report.Items.Add(det);

        foreach (double dy in new[] { 0.0, 4.34 })
        {
            void T(string v, double top, double left, double w, double h, double size = 8, bool bold = false, string font = "Tahoma", bool center = false)
            {
                var t = new Telerik.Reporting.TextBox
                {
                    Value = v,
                    Location = new PointU(Unit.Inch(left), Unit.Inch(top + dy)),
                    Size = new SizeU(Unit.Inch(w), Unit.Inch(h)),
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

            T(d.CompName, 0.573, 1.51, 5.062, 0.25, 12, true, "Trebuchet MS");
            T(d.CompAdd, 0.844, 1.51, 5.125, 0.156);
            T("Run Date:", 1.094, 5.625, 0.51, 0.135);
            T(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), 1.083, 6.25, 1.385, 0.156);
            T("Recall Order", 1.021, 3.562, 1.031, 0.24, 13, true, "Trebuchet MS");

            T("Client Name:", 1.531, 0.458, 0.635, 0.135);
            T(d.Client, 1.521, 1.208, 4.302, 0.156);
            T("Control Number:", 1.531, 5.688, 0.823, 0.135);
            T(d.ControlNo, 1.521, 6.562, 1.01, 0.156);
            if (d.AssDate != "")
            {
                T("DateAssigned:", 1.729, 0.458, 0.729, 0.135);
                T(d.AssDate, 1.729, 1.208, 1.01, 0.156);
            }
            T("Date Prepared:", 1.729, 5.688, 0.771, 0.135);
            T(d.PrepDate, 1.719, 6.562, 1.01, 0.156);

            T("To Officially Recall", 2.0, 0.458, 0.906, 0.135);
            T("Emp No.", 2.135, 0.458, 0.417, 0.135);
            T("Name", 2.135, 1.083, 0.281, 0.135);
            T("Report On", 2.0, 2.656, 0.521, 0.135);
            T("Date", 2.135, 2.656, 0.24, 0.135);
            if (d.ShowTime) T("Time", 2.135, 3.354, 0.229, 0.135);
            T("Reason For Recall / Instruction", 2.146, 4.073, 1.542, 0.135);
            T(d.EmpNo, 2.406, 0.458, 0.51, 0.156);
            T(d.EmpName, 2.406, 1.083, 1.51, 0.156);
            T(d.ReportOnD, 2.406, 2.656, 0.635, 0.156);
            if (d.ShowTime) T(d.ReportOnT, 2.406, 3.354, 0.635, 0.156);
            T(d.Reason, 2.406, 4.073, 3.562, 0.427);

            T("Received by:", 3.333, 0.5, 0.656, 0.135);
            T("signature over printed name", 3.479, 1.188, 1.417, 0.135);
            T("Prepared by:", 3.333, 2.708, 0.656, 0.135);
            T("Approved by:", 3.333, 5.125, 0.688, 0.135);
            if (d.Show1) { T(d.Staff1, 3.271, 3.0, 2.4, 0.156, 8, true, "Tahoma", true); T(d.Pos1, 3.5, 3.0, 2.4, 0.156, 8, false, "Tahoma", true); }
            if (d.Show2) { T(d.Staff2, 3.271, 5.43, 2.4, 0.156, 8, true, "Tahoma", true); T(d.Pos2, 3.5, 5.43, 2.4, 0.156, 8, false, "Tahoma", true); }
            T("D-OD-RO-F-003", dy == 0.0 ? 4.042 : 3.879, 0.479, 0.802, 0.135);

            L(1.958, 0.458, 7.24, 0.0104);
            L(2.302, 0.458, 7.24, 0.0104);
            L(2.344, 0.458, 7.24, 0.0208);
            L(2.875, 0.458, 7.24, 0.0104);
            L(2.917, 0.458, 7.24, 0.0208);
            L(3.458, 1.156, 1.51, 0.0104);
            if (d.Show1) L(3.448, 3.396, 1.635, 0.0104);
            if (d.Show2) L(3.448, 5.823, 1.635, 0.0104);
            if (dy == 0.0) L(4.427, 0.458, 7.24, 0.0104);
        }

        return new Telerik.Reporting.Processing.ReportProcessor()
            .RenderReport("PDF", new InstanceReportSource { ReportDocument = report }, null).DocumentBytes;
    }
}
