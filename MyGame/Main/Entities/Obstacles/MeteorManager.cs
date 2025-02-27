using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Main.Components;
using MyGame.Main.Messages;

namespace MyGame.Main.Entities.Obstacles;

public class MeteorManager
{
    private readonly Viewport _viewport = GameManager.GetService<GraphicsDevice>().Viewport;

    private readonly MessageComponent _messageComponent;

    private readonly TimeSpan _spawnRate = TimeSpan.FromSeconds(3);
    private TimeSpan _timer = TimeSpan.Zero;

    private readonly List<Meteor> _meteors = [];

    public MeteorManager()
    {
        var identifierComponent = new IdentifierComponent(IdentifierType.MeteorManager, true);
        _messageComponent = new MessageComponent(identifierComponent);
        _messageComponent.OnMessage += OnMessage;
    }

    public void Update(GameTime gameTime)
    {
        HandleObstacles(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var obstacle in _meteors)
        {
            obstacle.Draw(gameTime, spriteBatch);
        }
    }

    private void OnMessage(Message message)
    {
        switch (message)
        {
            case RemoveFromManager msg:
                HandleRemove(msg.Object as Meteor);
                break;
        }
    }

    private void HandleRemove(Meteor meteor)
    {
        _meteors.Remove(meteor);
    }

    private void HandleObstacles(GameTime gameTime)
    {
        _timer += gameTime.ElapsedGameTime;
        for (var i = _meteors.Count - 1; i >= 0; i--)
        {
            var meteor = _meteors[i];
            meteor.Update(gameTime);

            if (!(meteor.Position.Y > _viewport.Height + 100f))
            {
                continue;
            }

            _meteors[i].Dispose();
            _meteors.RemoveAt(i);
        }

        if (_timer < _spawnRate)
        {
            return;
        }

        _timer = TimeSpan.Zero;

        var random = new Random();
        var randomX = random.Next(0, _viewport.Width);
        var spawnPosition = new Vector2(randomX, -100);

        var newMeteor = new Meteor(spawnPosition);
        _meteors.Add(newMeteor);
    }
}