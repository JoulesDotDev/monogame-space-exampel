using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Main.Components.CollisionShapes;
using MyGame.Main.Entities;
using MyGame.Main.Managers;
using MyGame.Main.Messages;

namespace MyGame.Main.Components;

public class CollisionComponent
{
    private readonly ContentManager _content = GameManager.GetService<ContentManager>();
    private Texture2D _debugPixel;
    private readonly Color _debugColor = new(255, 0, 0, 64);
    private Texture2D _debugCircle;

    private readonly IdentifierComponent _identifierComponent;

    public int Layer { get; private set; }
    public int CheckLayer { get; private set; }
    public string Tag { get; private set; }

    private Vector2 Position { get; set; }
    public List<IBoundary> Boundaries { get; private set; }
    private List<IBoundary> LocalBoundaries { get; set; }
    public Rectangle BoundingBox { get; private set; }

    public CollisionComponent(
        Vector2 position,
        int layer,
        int checkLayer,
        List<IBoundary> boundaries,
        IdentifierComponent identifier
    )
    {
        LoadContent();
        Register();

        _identifierComponent = identifier;
        Layer = layer;
        CheckLayer = checkLayer;
        Tag = identifier.Type.ToString();

        Position = position;
        LocalBoundaries = boundaries;
        UpdateBoundaries();
    }

    private void LoadContent()
    {
        _debugPixel = _content.Load<Texture2D>("Sprite/pixel");
        _debugCircle = _content.Load<Texture2D>("Sprite/circle");
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var boundary in Boundaries)
        {
            switch (boundary)
            {
                case RectangleBoundary rb:
                    spriteBatch.Draw(_debugPixel, new Rectangle(rb.Rect.X, rb.Rect.Y, rb.Rect.Width, rb.Rect.Height),
                        _debugColor);
                    break;
                case CircleBoundary cb:
                {
                    var circleRect = new Rectangle(
                        (int)(cb.CircleCenter.X - cb.Radius),
                        (int)(cb.CircleCenter.Y - cb.Radius),
                        (int)(cb.Radius * 2),
                        (int)(cb.Radius * 2));
                    spriteBatch.Draw(_debugCircle, circleRect, _debugColor);
                    break;
                }
            }
        }
    }

    public void UpdatePosition(Vector2 position)
    {
        Position = position;
        UpdateBoundaries();
    }

    private void UpdateBoundaries()
    {
        Boundaries = [];
        foreach (var localBoundary in LocalBoundaries)
        {
            Boundaries.Add(TranslateBoundary(localBoundary, Position));
        }
    }


    private static IBoundary TranslateBoundary(IBoundary boundary, Vector2 offset)
    {
        switch (boundary)
        {
            case RectangleBoundary rb:
            {
                var localRect = rb.Rect;
                return new RectangleBoundary(
                    (int)(offset.X + localRect.X),
                    (int)(offset.Y + localRect.Y),
                    localRect.Width,
                    localRect.Height);
            }
            case CircleBoundary cb:
            {
                var localCenter = cb.CircleCenter;
                return new CircleBoundary(new Vector2(offset.X + localCenter.X, offset.Y + localCenter.Y), cb.Radius);
            }
        }

        throw new NotSupportedException("Boundary type not supported.");
    }

    public void UpdateBoundingBox()
    {
        if (Boundaries == null || Boundaries.Count == 0)
        {
            BoundingBox = Rectangle.Empty;
            return;
        }

        var overall = Boundaries[0].GetBoundingBox();

        for (var i = 1; i < Boundaries.Count; i++)
        {
            overall = Rectangle.Union(overall, Boundaries[i].GetBoundingBox());
        }

        BoundingBox = overall;
    }

    public void OnCollision(CollisionComponent other)
    {
        var collisionMessage = new CollisionMessage
        {
            CollidedWithId = other._identifierComponent.Id,
            CollidedWithType = other._identifierComponent.Type
        };

        MessageManager.SendMessage(collisionMessage, _identifierComponent.Id);
    }

    private void Register()
    {
        var registerMessage = new AddToManager()
        {
            Object = this
        };
        MessageManager.SendMessage(registerMessage, IdentifierType.CollisionManager);
    }

    public void Destroy()
    {
        var deregisterMessage = new RemoveFromManager()
        {
            Object = this
        };
        MessageManager.SendMessage(deregisterMessage, IdentifierType.CollisionManager);
    }
}