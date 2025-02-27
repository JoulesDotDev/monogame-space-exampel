using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Main.Components;
using MyGame.Main.Managers;
using MyGame.Main.Messages;

namespace MyGame.Main.Entities.Weapon.Projectile;

public abstract class Projectile : IDisposable
{
    private readonly Texture2D _sprite;
    public Vector2 Position;
    private readonly Vector2 _direction;
    protected float Speed = 100f;

    private readonly CollisionComponent _collisionComponent;
    private readonly MessageComponent _messageComponent;

    protected Projectile(
        Vector2 position,
        Vector2 direction,
        Texture2D sprite,
        IdentifierComponent identifier,
        CollisionComponent collisionComponent
    )
    {
        Position = position;
        _direction = direction;
        _sprite = sprite;
        _collisionComponent = collisionComponent;

        _messageComponent = new MessageComponent(identifier);
        _messageComponent.OnMessage += OnMessage;

        MessageManager.SendMessage(new AddToManager() { Object = this }, IdentifierType.ProjectileManager);
    }

    public virtual void Update(GameTime gameTime)
    {
        Position += _direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _collisionComponent.UpdatePosition(Position);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sprite, Position, Color.White);
    }

    protected virtual void OnMessage(Message message)
    {
        switch (message)
        {
            case CollisionMessage msg:
                HandleCollision(msg);
                break;
        }
    }

    protected virtual void HandleCollision(CollisionMessage message)
    {
        MessageManager.SendMessage(new RemoveFromManager() { Object = this }, IdentifierType.ProjectileManager);
        Dispose();
    }

    #region Dispose

    private bool _disposed;

    public virtual void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _collisionComponent.Destroy();
        _messageComponent.Dispose();

        GC.SuppressFinalize(this);
        _disposed = true;
    }

    #endregion
}