using System.Numerics;
using Raylib_CSharp.Interact;

namespace Engine.Extensions;

public static class ExtensionsInput
{
    extension(Input)
    {
        public static Vector2 GetVirtualMousePosition()
        {
            return VirtualViewport.ScreenToVirtual(Input.GetMousePosition());
        }
    }
}
