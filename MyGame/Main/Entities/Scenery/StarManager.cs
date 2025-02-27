using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Main.Entities.Scenery;

public class StarManager
{
    private readonly ContentManager _content = GameManager.GetService<ContentManager>();
    private readonly Viewport _viewport = GameManager.GetService<GraphicsDevice>().Viewport;

    private readonly List<Star> _stars = [];
    private readonly List<Texture2D> _sprites = [];

    private readonly TimeSpan _spawnRate = TimeSpan.FromSeconds(0.3f);
    private TimeSpan _timer = TimeSpan.Zero;

    public StarManager()
    {
        _sprites.Add(_content.Load<Texture2D>("Sprite/star_tiny"));
        _sprites.Add(_content.Load<Texture2D>("Sprite/star_small"));
        _sprites.Add(_content.Load<Texture2D>("Sprite/star_medium"));
    }

    public void Update(GameTime gameTime)
    {
        for (var i = _stars.Count - 1; i >= 0; i--)
        {
            _stars[i].Update(gameTime);

            if (_stars[i].Position.Y > _viewport.Height + 100f)
            {
                _stars.RemoveAt(i);
            }
        }

        _timer += gameTime.ElapsedGameTime;
        if (_timer <= _spawnRate)
        {
            return;
        }

        _timer = TimeSpan.Zero;

        var random = new Random();
        var randomX = random.Next(0, _viewport.Width);
        var position = new Vector2(randomX, -100f);
        var index = random.Next(0, _sprites.Count);
        var sprite = _sprites[index];
        var speed = random.Next(50, 300);

        var scale = 1f;
        if (index != 0)
        {
            scale = 0.15f;
        }

        var star = new Star(position, sprite, scale, speed);
        _stars.Add(star);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var star in _stars)
        {
            star.Draw(spriteBatch);
        }
    }
}