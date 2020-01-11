using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class testgameover : MonoBehaviour
{
    public GameObject GameOverUI;
    // Start is called before the first frame update
    void Start()
    {
        //GameOverUI = GameObject.FindGameObjectWithTag("Gameover");
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    Debug.Log("pressed");
        //    GameOverUI.SetActive(true);
        //}
    }

    public void DoGameOver()
    {
        GameOverUI.SetActive(true);
    }
}
