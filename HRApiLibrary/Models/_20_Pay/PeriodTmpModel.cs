namespace HRApiLibrary.Models._20_Pay;

public class PeriodTmpModel
{
    public  int     p1      { get; set; } = 0; // 1 = active, 0 = inactive
    public  int     p2      { get; set; } = 0; // 1 = active, 0 = inactive 
    public  int     p3      { get; set; } = 0; // 1 = active, 0 = inactive
    public  int     p4      { get; set; } = 0; // 1 = active, 0 = inactive
    public  int     p5      { get; set; } = 0; // 1 = active, 0 = inactive
    
    // Boolean bindings
    public bool     P1Bool  { get => p1 == 1; set => p1 = value ? 1 : 0;}
    public bool     P2Bool  { get => p2 == 1; set => p2 = value ? 1 : 0;}
    public bool     P3Bool  { get => p3 == 1; set => p3 = value ? 1 : 0;}
    public bool     P4Bool  { get => p4 == 1; set => p4 = value ? 1 : 0;}
    public bool     P5Bool  { get => p5 == 1; set => p5 = value ? 1 : 0;}
    
    
}