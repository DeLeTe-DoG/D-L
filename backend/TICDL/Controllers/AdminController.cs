using backend.Interfaces;
using backend.Models;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class AdminController : ControllerBase
{
  private readonly IAdmin _AdminService;

  public AdminController(IAdmin admin)
  {
    _AdminService = admin;
  }

  [HttpGet("drones")]
  public IActionResult GetAllDrones()
  {
    var drones = _AdminService.GetAllDrones();
    return Ok(drones);
  }

  [HttpPost("drones")]
  public IActionResult AddDrone([FromBody] DroneDTO drone)
  {
    if (string.IsNullOrWhiteSpace(drone.DroneName) || string.IsNullOrWhiteSpace(drone.DroneID))
    {
      return BadRequest("Имя дрона и ID обязательны!");
    }
    _AdminService.AddDrone(drone.DroneID, drone.DroneName);
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
  public IActionResult EditLantern(string id, [FromBody] LanternDTO newL)
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