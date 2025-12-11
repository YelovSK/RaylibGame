using System.Numerics;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace Engine.Extensions;

public static class ExtensionsGraphics
{
    extension(Graphics)
    {
        public static void DrawQuad(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, Color color)
        {
            Graphics.DrawTriangle(topLeft, bottomRight, topRight, color);
            Graphics.DrawTriangle(bottomRight, topLeft, bottomLeft, color);
        }
    }
}