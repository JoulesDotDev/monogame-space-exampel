using MyGame.Main.Entities;

namespace MyGame.Main.Messages;

public class CollisionMessage : Message
{
    public IdentifierType CollidedWithType { get; set; }
    public string CollidedWithId { get; set; }
}