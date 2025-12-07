using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Helpers;

namespace Game.Scenes;

public class MenuScene : Scene
{
    private const float BUTTON_WIDTH = Layout.VIRTUAL_WIDTH * 0.2f;
    private const float BUTTON_HEIGHT = Layout.VIRTUAL_HEIGHT * 0.1f;
    
    public override void Load()
    {
        var middle = Layout.Center(0, 0);
        
        var buttonOffset = BUTTON_HEIGHT * 1.2f;

        var play = AddButton("Play", middle.Y - buttonOffset);
        play.OnClick = () => SceneManager.Instance.LoadScene(new GameScene());
        
        var options = AddButton("Options", middle.Y);
        options.OnClick = () => SceneManager.Instance.LoadScene(new OptionsScene());
        
        var quit = AddButton("Quit", middle.Y + buttonOffset);
        quit.OnClick = Game.Instance.Close;
    }

    private ButtonComponent AddButton(string text, float y)
    {
        var middle = Layout.Center(BUTTON_WIDTH, 0);
        
        var buttonObject = new Entity(this, new Vector2(middle.X, y));
        var button = new ButtonComponent
        {
            Text = text,
            FontSize = (int)(0.04f * Layout.VIRTUAL_WIDTH),
            Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT),
        };
        buttonObject.AddComponent(button);
        AddEntity(buttonObject);
        return button;
    }
}