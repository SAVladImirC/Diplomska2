using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.ReadModels;

public record OrderReadModel(int Id, int CustomerId, OrderStatus Status, decimal Subtotal, DateTime CreatedAt);
