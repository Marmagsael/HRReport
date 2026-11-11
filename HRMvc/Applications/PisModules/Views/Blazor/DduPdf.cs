using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace HRMvc.Applications.PisModules.Views.Blazor;

public class DduData
{
    public string Number = "", Date = "", EmpNumber = "", EmpName = "", Client = "", Designation = "", License = "", LicExpire = "";
    public byte[]? Logo;
    public string Duty = "", FA = "", Note = "", CompName = "", CompAdd = "", SignNm = "", SignPos = "";
}

public static class DduPdf
{
    // -- The Duty Detail Order of the FoxPro ddo.frx : two copies on one letter page (positions in inches) --
    public static byte[] Build(DduData d)
    {
        var report = new Telerik.Reporting.Report();
        report.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
        report.PageSettings.Margins = new MarginsU(Unit.Inch(0), Unit.Inch(0), Unit.Inch(0), Unit.Inch(0));
        var det = new DetailSection { Height = Unit.Inch(10.4) };
        report.Items.Add(det);

        foreach (double dy in new[] { 0.0, 4.896 })
        {
            void T(string v, double top, double left, double w, double h, double size = 8, bool bold = false, string font = "Tahoma", bool center = false, bool grow = false)
            {
                var t = new Telerik.Reporting.TextBox
                {
                    Value = v,
                    Location = new PointU(Unit.Inch(left), Unit.Inch(top + dy)),
                    Size = new SizeU(Unit.Inch(w), Unit.Inch(h)),
                    CanGrow = grow
                };
                t.Style.Font.Name = font;
                t.Style.Font.Size = Unit.Point(size);
                t.Style.Font.Bold = bold;
                t.Style.TextAlign = center ? HorizontalAlign.Center : HorizontalAlign.Left;
                t.Style.VerticalAlign = VerticalAlign.Top;
                det.Items.Add(t);
            }
            void L(double top, double left, double w, double thick = 0.01)
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
                {
                    var pic = new Telerik.Reporting.PictureBox
                    {
                        Value = img,
                        Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional,
                        Location = new PointU(Unit.Inch(0.75), Unit.Inch(0.333 + dy)),
                        Size = new SizeU(Unit.Inch(0.76), Unit.Inch(0.76))
                    };
                    det.Items.Add(pic);
                }
            }
            T(d.CompName, 0.490, 1.5, 5.49, 0.22, 13, true, "Arial");
            T(d.CompAdd, 0.677, 1.5, 5.49, 0.18, 9, false, "Arial");
            T("Run Date:", 0.969, 5.375, 0.55, 0.14);
            T(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), 0.958, 6.0, 1.7, 0.16);
            T("Duty Detail Order", 0.906, 3.521, 1.7, 0.24, 13, true, "Trebuchet MS");
            T("HQS", 1.135, 0.49, 0.3, 0.14);
            T("DUTY DETAIL ORDER NO :", 1.26, 0.49, 1.4, 0.14);
            T(d.Number, 1.26, 1.875, 1.635, 0.16);
            T("Date :", 1.26, 6.25, 0.4, 0.14);
            T(d.Date, 1.26, 6.625, 1.0, 0.16);
            T("1.  Reference Section 4 & 6 of the implementing Rules and Regulations of P.D. 1866", 1.51, 0.49, 6.5, 0.14);
            T("2.  The following Guard/s are hereby assigned to render Security duties in places indicated", 1.635, 0.49, 6.5, 0.14);
            T("and hereby issued Agency or Company-owned Firearms properly identified.", 1.76, 0.677, 6.0, 0.14);

            L(1.958, 0.49, 7.51); L(2.146, 0.49, 7.51); L(2.188, 0.49, 7.51, 0.02);
            T("Emp. No.", 1.969, 0.49, 0.6, 0.14, 8, true);
            T("Name", 1.969, 1.125, 0.6, 0.14, 8, true);
            T("Place of Duty", 1.969, 3.021, 1.0, 0.14, 8, true);
            T("Duty Date", 1.969, 5.344, 0.8, 0.14, 8, true);
            T("FA's Issued", 1.969, 6.552, 0.9, 0.14, 8, true);

            T(d.EmpNumber, 2.25, 0.49, 0.6, 0.16);
            T(d.EmpName, 2.25, 1.125, 1.875, 0.16);
            T(d.Client, 2.25, 3.021, 2.271, 0.615, 8, false, "Tahoma", false, true);
            T(d.Duty, 2.25, 5.344, 1.135, 0.615, 8, false, "Tahoma", false, true);
            T(d.FA, 2.25, 6.542, 1.385, 0.615, 8, false, "Tahoma", false, true);
            T("Designation :", 2.448, 0.646, 0.75, 0.12, 7, true);
            T(d.Designation, 2.406, 1.417, 1.583, 0.15);
            T("Lic No.         :", 2.573, 0.646, 0.75, 0.12, 7, true);
            T(d.License, 2.542, 1.406, 1.594, 0.16);
            T("Expiry Date :", 2.75, 0.646, 0.75, 0.12, 7, true);
            T(d.LicExpire, 2.719, 1.406, 1.594, 0.16);
            T("Note :", 2.917, 0.646, 0.4, 0.14, 8, true);
            T(d.Note, 2.896, 1.135, 6.792, 0.292, 8, false, "Tahoma", false, true);

            L(3.219, 0.49, 7.51); L(3.26, 0.49, 7.51, 0.02);
            T("3.  Specific Instructions", 3.302, 0.49, 1.5, 0.14);
            T("a.  Security Officer or Security Guards in the Duty Detail Order must be in Proper Uniform and shall carry his firearms only from his", 3.438, 0.677, 7.2, 0.14);
            T("Agency Office to place of guard duty and vice versa.", 3.563, 0.677, 3.0, 0.14);
            T("b.  the issued Firearm/s to the Guards are licensed, a copy of which is in the possession of the guards.", 3.698, 0.677, 6.0, 0.14);
            T("4.  This Duty Detail Order serves as authority for the aforementioned Security Officers & Guards to carry their issued Firearms.", 3.823, 0.49, 7.3, 0.14);
            T("BY THE AUTHORITY OF THE CHIEF SECURITY OFFICER", 4.031, 2.875, 3.2, 0.14);
            T(d.SignNm, 4.313, 2.354, 3.771, 0.16, 8, true);
            T(d.SignPos, 4.458, 2.354, 3.771, 0.16);
            L(4.708, 0.49, 2.26);
            T("SIGNATURE OVER PRINTED NAME", 4.719, 0.75, 2.0, 0.14);
            T("D-OD-DDO-F-001", 4.875, 0.625, 1.2, 0.14);
            if (dy == 0.0) L(5.094, 0.49, 7.51);
        }

        return new Telerik.Reporting.Processing.ReportProcessor()
            .RenderReport("PDF", new InstanceReportSource { ReportDocument = report }, null).DocumentBytes;
    }
}
