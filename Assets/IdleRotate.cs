using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRotate : MonoBehaviour
{
    public float RotationSpeed = 3f;

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, (TimeManager.time * RotationSpeed * 50) % 360);
    }
}
