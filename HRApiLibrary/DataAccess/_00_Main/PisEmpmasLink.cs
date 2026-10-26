using System.Data;
using Dapper;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

// -- The old PIS ({Code}_OPis) is the source of truth; the new PIS ({Code}_Pis.Empmas) only links the employee --
// -- to an account (SystemId) and to CompanyUsers / CompanyInvite (EmpmasId). Shared by Register and Invite User --
public static class PisEmpmasLink
{
    // -- Id of the {PISSchema}.Empmas row of this EmpNumber; made from the old PIS name when there is none --
    // -- Runs inside the caller's transaction --
    public static async Task<int> EnsureAsync(IDbConnection cn, IDbTransaction tran, string pisSchema, V01EmpmasLookup oldPis)
    {
        string sql = $@"select Id from {pisSchema}.Empmas where EmpNumber = @EmpNumber order by Id limit 1 for update";
        int? id = await cn.ExecuteScalarAsync<int?>(sql, oldPis, tran);
        if (id != null) return id.Value;

        sql = $@"insert into {pisSchema}.Empmas (SystemId, EmpNumber, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix)
                                          values (0,        @EmpNumber, @LastName, @FirstName, @MiddleName, ifnull(@Suffix, ''));
                 select last_insert_id();";
        return await cn.ExecuteScalarAsync<int>(sql, oldPis, tran);
    }

    // -- Name fields where the new PIS differs from the old PIS (trimmed, any letter case; empty = blank) --
    // -- The new PIS email is not compared : only the old PIS email is used --
    public static List<string> Discrepancies(V01EmpmasLookup oldPis, V01EmpmasLookup newPis)
    {
        static bool Same(string? a, string? b) => string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);

        var different = new List<string>();
        if (!Same(oldPis.LastName,   newPis.LastName))      different.Add("Last Name");
        if (!Same(oldPis.FirstName,  newPis.FirstName))     different.Add("First Name");
        if (!Same(oldPis.MiddleName, newPis.MiddleName))    different.Add("Middle Name");
        if (!Same(oldPis.Suffix,     newPis.Suffix))        different.Add("Suffix");
        return different;
    }

    public static string DiscrepancyMessage(List<string> different, string who = "your")
    {
        return $"There is a discrepancy in {who} employee record ({string.Join(", ", different)}). " +
               "Please ask HR to correct it first" + (who == "your" ? ", then register again." : ".");
    }
}
