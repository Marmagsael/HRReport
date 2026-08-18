namespace HRApiLibrary.Models._90_Utils;

public class GridResultModel<T>
{
    public List<T> Data { get; set; } = new();
    public int Total { get; set; }
}
