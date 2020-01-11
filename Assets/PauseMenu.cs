using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject PauseMenuUI, resumeButton;
    public EventSystem eventSystem;

    // for lerping the pause menu from above
    //public Transform startMarker;
    //public Transform endMarker;
    //public float speed = 1.0f;
    //private float startTime;
    //private float journeyLength;

    void Start()
    {
        //startTime = TimeManager.time;
        //journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
        //Debug.Log(startMarker.position.y + " " + endMarker.position.y);
    }


    void Update()
    {
        bool pausePressed = false;
        // TODO: gamepad-ify this
        foreach (Gamepad gpad in Gamepad.all)
        {
            if (gpad.startButton.wasPressedThisFrame)
            {
                //float distCovered = (TimeManager.time - startTime) * speed;
                //float fractionOfJourney = distCovered / journeyLength;
                //PauseMenuUI.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
                //Debug.Log(startMarker.position.y + " " + endMarker.position.y);
                pausePressed = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePressed = true;
        }

        if (pausePressed)
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        toggleMenu();
    }

    public void Pause()
    {
        toggleMenu();
        eventSystem.SetSelectedGameObject(resumeButton);
    }

    public void Restart()
    {
        toggleMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restarting level");

    }

    public void Quit()
    {
        toggleMenu();
        SceneManager.LoadScene(0);
        Debug.Log("Quitting to main menu");

    }


    public void toggleMenu()
    {
        bool oldCondition = gameIsPaused;
        gameIsPaused = !gameIsPaused;
        PauseMenuUI.SetActive(gameIsPaused);
        (oldCondition == true ? new UnityAction(TimeManager.Resume) : TimeManager.Pause)();
    }
}
