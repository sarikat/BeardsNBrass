using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeavyMeleeAttack : MonoBehaviour
{
    bool isAttacking;
    float startAngle;

    BoxCollider2D weaponCollider;
    GameObject heavyWeapon;
    Gamepad playerGamepad;

    // Start is called before the first frame update
    void Start()
    {
        playerGamepad = GetComponent<PlayerInput>().GetGamepad();
        isAttacking = false;
        startAngle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (heavyWeapon == null)
        {
            Transform heavy = transform.Find("HeavyWeapon(Clone)");
            if (heavy != null)
            {
                heavyWeapon = heavy.gameObject;
                weaponCollider = heavyWeapon.GetComponent<BoxCollider2D>();
                weaponCollider.enabled = false;
            }
        }

        // if a heavy weapon is equipped
        if (heavyWeapon != null && playerGamepad.bButton.wasPressedThisFrame)
        {
            weaponCollider.enabled = true;

            if (!isAttacking)
            {
                Debug.Log("Heavy Attack");
                StartCoroutine(HeavyAttack());
            }
        }
    }

    IEnumerator HeavyAttack()
    {
        isAttacking = true;
        float playerAngle = Mathf.Atan2(playerGamepad.leftStick.x.ReadValue(),
                                  playerGamepad.leftStick.y.ReadValue()) * Mathf.Rad2Deg;

        // Debug.Log("Start: " + startAngle + " Dest: " + playerAngle);

        float deltaRot = playerAngle - startAngle;
        while (!Mathf.Approximately(deltaRot, 0))
        {
            yield return null;
            float increment = Mathf.Min(120f * TimeManager.deltaTime, Mathf.Abs(deltaRot));
            increment *= Mathf.Sign(deltaRot);
            deltaRot -= increment;

            heavyWeapon.transform.RotateAround(transform.position, -Vector3.forward, increment);
        }
        yield return null;
        startAngle = playerAngle;
        weaponCollider.enabled = false;
        isAttacking = false;
        // Debug.LogWarning("");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking && collision.tag == "Enemy")
        {
            // Debug.Log("HeavyHit");
            collision.GetComponent<Health>()?.TakeDamage(50);
        }
    }
}
