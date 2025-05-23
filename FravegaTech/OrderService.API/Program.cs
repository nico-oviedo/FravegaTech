using OrderService.Application.Services;
using OrderService.Data.Repositories;
using CounterServices = CounterService.Services;
using OrderApplicationServices = OrderService.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<CounterServices.ICounterService, CounterServices.CounterService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<OrderApplicationServices.IOrderService, OrderApplicationServices.OrderService>();

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
