using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Main.Entities.Obstacles;
using MyGame.Main.Entities.Player;
using MyGame.Main.Entities.Scenery;
using MyGame.Main.Managers;

namespace MyGame.Main;

public class GameManager : Game
{
    private static GameManager Instance { get; set; }
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Player _player;

    private MeteorManager _meteorManager;
    private CollisionManager _collisionManager;
    private StarManager _starManager;
    private ProjectileManager _projectileManager;

    public GameManager()
    {
        Content.RootDirectory = "Content";

        _graphics = new GraphicsDeviceManager(this);
        _graphics.SynchronizeWithVerticalRetrace = false;
        _graphics.ApplyChanges();

        IsMouseVisible = true;
        IsFixedTimeStep = false;
        Instance = this;
    }

    protected override void LoadContent()
    {
        Services.AddService(typeof(ContentManager), Content);
        Services.AddService(typeof(GraphicsDevice), _graphics.GraphicsDevice);

        _collisionManager = new CollisionManager();
        _meteorManager = new MeteorManager();
        _starManager = new StarManager();
        _projectileManager = new ProjectileManager();

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _player = new Player();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _player.Update(gameTime);

        _meteorManager.Update(gameTime);
        _collisionManager.Update(gameTime);
        _starManager.Update(gameTime);
        _projectileManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _player.Draw(_spriteBatch);

        _meteorManager.Draw(gameTime, _spriteBatch);
        _starManager.Draw(_spriteBatch);
        _projectileManager.Draw(_spriteBatch);

        if (System.Diagnostics.Debugger.IsAttached)
        {
            _collisionManager.Draw(_spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public static T GetService<T>() where T : class
    {
        return Instance.Services.GetService<T>();
    }
}