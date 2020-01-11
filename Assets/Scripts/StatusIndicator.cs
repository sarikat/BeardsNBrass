using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIndicator : MonoBehaviour
{
    public virtual void UpdateStatus(float progress) {
    }

    public virtual void Finish() {
        Destroy(gameObject);
    }

    public virtual void Disable() {
    }
}
