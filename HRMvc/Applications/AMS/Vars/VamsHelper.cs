namespace HRMvc.Applications.AMS.Vars;

public static class VamsHelper
{
    public static int ComputeOut(int inVal, int hrsLength)
    {
        int inMinutes = (inVal / 100 * 60) + (inVal % 100);
        int durMinutes = (hrsLength / 100 * 60) + (hrsLength % 100 == 50 ? 30 : 0);
        int total = inMinutes + durMinutes;
        return (total / 60 % 24) * 100 + (total % 60);
    }

    public static string FormatTime(int val)
    {
        if (val == 0) return "-";
        int h = val / 100;
        int m = val % 100;
        string ampm = h < 12 ? "AM" : "PM";
        int h12 = h % 12 == 0 ? 12 : h % 12;
        return $"{h12}:{m:D2} {ampm}";
    }

    public static bool IsDisabled(string? dutyType) => dutyType == "RD" || dutyType == "RN";

    public static string FormatOut(string? dutyType, int inVal, int hrsLength)
    {
        if (IsDisabled(dutyType) || inVal == 0 || hrsLength == 0) return "-";
        return FormatTime(ComputeOut(inVal, hrsLength));
    }
}
