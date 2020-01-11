using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteVariation : MonoBehaviour
{
    public Sprite[] Sprites;

    private void Awake() {
        int num = Random.Range(0, Sprites.Length);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = Sprites[num];
    }
}
