using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFade : MonoBehaviour
{
    public Color StartColor;
    public Color EndColor;
    public float Lifetime;

    SpriteRenderer sprite_renderer;
    float current_lifetime = 0;
    // Start is called before the first frame update
    void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        current_lifetime += TimeManager.deltaTime;
        if (current_lifetime > Lifetime) {
            Destroy(gameObject);
        }
        sprite_renderer.color = Color.Lerp(StartColor, EndColor, current_lifetime / Lifetime);
    }
}
