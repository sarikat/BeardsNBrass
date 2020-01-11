using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAttack : MonoBehaviour
{
    public float AnticipationDuration = 2f;
    public GameObject AnticipationIndicator;
    public Animator Spikes;

    Animator anim;
    float timer = 0;
    bool spike_active = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += TimeManager.deltaTime;
        if (timer >= AnticipationDuration && !spike_active) {
            spike_active = true;
            AnticipationIndicator.SetActive(false);
            Spikes.SetTrigger("Attack");
            Destroy(gameObject, 7f);
        }
    }
}
