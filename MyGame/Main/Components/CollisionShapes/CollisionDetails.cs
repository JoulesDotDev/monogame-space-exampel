using Microsoft.Xna.Framework;

namespace MyGame.Main.Components.CollisionShapes;

public record CollisionDetails(Vector2 Normal, float PenetrationDepth, Vector2 ContactPoint);