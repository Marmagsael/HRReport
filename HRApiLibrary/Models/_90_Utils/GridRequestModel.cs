namespace HRApiLibrary.Models._90_Utils;

public class GridRequestModel
{
    public int PageSize { get; set; }
    public int Offset { get; set; }
    public string SortField { get; set; } = "";
    public string SortDirection { get; set; } = "";
    public List<GridFilterModel> Filters { get; set; } = new();
}

public class GridFilterModel
{
    public string Field { get; set; } = "";
    public string Operator { get; set; } = "";
    public string Value { get; set; } = "";
    public string LogicalOperator { get; set; } = "";
}
