using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionBehaviour : MonoBehaviour
{
    public GameObject StatusVisualizer;

    protected bool interactable = true;
    protected StatusIndicator indicator;
    protected int players_in_range = 0;

    protected List<PlayerInput> playerInputsInRange;
    protected List<LockPicker> lockPickersInRange;

    protected void BaseStart()
    {
        playerInputsInRange = new List<PlayerInput>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (interactable)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                players_in_range++;
                var playerInput = other.gameObject.GetComponent<PlayerInput>();
                if (playerInput == null)
                {
                    Debug.LogError("Player doesn't have a playerInput component");
                }
                if (playerInputsInRange == null)
                {
                    Debug.LogError("waaah");
                }
                playerInputsInRange.Add(playerInput);
            }
            else
            {
                LockPicker lp = other.GetComponent<LockPicker>();
                if (lp)
                {
                    players_in_range++;
                    lockPickersInRange.Add(lp);
                }
            }
        }
    }

    protected int GetNumInteracting()
    {
        int num_interacting = 0;
        foreach (PlayerInput input in playerInputsInRange)
        {
            if (input.IsInteracting())
            {
                num_interacting++;
            }
        }
        if (lockPickersInRange != null)
        {
            foreach (LockPicker lp in lockPickersInRange)
            {
                if (lp.IsPicking())
                {
                    num_interacting++;
                }
            }
        }
        return num_interacting;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (interactable && other.gameObject.CompareTag("Player"))
        {
            players_in_range--;
            playerInputsInRange.Remove(other.gameObject.GetComponent<PlayerInput>());
        }
    }

    private void Update()
    {
        if (players_in_range > 0)
        {
            CheckInteraction();
        }
        else
        {
            if (indicator)
            {
                indicator.Finish();
                indicator = null;
            }
        }
    }

    protected virtual void CheckInteraction()
    {

    }

    protected virtual void DoInteraction()
    {

    }
}
