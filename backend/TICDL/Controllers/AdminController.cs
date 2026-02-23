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