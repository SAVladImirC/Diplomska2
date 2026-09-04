using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.Entities;
using Moq;
using Xunit;

namespace CleanArchitecture.Tests.UseCases;

public class CreateOrderUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_OverDiscountThreshold_Applies10PercentDiscount()
    {
        var validator = new Mock<IValidateOrderUseCase>();
        var products = new Mock<IProductLookup>();
        var orderWriter = new Mock<IAddOrder>();
        var unitOfWork = new Mock<IUnitOfWork>();

        products.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, UnitPrice = 60m });

        var useCase = new CreateOrderUseCase(
            validator.Object, new CalculateOrderTotalUseCase(), products.Object, orderWriter.Object, unitOfWork.Object);

        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }]
        };

        var result = await useCase.ExecuteAsync(request);

        Assert.Equal(120m, result.Subtotal);
        Assert.Equal(12m, result.DiscountApplied);
        Assert.Equal(108m, result.Total);
        orderWriter.Verify(w => w.AddAsync(It.IsAny<Order>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
