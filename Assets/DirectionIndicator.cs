using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public Transform TurnTable;

    public void SetAngle(float angle) {
        TurnTable.localEulerAngles = new Vector3(0, 180, angle);
    }
}
