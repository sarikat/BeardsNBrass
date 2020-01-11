using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftHealth : MonoBehaviour
{
    public Health health;
    
    
    private void Update() {
        float progress = health.GetPercentHealth();
        Vector3 new_scale = new Vector3(progress, 0.5f + 0.5f * progress, 1);
        transform.localScale = new_scale;
    }
}
