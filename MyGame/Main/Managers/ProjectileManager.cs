using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Main.Components;
using MyGame.Main.Entities;
using MyGame.Main.Entities.Weapon.Projectile;
using MyGame.Main.Messages;

namespace MyGame.Main.Managers;

public class ProjectileManager
{
    private readonly Viewport _viewport = GameManager.GetService<GraphicsDevice>().Viewport;

    private readonly IdentifierComponent _identifierComponent;
    private readonly MessageComponent _messageComponent;

    private List<Projectile> _projectiles = [];

    public ProjectileManager()
    {
        _identifierComponent = new IdentifierComponent(IdentifierType.ProjectileManager, true);
        _messageComponent = new MessageComponent(_identifierComponent);
        _messageComponent.OnMessage += OnMessage;
    }

    public void Update(GameTime gameTime)
    {
        for (var i = _projectiles.Count - 1; i >= 0; i--)
        {
            _projectiles[i].Update(gameTime);

            if (_projectiles[i].Position.Y > _viewport.Height || _projectiles[i].Position.Y < -100)
            {
                _projectiles[i].Dispose();
                _projectiles.RemoveAt(i);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var projectile in _projectiles)
        {
            projectile.Draw(spriteBatch);
        }
    }

    private void OnMessage(Message message)
    {
        switch (message)
        {
            case AddToManager msg:
                HandleRegister(msg.Object as Projectile);
                break;
            case RemoveFromManager msg:
                HandleDestroy(msg.Object as Projectile);
                break;
        }
    }

    private void HandleRegister(Projectile projectile)
    {
        _projectiles.Add(projectile);
    }

    private void HandleDestroy(Projectile projectile)
    {
        _projectiles.Remove(projectile);
    }
}