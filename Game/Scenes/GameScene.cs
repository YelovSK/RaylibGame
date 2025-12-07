using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Helpers;
using Game.Components;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;

namespace Game.Scenes;

public class GameScene : Scene
{
    private List<Entity> _platforms = new();
    private Entity _player;
    private PlayerController _playerController;
    private BoxColliderComponent _playerCollider;

    private const float PLAYER_SIZE = Layout.VIRTUAL_HEIGHT * 0.05f;

    public override void Load()
    {
        var middle = Layout.Center(0, 0);
        
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
        AddEntity(_player);

        // Create ground
        CreatePlatform(10, Layout.VIRTUAL_HEIGHT - 20, Layout.VIRTUAL_WIDTH - 20, 10);
        
        // Platforms
        CreatePlatform(10, Layout.VIRTUAL_HEIGHT - 80, 200, 10);
        CreatePlatform(100, Layout.VIRTUAL_HEIGHT - 250, 200, 10);
        CreatePlatform(200, Layout.VIRTUAL_HEIGHT - 160, 200, 10);
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
        AddEntity(platform);
        _platforms.Add(platform);
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (Input.IsKeyPressed(KeyboardKey.Escape))
        {
            SceneManager.Instance.LoadScene(new MenuScene());
        }
    }
}