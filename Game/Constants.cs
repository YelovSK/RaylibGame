using System.Reflection;

namespace Game;

public static class Constants
{
    public static readonly string APP_NAME = Assembly.GetExecutingAssembly().GetName().Name;
    public static readonly string GAME_DATA_PATH = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APP_NAME);
}