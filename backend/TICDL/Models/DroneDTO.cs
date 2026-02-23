using System.Runtime.CompilerServices;

namespace backend.Models;

public class DroneDTO
{
  public string DroneID { get; set;} = string.Empty;
  public string DroneName { get; set; } = string.Empty;
  public int Battery { get; set; }
  public bool DroneStatus { get; set; } = false;
}