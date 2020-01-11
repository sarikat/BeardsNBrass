using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            Collect();
        }
    }

    protected virtual void Collect() {
        Destroy(gameObject);
    }
}
