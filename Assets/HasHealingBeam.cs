using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHealingBeam : MonoBehaviour
{
    public float HealthRegen = 0.1f;
    public Health OtherHealth;
    public Health ThisHealth;

    private void Update() {
        OtherHealth.TakeDamage(-HealthRegen * ThisHealth.GetPercentHealth());
    }
}
