using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    private static Toast toast;

    private static float waveTimer = 60f;

    private static readonly float enemy_buffer = 2f;

    private static readonly int[] wavesToNumEnemies = { 10, 16, 24 };

    public static int num_fallenEnemies, currentWave, wave_buffer = 4;

    private static bool cooldownEnded, gameEnded, waveEnded;

    public GameObject[] Enemy;
    public GameObject[] enemySpawners;
    public Transform[] coinSpawners;
    public GameObject coinPrefab;
    public Dialogue[] wStartDialogues, wEndDialogues;

    public GameObject EndgameUI;
    public EventSystem eventSystem;
    public GameObject menuButton;
    public Text ScoreText;
    static DialogueManager dialogueManager;
    Treasure treasure;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
        }
        if (!instance)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        toast = GameObject.FindWithTag("Toast").GetComponent<Toast>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        num_fallenEnemies = 0;
        currentWave = 0;
        cooldownEnded = false;
        waveEnded = false;
        gameEnded = false;
    }

    private void Update()
    {
        if (treasure == null)
        {
            treasure = FindObjectOfType<Treasure>();
        }

        if (waveEnded)
        {
            StopWave();
        }

        else if (cooldownEnded)
        {
            cooldownEnded = false;
            StartWave(currentWave + 1);
        }

        else if (gameEnded)
        {
            gameEnded = false;
            Debug.LogWarning("Game Set and Match");
            ScoreText.text = "Congratulations!! Chester made it home!!";
            EndgameUI.SetActive(true);
            eventSystem.SetSelectedGameObject(menuButton);
        }
    }

    public static void StartWave(int wave_number)
    {
        currentWave = wave_number;
        //dialogueManager.StartDialogue(instance.wStartDialogues[currentWave], false);
        instance.StartCoroutine(instance.RunWave());
    }

    public static void StopWave()
    {
        waveEnded = false;
        instance.StopAllCoroutines();
        var enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemyObjs)
        {
            enemy.GetComponent<Health>().WaveKill();
        }
        instance.StartCoroutine(instance.EndOfWave());
    }

    IEnumerator RunWave()
    {
        yield return new WaitForSeconds(wave_buffer + 2);
        StartCoroutine(RunTimer());
        StartCoroutine(RainCoins());
        int spawner_num = 0;
        while (true)
        {
            spawner_num += 1;
            spawner_num %= 2;
            GameObject spawner = enemySpawners[spawner_num];
            int rand_index = Random.Range(0, Enemy.Length);
            Instantiate(Enemy[rand_index], spawner.transform.position, Quaternion.identity);

            yield return new WaitForSeconds(enemy_buffer);
        }
    }

    IEnumerator RainCoins()
    {
        while (true)
        {
            int rand_cooldown = Random.Range(5, 15);
            int rand_spawnind = Random.Range(0, coinSpawners.Length);
            int num_coins = Random.Range(1, 9);
            for (int i = 0; i < num_coins; i++)
            {
                yield return new WaitForSeconds(0.3f);
                int a = 360 / num_coins * i;
                Vector3 pos = RandCircle(coinSpawners[rand_spawnind].position, a);
                Instantiate(coinPrefab, pos, Quaternion.identity);
            }
            yield return new WaitForSeconds(rand_cooldown);
        }
    }

    Vector3 RandCircle(Vector3 ctr, int ang)
    {
        float angle = ang;
        float angX = ctr.x + Mathf.Sin(angle * Mathf.Deg2Rad);
        float angY = ctr.y + Mathf.Cos(angle * Mathf.Deg2Rad);

        return new Vector3(angX, angY, ctr.z);
    }

    IEnumerator RunTimer()
    {
        float remainder = waveTimer;
        toast.txt.enabled = true;
        toast.txt.color = new Color32(255, 225, 53, 255);
        while (remainder > 0)
        {
            string prepend = "Wave ends in 0:";
            if (remainder < 10f)
            {
                prepend += "0";
            }
            toast.txt.text = prepend + remainder.ToString("F1");
            remainder -= TimeManager.deltaTime;
            yield return null;
        }
        toast.txt.text = "";
        waveEnded = true;
    }

    IEnumerator EndOfWave()
    {
        num_fallenEnemies = 0;

        if (currentWave == 2)
        {
            gameEnded = true;
        }
        else
        {
            //dialogueManager.StartDialogue(wEndDialogues[currentWave], false);
            yield return new WaitForSeconds(wave_buffer + 2);
            toast.txt.enabled = true;

            float remainder = 10f;
            while (remainder > 0)
            {
                string prepend = "Next wave in 0:";
                if (remainder < 10f)
                {
                    prepend += "0";
                }
                toast.txt.text = prepend + remainder.ToString("F1");
                remainder -= TimeManager.deltaTime;
                yield return null;
            }
            toast.txt.text = "";
            cooldownEnded = true;
        }
        yield return new WaitForSeconds(1f);
    }

}
