using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBoids : MonoBehaviour, IMovement
{
    public float StoppingDistance = 0.5f;
    public float PerceptionRadius = 15f;
    public float MinPerceptionUpdateFrequency = 0.3f;
    public float SeperationRadius = 8f;
    public float SeperationMultiplier = 0.97f;
    public float CohesionMultiplier = 0.8f;
    public float ObstacleAvoidanceStrength = 2f;
    public float MaxForce = 0.1f;
    public float SlowingFactor = 0.95f;
    public float SteeringForce = 1f;
    public float MinVel = 1f;
    public float MaxVel = 2f;
    public float MinSpeed = 0.03f;

    bool controlsAreLocked;

    float max_speed;
    Vector3 acceleration;
    Transform target;
    float time_since_last_target_update = 99999f;
    Rigidbody2D rb;
    Animator anim;
    HasHeading heading;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        heading = GetComponent<HasHeading>();
        max_speed = Random.Range(MinVel, MaxVel);
    }

    // Update is called once per frame
    void Update()
    {
        if (controlsAreLocked)
        {
            return;
        }
        EfficientCheckSteering();
        SetVelocity();
    }

    void SetVelocity()
    {
        acceleration *= SlowingFactor;
        acceleration = acceleration.magnitude < 0.01f ? Vector3.zero : acceleration;


        if (target && Vector3.Distance(transform.position, target.position) < StoppingDistance && Vector3.Distance(transform.position, target.position) > StoppingDistance / 2)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            if (target)
            {
                Vector3 target_dir = (target.position - transform.position).normalized;
                heading.SetHeading((Vector2)target_dir);
                acceleration += target_dir;
                acceleration += Seperation() * SeperationMultiplier;
                Debug.DrawRay(transform.position, Seperation(), Color.red, 0, false);
                acceleration += Cohesion() * CohesionMultiplier;
                acceleration += ObstacleAvoidance() * ObstacleAvoidanceStrength;
                if (acceleration.magnitude > MaxForce)
                {
                    acceleration = acceleration.normalized * MaxForce;
                }
                Debug.DrawRay(transform.position, acceleration, Color.green, 0, false);
            }
            if (Vector3.Distance(transform.position, target.position) > StoppingDistance) {
                rb.velocity += VUtils.Vec3ToVec2(acceleration) * SteeringForce;
            } else {
                rb.velocity -= VUtils.Vec3ToVec2(acceleration) * SteeringForce;
            }

            rb.velocity = rb.velocity.magnitude > max_speed ? rb.velocity.normalized * max_speed : rb.velocity;
            rb.velocity = rb.velocity.magnitude < MinSpeed ? Vector2.zero : rb.velocity;
        }


        if (anim)
            anim.SetFloat("Speed", rb.velocity.magnitude);
        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void EfficientCheckSteering()
    {
        time_since_last_target_update += TimeManager.deltaTime;
        float dist = target ? Vector3.Distance(transform.position, target.position) : PerceptionRadius;
        dist = Mathf.Clamp(dist, 1, PerceptionRadius);
        float update_freq = (1 / (PerceptionRadius - dist + 1)) / MinPerceptionUpdateFrequency;
        if (time_since_last_target_update > update_freq)
        {
            Steering();
            time_since_last_target_update = 0;
        }
    }

    void Steering()
    {
        target = null;
        float min_dist = 9999;
        bool got_player = false;
        foreach (Collider2D col in Physics2D.OverlapCircleAll(VUtils.Vec3ToVec2(transform.position), PerceptionRadius))
        {
            if (col.gameObject.CompareTag("Player"))
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < min_dist)
                {
                    got_player = true;
                    min_dist = dist;
                    target = col.transform;
                }
            }
            if (!got_player && col.gameObject.CompareTag("Chester"))
            {
                target = col.transform;

            }
        }
    }

    Vector3 Seperation()
    {
        Vector3 steering = Vector3.zero;
        int total = 0;

        foreach (Collider2D col in Physics2D.OverlapCircleAll(VUtils.Vec3ToVec2(transform.position), SeperationRadius))
        {
            if (col.gameObject != gameObject)
            {
                AIBoids ai = col.gameObject.GetComponent<AIBoids>();
                if (ai)
                {
                    Vector3 line_to_other = col.transform.position - transform.position;
                    steering -= line_to_other / (line_to_other.magnitude * line_to_other.magnitude);
                    total++;
                }
            }
        }

        if (total > 0)
        {
            steering /= total;
            // steering -= VUtils.Vec2ToVec3(rb.velocity);
            // steering = Mathf.Min(steering, MaxForce);
        }
        return steering.normalized;
    }

    Vector3 Cohesion()
    {
        Vector3 steering = Vector3.zero;
        int total = 0;

        foreach (Collider2D col in Physics2D.OverlapCircleAll(VUtils.Vec3ToVec2(transform.position), SeperationRadius))
        {
            if (col.gameObject != gameObject)
            {
                AIBoids ai = col.gameObject.GetComponent<AIBoids>();
                if (ai)
                {
                    Vector3 line_to_other = col.transform.position - transform.position;
                    steering += line_to_other;
                    total++;
                }
            }
        }

        if (total > 0)
        {
            steering /= total;
            // steering -= VUtils.Vec2ToVec3(rb.velocity);
            // steering = Mathf.Min(steering, MaxForce);
        }
        return steering.normalized;
    }

    Vector3 ObstacleAvoidance()
    {
        Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        Vector2 steering = Vector3.zero;
        int total = 0;
        Vector2 future_pos;
        foreach (Vector2 dir in directions)
        {
            future_pos = VUtils.Vec3ToVec2(transform.position) + dir;
            if (Physics2D.OverlapPoint(future_pos, LayerMask.GetMask("Impassables", "Hazards")))
            {
                steering -= dir;
                total++;
            }
        }
        future_pos = VUtils.Vec3ToVec2(transform.position) + rb.velocity.normalized;
        if (Physics2D.OverlapPoint(future_pos, LayerMask.GetMask("Impassables", "Hazards")))
        {
            steering -= rb.velocity.normalized;
            total++;
        }
        if (total > 0)
        {
            steering /= total;
        }
        return VUtils.Vec2ToVec3(steering).normalized;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    public void LockControls()
    {
        controlsAreLocked = true;
    }

    public void UnlockControlsAndRestartMovement()
    {
        controlsAreLocked = false;

        // This will force the movement script to repath on the next frame
    }
}
