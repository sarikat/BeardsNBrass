using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPriestAI : MonoBehaviour
{
    public Health ObjectOfAttention;
    public float RegenRate = 0.5f;
    public ParticleSystem Beam;
    
    AISimpleAttack attack;
    AIBoids movement;
    BoxCollider2D col;
    Health h;

    // Start is called before the first frame update
    void Start()
    {
        attack = GetComponent<AISimpleAttack>();
        movement = GetComponent<AIBoids>();
        col = GetComponent<BoxCollider2D>();
        attack.enabled = false;
        movement.enabled = false;
        col.isTrigger = true;

        h = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (h.GetPercentHealth() != 1f) {
            attack.enabled = true;
            movement.enabled = true;
            Destroy(Beam.gameObject);
            col.isTrigger = false;
            this.enabled = false;
        } else {
            ObjectOfAttention.TakeDamage(-RegenRate);
        }
    }
}
