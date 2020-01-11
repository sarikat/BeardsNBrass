using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : Weapon
{
    public float AttackRadius = 0.3f;
    public float AttackAheadDistance = 1f;
    public float AttackJumpStrength = 3;
    public float KnockbackDistance = 2f;

    public Animator weapon_anim;
    public Transform weapon_trans;
    public ParticleSystem OptionalOnHitParticles;
    // public GameObject[] Hands;

    bool right_handed = true;

    Rigidbody2D rb;
    PlayerInput playerInput;


    private void Awake()
    {
        // weapon_anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        if (!gameObject.CompareTag("Player"))
        {
            weapon_anim = GetComponent<Animator>();
        }
        if (OptionalOnHitParticles)
        {
            OptionalOnHitParticles.Stop();
        }
    }

    protected override IEnumerator AttackCoroutine(Vector2 direction)
    {
        // float angle = Vector2.SignedAngle(Vector2.right, direction);
        // if (angle > 45 || angle < -135)
        // {
        //     weapon_anim.SetBool("AttackingRight", false);
        // }
        // else
        // {
        //     weapon_anim.SetBool("AttackingRight", true);
        // }

        if (gameObject.CompareTag("Player"))
        {
            if (right_handed)
                weapon_trans.localScale = new Vector3(1, 1, 1);
            else
                weapon_trans.localScale = new Vector3(-1, 1, 1);
        }

        right_handed = !right_handed;
        weapon_anim.SetTrigger("Attacking");

        yield return StartCoroutine(Slide(direction, AttackJumpStrength, AttackDuration));

        if (gameObject.CompareTag("Player"))
        {
            status?.MakeInvincible(AttackDuration + 0.6f);
        }
        if (OptionalOnHitParticles)
        {
            SoundManager.PlaySound(SoundManager.Sound.AOEAtk);
            OptionalOnHitParticles.Play();
        }

        var center = transform.position + (Vector3)direction * AttackAheadDistance;
        var colliders = Physics2D.OverlapCircleAll(center, AttackRadius);
        DebugDrawCircle(center, AttackRadius);

        foreach (var collider in colliders)
        {
            Health h = collider.gameObject.GetComponent<Health>();

            bool isEnemyAtk = gameObject.CompareTag("Enemy") && collider.CompareTag("Player");
            bool isPlayerAtk = gameObject.CompareTag("Player")
                            && collider.tag.StartsWith("Enemy", System.StringComparison.CurrentCulture);

            if (h && (isEnemyAtk || isPlayerAtk))
            {
                h.TakeDamage(Damage * attack_mod);
                var statusEffects = collider.GetComponent<StatusEffects>();
                if (statusEffects != null)
                {
                    HasHeading heading = GetComponent<HasHeading>();
                    if (KnockbackDistance > 0)
                    {
                        statusEffects.Knockback(heading.GetHeading(), KnockbackDistance, 0.05f);
                    }

                    if (statusEffects.tag != "Player" && StunDuration > 0)
                    {
                        statusEffects.Stun(StunDuration);
                    }
                    statusEffects.FlashSprite(0.3f);

                    if (StunDuration > 0)
                    {
                        statusEffects.OnHitStun(2f, 3f);
                    }
                }
            }
        }
    }

    void DebugDrawCircle(Vector3 center, float radius)
    {

        for (int i = 0; i < 20; ++i)
        {
            Vector2 rand = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            Vector3 end = new Vector3(center.x + rand.x * radius, center.y + rand.y * radius, 0);
            Debug.DrawLine(center, end, Color.red, 0.5f, false);
        }
    }

    IEnumerator Slide(Vector2 direction, float distance, float time)
    {
        playerInput?.LockControls();
        float slide_time = 0;
        // float current_distance = 0;

        while (slide_time < time)
        {
            slide_time += TimeManager.deltaTime;
            // float new_distance = Mathf.Exp(5 * ((slide_time / time) - 1)) * distance;

            rb.velocity = direction * distance / time;
            yield return null;
        }
        rb.velocity = Vector2.zero;
        playerInput?.UnlockControlsAndRestartMovement();
    }
}
