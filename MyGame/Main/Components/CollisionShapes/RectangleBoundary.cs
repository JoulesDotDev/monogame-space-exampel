using System;
using Microsoft.Xna.Framework;

namespace MyGame.Main.Components.CollisionShapes;

public class RectangleBoundary(int x, int y, int width, int height) : IBoundary
{
    public Rectangle Rect { get; set; } = new(x, y, width, height);

    public bool Contains(Vector2 point)
    {
        return Rect.Contains(point);
    }

    public Rectangle GetBoundingBox()
    {
        return Rect;
    }

    public CollisionDetails Intersects(IBoundary other)
    {
        return other switch
        {
            RectangleBoundary rb => GetRectRectDetails(this, rb),
            CircleBoundary cb => InvertCollisionDetails(cb.Intersects(this)),
            _ => null
        };
    }

    private static CollisionDetails GetRectRectDetails(RectangleBoundary a, RectangleBoundary b)
    {
        if (!a.Rect.Intersects(b.Rect))
        {
            return null;
        }

        var overlapX = Math.Min(a.Rect.Right, b.Rect.Right) - Math.Max(a.Rect.Left, b.Rect.Left);
        var overlapY = Math.Min(a.Rect.Bottom, b.Rect.Bottom) - Math.Max(a.Rect.Top, b.Rect.Top);

        if (overlapX < overlapY)
        {
            if (a.Rect.Center.X < b.Rect.Center.X)
            {
                return new CollisionDetails(
                    new Vector2(-1, 0),
                    overlapX,
                    new Vector2(a.Rect.Right - overlapX / 2f, Math.Max(a.Rect.Top, b.Rect.Top) + overlapY / 2f)
                );
            }

            return new CollisionDetails(
                new Vector2(1, 0),
                overlapX,
                new Vector2(a.Rect.Left + overlapX / 2f, Math.Max(a.Rect.Top, b.Rect.Top) + overlapY / 2f)
            );
        }
        else
        {
            if (a.Rect.Center.Y < b.Rect.Center.Y)
            {
                return new CollisionDetails(
                    new Vector2(0, -1),
                    overlapY,
                    new Vector2(Math.Max(a.Rect.Left, b.Rect.Left) + overlapX / 2f, a.Rect.Bottom - overlapY / 2f)
                );
            }

            return new CollisionDetails(
                new Vector2(0, 1),
                overlapY,
                new Vector2(Math.Max(a.Rect.Left, b.Rect.Left) + overlapX / 2f, a.Rect.Top + overlapY / 2f)
            );
        }
    }

    private static CollisionDetails InvertCollisionDetails(CollisionDetails details)
    {
        if (details == null)
        {
            return null;
        }

        return details with { Normal = -details.Normal };
    }
}