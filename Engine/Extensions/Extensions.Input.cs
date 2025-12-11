using System.Numerics;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Windowing;

namespace Engine.Extensions;

public static class ExtensionsInput
{
    extension(Input)
    {
        public static Vector2 GetVirtualMousePosition()
        {
            var mouse = Input.GetMousePosition();
            var scale = Math.Min(
                Window.GetScreenWidth() / (float)Application.Instance.VirtualWidth,
                Window.GetScreenHeight() / (float)Application.Instance.VirtualHeight
            );
    
            var offsetX = (Window.GetScreenWidth() - Application.Instance.VirtualWidth * scale) / 2;
            var offsetY = (Window.GetScreenHeight() - Application.Instance.VirtualHeight * scale) / 2;
    
            return new Vector2(
                (mouse.X - offsetX) / scale,
                (mouse.Y - offsetY) / scale
            );
        }
    }
}