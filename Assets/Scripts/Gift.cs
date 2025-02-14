using UnityEngine;

public enum GiftLocation {
    DASHBOARD, REARVIEW
}

[System.Serializable]
public class GiftSpawn {

    public GameObject spawnPoint;

    public bool taken;
}

public class Gift : MonoBehaviour
{
    
    public GiftLocation location;
}
