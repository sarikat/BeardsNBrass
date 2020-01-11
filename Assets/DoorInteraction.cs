using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : InteractionBehaviour
{
    public float InteractionTime;

    float progress;

    private void Start() {
        BaseStart();
    }
    protected override void CheckInteraction()
    {
        bool is_player_interacting = false;
        foreach (PlayerInput input in playerInputsInRange)
        {
            if (input.IsInteracting()) {
                is_player_interacting = true;
                break;
            }
        }

        if (is_player_interacting)
        {
            // Debug.Log("e");
            InteractionHelper();
        }
        else
        {
            if (indicator)
            {
                progress = 0;
                indicator.Finish();
                indicator = null;
            }

        }
    }

    protected override void DoInteraction()
    {
        Destroy(gameObject);
    }

    void InteractionHelper()
    {
        Debug.Log(progress);
        // Debug.Log("interaction helper, progress: " + progress.ToString() + ", interaction time: " + InteractionTime.ToString());

        progress += Time.deltaTime;
        if (StatusVisualizer && indicator == null)
        {
            GameObject indicator_object = Instantiate(StatusVisualizer, transform.position, Quaternion.identity);
            indicator = indicator_object.GetComponent<StatusIndicator>();
            indicator.UpdateStatus(progress / InteractionTime);
        }
        else if (indicator)
        {
            indicator.UpdateStatus(progress / InteractionTime);
        }

        if (progress >= InteractionTime)
        {
            indicator.Finish();
            indicator = null;
            interactable = false;
            DoInteraction();
        }
    }
}
