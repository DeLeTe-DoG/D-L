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

  private double GetDistance(double lat1, double lng1, double lat2, double lng2)
  {
      return Math.Sqrt(Math.Pow(lat1 - lat2, 2) + Math.Pow(lng1 - lng2, 2));
  }

  private string FindClosestParentId(LanternDTO lantern, List<LanternDTO> allLanterns, double baseLat, double baseLng)
  {
    double distToBase = GetDistance(lantern.Coordinates.lat, lantern.Coordinates.lng, baseLat, baseLng);

    if (distToBase < 0.0001) return "0"; 

    string closestId = "0";
    double minDistance = distToBase;

    foreach (var lant in allLanterns)
    {
        if (lant.Id == lantern.Id) continue;

        double candidateDistToBase = GetDistance(lant.Coordinates.lat, lant.Coordinates.lng, baseLat, baseLng);

        if (candidateDistToBase < distToBase)
        {
            double distBetween = GetDistance(lantern.Coordinates.lat, lantern.Coordinates.lng, 
                                            lant.Coordinates.lat, lant.Coordinates.lng);
            
            if (distBetween < minDistance)
            {
                minDistance = distBetween;
                closestId = lant.Id;
            }
        }
    }
    return closestId;
  }

  private object BuildTreeFromCoordinates(List<LanternDTO> lanterns)
  {
    var baseLat = 0.0;
    var baseLng = 0.0;

    var tree = lanterns.Select(l => new
    {
      id = l.Id,
      desc = l.LanternName,
      status = l.Status,
    
      pId = FindClosestParentId(l, lanterns, baseLat, baseLng)
    }).ToList();

    return new {nodes = tree};
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
    string id = drone.DroneID;
    List<DroneDTO> drones = _AdminService.GetAllDrones();
    if (drones.FirstOrDefault(d => d.DroneID == id) == null)
    {
      _AdminService.AddDrone(drone.DroneID, drone.DroneName);
      Console.WriteLine("[SIGNALR: Подключен и сохранён дрон: ]" + id);
    }
    Console.WriteLine("[SIGNALR] Дрон подключен" + id);
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
    if(!string.IsNullOrWhiteSpace(newL.LanternName))
    {
      lantern.LanternName = newL.LanternName;
    }
    if (!Double.IsNaN(newL.Coordinates.lat) & !Double.IsNaN(newL.Coordinates.lng))
    {
      lantern.Coordinates = newL.Coordinates;
    }
    _AdminService.EditLantern(id, newL);

    if (lantern.Status != newL.Status)
    {

      var allLaneterns = _AdminService.GetAllLanterns();
      var TaskTree = BuildTreeFromCoordinates(allLaneterns);

      await _DroneHubService.Clients.All.SendAsync("RecieveMission", TaskTree);
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