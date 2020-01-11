using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSummonAttack : MonoBehaviour
{
    public GameObject Spike;
    // public int NumSpikes = 4;
    // public CircleCollider2D Bounds;
    public float Frequency = 20f;

    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer += TimeManager.deltaTime;
        if (timer >= Frequency) {
            timer = 0;
            SpikeAttack();
        }
    }

    void SpikeAttack() {
        foreach(GameObject player in PlayerManager.GetAllPlayers()) {
            if (Vector3.Distance(player.transform.position, transform.position) < 15) {
                Instantiate(Spike, player.transform.position, Quaternion.identity);
            }
        }
    }
}
