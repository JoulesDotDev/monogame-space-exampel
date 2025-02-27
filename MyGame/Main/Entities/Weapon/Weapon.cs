using System;
using Microsoft.Xna.Framework;

namespace MyGame.Main.Entities.Weapon;

public abstract class Weapon
{
    protected TimeSpan FireRate;
    protected TimeSpan Cooldown = TimeSpan.Zero;

    public abstract void Update(GameTime gameTime);
    public abstract void Fire(Vector2 position, Vector2 direction, int layer, int checkLayer);
}