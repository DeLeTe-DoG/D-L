using backend.Models;

namespace backend.Models;

public class LanternDTO
{
  public int Id { get; set; }
  public List<CoordinatesDTO> Coordinates { get; set; } = [];
  public string Status { get; set; } = String.Empty; 
}