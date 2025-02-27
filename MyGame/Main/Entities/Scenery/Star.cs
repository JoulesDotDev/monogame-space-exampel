using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Main.Entities.Scenery;

public class Star(Vector2 position, Texture2D sprite, float scale, float speed)
{
    public Vector2 Position = position;

    public void Update(GameTime gameTime)
    {
        Position.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(sprite, Position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }
}