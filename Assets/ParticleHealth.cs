using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHealth : MonoBehaviour
{
    public Health BossHealth;
    public float RegenRate = 0.5f;
    public Health ThisHealth;

    ParticleSystem ps;
    float max_emission;

    private void Start() {
        ps = GetComponent<ParticleSystem>();
        var emission_mod = ps.emission;
        max_emission = emission_mod.rateOverTime.constant;
    }
    
    private void Update() {
        if (BossHealth)
            BossHealth.TakeDamage(-RegenRate);
        float progress = ThisHealth.GetPercentHealth();
        var emission_mod = ps.emission;
        emission_mod.rateOverTime = max_emission * progress;
    }
}
