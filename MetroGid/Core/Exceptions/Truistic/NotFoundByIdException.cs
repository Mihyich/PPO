namespace MetroGid.Core.Exceptions.Truistic;

public class NotFoundByIdException : Exception
{
    public string EntityName { get; }
    public int Id { get; }

    public NotFoundByIdException(string entityName, int id)
        : base($"Сущность '{entityName}' с ID '{id}' не найдена")
    {
        EntityName = entityName ?? throw new ArgumentNullException(nameof(entityName));
        Id = id;
    }
}