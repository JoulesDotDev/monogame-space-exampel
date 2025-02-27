using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Main.Components;
using MyGame.Main.Components.CollisionShapes;
using MyGame.Main.Managers;
using MyGame.Main.Messages;

namespace MyGame.Main.Entities.Weapon.Projectile;

public class LaserProjectile : Projectile
{
    private static readonly ContentManager Content = GameManager.GetService<ContentManager>();

    private static Texture2D GetLaserSprite()
    {
        return Content.Load<Texture2D>("Sprite/laser_green");
    }

    private static CollisionComponent CreateCollisionComponent(Vector2 position, IdentifierComponent identifier)
    {
        var sprite = GetLaserSprite();

        return new CollisionComponent(
            position,
            0,
            1,
            [
                new CircleBoundary(new Vector2(sprite.Width / 2f, 5), 5f)
            ],
            identifier
        );
    }

    private LaserProjectile(
        Vector2 position,
        Vector2 direction,
        Texture2D sprite,
        IdentifierComponent identifier,
        CollisionComponent collisionComponent
    ) : base(position, direction, sprite, identifier, collisionComponent)
    {
        Speed = 500f;
    }

    public static void Create(Vector2 position, Vector2 direction)
    {
        var identifier = new IdentifierComponent(IdentifierType.Projectile);
        var sprite = GetLaserSprite();
        position += new Vector2(-sprite.Width / 2f, 0);

        var collision = CreateCollisionComponent(position, identifier);
        var _ = new LaserProjectile(position, direction, sprite, identifier, collision);
    }
}