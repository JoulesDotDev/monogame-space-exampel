using System;
using MyGame.Main.Managers;
using MyGame.Main.Messages;

namespace MyGame.Main.Components;

public class MessageComponent : IDisposable
{
    private readonly IdentifierComponent _identifierComponent;
    public readonly string Id;
    public event Action<Message> OnMessage;

    public MessageComponent(IdentifierComponent identifier)
    {
        _identifierComponent = identifier;
        Id = _identifierComponent.Id;
        MessageManager.RegisterComponent(_identifierComponent.Id, this);
    }

    public void ReceiveMessage(Message message)
    {
        OnMessage?.Invoke(message);
    }

    public void Dispose()
    {
        MessageManager.UnregisterComponent(_identifierComponent.Id);
        GC.SuppressFinalize(this);
    }
}