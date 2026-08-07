using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VerticalSlice.Infrastructure.Entities;
using VerticalSlice.Infrastructure.Persistence;
using VerticalSlice.Infrastructure.Services;
using VerticalSlice.Web.Features.Orders.CreateOrder;
using VerticalSlice.Web.Features.Orders.DeleteOrder;
using VerticalSlice.Web.Features.Orders.GetOrders;
using VerticalSlice.Web.Features.Orders.UpdateOrder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=verticalslice.db"));

builder.Services.AddScoped<IEmailService, ConsoleEmailService>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCreateOrder();
app.MapGetOrders();
app.MapUpdateOrder();
app.MapDeleteOrder();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Customers.Any())
    {
        db.Customers.Add(new Customer { Name = "Ана Петровска", Email = "ana@example.com" });
        db.Products.AddRange(
            new Product { Name = "Тастатура", UnitPrice = 45m },
            new Product { Name = "Монитор", UnitPrice = 120m });
        db.SaveChanges();
    }
}

app.Run();
