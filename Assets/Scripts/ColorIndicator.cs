using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorIndicator : StatusIndicator
{
    public Color Start;
    public Color End;
    public SpriteRenderer Indicator;
    
    public override void UpdateStatus(float progress) {
        Indicator.color = Color.Lerp(Start, End, progress);
    }
}
