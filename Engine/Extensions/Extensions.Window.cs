using Raylib_CSharp.Windowing;

namespace Engine.Extensions;

public static class ExtensionsWindow
{
    extension(Window)
    {
        public static void SetBorderless()
        {
            Window.SetState(ConfigFlags.UndecoratedWindow | ConfigFlags.TopmostWindow);
            
            var monitor = Window.GetCurrentMonitor();
            var width = Window.GetMonitorWidth(monitor);
            var height = Window.GetMonitorHeight(monitor);
                    
            Window.SetSize(width, height);
            Window.SetPosition(0, 0);
        }
        
        public static void UnsetBorderless()
        {
            var monitor = Window.GetCurrentMonitor();
            var width = Window.GetMonitorWidth(monitor);
            var height = Window.GetMonitorHeight(monitor);
            
            Window.SetSize(width / 2, height / 2);
            Window.SetPosition(width / 2, height / 2);
                    
            Window.ClearState(ConfigFlags.UndecoratedWindow | ConfigFlags.TopmostWindow); 
        }
    }
}