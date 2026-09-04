using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.ReadModels;
using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.Enums;
using Moq;
using Xunit;

namespace CleanArchitecture.Tests.UseCases;

public class GetOrdersUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_MapsReadModelsAndAppliesDiscount()
    {
        var queries = new Mock<IOrderQueries>();
        queries.Setup(q => q.GetAllAsync()).ReturnsAsync(
        [
            new OrderReadModel(1, 1, OrderStatus.Pending, 120m, DateTime.UtcNow),
            new OrderReadModel(2, 1, OrderStatus.Shipped, 60m, DateTime.UtcNow)
        ]);

        var useCase = new GetOrdersUseCase(queries.Object, new CalculateOrderTotalUseCase());

        var result = await useCase.ExecuteAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(108m, result[0].Total);
        Assert.Equal("Pending", result[0].Status);
        Assert.Equal(60m, result[1].Total);
    }
}
