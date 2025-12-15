using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Enums;
using Game.Components;
using Raylib_CSharp.Colors;

namespace Game.Scenes;

public class GameScene : Scene
{
    private List<Entity> _platforms = [];
    private Entity _player;
    private BoxColliderComponent _playerCollider;

    private const float PLAYER_SIZE = 20f;

    public override void Load()
    {
        CreateEntity().AddComponent<GameSceneController>();

        var backgroundA = CreateEntity();
        var background = backgroundA.AddComponent<SpriteComponent>();
        backgroundA.Transform.Position = new Vector2(-1000, -1000);
        background.Width = 2000;
        background.Height = 2000;
        background.Color = Color.SkyBlue;
        
        // Create player
        _player = CreateEntity();
        _player.Transform.Position = new Vector2(10, 10);
        _player.AddComponent<PlayerController>();
        var playerSprite = _player.AddComponent<SpriteComponent>();
        playerSprite.Width = (int)PLAYER_SIZE;
        playerSprite.Height = (int)PLAYER_SIZE;
        playerSprite.Color = Color.White;

        // Create ground
        CreatePlatform(10, Application.Instance.VirtualHeight - 20, 600, 10);
        
        // Platforms
        CreatePlatform(10, Application.Instance.VirtualHeight - 80, 200, 10);
        CreatePlatform(100, Application.Instance.VirtualHeight - 250, 200, 10);
        CreatePlatform(200, Application.Instance.VirtualHeight - 160, 200, 10);

        var camera = CreateEntity().AddComponent<CameraComponent>();
        camera.Target = _player;
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
}