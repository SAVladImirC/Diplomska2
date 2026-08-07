using FluentValidation.Results;

namespace VerticalSlice.Web.Common;

/// <summary>Plumbing shared across slices for turning validation failures into a
/// ProblemDetails-friendly shape. Not business logic, so sharing it doesn't
/// contradict the deliberate per-slice duplication of business rules elsewhere.</summary>
public static class ValidationResultExtensions
{
    public static IDictionary<string, string[]> ToErrorDictionary(this ValidationResult result) =>
        result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
}
