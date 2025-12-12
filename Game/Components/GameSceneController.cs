using Engine;
using Game.Scenes;
using Raylib_CSharp.Interact;

namespace Game.Components;

public class GameSceneController : Component, IUpdatable
{
    public void Update(float dt)
    {
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