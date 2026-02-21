using backend.Models;

namespace backend.Interfaces;

public interface IAdmin
{
  List<LanternDTO> GetAllLanterns();
  List<DroneDTO> GetAllDrones();
  DroneDTO AddDrone(string ID, string Name);
  LanternDTO AddLantern(CoordinatesDTO coords, string Name);
  LanternDTO EditLantern(string id, LanternDTO NewData);
  LanternDTO DeleteLantern(string Id);
  DroneDTO DeleteDrone(string Id);
}