using MyGame.Main.Components;

namespace MyGame.Main.Messages;

public class RemoveFromManager : Message
{
    public object Object { get; set; }
}