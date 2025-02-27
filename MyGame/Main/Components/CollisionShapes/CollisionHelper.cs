namespace MyGame.Main.Components.CollisionShapes;

using Microsoft.Xna.Framework;

public static class CollisionHelper
{
    public static bool Intersects(Rectangle rect, Vector2 circleCenter, float circleRadius)
    {
        var nearestX = MathHelper.Clamp(circleCenter.X, rect.Left, rect.Right);
        var nearestY = MathHelper.Clamp(circleCenter.Y, rect.Top, rect.Bottom);

        var deltaX = circleCenter.X - nearestX;
        var deltaY = circleCenter.Y - nearestY;

        return deltaX * deltaX + deltaY * deltaY <= circleRadius * circleRadius;
    }
}