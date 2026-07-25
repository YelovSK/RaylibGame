using System.Numerics;

using Raylib_cs;

namespace Engine.Extensions;

public static class ExtensionsGraphics
{
    extension(Raylib)
    {
        public static void DrawQuad(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, Color color)
        {
            Raylib.DrawTriangle(topLeft, bottomRight, topRight, color);
            Raylib.DrawTriangle(bottomRight, topLeft, bottomLeft, color);
        }
    }
}
