using System;
using Microsoft.Xna.Framework;
using MyGame.Main.Entities.Weapon.Projectile;

namespace MyGame.Main.Entities.Weapon;

public class LaserWeapon : Weapon
{
    public LaserWeapon()
    {
        FireRate = TimeSpan.FromSeconds(0.1f);
    }

    public override void Update(GameTime gameTime)
    {
        Cooldown += gameTime.ElapsedGameTime;
    }

    public override void Fire(Vector2 position, Vector2 direction, int layer, int checkLayer)
    {
        if (Cooldown < FireRate)
        {
            return;
        }

        Cooldown = TimeSpan.Zero;

        LaserProjectile.Create(position, direction);
    }

    public void FireMultiple(Vector2 leftPos, Vector2 rightPos, Vector2 direction, int layer, int checkLayer)
    {
        if (Cooldown < FireRate)
        {
            return;
        }

        LaserProjectile.Create(leftPos, direction);
        LaserProjectile.Create(rightPos, direction);

        Cooldown = TimeSpan.Zero;
    }
}