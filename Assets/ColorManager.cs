using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public SpriteRenderer[] sprite_renderers;

    public void SetColor(Color color) {
        foreach(SpriteRenderer sr in sprite_renderers) {
            sr.color = color;
        }
    }
}
