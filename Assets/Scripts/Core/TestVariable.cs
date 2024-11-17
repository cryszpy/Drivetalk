using UnityEngine;

public class TestVariable : MonoBehaviour
{
    [Header("Custom Scriptable Variables")]
    [Tooltip("Custom tooltip for a reference variable.")]
    public FloatReference _testFloatReference;
    public BoolReference _tbr;

    [Space]
    [Header("Regular Unity Variables")]
    [Tooltip("Custom tooltip for a vanilla variable.")]
    public float _testLongVariableNameExample;
    public float _testExample;
    public bool _testBool;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            _testFloatReference._refValue.Serialize();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            _testFloatReference._refValue.Deserialize();
        }
    }
}
