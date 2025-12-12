
using Engine;
using Raylib_CSharp.Interact;

namespace Game.Components;

public class OptionsSceneController : Component, IUpdatable
{
    public void Update(float dt)
    {
        if (Input.IsKeyPressed(KeyboardKey.Escape))
        {
            SceneManager.Instance.Pop();
        }
    }
}