using HRApiLibrary.Models._90_Utils;
using Telerik.Blazor.Components;

namespace HRMvc.Applications.Vars
{
    public static class GridHelper
    {
        public static List<GridFilterModel> GetFilters(GridReadEventArgs args)
        {
            var filters = new List<GridFilterModel>();

            foreach (var item in args.Request.Filters)
            {
                if (item is Telerik.DataSource.FilterDescriptor filter)
                {
                    filters.Add(new GridFilterModel
                    {
                        Field = filter.Member,
                        Operator = GetOperator(filter.Operator),
                        Value = filter.Value?.ToString() ?? "",
                        LogicalOperator = "AND"
                    });

                   
                }

                else if (item is Telerik.DataSource.CompositeFilterDescriptor composite)
                {

                    var logicalOperator = composite.LogicalOperator == Telerik.DataSource.FilterCompositionLogicalOperator.Or ? "OR"  : "AND";
                    foreach (var child in composite.FilterDescriptors)
                    {
                        if (child is Telerik.DataSource.FilterDescriptor childFilter)
                        {
                            filters.Add(new GridFilterModel
                            {
                                Field = childFilter.Member,
                                Operator = GetOperator(childFilter.Operator),
                                Value = childFilter.Value?.ToString() ?? "",
                                LogicalOperator = logicalOperator
                            });
                        }
                    }
                }
            }

            return filters;
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
                _ => "CONTAINS"
            };
        }
    }
}