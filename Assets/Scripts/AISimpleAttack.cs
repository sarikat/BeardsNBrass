using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISimpleAttack : MonoBehaviour
{
    public float AttackRange = 1.7f;
    public float AttackMinSeperation = 0f;
    public float AttackAnticipation = 0.3f;
    public float AttackCooldown = 4f;


    StatusEffects status;
    Weapon weapon;
    float time_since_last_attack = 999f;
    bool attacking = false;

    private SoundManager.Sound[] LittleSounds;
    


    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        status = GetComponent<StatusEffects>();
        LittleSounds = new SoundManager.Sound[] {
            SoundManager.Sound.LEnemy1,
            SoundManager.Sound.LEnemy2,
            SoundManager.Sound.LEnemy3
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (time_since_last_attack >= AttackCooldown && !attacking && !status.ControlsAreLocked)
        {
            GameObject target = null;
            var cols = Physics2D.OverlapCircleAll(transform.position, AttackRange);
            foreach (Collider2D col in cols)
            {
                if (col.gameObject.CompareTag("Player") && Vector3.Distance(col.transform.position, transform.position) > AttackMinSeperation)
                {
                    if (!target)
                    {
                        target = col.gameObject;
                    }
                    else
                    {
                        float dist = Vector3.Distance(transform.position, target.transform.position);
                        dist -= (transform.position - col.transform.position).magnitude;
                        target = dist > 0 ? target : col.gameObject;
                    }
                }
            }
            if (target)
            {
                attacking = true;
                StartCoroutine(PrepareAttack(VUtils.Vec3ToVec2(target.transform.position - transform.position).normalized));
            }
        }
        else
        {
            time_since_last_attack += TimeManager.deltaTime;
        }
    }

    IEnumerator PrepareAttack(Vector2 direction)
    {
        status.FlashSpriteColor(AttackAnticipation, new Color(255, 255, 255, 255));
        status.Stun(AttackAnticipation);

        yield return new TimeManager.WaitForSeconds(AttackAnticipation + 0.02f);

        time_since_last_attack = 0;
        attacking = false;
        weapon.Attack(direction);

        // if little enemy, do this else

        if (gameObject.name == "AOE Enemy")
        {
            SoundManager.PlaySound(SoundManager.Sound.BEnemy1);
        }
        else
        {
            int idx = Random.Range(0, 3);

            SoundManager.PlaySound(LittleSounds[idx]);
        }

        
        if (direction.x > 0) {
            transform.localScale = new Vector3(1, 1, 1);
        } else {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
