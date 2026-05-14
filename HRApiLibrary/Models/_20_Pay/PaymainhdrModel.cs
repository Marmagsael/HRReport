namespace HRApiLibrary.Models._20_Pay;

public class PaymainhdrModel
{
	public string? 			Trn 			{get; set; } 
	public double 			ClRate 			{get; set; } 
	public double 			MinRate 		{get; set; } 
	public int     			UserId 			{get; set; } 
	public string? 			Status 			{get; set; } 
	public DateTime			DateCreated		{get; set; } 
	public DateTime?		DatePosted 		{get; set; } 
	public DateTime?		AttStart 		{get; set; } 
	public DateTime?		AttEnd 			{get; set; } 
}