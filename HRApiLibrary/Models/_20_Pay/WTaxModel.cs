namespace HRApiLibrary.Models._20_Pay;

public class WTaxModel
{
    public int Id { get; set; }

    public DateTime From_ { get; set; }

    public DateTime To_ { get; set; }

    public string? CountryCode { get; set; }

    public string? PeriodCode { get; set; }

    public string? TaxCode { get; set; }

    public string? SAmt { get; set; }

    public string? EAmt { get; set; }

    public string? Fix { get; set; }

    public string? Percentage { get; set; }
}
