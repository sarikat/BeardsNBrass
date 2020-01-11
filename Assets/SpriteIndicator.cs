using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteIndicator : StatusIndicator
{
    public Sprite[] ProgressSprites;
    public bool AutoHide = true;
    public float AutoHideTimer = 2f;
    SpriteRenderer renderer;
    float timer = 0f;


    private void Awake() {
        renderer = GetComponent<SpriteRenderer>();
    }

    public override void UpdateStatus(float progress) {
        progress = Mathf.Max(0, progress);
        if (AutoHide) {
            if (progress <= 0) {
                timer += TimeManager.deltaTime;
                if (timer >= AutoHideTimer) {
                    Disable();
                    return;
                }
            } else {
                timer = 0;
            }
        }
        renderer.enabled = true;
        int sprite_num = (int)Mathf.Round((progress) * (ProgressSprites.Length - 1));
        if (ProgressSprites.Length == 2) {
            sprite_num = progress > 0 ? 1 : 0;
        }
        renderer.sprite = ProgressSprites[sprite_num];
    }

    public override void Disable() {
        renderer.enabled = false;
    }
}
