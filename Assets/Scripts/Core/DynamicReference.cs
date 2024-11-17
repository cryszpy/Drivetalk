// Empty base class because Custom PropertyDrawers don't support multi-parameter generic classes.
public abstract class DynamicReferenceBase { }

[System.Serializable]
public abstract class DynamicReference<T, SO> : DynamicReferenceBase where SO : ScriptableVariable<T>
{
    public bool _useReference = true;

    public T _varValue;
    public SO _refValue;

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