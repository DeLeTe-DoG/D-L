using backend.Interfaces;
using backend.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<IAdmin, AdminService>();
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5003);
});

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAll");
app.MapControllers();

app.MapHub<DroneHubService>("/droneHub");

app.MapGet("/", () => "Система управленя дронами");

app.Run();