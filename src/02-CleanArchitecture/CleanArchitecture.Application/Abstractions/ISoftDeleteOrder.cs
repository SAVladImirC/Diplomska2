namespace CleanArchitecture.Application.Abstractions;

/// <summary>
/// Deliberately narrower than N-Tier's generic Delete: this contract only ever
/// promises a soft delete, so an implementation can honor it unconditionally --
/// there is no status for which "mark as deleted" can meaningfully fail.
/// </summary>
public interface ISoftDeleteOrder
{
    Task SoftDeleteAsync(int orderId);
}
