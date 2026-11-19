using Microsoft.AspNetCore.WebUtilities;

namespace HRMvc.Applications.PayrollTransaction.Vars;

public static class PeriodQuery
{
    public static (int Year, int Month, int Period)? Read(string uri)
    {
        var q = QueryHelpers.ParseQuery(new Uri(uri).Query);
        if (int.TryParse(q["y"], out var y) && int.TryParse(q["m"], out var m) && int.TryParse(q["p"], out var p)
            && y > 2000 && m is >= 1 and <= 12 && p is >= 1 and <= 5)
            return (y, m, p);
        return null;
    }
}
