using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=cleanarchitecture.db"));

// EfOrderRepository implements every tiny role interface at once; each interface is
// exposed separately so a consumer only ever needs to declare the one(s) it uses.
builder.Services.AddScoped<EfOrderRepository>();
builder.Services.AddScoped<IGetOrders>(sp => sp.GetRequiredService<EfOrderRepository>());
builder.Services.AddScoped<IAddOrder>(sp => sp.GetRequiredService<EfOrderRepository>());
builder.Services.AddScoped<IUpdateOrder>(sp => sp.GetRequiredService<EfOrderRepository>());
builder.Services.AddScoped<ISoftDeleteOrder>(sp => sp.GetRequiredService<EfOrderRepository>());
builder.Services.AddScoped<IOrderRepository>(sp => sp.GetRequiredService<EfOrderRepository>());

builder.Services.AddScoped<ICustomerLookup, EfCustomerLookup>();
builder.Services.AddScoped<IProductLookup, EfProductLookup>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<IEmailService, ConsoleEmailService>();
builder.Services.AddScoped<IPdfGenerator, SimplePdfGenerator>();

builder.Services.AddScoped<IValidateOrderUseCase, ValidateOrderUseCase>();
builder.Services.AddScoped<ICalculateOrderTotalUseCase, CalculateOrderTotalUseCase>();
builder.Services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();
builder.Services.AddScoped<IUpdateOrderUseCase, UpdateOrderUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

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
