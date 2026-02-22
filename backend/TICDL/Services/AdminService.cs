using backend.Interfaces;
using backend.Models;

namespace backend.Service;

public class AdminService : IAdmin
{
    private readonly List<DroneDTO> _drones = new();
    private readonly List<LanternDTO> _lanterns = new ()
    {
        // База (Дрон-станция) обычно находится в начале маршрута
        new LanternDTO { 
            Id = "L1", 
            LanternName = "Входная группа (База)", 
            Status = 0, 
            Coordinates = new CoordinatesDTO { lat = 55.755800, lng = 49.123300} 
        },
        // Фонарь поблизости - всё в порядке
        new LanternDTO { 
            Id = "L2", 
            LanternName = "Центральная аллея - 1", 
            Status = 0, 
            Coordinates = new CoordinatesDTO { lat = 55.756000, lng = 49.124000} 
        },
        // А вот тут симулируем поломку (Status = 1)
        new LanternDTO { 
            Id = "L3", 
            LanternName = "Центральная аллея - 2 (СБОЙ)", 
            Status = 1, 
            Coordinates = new CoordinatesDTO { lat = 55.756200, lng = 49.124800} 
        },
        // Дальний фонарь в тупике
        new LanternDTO { 
            Id = "L4", 
            LanternName = "Фонтанная площадь", 
            Status = 0, 
            Coordinates = new CoordinatesDTO { lat = 55.756500, lng = 49.125500} 
        },
        // Ветка в сторону от основной линии
        new LanternDTO { 
            Id = "L5", 
            LanternName = "Боковая дорожка (СБОЙ)", 
            Status = 1, 
            Coordinates = new CoordinatesDTO { lat = 55.755900, lng = 49.125000} 
        }
    };

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
    public DroneDTO AddDrone(string Name)
    {
        var drone = new DroneDTO
        {
            DroneID = "DRN_" + GenLine(),
            DroneName = Name
        };

        _drones.Add(drone);
        Console.WriteLine($"[СЕРВЕР] Дрон добавлен: {drone.DroneName} ({drone.DroneID})");
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
        Console.WriteLine($"[СЕРВЕР] Фонарь добавлен: {lantern.LanternName} ({lantern.Id})");
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
            Console.WriteLine($"[СЕРВЕР] Фонарь  {lantern.LanternName} ({lantern.Id}) удалён");
        }
        return lantern;
    }

    public DroneDTO DeleteDrone(string Id)
    {
        var drone = _drones.FirstOrDefault(d => d.DroneID == Id);
        if (drone != null)
        {
            _drones.Remove(drone);
            Console.WriteLine($"[СЕРВЕР] Дрон {drone.DroneName} ({drone.DroneID}) удалён");
        }
        return drone;
    }

    public List<LanternDTO> GetAllLanterns() => _lanterns;
    public List<DroneDTO> GetAllDrones() { 
        Console.WriteLine("[СЕРВЕР] Вывод списка дронов: ");
        foreach (var drone in _drones)
        {
            Console.WriteLine($"-------- {drone.DroneName} ({drone.DroneID}), {drone.Battery}, {drone.DroneStatus}.");
        }
        return _drones;
    }
}