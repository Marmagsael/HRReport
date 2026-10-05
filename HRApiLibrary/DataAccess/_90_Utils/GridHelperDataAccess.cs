using HRApiLibrary.Models._90_Utils;

namespace HRApiLibrary.DataAccess._90_Utils
{
    public class GridHelperDataAccess
    {
        public static string BuildWhere( List<GridFilterModel> filters, Dictionary<string, string> columns, Dictionary<string, object> parameters)
        {
            var andConditions = new List<string>();
            var orConditions = new List<string>();

            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];

                if (!columns.TryGetValue(filter.Field, out var column))
                {
                    continue;
                }

                var parameterName = $"FilterValue{i}";
                string condition;

                switch (filter.Operator)
                {
                    case "=":
                    case "<>":
                    case ">":
                    case ">=":
                    case "<":
                    case "<=":

                        if (string.IsNullOrWhiteSpace(filter.Value))
                            continue;

                        condition = $"{column} {filter.Operator} @{parameterName}";
                        parameters[parameterName] = ConvertFilterValue(filter.Value);
                        break;


                    case "STARTS":

                        if (string.IsNullOrWhiteSpace(filter.Value))
                            continue;

                        condition = $"{column} LIKE @{parameterName}";
                        parameters[parameterName] = $"{filter.Value}%";
                        break;


                    case "ENDS":

                        if (string.IsNullOrWhiteSpace(filter.Value))
                            continue;

                        condition = $"{column} LIKE @{parameterName}";
                        parameters[parameterName] = $"%{filter.Value}";
                        break;


                    case "CONTAINS":

                        if (string.IsNullOrWhiteSpace(filter.Value))
                            continue;

                        condition = $"{column} LIKE @{parameterName}";
                        parameters[parameterName] = $"%{filter.Value}%";
                        break;


                    case "NOT CONTAINS":

                        if (string.IsNullOrWhiteSpace(filter.Value))
                            continue;

                        condition = $"{column} NOT LIKE @{parameterName}";
                        parameters[parameterName] = $"%{filter.Value}%";
                        break;


                    case "IS NULL":

                        condition = $"{column} IS NULL";
                        break;


                    case "IS NOT NULL":

                        condition = $"{column} IS NOT NULL";
                        break;


                    case "IS EMPTY":

                        condition = $"({column} IS NULL OR {column} = '')";
                        break;


                    case "IS NOT EMPTY":

                        condition = $"({column} IS NOT NULL AND {column} <> '')";
                        break;


                    default:
                        continue;
                }

                if (filter.LogicalOperator == "OR")
                {
                    orConditions.Add(condition);
                }
                else
                {
                    andConditions.Add(condition);
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


        private static object ConvertFilterValue(string value)
        {
            if (DateTime.TryParse(value, out var date))
            {
                return date.Date;
            }

            return value;
        }
    }
}

