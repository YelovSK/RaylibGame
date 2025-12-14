namespace Engine.Components;

public struct CheckboxComponent
{
    public bool IsChecked;
    public Action<bool>? Action;
}