using BuyerService.Application.Mappers;
using BuyerService.Data.Repositories;
using BuyerApplicationServices = BuyerService.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddAutoMapper(typeof(BuyerProfile));

builder.Services.AddScoped<IBuyerRepository, BuyerRepository>();
builder.Services.AddScoped<BuyerApplicationServices.IBuyerService, BuyerApplicationServices.BuyerService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
