using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyTypes;
    private GameObject[] enemySpawners;

    public GameObject EndgameUI;
    public EventSystem eventSystem;
    public GameObject menuButton;
    public Text ScoreText;

    public static bool canSpawn, endOfGame;
    public static int eatenGems;
    private static int spawnerNum;
    private static float spawnThreshold = 1.2f, counter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // TODO (sarikat): set this to true once the tutorial is finished
        canSpawn = false;

        endOfGame = false;
        eatenGems = 0;
        spawnerNum = 0;

        enemySpawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
    }

    // Update is called once per frame
    void Update()
    {
        if (endOfGame)
        {
            canSpawn = false;
            Debug.LogWarning("Game Set and Match");
            ScoreText.text = "Congratulations!! Chester made it home!!";
            EndgameUI.SetActive(true);
            eventSystem.SetSelectedGameObject(menuButton);
        }
        else if (canSpawn)
        {
            if (counter < spawnThreshold)
            {
                counter += TimeManager.deltaTime;
            }
            else
            {
                counter = 0f;
                int enemyType = 0; // Gems 0-2

                GameObject spawner = enemySpawners[spawnerNum];
                spawnerNum++;
                spawnerNum %= enemySpawners.Length;

                float dist = (spawner.transform.position - PlayerManager.GetCenterOfMass()).magnitude;
                // Debug.LogWarning(spawner.name + ": " + dist);
                if (dist > 20f)
                {
                    return;
                }

                // Gems 3-4
                if (eatenGems >= 3 && eatenGems < 6)
                {
                    enemyType = Random.Range(0, 2);
                }
                // Gems 5-7
                else if (eatenGems >= 6)
                {
                    enemyType = Random.Range(0, enemyTypes.Length);
                }

                Vector3 spawnPoint = spawner.transform.Find("SpawnPoint").position;

                Instantiate(enemyTypes[enemyType], spawnPoint, Quaternion.identity);
            }
        }
    }
}
