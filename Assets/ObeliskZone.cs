using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskZone : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 6f);

        foreach(var coldr in colliders)
        {
            if (coldr.gameObject.layer == 8)
            {
                var hlth = coldr.GetComponent<Health>();
                hlth.TakeDamage(100000);
            }
        }
    }
}
