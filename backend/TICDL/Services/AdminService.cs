using backend.Interfaces;
using backend.Models;

namespace backend.Service;

public class AdminService : IAdmin
{
    private readonly List<LanternDTO> _lanterns = new();
    private readonly List<DroneDTO> _drones = new();

    // Оптимизировал рандомайзер, используя Random.Shared (как обсуждали ранее)
    private string GenLine()
    {
        var chars = "!@#$%^&*()_+";
        int cnt = 5;
        char[] result = new char[cnt];
        
        for (int i = 0; i < cnt; i++)
        {
            // Используем chars.Length (без +1, иначе будет ошибка выхода за пределы)
            result[i] = chars[Random.Shared.Next(0, chars.Length)];
        }
        return new string(result);
    }

    // Исправлено: возвращает DroneDTO
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
            LanterName = Name,
            Coordinates = coords,
            Id = "LNTR_" + GenLine()
        };
        _lanterns.Add(lantern);
        return lantern;
    }
    
    public LanternDTO EditLantern(LanternDTO oldLantern, LanternDTO NewData)
    {
        var lantern = _lanterns.FirstOrDefault(l => l.Id == oldLantern.Id);
        if (lantern != null)
        {
            lantern.Coordinates = NewData.Coordinates;
            lantern.LanterName = NewData.LanterName;
            return lantern;
        }
        return null; // Или выбросить исключение
    }

    // Исправлено: возвращает LanternDTO
    public LanternDTO DeleteLantern(string Id)
    {
        var lantern = _lanterns.FirstOrDefault(l => l.Id == Id);
        if (lantern != null)
        {
            _lanterns.Remove(lantern);
        }
        return lantern;
    }

    // Исправлено: возвращает DroneDTO (а не string)
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