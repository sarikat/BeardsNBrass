using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treasure : MonoBehaviour
{
    public Text text;
    public testgameover GameOverTrigger;
    public int Gold { get => gold; }
    int gold_value = 5;
    int gold;
    // Start is called before the first frame update
    void Start()
    {
        gold = 500;
        GameOverTrigger = GameObject.FindGameObjectWithTag("Gameover").GetComponent<testgameover>();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            var body = collider.GetComponent<DamagingEnemyBody>();
            body.routine = body.GoldTheft();
            body.StartCoroutine(body.routine);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            var body = collision.GetComponent<DamagingEnemyBody>();
            body.StopCoroutine(body.routine);
        }
    }*/

    public void StealGold()
    {
        gold -= gold_value;
        UpdateText();

        if (gold <= 0)
        {
            GameOverTrigger.DoGameOver();
        }
    }

    void UpdateText()
    {
        text.text = gold.ToString() + " Gold";
    }

    public void AddGold()
    {
        gold += gold_value;
        UpdateText();
    }
}
