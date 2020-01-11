using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Weapon
{
    public GameObject FireballProjectiles;
    public float ProjectileLeeway = 0.3f;
    public float ProjectileSpeed = 4f;
    private SoundManager.Sound[] BigSounds;

    // Start is called before the first frame update
    void Start()
    {
        BigSounds = new SoundManager.Sound[] {
            SoundManager.Sound.BEnemy1,
            SoundManager.Sound.BEnemy2,
            SoundManager.Sound.BEnemy3
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override IEnumerator AttackCoroutine(Vector2 direction) {
        int idx = Random.Range(0, 3);
        SoundManager.PlaySound(BigSounds[idx]);
        SoundManager.PlaySound(SoundManager.Sound.Fireball);
        Vector3 instantiation_point = transform.position + VUtils.Vec2ToVec3(direction).normalized * ProjectileLeeway;
        GameObject fireball = Instantiate(FireballProjectiles, instantiation_point, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * ProjectileSpeed;
        yield return null;
    }
}
