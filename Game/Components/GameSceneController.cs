using Engine;
using Game.Scenes;

using Raylib_cs;

namespace Game.Components;

public class GameSceneController : Component, IUpdatable
{
    public void Update(float dt)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            SceneManager.Instance.Pop();
        }
        
        if (Raylib.IsKeyPressed(KeyboardKey.R))
        {
            SceneManager.Instance.Pop();
            SceneManager.Instance.Push(new GameScene());
        }
    }
}
