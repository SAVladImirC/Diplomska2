using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NTier.Data;
using NTier.Data.Entities;
using NTier.Data.Repositories;
using NTier.Services;
using NTier.Services.Dtos;
using Xunit;

namespace NTier.Tests;

public class OrderServiceValidationTests
{
    private static async Task<(OrderService Service, Customer Customer, Product Product)> CreateServiceAsync()
    {
        var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        var customer = new Customer { Name = "Тест Клиент", Email = "test@example.com" };
        var product = new Product { Name = "Widget", UnitPrice = 60m };
        context.Customers.Add(customer);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new OrderService(
            new OrderRepository(context), new Repository<Product>(context), new Repository<Customer>(context));
        return (service, customer, product);
    }

    private static List<ValidationResult> ValidateAsPresentationLayerWould(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public async Task CreateOrder_WithNoItems_ThrowsInServiceLayer()
    {
        var (service, customer, _) = await CreateServiceAsync();
        var request = new CreateOrderRequest { CustomerId = customer.Id, Items = [] };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateOrder(request));
    }

    [Fact]
    public async Task CreateOrder_WithNonPositiveQuantity_ThrowsInServiceLayer()
    {
        var (service, customer, product) = await CreateServiceAsync();
        var request = new CreateOrderRequest
        {
            CustomerId = customer.Id,
            Items = [new CreateOrderItemRequest { ProductId = product.Id, Quantity = 0 }]
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateOrder(request));
    }

    [Fact]
    public void CreateOrderRequest_WithNoItems_IsAlsoRejectedByPresentationLayerModelValidation()
    {
        var request = new CreateOrderRequest { CustomerId = 1, Items = [] };

        var errors = ValidateAsPresentationLayerWould(request);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateOrderRequest.Items)));
    }

    [Fact]
    public void CreateOrderItemRequest_WithNonPositiveQuantity_IsAlsoRejectedByPresentationLayerModelValidation()
    {
        var item = new CreateOrderItemRequest { ProductId = 1, Quantity = 0 };

        var errors = ValidateAsPresentationLayerWould(item);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateOrderItemRequest.Quantity)));
    }
}
