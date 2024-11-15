[System.Serializable]
public abstract class DynamicReference<T>
{
    public bool _useReference = true;

    public T _varValue;
    public ScriptableVariable<T> _refValue;

    public T Value
    {
        get
        {
            return _useReference ? _refValue.Value : _varValue;
        }
        set
        {
            if (_useReference)
            {
                _refValue.Value = value;
            }
            else
            {
                _varValue = value;
            }
        }
    }
}