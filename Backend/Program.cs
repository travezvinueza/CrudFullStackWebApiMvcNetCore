using Backend.Data;
using Backend.Service;
using Backend.Service.Impl;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers(); //agregue  el middleware de autenticación y autorización
builder.Services.AddHttpContextAccessor();//agregue Http

builder.Services.AddDbContext<DatabaseContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL")));

//agregue mis servicios
builder.Services.AddTransient<IProductoService, ProductoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); //agregue  para con archivos

app.MapControllers(); //agregue  esta linea para que funcione el controlador de rutas

app.Run();