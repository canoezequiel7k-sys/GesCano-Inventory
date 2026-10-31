namespace Inventory.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entity, object id)
        : base($"No se encontró {entity} con identificador '{id}'.")
    {
    }
}
