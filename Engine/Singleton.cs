namespace Engine;

public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;
    private static readonly Lock _lockObj = new();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    _instance ??= new T();
                }
            }

            return _instance;
        }
        protected set => _instance = value;
    }

    protected Singleton()
    {
    }
}