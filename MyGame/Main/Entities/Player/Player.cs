using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Main.Components;
using MyGame.Main.Components.CollisionShapes;
using MyGame.Main.Entities.Weapon;
using MyGame.Main.Messages;

namespace MyGame.Main.Entities.Player;

public class Player : IDisposable
{
    private readonly ContentManager _content = GameManager.GetService<ContentManager>();

    private Texture2D _sprite;

    private readonly CollisionComponent _collisionComponent;
    private readonly HealthComponent _healthComponent;
    private readonly MessageComponent _messageComponent;

    private readonly Weapon.Weapon _activeWeapon;

    private Vector2 _position;
    private float Speed { get; set; } = 600f;

    public Player()
    {
        LoadContent(_content);

        var identifierComponent = new IdentifierComponent(IdentifierType.Player);

        var xCenter = _sprite.Width / 2;
        var yCenter = _sprite.Height / 2;

        _collisionComponent =
            new CollisionComponent(
                _position,
                0,
                1,
                [
                    new RectangleBoundary(xCenter - 10, 0, 20, _sprite.Height),
                    new RectangleBoundary(0, yCenter - 10, _sprite.Width, 20),
                    new RectangleBoundary(
                        xCenter - (_sprite.Width - 30) / 2,
                        yCenter + 10,
                        _sprite.Width - 30,
                        30)
                ],
                identifierComponent
            );

        _healthComponent = new HealthComponent(3, identifierComponent);
        _activeWeapon = new LaserWeapon();

        _messageComponent = new MessageComponent(identifierComponent);
        _messageComponent.OnMessage += OnMessage;
    }

    private void LoadContent(ContentManager content)
    {
        _sprite = content.Load<Texture2D>("Sprite/player_ship");
    }

    public void Update(GameTime gameTime)
    {
        if (!_healthComponent.IsAlive)
        {
            return;
        }

        var direction = Vector2.Zero;
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.W))
        {
            direction.Y = -1;
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            direction.Y = 1;
        }

        if (keyboardState.IsKeyDown(Keys.A))
        {
            direction.X = -1;
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            direction.X = 1;
        }

        if (direction != Vector2.Zero)
        {
            direction = Vector2.Normalize(direction);
        }

        _position += direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        _collisionComponent.UpdatePosition(_position);
        _activeWeapon.Update(gameTime);

        if (!keyboardState.IsKeyDown(Keys.Space))
        {
            return;
        }

        if (_activeWeapon is LaserWeapon weapon)
        {
            var leftPos = _position + new Vector2(0, 5);
            var rightPos = _position + new Vector2(_sprite.Width, 5);

            weapon.FireMultiple(leftPos, rightPos, new Vector2(0, -1), 0, 1);
        }
        else
        {
            _activeWeapon.Fire(
                _position + new Vector2(_sprite.Width / 2f, -_sprite.Height / 2f + 5f),
                new Vector2(0, -1),
                0,
                1);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_healthComponent.IsAlive)
        {
            return;
        }

        spriteBatch.Draw(_sprite, _position, Color.White);
    }

    #region Messaging

    private void OnMessage(Message message)
    {
        switch (message)
        {
            case CollisionMessage msg:
                HandleCollision(msg);
                break;
            case DeathMessage:
                HandleDeath();
                break;
        }
    }

    private void HandleCollision(CollisionMessage message)
    {
        if (message.CollidedWithType == IdentifierType.Meteor)
        {
            _healthComponent.TakeDamage(1);
        }
    }

    private void HandleDeath()
    {
        Dispose();
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