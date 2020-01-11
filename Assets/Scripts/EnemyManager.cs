using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject Enemy;
    public GameObject[] enemySpawners;
    public float SpawnRadius = 7;
    public int MaxNumberEnemies = 3;

    public static int num_enemies;

    float give_5_sec;

    private void Update()
    {
        if (PlayerManager.GetNumPlayers() > 0)
        {
            if (give_5_sec < 5)
            {
                give_5_sec += TimeManager.deltaTime;
            }
            else
            {
                while (num_enemies < MaxNumberEnemies)
                {
                    Vector3 pos = transform.position + GetRandomOnUnitCircle() * SpawnRadius;
                    if (!Physics2D.OverlapCircle(pos, 1, LayerMask.GetMask("Impassables")))
                    {
                        num_enemies++;
                        Instantiate(Enemy, transform.position + GetRandomOnUnitCircle() * SpawnRadius, Quaternion.identity);
                    }
                }
            }
        }
    }

    static Vector3 GetRandomOnUnitCircle()
    {
        Vector2 v2 = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
        Vector3 v3 = new Vector3(v2.x, v2.y, 0);
        return v3;
    }

    public static void EnemyDied()
    {
        num_enemies--;
    }
}
