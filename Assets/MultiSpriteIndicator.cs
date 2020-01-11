using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSpriteIndicator : StatusIndicator
{
    public SpriteIndicator[] Indicators;


    public override void UpdateStatus(float progress) {
        if (progress < 0) {
            Disable();
            return;
        }

        foreach (SpriteIndicator ind in Indicators) {
            ind.UpdateStatus(progress);
        }
    }

    public override void Disable() {
        foreach (SpriteIndicator ind in Indicators) {
            ind.Disable();
        }
    }
}
