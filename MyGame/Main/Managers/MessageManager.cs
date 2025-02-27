using System;
using System.Collections.Generic;
using MyGame.Main.Components;
using MyGame.Main.Entities;
using MyGame.Main.Messages;

namespace MyGame.Main.Managers;

public static class MessageManager
{
    private static readonly Dictionary<string, MessageComponent> Components = new();

    public static void RegisterComponent(string identifier, MessageComponent component)
    {
        Components[identifier] = component;
    }

    public static void UnregisterComponent(string identifier)
    {
        Components.Remove(identifier);
    }

    public static void SendMessage(Message message, string destinationId)
    {
        if (Components.TryGetValue(destinationId, out var target))
        {
            message.Sender = target.Id;
            target.ReceiveMessage(message);
        }
        else
        {
            Console.WriteLine($"Destination '{destinationId}' not found. {message.Sender}");
        }
    }

    public static void SendMessage(Message message, IdentifierType destinationIdentifier)
    {
        SendMessage(message, destinationIdentifier.ToString());
    }
}