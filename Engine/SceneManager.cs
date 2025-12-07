namespace Engine;

public class SceneManager : Singleton<SceneManager>
{
    private const float FIXED_DELTA_TIME = 1f / 60f;
    private float _accumulator = 0f;
    private Scene? _currentScene;

    public void LoadScene(Scene newScene)
    {
        _currentScene?.OnDestroy();
        _currentScene = newScene;
        _currentScene.Load();
        _currentScene.Start();
    }

    public void Update(float dt)
    {
        _currentScene?.Update(dt);
        
        _accumulator += dt;
        if (_accumulator >= FIXED_DELTA_TIME) {
            _currentScene?.FixedUpdate();
            _accumulator = 0f;
        }
    }

    public void Draw()
    {
        _currentScene?.Draw();
    }
}