using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasOrganicMotion : MonoBehaviour
{
    public float Speed = 3;
    public float Strength = .2f;
    public float ForceDissapation = 1;

    float applied_force = 0;
    float original_rot;
    Vector3 scale;

    private void Awake() {
        scale = transform.localScale;
        Speed += Speed * Random.Range(-0.2f, 0.2f);
        Strength += Strength * Random.Range(-0.2f, 0.2f);
        original_rot = transform.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        float c = scale.y + (Mathf.Sin(TimeManager.time * Speed)) * Strength;
        Vector3 new_scale = scale;
        new_scale.y = c;

        c = scale.x + applied_force * Mathf.Sin(TimeManager.time * Speed * 1.5f) * Strength;
        new_scale.x = c;
        transform.localScale = new_scale;

        applied_force = Mathf.Max(0, applied_force - TimeManager.deltaTime / ForceDissapation);

        c = Mathf.Sin(TimeManager.time * Speed) * Strength * 60;
        Vector3 rot = new Vector3(0, 0, original_rot + c);
        transform.eulerAngles = rot;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        applied_force = 3;
    }
}
