using System.Collections.Generic;
using UnityEngine;

public class DiegeticControl : MonoBehaviour
{
    
    public List<UIElementSlider> diegeticsList = new();

    public void FixedUpdate() {

        foreach (var slider in diegeticsList) {

            slider.OutlineRaycast();
        }
    }
}
