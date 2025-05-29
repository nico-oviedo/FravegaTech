using OrderService.Application.Mappers;
using OrderService.Application.Services;
using OrderService.Application.Services.Interfaces;
using OrderService.Data.Repositories;
using SharedKernel.ServiceClients;
using CounterServices = CounterService.Services;
using OrderApplicationServices = OrderService.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddAutoMapper(typeof(OrderProfile));
builder.Services.AddAutoMapper(typeof(EventProfile));

builder.Services.AddHttpClient<BuyerServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7185/api/v1/Buyers/");
});

builder.Services.AddHttpClient<ProductServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7172/api/v1/Products/");
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<CounterServices.ICounterService, CounterServices.CounterService>();
builder.Services.AddScoped<IEventValidationService, EventValidationService>();
builder.Services.AddScoped<IOrderValidationService, OrderValidationService>();
builder.Services.AddScoped<IOrderExternalDataService, OrderExternalDataService>();
builder.Services.AddScoped<IOrderService, OrderApplicationServices.OrderService>();

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
