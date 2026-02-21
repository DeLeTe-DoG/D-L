namespace backend.Models;

public class LogDTO
{
  public string LogID { get; set;} = string.Empty;
  public string DroneID { get; set;} = string.Empty;
  public string Result { get; set; } = string.Empty;
  public DateTime RepairTime { get; set;}
  public DateTime LogTime { get; set; } 
}