// MBPay FoxPro report (.frx) -> Telerik .trdp tools. See README.md.
//   dump     <zip> <frx>                       list the report objects
//   convert  <zip> <frx> <out.trdp> <cfg.json> write the Telerik report
//   register <trdp> <out.pdf> <yymmpp> [trns]  render with HRApiLibrary data (other report names in Render.cs)
//   regcheck                                   parity of the DA against plain SQL
switch (args.FirstOrDefault())
{
    case "dump":    FrxDump.Run(args[1..]); break;
    case "convert": FrxConvert.Run(args[1..]); break;
    case null:      Console.WriteLine("usage: dump | convert | <report> ... (see Program.cs)"); break;
    default:        await Render.Run(args); break;
}

static class Repo
{
    //--- Repo root = the folder holding HRSys.sln, found upward from the exe ---
    public static string Root { get; } = Find();
    static string Find()
    {
        for (var d = new DirectoryInfo(AppContext.BaseDirectory); d != null; d = d.Parent)
            if (File.Exists(Path.Combine(d.FullName, "HRSys.sln"))) return d.FullName;
        throw new DirectoryNotFoundException("HRSys.sln not found above " + AppContext.BaseDirectory);
    }
}
