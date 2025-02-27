using Microsoft.Xna.Framework;

namespace MyGame.Main.Components.CollisionShapes;

public interface IBoundary
{
    bool Contains(Vector2 point);
    Rectangle GetBoundingBox();
    CollisionDetails Intersects(IBoundary other);
}