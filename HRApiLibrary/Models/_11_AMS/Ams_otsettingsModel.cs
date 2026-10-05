public class Ams_otsettingsModel
{
    // raw int galing sa DB (alias = NeedsOTFiling → map sa Needsotfiling)
    public int Needsotfiling { get; set; }

    // wrapper para sa UI binding (bool)
    public bool NeedsotfilingBool
    {
        get => Needsotfiling == 1;
        set => Needsotfiling = value ? 1 : 0;
    }

    public int Maxothour { get; set; }
    public string Empname { get; set; } = string.Empty;
    public string Empnumber { get; set; } = string.Empty;
}
