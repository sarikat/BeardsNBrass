using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControlsTrigger : MonoBehaviour
{
    public Image controlsimg;
    private bool showControls;
    // Start is called before the first frame update
    void Start()
    {
        showControls = false;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Gamepad gpad in Gamepad.all)
        {
            if (gpad.startButton.wasPressedThisFrame)
            {
                showControls = !showControls;
                controlsimg.gameObject.SetActive(showControls);
                Debug.Log("Flashing controls: " + showControls);
            }
        }

    }
}
