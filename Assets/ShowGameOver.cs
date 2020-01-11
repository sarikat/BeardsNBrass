using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowGameOver : MonoBehaviour
{
    public GameObject GameOverUI, yesButton;
    public EventSystem eventSystem;

    void OnEnable()
    {
        StartCoroutine(showScreen());
    }

    IEnumerator showScreen()
    {
        yield return new WaitForSeconds(1);
        GameOverUI.SetActive(true);
        eventSystem.SetSelectedGameObject(yesButton);
    }
}
