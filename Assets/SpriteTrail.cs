using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTrail : MonoBehaviour
{

    public GameObject SpriteParticle;
    public float Frequency;
    public bool IsDistanceBased;

    float cooldown = 99999;
    Vector3 old_position;

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = transform.position - old_position;
        if (IsDistanceBased) {
            cooldown += Vector3.Distance(old_position, transform.position);
        } else {
            cooldown += TimeManager.deltaTime;
        }
        old_position = transform.position;
        if (cooldown > Frequency) {
            cooldown = 0;
            GameObject new_particle = Instantiate(SpriteParticle, transform.position, Quaternion.identity);
            Vector3 rot = new Vector3(0, 0, Mathf.Atan2(dir.x, dir.y) * -Mathf.Rad2Deg);
            new_particle.transform.eulerAngles = rot;
        }
    }
}
