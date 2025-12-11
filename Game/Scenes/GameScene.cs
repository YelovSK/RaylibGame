using System.Numerics;
using Engine;
using Engine.Components;
using Game.Components;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;

namespace Game.Scenes;

public class GameScene : Scene
{
    private List<Entity> _platforms = [];
    private Entity _player;
    private PlayerController _playerController;
    private BoxColliderComponent _playerCollider;

    private const float PLAYER_SIZE = 20f;

    public override void Load()
    {
        // Create player
        _player = new Entity(this);
        _player.Transform.Position = new Vector2(10, 10);
        _player.AddComponent(new SpriteComponent
        {
            Width = (int)PLAYER_SIZE,
            Height = (int)PLAYER_SIZE,
            Color = Color.White
        });
        _playerCollider = _player.AddComponent(new BoxColliderComponent
        {
            Bounds = new(_player.Transform.Position.X, _player.Transform.Position.Y, PLAYER_SIZE, PLAYER_SIZE)
        });
        _playerController = _player.AddComponent(new PlayerController());

        // Create ground
        CreatePlatform(10, Application.Instance.VirtualHeight - 20, 600, 10);
        
        // Platforms
        CreatePlatform(10, Application.Instance.VirtualHeight - 80, 200, 10);
        CreatePlatform(100, Application.Instance.VirtualHeight - 250, 200, 10);
        CreatePlatform(200, Application.Instance.VirtualHeight - 160, 200, 10);
        
        var camera = new Entity(this);
        var cameraComponent = camera.AddComponent(new CameraComponent());
        cameraComponent.Target = _player;
    }

    private void CreatePlatform(int x, int y, int width, int height)
    {
        var platform = new Entity(this, new Vector2(x, y));
        platform.AddComponent(new SpriteComponent
        {
            Width = width,
            Height = height,
            Color = Color.Green
        });
        platform.AddComponent(new BoxColliderComponent
        {
            Bounds = new(platform.Transform.Position.X, platform.Transform.Position.Y, width, height)
        });
        _platforms.Add(platform);
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (Input.IsKeyPressed(KeyboardKey.Escape))
        {
            SceneManager.Instance.Push(new MenuScene());
        }
        
        if (Input.IsKeyPressed(KeyboardKey.R))
        {
            SceneManager.Instance.Pop();
            SceneManager.Instance.Push(new GameScene());
            GC.Collect();
        }
    }
}