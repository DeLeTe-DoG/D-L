using backend.Interfaces;
using backend.Models;
using backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api")]
public class AdminController : ControllerBase
{
  private readonly IAdmin _AdminService;
  private readonly IHubContext<DroneHubService> _DroneHubService;

  public AdminController(IAdmin admin, IHubContext<DroneHubService> hub)
  {
    _AdminService = admin;
    _DroneHubService = hub;
  }

  public class LanternNode
  {
    public LanternDTO Lantern { get; set; }
    public LanternNode? Left { get; set; }
    public LanternNode? Right { get; set; }

    public LanternNode(LanternDTO lantern)
    {
        Lantern = lantern;
    }
  }

  public static LanternNode? BuildBinaryTree(
    List<LanternDTO> lanterns,
    double centerLat,
    double centerLng)
  {
    if (lanterns == null || lanterns.Count == 0)
        return null;

    LanternNode? root = null;

    foreach (var lantern in lanterns)
    {
        root = Insert(root, lantern);
    }

    return root;


    LanternNode Insert(LanternNode? node, LanternDTO lantern)
    {
        if (node == null)
            return new LanternNode(lantern);

        var current = node;

        double newDist = GetDistance(lantern);
        double currentDist = GetDistance(current.Lantern);

        if (newDist < currentDist)
            current.Left = Insert(current.Left, lantern);
        else
            current.Right = Insert(current.Right, lantern);

        return current;
    }

    double GetDistance(LanternDTO l)
    {
        double dx = l.Coordinates.lat - centerLat;
        double dy = l.Coordinates.lng - centerLng;
        return Math.Sqrt(dx * dx + dy * dy);
    }
  }

  public static List<List<LanternDTO>> GetRoutesToBroken(LanternNode? root)
  {
    var routes = new List<List<LanternDTO>>();

    if (root == null)
        return routes;

    Traverse(root, new List<LanternDTO>());

    return routes;


    void Traverse(LanternNode? node, List<LanternDTO> currentPath)
    {
        if (node == null)
            return;

        // Добавляем текущий фонарь в путь
        currentPath.Add(node.Lantern);

        // Если фонарь сломан (Status == 0)
        if (node.Lantern.Status == 0)
        {
            routes.Add(new List<LanternDTO>(currentPath));
        }

        // Рекурсивный обход
        Traverse(node.Left, currentPath);
        Traverse(node.Right, currentPath);

        // Убираем текущий узел при возврате назад (backtracking)
        currentPath.RemoveAt(currentPath.Count - 1);
    }
  }


  [HttpGet("drones")]
  public IActionResult GetAllDrones()
  {
    var drones = _AdminService.GetAllDrones();
    return Ok(drones);
  }
  [HttpGet("broadcast")]
  public async Task<IActionResult> Broadcast([FromQuery] string data)
  {
    if (string.IsNullOrEmpty(data))
    {
      BadRequest("Данных нет");
    }

    await _DroneHubService.Clients.All.SendAsync("RecieveCommand", data);
    return Ok(new {
      message = "Данные отправлены",
      sentData = data,
      timeStamp = DateTime.Now
    });
  }

  [HttpPost("drones")]
  public IActionResult AddDrone([FromBody] DroneDTO drone)
  {
    if (drone.DroneName != null)
    {
      _AdminService.AddDrone(drone.DroneName);
    }
    return Ok("Дрон " + drone.DroneName + " добавлен");
  }

  [HttpDelete("drones/{id}")]
  public IActionResult DeleteDrone(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      return BadRequest("Id дрона обязателен!");
    }
    _AdminService.DeleteDrone(id);
    return Ok("Дрон " + id + " удалён");
  }

  [HttpGet("lanterns")]
  public IActionResult GetAllLanterns()
  {
    var lantern = _AdminService.GetAllLanterns();
    return Ok(lantern);
  }

  [HttpPost("lanterns")]
  public IActionResult AddLantern([FromBody] LanternDTO dto)
  {
    if (string.IsNullOrWhiteSpace(dto.LanternName) || double.IsNaN(dto.Coordinates.lat) || double.IsNaN(dto.Coordinates.lng))
    {
      return BadRequest("Координаты и имя обязательны!");
    }
    _AdminService.AddLantern(dto.Coordinates, dto.LanternName);
    return Ok("Фонарь " + dto.LanternName + " добавлен");
  }


  [HttpPatch("lanterns/{id}")]
  public async Task<IActionResult> EditLantern(string id, [FromBody] LanternDTO newL)
  {
    var lantern = _AdminService.GetAllLanterns().FirstOrDefault(l => l.Id == id);
    if (lantern == null) return NotFound();

    _AdminService.EditLantern(id, newL);

    if (lantern.Status != newL.Status)
    {
      var allLaneterns = _AdminService.GetAllLanterns();
      var TaskTree = GetRoutesToBroken(BuildBinaryTree(allLaneterns, 0.0, 0.0));
      await _DroneHubService.Clients.All.SendAsync("RecieveMission", TaskTree);
      Console.WriteLine($"[СЕРВЕР] Неисправен фонарь {newL.Id}");
      Console.WriteLine($"[СЕРВЕР] аршруты до неисправленных фонарей");
      foreach (var route in TaskTree)
      {
          Console.WriteLine("    Маршрут:");
          foreach (var item in route)
          {
              Console.Write($"    {item.Id} -> ");
          }
          Console.WriteLine("END");
      }
    }
    return Ok(newL);  
  }

  [HttpDelete("lanterns/{id}")]
  public IActionResult DeleteLantern(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      return BadRequest("ID фонаря обязателен!");
    }
    _AdminService.DeleteLantern(id);
    return Ok("Фонарь " + id + " удалён");
  }
}