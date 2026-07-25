
using Raylib_cs;

namespace Engine.Extensions;

public static class ExtensionsWindow
{
    extension(Raylib)
    {
        public static void SetBorderless()
        {
            Raylib.SetWindowState(ConfigFlags.UndecoratedWindow | ConfigFlags.TopmostWindow);
            
            var monitor = Raylib.GetCurrentMonitor();
            var width = Raylib.GetMonitorWidth(monitor);
            var height = Raylib.GetMonitorHeight(monitor);
                    
            Raylib.SetWindowSize(width, height);
            Raylib.SetWindowPosition(0, 0);
        }
        
        public static void UnsetBorderless()
        {
            var monitor = Raylib.GetCurrentMonitor();
            var width = Raylib.GetMonitorWidth(monitor);
            var height = Raylib.GetMonitorHeight(monitor);
            
            Raylib.SetWindowSize(width / 2, height / 2);
            Raylib.SetWindowPosition(width / 2, height / 2);

            Raylib.ClearWindowState(ConfigFlags.UndecoratedWindow | ConfigFlags.TopmostWindow);
        }
    }
}
