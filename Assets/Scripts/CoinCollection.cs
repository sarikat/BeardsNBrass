using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollection : CollectionBehaviour
{
    private bool retrieved;

    public AudioClip spawnAud, retrieveAud;

    private void Start()
    {
        retrieved = false;
        AudioSource.PlayClipAtPoint(spawnAud, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !retrieved)
        {
            Treasure treasure = FindObjectOfType<Treasure>();

            if (treasure != null)
            {
                retrieved = true;
                AudioSource.PlayClipAtPoint(retrieveAud, transform.position);
                treasure.AddGold();
                Vector3 newPos = treasure.transform.position;
                newPos.x += Random.Range(-1f, 1f);
                newPos.y += Random.Range(-1f, 1f);
                transform.position = newPos;
            }
        }
    }
}
