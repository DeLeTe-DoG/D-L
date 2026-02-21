using backend.Models;

namespace backend.Interfaces;

public interface IAdmin
{
  List<LanternDTO> GetAll();
  LanternDTO Add(CoordinatesDTO coords, string status);
  LanternDTO Edit(CoordinatesDTO coords, string status);
  LanternDTO Delete();

}