using HRApiLibrary.Models._90_Utils;
using Telerik.Blazor.Components;
using Telerik.DataSource;

namespace HRMvc.Applications.Vars
{
    public static class GridHelper
    {
        public static List<GridFilterModel> GetFilters(GridReadEventArgs args)
        {
            var filters = new List<GridFilterModel>();

            foreach (var item in args.Request.Filters)
            {
                AddFilter(item, "AND", filters);
            }

            return filters;
        }


        private static void AddFilter(IFilterDescriptor item, string logicalOperator, List<GridFilterModel> filters)
        {
            if (item is Telerik.DataSource.FilterDescriptor filter)
            {
                filters.Add(new GridFilterModel
                {
                    Field = filter.Member,
                    Operator = GetOperator(filter.Operator),
                    Value = filter.Value?.ToString() ?? "",
                    LogicalOperator = logicalOperator,
                });
            }

            else if (item is Telerik.DataSource.CompositeFilterDescriptor composite)
            {
                var op = composite.LogicalOperator == Telerik.DataSource.FilterCompositionLogicalOperator.Or ? "OR" : "AND";

                foreach (var child in composite.FilterDescriptors)
                {
                    AddFilter(child, op, filters);
                }
            }
        }


        private static string GetOperator(Telerik.DataSource.FilterOperator filterOperator)
        {
            return filterOperator switch
            {
                Telerik.DataSource.FilterOperator.IsEqualTo => "=",
                Telerik.DataSource.FilterOperator.IsNotEqualTo => "<>",
                Telerik.DataSource.FilterOperator.IsGreaterThan => ">",
                Telerik.DataSource.FilterOperator.IsGreaterThanOrEqualTo => ">=",
                Telerik.DataSource.FilterOperator.IsLessThan => "<",
                Telerik.DataSource.FilterOperator.IsLessThanOrEqualTo => "<=",
                Telerik.DataSource.FilterOperator.StartsWith => "STARTS",
                Telerik.DataSource.FilterOperator.EndsWith => "ENDS",
                Telerik.DataSource.FilterOperator.Contains => "CONTAINS",
                Telerik.DataSource.FilterOperator.DoesNotContain => "NOT CONTAINS",
                Telerik.DataSource.FilterOperator.IsNull => "IS NULL",
                Telerik.DataSource.FilterOperator.IsNotNull => "IS NOT NULL",
                Telerik.DataSource.FilterOperator.IsEmpty => "IS EMPTY",
                Telerik.DataSource.FilterOperator.IsNotEmpty => "IS NOT EMPTY",
                _ => "CONTAINS"
            };
        }
    }
}