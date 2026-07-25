using System.Numerics;

using Raylib_cs;

namespace Engine.Extensions;

public static class ExtensionsInput
{
    extension(Raylib)
    {
        public static Vector2 GetVirtualMousePosition()
        {
            return VirtualViewport.ScreenToVirtual(Raylib.GetMousePosition());
        }
    }
}
