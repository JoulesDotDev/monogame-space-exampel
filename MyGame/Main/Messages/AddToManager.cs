using MyGame.Main.Components;

namespace MyGame.Main.Messages;

public class AddToManager : Message
{
    public object Object { get; set; }
}