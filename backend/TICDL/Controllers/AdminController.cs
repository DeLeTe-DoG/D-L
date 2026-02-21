using backend.Interfaces;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/server")]
public class AdminController : ControllerBase
{
  private readonly IAdmin _AdminService;

  public AdminController(IAdmin admin)
  {
    _AdminService = admin;
  }

  
}