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
    private BoxColliderComponent _playerCollider;

    private const float PLAYER_SIZE = 20f;

    public override void Load()
    {
        // Create player
        _player = CreateEntity();
        _player.Transform.Position = new Vector2(10, 10);
        
        var playerSprite = _player.AddComponent<SpriteComponent>();
        playerSprite.Width = (int)PLAYER_SIZE;
        playerSprite.Height = (int)PLAYER_SIZE;
        playerSprite.Color = Color.White;

        var playerCollider = _player.AddComponent<BoxColliderComponent>();
        playerCollider.Bounds = new(_player.Transform.Position.X, _player.Transform.Position.Y, PLAYER_SIZE, PLAYER_SIZE);
        _player.AddComponent<PlayerController>();

        // Create ground
        CreatePlatform(10, Application.Instance.VirtualHeight - 20, 600, 10);
        
        // Platforms
        CreatePlatform(10, Application.Instance.VirtualHeight - 80, 200, 10);
        CreatePlatform(100, Application.Instance.VirtualHeight - 250, 200, 10);
        CreatePlatform(200, Application.Instance.VirtualHeight - 160, 200, 10);

        var camera = CreateEntity();
        var cameraComponent = camera.AddComponent<CameraComponent>();
        cameraComponent.Target = _player;
    }

    private void CreatePlatform(int x, int y, int width, int height)
    {
        var platform = CreateEntity();
        platform.Transform.Position = new Vector2(x, y);
        
        var sprite = platform.AddComponent<SpriteComponent>();
        sprite.Width = width;
        sprite.Height = height;
        sprite.Color = Color.Green;

        var collider = platform.AddComponent<BoxColliderComponent>();
        collider.Bounds = new(platform.Transform.Position.X, platform.Transform.Position.Y, width, height);
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
        }
    }
}