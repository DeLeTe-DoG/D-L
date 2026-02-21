using backend.Models;

namespace backend.Models;

public class LanternDTO
{
  public string Id { get; set; } = string.Empty;
  public string LanterName { get; set; } = string.Empty; 
  public CoordinatesDTO Coordinates { get; set; }
  public string Status { get; set; } = String.Empty; 
}