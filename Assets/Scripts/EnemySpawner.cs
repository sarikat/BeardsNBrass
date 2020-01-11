using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemies;
    public bool is_boss = false;
    public float[] enemy_weights;
    public int max_enemies = 6;

    public int remainingEnemies;

    public bool tutorial;
    public int toastText = 0;
    public float prepTime = 2.8f;

    private static float randMin = 0.7f, randMax = 1.2f;

    public float bossMin = 2f, bossMax = 2.8f;
    public GameObject SpawnEffect;

    private Transform spawnPoint;
    private Toast toast;
    float total = 0;
    bool is_spawning = false;
    float counter = 999f;

    // Start is called before the first frame update
    void Start()
    {
        toast = GameObject.FindGameObjectWithTag("Toast").GetComponent<Toast>();

        foreach (float val in enemy_weights) {
            total += val;
        }
        remainingEnemies = 0;
        spawnPoint = transform.Find("SpawnPoint");
        foreach (Collider2D col in Physics2D.OverlapCircleAll(VUtils.Vec3ToVec2(transform.position), 10))
        {
            AIBoids ai = col.gameObject.GetComponent<AIBoids>();
            if (ai)
            {
                remainingEnemies++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        counter += TimeManager.deltaTime;
        if (is_spawning && counter >= prepTime && remainingEnemies < max_enemies) {
            Spawn();
        }
    }

    void Spawn() {
        switch (toastText)
        {
            case 1:
                toast.ShowToast(("Grapple and throw enemies into the priests on the floating islands!", 6));
                toastText = 0;
                break;
            case 2:
                toast.ShowToast(("Remember to drink mead with LB if you're running low on health! But beware -- it will slow movement!", 6));
                toastText = 0;
                break;
            case 4:
                toast.ShowToast(("Grapple and throw enemies into the health crystals to stop Geminator from regenerating health!", 7));
                toastText = 0;
                break;
            default:
                break;
        }
        float val = Random.Range(0f, total);
        GameObject spawn = enemies[0];
        for (int i = 0; i < enemies.Length; i++) {
            if (val <= enemy_weights[i]) {
                spawn = enemies[i];
                break;
            } else {
                val -= enemy_weights[i];
            }
        }
        SoundManager.PlaySound(SoundManager.Sound.Fireball);
        Vector3 spawn_point = VUtils.Vec2ToVec3(Random.insideUnitCircle) * 2 + transform.position;
        Instantiate(SpawnEffect, spawn_point, Quaternion.identity);
        GameObject enemy = Instantiate(spawn, spawn_point, Quaternion.identity);
        enemy.transform.parent = transform;
        remainingEnemies++;
        counter = 0;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (is_boss && other.CompareTag("Player")) {
            is_spawning = true;
        }
    }

    public void SpawnEnemies()
    {

        // for the tutorial -- we want to manually spawn the enemies
        if (!tutorial)
        {
            is_spawning = true;
        }
    }
}
