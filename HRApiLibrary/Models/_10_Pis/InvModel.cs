namespace HRApiLibrary.Models._10_Pis;

public class InvModel
{
    public int          Id                          {get; set; } 
    public string?      Name                        {get; set; } 
    public int          TypeId                      {get; set; } 
    public int          MakeId                      {get; set; } 
    public int          ModelId                     {get; set; } 
    public int          CategoryId                  {get; set; } 
    public int          BrandId                     {get; set; } 
    public string?      Description                 {get; set; } 
    public string?      SerialNo                    {get; set; } 
    public DateTime     DatePurchased               {get; set; } 
    public DateTime     DateWarrantyExpiration      {get; set; }  
    public double       UnitCost                    {get; set; } 
    public string?      Status                      {get; set; } 
    public int          DeploymentId                {get; set; } 
    public int          EmpmasId                    {get; set; } 
    public string?      EmpNumber                   {get; set; } 
    public DateTime     DateAssignment              {get; set; } 
    public string?      AssignmentNo                {get; set; } 
    public DateTime     DateEncoded                 {get; set; } 
    public int          EncodedbyId                 {get; set; } 
}