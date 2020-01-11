using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public GameObject colorScreen;

    public void PlayGame()
    {
        
        Debug.Log("Play main game");
		StartCoroutine(beginGame());
    }

    IEnumerator beginGame()
	{
		fadeToColor();
		yield return new WaitForSeconds(1.3f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void StartTutorial()
    {
        Debug.Log("Play the tutorial");
        fadeToColor();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
        fadeToColor();
    }

    private void fadeToColor()
    {
        colorScreen.SetActive(true);
    }
}
