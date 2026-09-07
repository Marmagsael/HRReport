using HRApiLibrary.Models._90_Utils;

namespace HRApiLibrary.DataAccess._90_Utils
{
    public class GridHelperDataAccess
    {

        public static string BuildWhere(
          List<GridFilterModel> filters,
          Dictionary<string, string> columns,
          Dictionary<string, object> parameters)
        {
            var conditions = new List<string>();

            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];

                if (!columns.TryGetValue(filter.Field, out var column))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(filter.Value))
                {
                    continue;
                }

                var parameterName = $"FilterValue{i}";

                switch (filter.Operator)
                {
                    case "=":
                    case "<>":
                    case ">":
                    case ">=":
                    case "<":
                    case "<=":

                        conditions.Add(
                            $"{column} {filter.Operator} @{parameterName}");

                        parameters[parameterName] = filter.Value;
                        break;

                    case "STARTS":

                        conditions.Add(
                            $"{column} LIKE @{parameterName}");

                        parameters[parameterName] = $"{filter.Value}%";
                        break;

                    case "ENDS":

                        conditions.Add(
                            $"{column} LIKE @{parameterName}");

                        parameters[parameterName] = $"%{filter.Value}";
                        break;

                    case "CONTAINS":

                        conditions.Add(
                            $"{column} LIKE @{parameterName}");

                        parameters[parameterName] = $"%{filter.Value}%";
                        break;

                    case "NOT CONTAINS":

                        conditions.Add(
                            $"{column} NOT LIKE @{parameterName}");

                        parameters[parameterName] = $"%{filter.Value}%";
                        break;
                }
            }

            if (!conditions.Any())
            {
                return "";
            }

            // Group OR conditions together
            var orConditions = new List<string>();
            var andConditions = new List<string>();

            for (int i = 0; i < filters.Count && i < conditions.Count; i++)
            {
                if (filters[i].LogicalOperator == "OR")
                {
                    orConditions.Add(conditions[i]);
                }
                else
                {
                    andConditions.Add(conditions[i]);
                }
            }

            if (orConditions.Any())
            {
                andConditions.Add(
                    "(" + string.Join(" OR ", orConditions) + ")");
            }

            return andConditions.Any()
                ? "WHERE " + string.Join(" AND ", andConditions)
                : "";
        }
    }
}
