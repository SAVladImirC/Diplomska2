using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.Entities;
using Moq;
using Xunit;

namespace CleanArchitecture.Tests.UseCases;

/// <summary>
/// Contrast with NTier.Tests.Fakes.FakeOrderService: this use case only needs mocks
/// for the four small interfaces it actually declares, none of which are unrelated to
/// order creation (no invoicing or email mock required here).
/// </summary>
public class CreateOrderUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_OverDiscountThreshold_Applies10PercentDiscount()
    {
        var validator = new Mock<IValidateOrderUseCase>();
        var products = new Mock<IProductLookup>();
        var orderWriter = new Mock<IAddOrder>();

        products.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, UnitPrice = 60m });

        var useCase = new CreateOrderUseCase(
            validator.Object, new CalculateOrderTotalUseCase(), products.Object, orderWriter.Object);

        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            Items = [new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }] // 120 subtotal
        };

        var result = await useCase.ExecuteAsync(request);

        Assert.Equal(120m, result.Subtotal);
        Assert.Equal(12m, result.DiscountApplied);
        Assert.Equal(108m, result.Total);
        orderWriter.Verify(w => w.AddAsync(It.IsAny<Order>()), Times.Once);
    }
}
