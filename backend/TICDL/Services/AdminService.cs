using backend.Interfaces;
using backend.Models;

namespace backend.Service;

public class AdminService : IAdmin
{
    private readonly List<LanternDTO> _lanterns = new();
    private readonly List<DroneDTO> _drones = new();

    private string GenLine()
    {
        var chars = "qwertyuioopasdfghjklzxcvbnm";
        int cnt = 5;
        char[] result = new char[cnt];
        
        for (int i = 0; i < cnt; i++)
        {
            result[i] = chars[Random.Shared.Next(0, chars.Length)];
        }
        return new string(result);
    }

    public DroneDTO AddDrone(string ID, string Name)
    {
        var drone = new DroneDTO
        {
            DroneID = ID,
            DroneName = Name
        };

        _drones.Add(drone);
        return drone;
    }

    
    public LanternDTO AddLantern(CoordinatesDTO coords, string Name)
    {
        var lantern = new LanternDTO
        {
            LanternName = Name,
            Coordinates = coords,
            Id = "LNTR_" + GenLine()
        };
        _lanterns.Add(lantern);
        return lantern;
    }
    
    public LanternDTO EditLantern(string id, LanternDTO NewData)
    {
        var lantern = _lanterns.FirstOrDefault(l => l.Id == id);
        if (lantern != null)
        {
            lantern.Coordinates = NewData.Coordinates;
            lantern.LanternName = NewData.LanternName;
            return lantern;
        }
        return null;
    }

    public LanternDTO DeleteLantern(string Id)
    {
        var lantern = _lanterns.FirstOrDefault(l => l.Id == Id);
        if (lantern != null)
        {
            _lanterns.Remove(lantern);
        }
        return lantern;
    }

    public DroneDTO DeleteDrone(string Id)
    {
        var drone = _drones.FirstOrDefault(d => d.DroneID == Id);
        if (drone != null)
        {
            _drones.Remove(drone);
        }
        return drone;
    }

    public List<LanternDTO> GetAllLanterns() => _lanterns;
    public List<DroneDTO> GetAllDrones() => _drones;
}