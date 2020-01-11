using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPicker : MonoBehaviour
{
    public float MovementSpeed = 3f;
    public float SkiddishDistance = 5;
    public float SkiddishTime = 3;
    public StatusIndicator indicator;

    Rigidbody2D rb;
    bool picking = false;
    float distance_to_com = 999;
    float alone_timer = 0;
    Vector3 target;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        UpdateIndicator();
    }

    public bool IsPicking() {
        return picking;
    }

    void Move() {
        Vector3 com = PlayerManager.GetCenterOfMass();
        distance_to_com = Vector3.Distance(com, transform.position);

        if (distance_to_com > SkiddishDistance) {
            alone_timer += TimeManager.deltaTime;
        } else {
            alone_timer = 0;
        }

        if (picking) {
            if (Vector3.Distance(target, transform.position) < 0.8f) {
                rb.velocity = Vector3.zero;
            } else {
                Vector3 direction = (target - transform.position).normalized;
                rb.velocity = direction * MovementSpeed;
            }
        } else if (PlayerManager.GetNumPlayers() > 0) {
            Vector3 direction = (com - transform.position).normalized;
            
            rb.velocity = direction * MovementSpeed;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player") && distance_to_com < SkiddishDistance) {
            rb.velocity = Vector3.zero;
        }
        InteractionBehaviour ib = other.GetComponent<InteractionBehaviour>();
        if (ib && alone_timer < SkiddishTime) {
            picking = true;
            target = other.transform.position;
        }
    }

    void UpdateIndicator() {
        if (picking) {
            indicator.UpdateStatus(Mathf.Min(1, alone_timer / SkiddishTime));
        }
    }
}
