using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Main.Components;
using MyGame.Main.Entities;
using MyGame.Main.Messages;

namespace MyGame.Main.Managers;

public class CollisionManager
{
    private readonly MessageComponent _messageComponent;

    private readonly List<CollisionComponent> _collisionComponents = [];

    public CollisionManager()
    {
        var identifierComponent = new IdentifierComponent(IdentifierType.CollisionManager, true);
        _messageComponent = new MessageComponent(identifierComponent);
        _messageComponent.OnMessage += OnMessage;
    }

    public void Update(GameTime gameTime)
    {
        foreach (var component in _collisionComponents)
        {
            component.UpdateBoundingBox();
        }

        for (var i = 0; i < _collisionComponents.Count; i++)
        {
            var compA = _collisionComponents[i];

            for (var j = i + 1; j < _collisionComponents.Count; j++)
            {
                var compB = _collisionComponents[j];

                if (compA.CheckLayer != compB.CheckLayer)
                {
                    continue;
                }

                if (!compA.BoundingBox.Intersects(compB.BoundingBox))
                {
                    continue;
                }

                foreach (var collisionDetails in compA.Boundaries
                             .SelectMany(boundaryA => compB.Boundaries,
                                 (boundaryA, boundaryB) => boundaryA.Intersects(boundaryB))
                             .Where(collisionDetails => collisionDetails != null))
                {
                    compA.OnCollision(compB);
                    compB.OnCollision(compA);
                }
            }
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var collisionComponent in _collisionComponents)
        {
            collisionComponent.Draw(spriteBatch);
        }
    }

    #region Messaging

    private void OnMessage(Message message)
    {
        switch (message)
        {
            case AddToManager msg:
                HandleRegister(msg.Object as CollisionComponent);
                break;
            case RemoveFromManager msg:
                HandleDestroy(msg.Object as CollisionComponent);
                break;
        }
    }

    private void HandleRegister(CollisionComponent collisionComponent)
    {
        _collisionComponents.Add(collisionComponent);
    }

    private void HandleDestroy(CollisionComponent collisionComponent)
    {
        _collisionComponents.Remove(collisionComponent);
    }

    #endregion
}