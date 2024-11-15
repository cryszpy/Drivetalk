using UnityEngine;

public class TestVariable : MonoBehaviour
{
    public FloatReference _testReference;
    public float _testExample;
    public bool _testBool;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            _testReference._refValue.Serialize();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            _testReference._refValue.Deserialize();
        }
    }
}
