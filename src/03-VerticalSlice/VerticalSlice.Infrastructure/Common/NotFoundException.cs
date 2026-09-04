namespace VerticalSlice.Infrastructure.Common;

public class NotFoundException(string entityName, int id) : Exception($"{entityName} {id} does not exist.");
