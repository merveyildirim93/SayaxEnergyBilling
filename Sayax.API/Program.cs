using Microsoft.EntityFrameworkCore;
using Sayax.Application.Interfaces;
using Sayax.Application.Repositories;
using Sayax.Application.Services;
using Sayax.Infrastructure.Data;
using Sayax.Infrastructure.Repositories;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SayaxDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SayaxConnection")));

builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IConsumptionRepository, ConsumptionRepository>();
builder.Services.AddScoped<IPriceRepository, PriceRepository>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular frontend’in çalýþtýðý origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("tr-TR");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
