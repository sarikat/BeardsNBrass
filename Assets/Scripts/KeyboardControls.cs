using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControls : MonoBehaviour
{
    public float MovementSpeed = 5;
    public float AttackRadius = 0.3f;
    public float AttackJumpStrength = 3;
    public float AttackSpeed = 0.5f;
    public float AttackDuration = 0.5f;
    public Animator animator;

    Rigidbody2D rb;

    float attack_cooldown = 5f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attack_cooldown > AttackDuration)
        {
            Move();
        }

        attack_cooldown += TimeManager.deltaTime;
        if (Input.GetAxisRaw("Fire1") > 0 && attack_cooldown > AttackSpeed)
        {
            attack_cooldown = 0;
            StartCoroutine(Attack());
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 direction = new Vector2(horizontal, vertical).normalized;

        rb.velocity = direction * MovementSpeed;

        animator.SetFloat("Speed", Mathf.Abs(MovementSpeed));
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        // angle *= Mathf.Rad2Deg;
        Debug.Log(angle);
        if (direction != Vector2.zero)
        {
            if (angle > 45 || angle < -135)
            {
                animator.SetBool("FacingRight", false);
            }
            else
            {
                animator.SetBool("FacingRight", true);
            }
        }
    }

    IEnumerator Attack()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        if (angle > 45 || angle < -135)
        {
            animator.SetBool("AttackingRight", false);
        }
        else
        {
            animator.SetBool("AttackingRight", true);
        }
        animator.SetTrigger("Attack");

        yield return StartCoroutine(Slide(dir, AttackJumpStrength, AttackDuration));

        var center = transform.position + (Vector3)dir * AttackRadius * 2f;
        var colliders = Physics2D.OverlapCircleAll(center, AttackRadius);
        DebugDrawCircle(center, AttackRadius);

        foreach (var collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                collider.GetComponent<Health>()?.TakeDamage(10);
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

    IEnumerator Slide(Vector2 dir, float distance, float time)
    {
        float slide_time = 0;
        float current_distance = 0;

        while (slide_time < time)
        {
            slide_time += TimeManager.deltaTime;
            float new_distance = Mathf.Exp(5 * ((slide_time / time) - 1)) * distance;

            Vector3 current_position = transform.position;
            current_position.x = current_position.x + dir.x * (new_distance - current_distance);
            current_position.y = current_position.y + dir.y * (new_distance - current_distance);
            transform.position = current_position;
            current_distance = new_distance;
            yield return null;
        }
    }
}
