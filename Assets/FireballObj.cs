using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballObj : MonoBehaviour
{
    public float ExplosionRadius = 1.2f;
    public float Lifetime = 3f;
    public int Damage = 5;
    public int StunDamage = 0;
    public float KnockbackDistance = 0.1f;
    public float SeekingStrength = 3f;
    public GameObject Explosion;

    Transform target;
    Rigidbody2D rb;
    float counter = 0;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        counter += TimeManager.deltaTime;
        if (counter >= Lifetime) {
            Explode();
        }
        if (!target) {
            Vector3 closest_player = Vector3.up * 9999;
            foreach(GameObject player in PlayerManager.GetAllPlayers()) {
                if (Vector3.Distance(player.transform.position, transform.position) < Vector3.Distance(closest_player, transform.position)) {
                    closest_player = player.transform.position;
                    target = player.transform;
                }
            }
        }

        Vector3 targeting = (target.position - transform.position).normalized;
        rb.velocity = (rb.velocity + VUtils.Vec3ToVec2(targeting * 0.1f * SeekingStrength)).normalized * rb.velocity.magnitude;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Projectile"))
        {

            Explode();
        }
    }

    void Explode() {
        var colliders = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);

        foreach (var collider in colliders)
        {
            Health h = collider.gameObject.GetComponent<Health>();
            if (h && collider.tag != gameObject.tag)
            {
                h.TakeDamage(Damage);
                var statusEffects = collider.GetComponent<StatusEffects>();
                if (statusEffects != null)
                {
                    HasHeading heading = GetComponent<HasHeading>();
                    statusEffects.FlashSprite(0.3f);
                }
            }
        }
        SoundManager.PlaySound(SoundManager.Sound.AOEAtk);
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
