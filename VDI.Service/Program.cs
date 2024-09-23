var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapControllers();

app.Run();