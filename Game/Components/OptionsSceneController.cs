
using Engine;

using Raylib_cs;

namespace Game.Components;

public class OptionsSceneController : Component, IUpdatable
{
    public void Update(float dt)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            SceneManager.Instance.Pop();
        }
    }
}
