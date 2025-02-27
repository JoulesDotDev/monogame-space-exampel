using System;
using Microsoft.Xna.Framework;
using MyGame.Main.Const;

namespace MyGame.Main.Components.CollisionShapes;

public class CircleBoundary(Vector2 center, float radius) : IBoundary
{
    public Vector2 CircleCenter { get; private set; } = center;
    public float Radius { get; private set; } = radius;

    private static readonly float Tolerance = Numbers.FloatEpsilon;

    public bool Contains(Vector2 point)
    {
        return Vector2.Distance(point, CircleCenter) <= Radius;
    }

    public Rectangle GetBoundingBox()
    {
        return new Rectangle(
            (int)(CircleCenter.X - Radius),
            (int)(CircleCenter.Y - Radius),
            (int)(2 * Radius),
            (int)(2 * Radius)
        );
    }

    public CollisionDetails Intersects(IBoundary other)
    {
        return other switch
        {
            CircleBoundary cb => GetCircleCircleDetails(this, cb),
            RectangleBoundary rb => GetCircleRectDetails(this, rb),
            _ => null
        };
    }

    private static CollisionDetails GetCircleCircleDetails(CircleBoundary a, CircleBoundary b)
    {
        var centerToCenter = b.CircleCenter - a.CircleCenter;
        var distance = centerToCenter.Length();
        var sumRadii = a.Radius + b.Radius;

        if (distance >= sumRadii)
        {
            return null;
        }

        Vector2 normal;
        if (distance < 0.0001f)
        {
            normal = new Vector2(1, 0);
        }
        else
        {
            normal = centerToCenter / distance;
        }

        var penetrationDepth = sumRadii - distance;

        var contactPoint = a.CircleCenter + normal * a.Radius;

        return new CollisionDetails(normal, penetrationDepth, contactPoint);
    }

    private static CollisionDetails GetCircleRectDetails(CircleBoundary circle, RectangleBoundary rect)
    {
        var closestX = MathHelper.Clamp(circle.CircleCenter.X, rect.Rect.Left, rect.Rect.Right);
        var closestY = MathHelper.Clamp(circle.CircleCenter.Y, rect.Rect.Top, rect.Rect.Bottom);
        var closestPoint = new Vector2(closestX, closestY);

        var centerToClosest = circle.CircleCenter - closestPoint;
        var distance = centerToClosest.Length();

        if (distance > circle.Radius)
        {
            return null;
        }

        if (rect.Rect.Contains(circle.CircleCenter))
        {
            var distToLeft = circle.CircleCenter.X - rect.Rect.Left;
            var distToRight = rect.Rect.Right - circle.CircleCenter.X;
            var distToTop = circle.CircleCenter.Y - rect.Rect.Top;
            var distToBottom = rect.Rect.Bottom - circle.CircleCenter.Y;

            var minDist = MathHelper.Min(distToLeft, MathHelper.Min(distToRight,
                MathHelper.Min(distToTop, distToBottom)));

            if (Math.Abs(minDist - distToLeft) < Tolerance)
            {
                return new CollisionDetails(
                    new Vector2(-1, 0),
                    distToLeft + circle.Radius,
                    new Vector2(rect.Rect.Left, circle.CircleCenter.Y)
                );
            }

            if (Math.Abs(minDist - distToRight) < Tolerance)
            {
                return new CollisionDetails(
                    new Vector2(1, 0),
                    distToRight + circle.Radius,
                    new Vector2(rect.Rect.Right, circle.CircleCenter.Y)
                );
            }

            if (Math.Abs(minDist - distToTop) < Tolerance)
            {
                return new CollisionDetails(
                    new Vector2(0, -1),
                    distToTop + circle.Radius,
                    new Vector2(circle.CircleCenter.X, rect.Rect.Top)
                );
            }

            return new CollisionDetails(
                new Vector2(0, 1),
                distToBottom + circle.Radius,
                new Vector2(circle.CircleCenter.X, rect.Rect.Bottom)
            );
        }

        if (distance < 0.0001f)
        {
            return new CollisionDetails(
                new Vector2(1, 0),
                circle.Radius,
                closestPoint
            );
        }

        var normal = centerToClosest / distance;
        var penetrationDepth = circle.Radius - distance;
        return new CollisionDetails(normal, penetrationDepth, closestPoint);
    }
}