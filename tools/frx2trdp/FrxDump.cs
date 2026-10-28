// Dumps a FoxPro report (.frx + .frt) as text: one line per object.
// usage: frx2trdp dump <zip> <path/inside/zip/name.frx>
using System.IO.Compression;
using System.Text;

static class FrxDump
{
    public static void Run(string[] args)
    {

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var enc = Encoding.GetEncoding(1252);

        var zip = ZipFile.OpenRead(args[0]);
        ZipArchiveEntry Find(string p) => zip.Entries.First(e => string.Equals(e.FullName, p, StringComparison.OrdinalIgnoreCase));
        byte[] Read(ZipArchiveEntry e) { using var s = e.Open(); using var m = new MemoryStream(); s.CopyTo(m); return m.ToArray(); }

        var frxPath = args[1];
        var dbf = Read(Find(frxPath));
        var fpt = Read(Find(Path.ChangeExtension(frxPath, null) + ".frt"));
        int blockSize = (fpt[6] << 8) | fpt[7];

        int nRec = BitConverter.ToInt32(dbf, 4);
        int hdrLen = BitConverter.ToUInt16(dbf, 8);
        int recLen = BitConverter.ToUInt16(dbf, 10);

        var fields = new List<(string Name, char Type, int Off, int Len, int Dec)>();
        int off = 1;
        for (int p = 32; dbf[p] != 0x0D; p += 32)
        {
            var name = enc.GetString(dbf, p, 11).TrimEnd('\0');
            char type = (char)dbf[p + 11];
            int len = dbf[p + 16], dec = dbf[p + 17];
            fields.Add((name, type, off, len, dec));
            off += len;
        }

        string Memo(int block)
        {
            if (block <= 0) return "";
            int p = block * blockSize;
            int len = (fpt[p + 4] << 24) | (fpt[p + 5] << 16) | (fpt[p + 6] << 8) | fpt[p + 7];
            return enc.GetString(fpt, p + 8, len).TrimEnd('\0');
        }

        string Val(byte[] r, (string Name, char Type, int Off, int Len, int Dec) f)
        {
            switch (f.Type)
            {
                case 'M': case 'G': return Memo(BitConverter.ToInt32(r, f.Off));
                case 'L': return r[f.Off] == (byte)'T' ? "T" : "F";
                case 'I': return BitConverter.ToInt32(r, f.Off).ToString();
                default: return enc.GetString(r, f.Off, f.Len).Trim();
            }
        }

        string[] want = ["OBJTYPE", "OBJCODE", "EXPR", "VPOS", "HPOS", "WIDTH", "HEIGHT", "FONTFACE", "FONTSIZE", "FONTSTYLE", "PICTURE", "TAG", "SUPEXPR", "OFFSET", "PLAIN", "TOTALTYPE", "RESETTOTAL", "STRETCH"];
        var idx = want.Select(w => fields.FindIndex(f => f.Name == w)).ToArray();

        // OBJTYPE: 1 report, 5 label, 6 line, 7 box, 8 field, 9 band, 17 picture, 18 variable, 23 font, 25 dataenv, 26 cursor
        // band OBJCODE: 0 title, 1 pghdr, 2 colhdr, 3 grphdr, 4 detail, 5 grpftr, 6 colftr, 7 pgftr, 8 summary
        for (int i = 0; i < nRec; i++)
        {
            var r = dbf.AsSpan(hdrLen + i * recLen, recLen).ToArray();
            if (r[0] == (byte)'*') continue;
            var vals = want.Select((w, k) => idx[k] < 0 ? "" : Val(r, fields[idx[k]])).ToArray();
            var d = want.Zip(vals).ToDictionary(x => x.First, x => x.Second);
            int type = int.TryParse(d["OBJTYPE"], out var t) ? t : -1;
            if (type is 1 or 23 or 25 or 26 or 10) continue;
            string kind = type switch { 5 => "LBL", 6 => "LINE", 7 => "BOX", 8 => "FLD", 9 => "BAND", 17 => "PIC", 18 => "VAR", _ => "T" + type };
            double v = double.Parse(d["VPOS"] == "" ? "0" : d["VPOS"]) / 10000, h = double.Parse(d["HPOS"] == "" ? "0" : d["HPOS"]) / 10000;
            double w = double.Parse(d["WIDTH"] == "" ? "0" : d["WIDTH"]) / 10000, hh = double.Parse(d["HEIGHT"] == "" ? "0" : d["HEIGHT"]) / 10000;
            string expr = d["EXPR"].Replace("\r", " ").Replace("\n", " ");
            string extra = "";
            if (type == 9) extra = $" code={d["OBJCODE"]} expr={d["TAG"]}";
            if (type == 18) extra = $" name={d["TAG"]} total={d["TOTALTYPE"]} reset={d["RESETTOTAL"]}";
            if (type == 8 && d["TOTALTYPE"] is not ("" or "0")) extra += $" total={d["TOTALTYPE"]} reset={d["RESETTOTAL"]}";
            if (type is 5 or 8) extra += $" font={d["FONTFACE"]} {d["FONTSIZE"]} st={d["FONTSTYLE"]}";
            if (d["PICTURE"] != "") extra += $" pic={d["PICTURE"].Replace("\n", " ")}";
            if (d["SUPEXPR"] != "") extra += $" printwhen={d["SUPEXPR"].Replace("\n", " ")}";
            Console.WriteLine($"{kind,-4} v={v,7:0.000} h={h,7:0.000} w={w,6:0.000} h={hh,6:0.000} {expr}{extra}");
        }
    }
}
