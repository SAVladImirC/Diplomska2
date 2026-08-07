using Microsoft.EntityFrameworkCore;
using NTier.Data;
using NTier.Data.Entities;
using NTier.Data.Repositories;
using NTier.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=ntier.db"));

// IRepository<Order> resolves to OrderRepository -- callers coding against the generic
// contract get the delivered-order LSP surprise described in the thesis.
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();
builder.Services.AddScoped<IRepository<Product>, Repository<Product>>();
builder.Services.AddScoped<IRepository<Customer>, Repository<Customer>>();
builder.Services.AddScoped<IOrderService, OrderService>();

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
