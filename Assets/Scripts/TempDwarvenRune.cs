using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempDwarvenRune : MonoBehaviour
{
    public float lifetime;
    public float elapsed;
    public GameObject Enemy;
    public int damage;

    private PlayerHealth playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        elapsed = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
        else
        {
            elapsed += Time.deltaTime;

        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
