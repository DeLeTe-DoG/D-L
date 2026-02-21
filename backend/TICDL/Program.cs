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

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAll");
app.MapControllers();

app.MapGet("/", () => "Система управленя дронами");

app.Run();