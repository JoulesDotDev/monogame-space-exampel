using MyGame.Main.Entities;

namespace MyGame.Main.Components;

public class IdentifierComponent
{
    public string Id { get; private set; }
    public IdentifierType Type { get; private set; }

    private static int _nextId;

    public IdentifierComponent(IdentifierType identifierType, bool isUnique = false)
    {
        Id = identifierType.ToString();

        if (!isUnique)
        {
            Id += GenerateUniqueId();
        }

        Type = identifierType;
    }

    private static string GenerateUniqueId()
    {
        return $"_entity_{_nextId++}";
    }
}