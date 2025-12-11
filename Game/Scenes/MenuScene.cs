using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Enums;
using Engine.Helpers;

namespace Game.Scenes;

public class MenuScene : Scene
{
    private readonly float BUTTON_WIDTH = Application.Instance.VirtualWidth * 0.2f;
    private readonly float BUTTON_HEIGHT = Application.Instance.VirtualHeight * 0.1f;
    
    public override void Load()
    {
        var middle = VirtualLayout.AnchorToScreen((int)BUTTON_WIDTH, (int)BUTTON_HEIGHT, Anchor.Center);
        
        var buttonOffset = BUTTON_HEIGHT * 1.2f;

        var play = AddButton("Play", middle.Y - buttonOffset);
        play.OnClick = () =>
        {
            if (SceneManager.Instance.HasScene<GameScene>())
            {
                SceneManager.Instance.Pop();
            }
            else
            {
                SceneManager.Instance.Push(new GameScene());
            }
        };
        
        var options = AddButton("Options", middle.Y);
        options.OnClick = () => SceneManager.Instance.Push(new OptionsScene());
        
        var quit = AddButton("Quit", middle.Y + buttonOffset);
        quit.OnClick = Application.Instance.Close;
    }

    private ButtonComponent AddButton(string text, float y)
    {
        var middle = VirtualLayout.Center(BUTTON_WIDTH, 0);

        var buttonObject = CreateEntity();
        buttonObject.Transform.Position = middle with { Y = y };
        var button = buttonObject.AddComponent<ButtonComponent>();
        button.Text = text;
        button.FontSize = (int)(0.04f * Application.Instance.VirtualWidth);
        button.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
        button.RenderSpace = RenderSpace.Screen;
        
        return button;
    }
}