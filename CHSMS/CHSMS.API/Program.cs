var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
//testing push
//test2

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
