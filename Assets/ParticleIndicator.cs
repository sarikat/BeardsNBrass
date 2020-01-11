using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleIndicator : StatusIndicator
{
    public ParticleSystem ps;
    public int MaxParticles = 10;
    public int MinParticles = 3;
    
    public override void UpdateStatus(float progress) {
        var em = ps.emission;
        if (progress == 0) {
            em.enabled = false;
        } else {
            em.enabled = true;
            em.rateOverTime = MinParticles + progress * (MaxParticles - MinParticles);
        }
    }
}
