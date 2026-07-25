using System.Numerics;
using Engine;
using Engine.Components;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Transformations;

namespace Game.Scenes;

public class MenuScene : Scene
{
    private readonly float BUTTON_WIDTH = Application.Instance.VirtualWidth * 0.2f;
    private readonly float BUTTON_HEIGHT = Application.Instance.VirtualHeight * 0.1f;
    
    public override void Load()
    {
        var buttonOffset = BUTTON_HEIGHT * 1.2f;

        var background = CreateEntity();
        var backgroundSprite = background.AddComponent<SpriteComponent>();
        backgroundSprite.Width = Application.Instance.VirtualWidth;
        backgroundSprite.Height = Application.Instance.VirtualHeight;
        backgroundSprite.Color = Color.SkyBlue;

        var play = AddButton("Play", -buttonOffset);
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
        
        var options = AddButton("Options", 0);
        options.OnClick = () => SceneManager.Instance.Push(new OptionsScene());
        
        var quit = AddButton("Quit", buttonOffset);
        quit.OnClick = Application.Instance.Close;
    }

    private ButtonComponent AddButton(string text, float yOffset)
    {
        var buttonObject = CreateEntity();
        var button = buttonObject.AddComponent<ButtonComponent>();
        button.Text = text;
        button.FontSize = (int)(0.04f * Application.Instance.VirtualWidth);
        button.Rect.Anchor = new Vector2(0.5f);
        button.Rect.Pivot = new Vector2(0.5f);
        button.Rect.Offset = new Vector2(0, yOffset);
        button.Rect.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
        
        return button;
    }
}
