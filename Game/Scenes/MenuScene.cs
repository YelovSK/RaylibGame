using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Enums;
using Engine.Systems;
using Engine.Systems.Render;
using Engine.Systems.Update;
using Raylib_CSharp.Colors;

namespace Game.Scenes;

public class MenuScene : Scene
{
    private static readonly float BUTTON_WIDTH = Application.Instance.VirtualWidth * 0.2f;
    private static readonly float BUTTON_HEIGHT = Application.Instance.VirtualHeight * 0.1f;
    
    protected override void Load(World world)
    {
        var middle = VirtualLayout.AnchorToScreen((int)BUTTON_WIDTH, (int)BUTTON_HEIGHT, Anchor.Center); 
        var buttonOffset = BUTTON_HEIGHT * 1.2f; 
        
        // Background
        var background = world.CreateEntity();
        world.AddComponent(background, new TransformComponent
        {
            Position = Vector2.Zero,
        });
        world.AddComponent(background, new SpriteComponent
        {
            Size = new Vector2(Application.Instance.VirtualWidth,  Application.Instance.VirtualHeight),
            Color = Color.SkyBlue
        });
        
        // Buttons
        AddButton(world, "Play", middle.Y - buttonOffset, () =>
        {
            SceneManager.Instance.Load(new GameScene());
        });
        
        AddButton(world, "Options", middle.Y, () =>
        {
            SceneManager.Instance.Load(new OptionsScene());
        });
        
        AddButton(world, "Quit", middle.Y + buttonOffset, () =>
        {
            Application.Instance.Close();
        });

        world.AddSystem<UiPointerSystem>();
        world.AddSystem<ButtonTiltSystem>();
        world.AddRenderSystem<SpriteRenderSystem>();
        world.AddRenderSystem<ButtonRenderSystem>();
        world.AddRenderSystem<ButtonLabelRenderSystem>();
    }
    
    private static void AddButton(World world, string text, float y, Action action)
    {
        var middle = VirtualLayout.Center(BUTTON_WIDTH, 0); 
        
        var button = world.CreateEntity();
        world.AddComponent(button, new RectTransform()
        {
            Position = middle with { Y = y },
            Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT),
        });
        
        world.AddComponent(button, new ButtonStyleComponent()
        {
            StrokeWidth = BUTTON_HEIGHT * 0.06f,
            NormalColor = Color.DarkGray,
            HoverColor = Color.Gray,
            PressedColor = Color.LightGray,
        });
        
        world.AddComponent(button, new LabelComponent()
        {
            Text = text,
            TextColor = Color.White,
            FontSize = 20,
        });

        world.AddComponent(button, new UiPointerStateComponent()
        {
            Action = action,
        });
        world.AddComponent(button, new ButtonVisualStateComponent());
    }
}