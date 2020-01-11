using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public float AttackSpeed = 0.5f;
    public int Damage = 10;
    public int StunDamage = 1;
    public float StunDuration;
    public float AttackDuration = 0.5f;
    public int AttackStamina = 20;
    public float as_mod = 1;

    Animator character_anim;
    //Initialized low so that characters can attack immediatly
    float last_attack = -1;
    protected StatusEffects status;
    protected Stamina stamina;

    protected float attack_mod = 1;



    private void Awake()
    {
        character_anim = GetComponent<Animator>();
        status = GetComponent<StatusEffects>();
        stamina = GetComponent<Stamina>();
    }

    public void Attack(Vector2 direction)
    {
        bool stamina_check = true;
        if (stamina)
        {
            stamina_check = stamina.TryExertStamina(AttackStamina);
        }
        if (TimeManager.time - last_attack >= AttackSpeed * as_mod && stamina_check)
        {
            StartCoroutine(AttackCoroutine(direction));
            last_attack = TimeManager.time;
        }
    }

    protected virtual IEnumerator AttackCoroutine(Vector2 direction)
    {
        yield return null;
    }

    public void AttackUp(float duration, float mod) {
        StartCoroutine(AttackUpCo(duration, mod));
    }

    IEnumerator AttackUpCo(float duration, float mod) {
        float temp = attack_mod;
        attack_mod = mod;
        float timer = 0;
        while (timer < duration) {
            timer += TimeManager.deltaTime;
            yield return null;
        }
        attack_mod = temp;
    }
}
