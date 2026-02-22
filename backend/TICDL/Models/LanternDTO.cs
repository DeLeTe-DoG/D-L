using System.Reflection.Metadata;
using backend.Models;

namespace backend.Models;

public class LanternDTO
{
  public string Id { get; set; } = string.Empty;
  public string LanternName { get; set; } = string.Empty; 
  public CoordinatesDTO Coordinates { get; set; }
  public string Place { get; set; } = string.Empty;
  public int Status { get; set; }
}