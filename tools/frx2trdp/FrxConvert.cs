// Converts a FoxPro report (.frx/.frt inside the MBPay zip) to a Telerik .trdp (definition.xml in a zip).
// usage: frx2trdp convert <zip> <frx path in zip> <out.trdp> <config.json>
// config: { "Detail": "Dx2", "Header": "rh", "HeaderPrefix": "H", "Title": "...", "Params": { "Fileloc.coname": "CoName" },
//           "Replace": { "Dx2.e24@9.469": "Dx2.e34" }, "ForceSum": ["Dx2.d38"], "PageHeaderToGroup": true }
using System.IO.Compression;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

static class FrxConvert
{
    public static void Run(string[] args)
    {

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var enc = Encoding.GetEncoding(1252);
        var inv = System.Globalization.CultureInfo.InvariantCulture;

        var zip = ZipFile.OpenRead(args[0]);
        byte[] Read(string p) { var e = zip.Entries.First(x => string.Equals(x.FullName, p, StringComparison.OrdinalIgnoreCase)); using var s = e.Open(); using var m = new MemoryStream(); s.CopyTo(m); return m.ToArray(); }
        var cfg = JsonDocument.Parse(File.ReadAllText(args[3])).RootElement;
        string Cfg(string k, string d = "") => cfg.TryGetProperty(k, out var v) ? v.GetString() ?? d : d;
        var detailAlias = Cfg("Detail").ToLower();
        var headerAlias = Cfg("Header").ToLower();
        var headerPrefix = Cfg("HeaderPrefix", "H");
        var paramMap = cfg.TryGetProperty("Params", out var pm) ? pm.EnumerateObject().ToDictionary(x => x.Name.ToLower(), x => x.Value.GetString()!) : new();
        var replace = cfg.TryGetProperty("Replace", out var rp) ? rp.EnumerateObject().ToDictionary(x => x.Name.ToLower(), x => x.Value.GetString()!) : new();
        var forceSum = cfg.TryGetProperty("ForceSum", out var fs) ? fs.EnumerateArray().Select(x => x.GetString()!.ToLower()).ToHashSet() : new();
        bool pghdrToGroup = cfg.TryGetProperty("PageHeaderToGroup", out var pg) && pg.GetBoolean();
        // NoGrow: stretch fields keep their size (Telerik pushes every item below a grown text down; FoxPro does not)
        bool noGrow = cfg.TryGetProperty("NoGrow", out var ng) && ng.GetBoolean();
        // Rename: "alias.field" -> Telerik field name; Labels: label text prefix -> expression; PrintWhen: field expr -> its PRINT WHEN
        Dictionary<string, string> Map(string k) => cfg.TryGetProperty(k, out var m) ? m.EnumerateObject().ToDictionary(x => x.Name.ToLower(), x => x.Value.GetString()!) : new();
        var rename = Map("Rename");
        var labels = Map("Labels");
        var printWhen = Map("PrintWhen");
        // ParamTypes: report parameter -> Telerik type (Float, Integer...); FontParam: FRX font -> report parameter that
        // gives the font at run time (the FontMap font stays as the design-time font)
        var paramTypes = Map("ParamTypes");
        var fontParam = Map("FontParam");
        var constants = cfg.TryGetProperty("Constants", out var cs) ? cs.EnumerateObject().ToDictionary(x => x.Name.ToLower(), x => x.Value.GetString()!) : new();
        // Exprs: whole FoxPro expression (any case, spaces ignored) -> Telerik expression; FontMap: printer font -> PDF font
        string Key(string s) => Regex.Replace(s, @"\s+", "").ToLower();
        var exprs = cfg.TryGetProperty("Exprs", out var ex) ? ex.EnumerateObject().ToDictionary(x => Key(x.Name), x => x.Value.GetString()!) : new();
        var fontMap = cfg.TryGetProperty("FontMap", out var fm) ? fm.EnumerateObject().ToDictionary(x => x.Name.ToLower(), x => x.Value.GetString()!) : new();

        //--- Read the FRX table -------------------------------------------------------------------------------------------
        var dbf = Read(args[1]);
        var fpt = Read(Path.ChangeExtension(args[1], null) + ".frt");
        int bs = (fpt[6] << 8) | fpt[7];
        int nRec = BitConverter.ToInt32(dbf, 4), hl = BitConverter.ToUInt16(dbf, 8), rl = BitConverter.ToUInt16(dbf, 10);
        var cols = new List<(string N, char T, int O, int L)>(); int off = 1;
        for (int p = 32; dbf[p] != 0x0D; p += 32) { cols.Add((enc.GetString(dbf, p, 11).TrimEnd('\0'), (char)dbf[p + 11], off, dbf[p + 16])); off += dbf[p + 16]; }
        string Memo(int b) { if (b <= 0) return ""; int p = b * bs; int len = (fpt[p + 4] << 24) | (fpt[p + 5] << 16) | (fpt[p + 6] << 8) | fpt[p + 7]; return enc.GetString(fpt, p + 8, len).TrimEnd('\0'); }
        var recs = new List<Dictionary<string, string>>();
        for (int i = 0; i < nRec; i++)
        {
            var r = dbf.AsSpan(hl + i * rl, rl).ToArray();
            if (r[0] == (byte)'*') continue;
            var d = new Dictionary<string, string>();
            foreach (var c in cols)
                d[c.N] = c.T switch
                {
                    'M' or 'G' => Memo(BitConverter.ToInt32(r, c.O)),
                    'L' => r[c.O] == (byte)'T' ? "T" : "F",
                    'I' => BitConverter.ToInt32(r, c.O).ToString(),
                    _ => enc.GetString(r, c.O, c.L).Trim()
                };
            recs.Add(d);
        }
        double N(Dictionary<string, string> d, string k) => double.TryParse(d.GetValueOrDefault(k, "0"), System.Globalization.NumberStyles.Any, inv, out var v) ? v : 0;
        int I(Dictionary<string, string> d, string k) => (int)N(d, k);

        //--- Report header record: paper and orientation ----------------------------------------------------------------
        var hdr = recs.First(r => I(r, "OBJTYPE") == 1);
        // Paper / Landscape in the config override the FRX (dot-matrix reports have no PAPERSIZE)
        bool landscape = cfg.TryGetProperty("Landscape", out var ls) ? ls.GetBoolean() : hdr["EXPR"].Contains("ORIENTATION=1");
        var paperCode = Regex.Match(hdr["EXPR"], @"PAPERSIZE=(\d+)").Groups[1].Value;
        string paper = Cfg("Paper", paperCode switch { "5" => "Legal", "9" => "A4", "14" => "Folio", _ => "Letter" });
        double pageW = paper switch { "Legal" => landscape ? 14 : 8.5, "A4" => landscape ? 11.69 : 8.27, "Folio" => landscape ? 13 : 8.5, _ => landscape ? 11 : 8.5 };
        // WIDTH 0 = the printer's page width: use the right edge of the farthest object instead
        double frxW = N(hdr, "WIDTH") / 10000;
        if (frxW <= 0) frxW = recs.Where(r => I(r, "OBJTYPE") is 5 or 6 or 7 or 8).Max(r => (N(r, "HPOS") + N(r, "WIDTH")) / 10000) + 0.1;
        double sideMargin = Math.Max(0.2, Math.Round((pageW - frxW) / 2, 2));
        double reportW = Math.Min(frxW, pageW - 2 * sideMargin);

        //--- Bands: absolute start of each band in the designer (each band is followed by a 0.1927in separator bar) -----
        const double Sep = 0.19271;
        var bands = new List<(int Code, double Start, double Height, string Expr, bool PageBreak)>();
        double y = 0;
        foreach (var b in recs.Where(r => I(r, "OBJTYPE") == 9))
        {
            double h = N(b, "HEIGHT") / 10000;
            bands.Add((I(b, "OBJCODE"), y, h, b["EXPR"], b["PAGEBREAK"] == "T"));
            y += h + Sep;
        }
        // 0 title, 1 page header, 3 group header, 4 detail, 5 group footer, 7 page footer, 8 summary
        (int Code, double Start, double Height, string Expr, bool PageBreak) BandOf(double v)
        {
            var cand = bands.Where(b => v >= b.Start - 0.001 && v < b.Start + Math.Max(b.Height, 0.001)).ToList();
            if (cand.Count > 0) return cand.Last();
            return bands.Where(b => b.Start <= v).OrderBy(b => b.Start).Last();
        }

        //--- Expression translation ----------------------------------------------------------------------------------
        var warnings = new List<string>();
        string Pascal(string s) => s.Length == 0 ? s : char.ToUpper(s[0]) + s[1..];
        string FieldRef(string alias, string field)
        {
            if (constants.TryGetValue(alias.ToLower() + "." + field.ToLower(), out var k)) return k;
            if (rename.TryGetValue(alias.ToLower() + "." + field.ToLower(), out var rn)) return "Fields." + rn;
            alias = alias.ToLower();
            if (alias == detailAlias) return "Fields." + Pascal(field);
            if (alias == headerAlias) return "Fields." + headerPrefix + Pascal(field);
            if (paramMap.TryGetValue(alias + "." + field.ToLower(), out var p)) return "Parameters." + p + ".Value";
            warnings.Add($"unknown alias {alias}.{field}");
            return "Fields." + Pascal(field);
        }
        string Expr(string e)
        {
            e = e.Trim();
            if (exprs.TryGetValue(Key(e), out var mapped)) return mapped;
            if (paramMap.TryGetValue(e.ToLower(), out var p)) return "Parameters." + p + ".Value";
            e = Regex.Replace(e, @"\[([^\]]*)\]", m => "\"" + m.Groups[1].Value + "\"");                                // [text] -> "text"
            e = Regex.Replace(e, @"'([^']*)'", m => "\"" + m.Groups[1].Value + "\"");
            e = Regex.Replace(e, @"(?i)\bdatetime\(\)", "Parameters.RunDate.Value");
            e = Regex.Replace(e, @"(?i)\b_pageno\b", "PageNumber");
            e = Regex.Replace(e, @"(?i)\b_pagetotal\b", "PageCount");
            e = Regex.Replace(e, @"(?i)\b(transform|str)\(", "CStr(");
            // FoxPro memory variables (lcFinPeriod) given in Params become report parameters
            foreach (var pv in paramMap.Where(x => !x.Key.Contains('.')))
                e = Regex.Replace(e, $@"(?i)\b{Regex.Escape(pv.Key)}\b", "Parameters." + pv.Value + ".Value");
            e = Regex.Replace(e, @"(?i)\b(alltr|alltrim|alltri|trim|rtrim)\(", "Trim(");
            e = Regex.Replace(e, @"(?i)!\s*empty\(([^()]*)\)", m => $"(IsNull({m.Groups[1].Value}, \"\") <> \"\")");
            e = Regex.Replace(e, @"(?i)\bempty\(([^()]*)\)", m => $"(IsNull({m.Groups[1].Value}, \"\") = \"\")");
            e = Regex.Replace(e, @"(?i)\.and\.", " And ").Replace("==", "=");
            e = Regex.Replace(e, @"(?i)\.or\.", " Or ");
            e = Regex.Replace(e, @"\b([A-Za-z_][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*)\b", m =>
                m.Groups[1].Value is "Fields" or "Parameters" || m.Groups[2].Value == "Value" ? m.Value : FieldRef(m.Groups[1].Value, m.Groups[2].Value));
            if (Regex.IsMatch(e, @"(?i)\b(iif|left|right|substr|str|dtoc|nvl|padl|padr|upper|lower)\(")) warnings.Add("check expression: " + e);
            return e;
        }
        string Fmt(string pic, string expr)
        {
            pic = pic.Replace("\"", "").Trim();
            if (pic == "") return expr.Contains("RunDate") ? "{0:MM/dd/yy hh:mm:ss tt}" : "";
            bool z = pic.Contains("@Z");
            // "@ZI 999.99": the function codes before the space are not part of the template
            string tpl = pic.StartsWith("@") ? (pic.Contains(' ') ? pic[(pic.IndexOf(' ') + 1)..] : "") : pic;
            var num = Regex.Match(tpl, @"[9,]+(\.(9+))?");
            if (!num.Success) return expr.Contains("Sum(") ? "{0:#,##0.00}" : "";
            int dec = num.Groups[2].Value.Length;
            string prefix = tpl[..num.Index].Trim();
            string suffix = tpl[(num.Index + num.Length)..].TrimEnd();
            string f = (prefix != "" ? "'" + prefix + "'" : "") + "#,##0" + (dec > 0 ? "." + new string('0', dec) : "") + (suffix != "" ? "'" + suffix + "'" : "");
            // @( : negative numbers in parentheses
            bool paren = pic.StartsWith("@") && pic.Split(' ')[0].Contains('(');
            if (paren) return "{0:" + f + ";(" + f + ")" + (z ? ";''" : "") + "}";
            return z ? "{0:" + f + ";-" + f + ";''}" : "{0:" + f + "}";
        }
        // FoxPro picture function codes: @I center, @J right, @! upper case
        string PicCodes(string pic) { pic = pic.Replace("\"", "").Trim(); return pic.StartsWith("@") ? pic.Split(' ')[0].ToUpper() : ""; }
        string X(string s) => SecurityElement.Escape(s);
        string In(double v) => v.ToString("0.###", inv) + "in";

        //--- Section heights. An item taller than its band (FoxPro lets text overflow the band) is moved up and cut to the
        //--- band so Telerik does not grow the section; Style.Visible (not Visible) keeps a hidden item's space like PRINT WHEN.
        //--- A page header moved into the group header keeps its own height ---
        double BH(int code) => bands.Where(b => b.Code == code).Select(b => b.Height).DefaultIfEmpty(0).First();
        double grpHdrH = pghdrToGroup ? BH(1) + BH(3) : BH(3);
        //--- Groups, outer first. FRX lists the headers outer -> inner and the footers inner -> outer. Group 0 uses the
        //--- section names grphdr/grpftr, group i>0 grphdr{i}/grpftr{i} ---
        var grpHdrs = bands.Where(b => b.Code == 3).ToList();
        var grpFtrs = bands.Where(b => b.Code == 5).Reverse().ToList();
        string GSec(string kind, int i) => i == 0 ? kind : kind + i;
        double SecH(string sec) => sec switch
        {
            "pghdr" => BH(1), "grphdr" => grpHdrH, "detail" => BH(4), "grpftr" => grpFtrs.Select(b => b.Height).DefaultIfEmpty(0).First(),
            "pgftr" => BH(7), "summary" => BH(8),
            _ when sec.StartsWith("grphdr") => grpHdrs[int.Parse(sec[6..])].Height,
            _ when sec.StartsWith("grpftr") => grpFtrs[int.Parse(sec[6..])].Height,
            _ => 99
        };
        //--- Build items per section ----------------------------------------------------------------------------------
        var sections = new Dictionary<string, List<string>> { ["pghdr"] = [], ["grphdr"] = [], ["detail"] = [], ["grpftr"] = [], ["pgftr"] = [], ["summary"] = [], ["title"] = [] };
        for (int i = 1; i < Math.Max(grpHdrs.Count, grpFtrs.Count); i++) { sections[GSec("grphdr", i)] = []; sections[GSec("grpftr", i)] = []; }
        var sectionOfCode = new Dictionary<int, string> { [0] = "title", [1] = "pghdr", [3] = "grphdr", [4] = "detail", [5] = "grpftr", [7] = "pgftr", [8] = "summary" };
        string SectionOf((int Code, double Start, double Height, string Expr, bool PageBreak) b) => b.Code switch
        {
            3 => GSec("grphdr", grpHdrs.FindIndex(x => x.Start == b.Start)),
            5 => GSec("grpftr", grpFtrs.FindIndex(x => x.Start == b.Start)),
            _ => sectionOfCode.GetValueOrDefault(b.Code, "detail")
        };
        int seq = 0;
        foreach (var r in recs)
        {
            int t = I(r, "OBJTYPE");
            if (t is not (5 or 6 or 7 or 8)) continue;
            double v = N(r, "VPOS") / 10000, h = N(r, "HPOS") / 10000, w = N(r, "WIDTH") / 10000, hh = N(r, "HEIGHT") / 10000;
            var band = BandOf(v);
            string sec = SectionOf(band);
            if (sec == "pghdr" && pghdrToGroup) sec = "grphdr";
            double top = Math.Max(0, v - band.Start);
            if (sec == "grphdr" && pghdrToGroup && band.Code == 1) top = v - band.Start;
            // 0.002in spare: the 3-decimal inch values in the XML must not round back over the band (Telerik drops a page
            // header/footer item that crosses the bottom)
            if (t is 5 or 8 && top + hh > SecH(sec) - 0.002) { hh = Math.Min(hh, SecH(sec) - 0.002); top = Math.Max(0, SecH(sec) - 0.002 - hh); }
            string name = "i" + (++seq);
            string expr = r["EXPR"].Replace("\r", " ").Replace("\n", " ");
            if (replace.TryGetValue($"{expr.ToLower()}@{(N(r, "HPOS") / 10000).ToString("0.000", inv)}", out var rep)) expr = rep;
            //--- PRINT WHEN: Telerik pulls the items right of a hidden item to the left, so nothing is hidden: a text gets an
            //--- IIf() value and a line or box a transparent colour ---
            string supExpr = t == 8 && printWhen.TryGetValue(expr.Trim().ToLower(), out var pw) ? pw : r["SUPEXPR"].Trim();
            string cond = supExpr == "" ? "" : Expr(supExpr);
            string When(string v) => cond == "" ? v : $"=IIf({cond}, {v[1..]}, Null)";
            string vis = cond == "" ? "" : $"<Bindings><Binding Path=\"Style.LineColor\" Expression=\"=IIf({X(cond)}, 'Black', 'Transparent')\" /><Binding Path=\"Style.BorderColor.Default\" Expression=\"=IIf({X(cond)}, 'Black', 'Transparent')\" /></Bindings>";
            int st = I(r, "FONTSTYLE");
            string face = fontMap.GetValueOrDefault(r["FONTFACE"].ToLower(), r["FONTFACE"]);
            string fontBind = fontParam.TryGetValue(r["FONTFACE"].ToLower(), out var fp) ? $"<Bindings><Binding Path=\"Style.Font.Name\" Expression=\"=Parameters.{fp}.Value\" /></Bindings>" : "";
            string font = $"<Font Name=\"{X(face)}\" Size=\"{N(r, "FONTSIZE")}pt\" Bold=\"{(st & 1) == 1}\" Italic=\"{(st & 2) == 2}\"{((st & 4) == 4 ? " Underline=\"True\"" : "")} />";
            switch (t)
            {
                case 5: // label
                    {
                        string text = expr.Trim().Trim('"');
                        var lab = labels.FirstOrDefault(l => text.ToLower().StartsWith(l.Key));
                        string lv = lab.Key != null ? lab.Value
                                  : cond == "" ? text : When("=\"" + text.Replace("\"", "'") + "\"");
                        sections[sec].Add($"<TextBox Width=\"{In(w * 1.12 + 0.05)}\" Height=\"{In(hh)}\" Left=\"{In(h)}\" Top=\"{In(top)}\" Value=\"{X(lv)}\" TextWrap=\"False\" Name=\"{name}\"><Style>{font}</Style>{fontBind}</TextBox>");
                        break;
                    }
                case 8: // field
                    {
                        int totalType = I(r, "TOTALTYPE");
                        string value;
                        // TOTALTYPE 1 is a record counter: the row number in the detail, a count in a footer. In the page footer a
                        // report-wide total (RESETTOTAL 1, or reccount()) uses Exec over the report, a page total PageExec ---
                        bool reportWide = sec == "pgftr" && (I(r, "RESETTOTAL") == 1 || Regex.IsMatch(expr, @"(?i)\breccount\(\)"));
                        string rptName = Cfg("Name", "Report1");
                        if (totalType == 1 || Regex.IsMatch(expr.Trim(), @"(?i)^reccount\(\)$"))
                            value = sec == "detail" ? (bands.Any(b => b.Code == 3) ? "=RowNumber(\"grp\")" : "=RowNumber()")
                                  : sec == "pgftr" ? (reportWide ? $"=Exec(\"{rptName}\", Count(1))" : "=PageExec(\"detail\", Count(1))") : "=Count(1)";
                        else
                        {
                            string e = Expr(expr);
                            bool sum = totalType == 2 || (sec.StartsWith("grpftr") || sec is "pgftr" or "summary") && forceSum.Contains(expr.Trim().ToLower());
                            if (sum) e = sec == "pgftr" ? (reportWide ? $"Exec(\"{rptName}\", Sum({e}))" : $"PageExec(\"detail\", Sum({e}))") : $"Sum({e})";
                            value = "=" + e;
                        }
                        string codes = PicCodes(r["PICTURE"]);
                        if (codes.Contains('!')) value = "=ToUpper(" + value[1..] + ")";
                        value = When(value);
                        string fmt = Fmt(r["PICTURE"], value);
                        // FoxPro: OFFSET 0 left, 1 right, 2 center; numbers in a picture default to right
                        int ofs = I(r, "OFFSET");
                        string align = codes.Contains('I') || ofs == 2 ? "Center"
                                     : codes.Contains('J') || ofs == 1 || (ofs == 0 && (fmt.StartsWith("{0:#") || fmt.StartsWith("{0:'"))) ? "Right" : "Left";
                        sections[sec].Add($"<TextBox Width=\"{In(w + 0.02)}\" Height=\"{In(hh)}\" Left=\"{In(h)}\" Top=\"{In(top)}\" Value=\"{X(value)}\"{(fmt != "" ? $" Format=\"{X(fmt)}\"" : "")} CanGrow=\"{(r["STRETCH"] == "T" && !noGrow)}\" TextWrap=\"{(r["STRETCH"] == "T" && !noGrow)}\" Name=\"{name}\"><Style TextAlign=\"{align}\">{font}</Style>{fontBind}</TextBox>");
                        break;
                    }
                case 6: // line
                    {
                        bool vert = w < hh;
                        double pen = Math.Max(0.5, N(r, "PENSIZE"));
                        sections[sec].Add(vert
                            ? $"<Shape Width=\"0.02in\" Height=\"{In(hh)}\" Left=\"{In(h - 0.01)}\" Top=\"{In(top)}\" Name=\"{name}\"><ShapeType><LineShape Direction=\"NS\" /></ShapeType><Style><BorderWidth Default=\"{pen}pt\" /></Style>{vis}</Shape>"
                            : $"<Shape Width=\"{In(w)}\" Height=\"0.02in\" Left=\"{In(h)}\" Top=\"{In(top - 0.01)}\" Name=\"{name}\"><ShapeType><LineShape Direction=\"EW\" /></ShapeType><Style><BorderWidth Default=\"{pen}pt\" /></Style>{vis}</Shape>");
                        break;
                    }
                case 7: // box
                    sections[sec].Add($"<Panel Width=\"{In(w)}\" Height=\"{In(hh)}\" Left=\"{In(h)}\" Top=\"{In(top)}\" Name=\"{name}\"><Style><BorderStyle Default=\"Solid\" /><BorderWidth Default=\"0.5pt\" /></Style>{vis}</Panel>");
                    break;
            }
        }

        string Items(string sec) => string.Join("\n", sections[sec]);

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine($"<Report Width=\"{In(reportW)}\" Name=\"{X(Cfg("Name", "Report1"))}\" SnapGridSize=\"0.05in\" xmlns=\"http://schemas.telerik.com/reporting/2025/2.0\">");
        sb.AppendLine("<Items>");
        if (!pghdrToGroup && BH(1) > 0) sb.AppendLine($"<PageHeaderSection Height=\"{In(BH(1))}\" Name=\"pageHeader\"><Items>{Items("pghdr")}</Items></PageHeaderSection>");
        sb.AppendLine($"<DetailSection Height=\"{In(BH(4))}\" Name=\"detail\"><Items>{Items("detail")}</Items></DetailSection>");
        if (BH(7) > 0) sb.AppendLine($"<PageFooterSection Height=\"{In(BH(7))}\" Name=\"pageFooter\"><Items>{Items("pgftr")}</Items></PageFooterSection>");
        if (BH(8) > 0) sb.AppendLine($"<ReportFooterSection Height=\"{In(BH(8))}\" Name=\"reportFooter\"><Items>{Items("summary")}</Items></ReportFooterSection>");
        sb.AppendLine("</Items>");
        sb.AppendLine($"<PageSettings PaperKind=\"{paper}\" Landscape=\"{landscape}\" ColumnCount=\"1\" ColumnSpacing=\"0in\"><Margins><MarginsU Left=\"{In(sideMargin)}\" Right=\"{In(sideMargin)}\" Top=\"0.25in\" Bottom=\"0.25in\" /></Margins></PageSettings>");
        //--- A group band without an expression is not a group; FoxPro '' (one group for the report) stays a group ---
        var groups = grpHdrs.Select((g, i) => (g, i)).Where(x => !string.IsNullOrEmpty(x.g.Expr)).ToList();
        if (groups.Count > 0)
        {
            foreach (var (g, i) in groups)
            {
                string n = i == 0 ? "" : (i + 1).ToString();
                double hdrH = i == 0 ? grpHdrH : g.Height, ftrH = i < grpFtrs.Count ? grpFtrs[i].Height : 0;
                sb.AppendLine($"{(g == groups[0].g ? "<Groups>" : "")}<Group Name=\"grp{n}\">");
                sb.AppendLine($"<GroupHeader><GroupHeaderSection Height=\"{In(hdrH)}\" PrintOnEveryPage=\"True\" Name=\"grpHeader{n}\"><Items>{Items(GSec("grphdr", i))}</Items></GroupHeaderSection></GroupHeader>");
                sb.AppendLine($"<GroupFooter><GroupFooterSection Height=\"{In(ftrH)}\" Name=\"grpFooter{n}\"{(g.PageBreak ? " PageBreak=\"After\"" : "")}><Items>{(sections.ContainsKey(GSec("grpftr", i)) ? Items(GSec("grpftr", i)) : "")}</Items></GroupFooterSection></GroupFooter>");
                sb.AppendLine($"<Groupings><Grouping Expression=\"={X(Expr(g.Expr))}\" /></Groupings>");
                sb.AppendLine($"</Group>{(g == groups[^1].g ? "</Groups>" : "")}");
            }
        }
        sb.AppendLine("<ReportParameters>");
        foreach (var p in paramMap.Values.Concat(fontParam.Values).Distinct().Append("RunDate"))
            sb.AppendLine(p == "RunDate"
                ? "<ReportParameter Name=\"RunDate\" Type=\"DateTime\"><Value><String>=Now()</String></Value></ReportParameter>"
                : paramTypes.TryGetValue(p.ToLower(), out var pt)
                ? $"<ReportParameter Name=\"{p}\" Type=\"{pt}\"><Value><String>0</String></Value></ReportParameter>"
                : $"<ReportParameter Name=\"{p}\"><Value><String></String></Value></ReportParameter>");
        sb.AppendLine("</ReportParameters>");
        sb.AppendLine("</Report>");

        //--- Package as .trdp --------------------------------------------------------------------------------------------
        if (File.Exists(args[2])) File.Delete(args[2]);
        using (var outZip = ZipFile.Open(args[2], ZipArchiveMode.Create))
        {
            using (var w = new StreamWriter(outZip.CreateEntry("definition.xml").Open(), new UTF8Encoding(false))) w.Write(sb.ToString());
            using (var w = new StreamWriter(outZip.CreateEntry("[Content_Types].xml").Open(), new UTF8Encoding(true)))
                w.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"xml\" ContentType=\"application/zip\" /></Types>");
        }
        File.WriteAllText(Path.ChangeExtension(args[2], ".xml"), sb.ToString());
        Console.WriteLine($"paper={paper} landscape={landscape} width={reportW} margin={sideMargin} bands={string.Join(",", bands.Select(b => $"{b.Code}@{b.Start:0.000}+{b.Height:0.000}"))}");
        Console.WriteLine($"items: " + string.Join(", ", sections.Where(s => s.Value.Count > 0).Select(s => $"{s.Key}={s.Value.Count}")));
        foreach (var w in warnings.Distinct()) Console.WriteLine("WARN " + w);
    }
}
