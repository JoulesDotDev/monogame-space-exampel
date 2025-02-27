using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Main.Components;
using MyGame.Main.Components.CollisionShapes;
using MyGame.Main.Managers;
using MyGame.Main.Messages;

namespace MyGame.Main.Entities.Obstacles;

public class Meteor : IDisposable
{
    private readonly ContentManager _content = GameManager.GetService<ContentManager>();

    private Texture2D _sprite;

    private readonly CollisionComponent _collisionComponent;
    private readonly HealthComponent _healthComponent;
    private readonly MessageComponent _messageComponent;

    public Vector2 Position { get; private set; }
    private float Speed { get; set; } = 400f;
    private int MaxHealth { get; set; } = 1;

    public Meteor(Vector2 position)
    {
        LoadContent();

        Position = position;

        var identifierComponent = new IdentifierComponent(IdentifierType.Meteor);

        _collisionComponent =
            new CollisionComponent(
                Position,
                0,
                1,
                [new CircleBoundary(new Vector2(_sprite.Width / 2f, _sprite.Height / 2f), _sprite.Width * 0.34f)],
                identifierComponent
            );

        _healthComponent = new HealthComponent(MaxHealth, identifierComponent);

        _messageComponent = new MessageComponent(identifierComponent);
        _messageComponent.OnMessage += OnMessage;
    }

    private void LoadContent()
    {
        _sprite = _content.Load<Texture2D>("Sprite/meteor_small");
    }

    public void Update(GameTime gameTime)
    {
        var yPosition = Position.Y + Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position = new Vector2(Position.X, yPosition);
        _collisionComponent.UpdatePosition(Position);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sprite, Position, Color.White);
    }

    #region Messaging

    private void OnMessage(Message message)
    {
        switch (message)
        {
            case DeathMessage:
                HandleDeath();
                break;
            case CollisionMessage msg:
                HandleCollision(msg);
                break;
        }
    }

    private void HandleDeath()
    {
        var removeMessage = new RemoveFromManager() { Object = this };
        MessageManager.SendMessage(removeMessage, IdentifierType.MeteorManager);
        Dispose();
    }

    private void HandleCollision(CollisionMessage message)
    {
        switch (message.CollidedWithType)
        {
            case IdentifierType.Player:
                _healthComponent.TakeDamage(MaxHealth);
                break;
            case IdentifierType.Projectile:
                _healthComponent.TakeDamage(2);
                break;
        }
    }

    #endregion

    #region Dispose

    private bool _disposed;

    public void Dispose()
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