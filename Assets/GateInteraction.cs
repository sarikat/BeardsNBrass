using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateInteraction : InteractionBehaviour
{
    public float InteractionTime;

    PlayerManager players;

    float progress;

    int numNeededToInteract;

    //private Toast toast;

    void Start()
    {
        BaseStart();
        players = GetComponent<PlayerManager>();
   
    }

    protected override void CheckInteraction()
    {
        bool is_player_interacting = false;
        foreach (PlayerInput input in playerInputsInRange)
        {
            if (input.IsInteracting())
            {
                is_player_interacting = true;
                break;
            }
        }

        if (is_player_interacting)
        {
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

    void InteractionHelper()
    {
        // Debug.Log("interaction helper, progress: " + progress.ToString() + ", interaction time: " + InteractionTime.ToString());

        progress += Time.deltaTime;
        if (StatusVisualizer && indicator == null)
        {
            GameObject indicator_object = Instantiate(StatusVisualizer, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            indicator = indicator_object.GetComponent<StatusIndicator>();
            indicator.UpdateStatus(progress / InteractionTime);
        }
        else if (indicator)
        {
            indicator.UpdateStatus(progress / InteractionTime);
        }

        // add the condition for all 3 players to have done stuff
        if (progress >= InteractionTime)
        {
            indicator.Finish();
            indicator = null;
            interactable = false;
            DoInteraction();
        }
    }

    protected override void DoInteraction()
    {


        Destroy(gameObject);
        // place a green bubble or some indicator above the chest


    }

    public static void SpawnWeaponPickup(string weaponName, Vector3 spawnPos)
    {
        //GameObject pickup = Instantiate(weaponPickupPrefabStatic, spawnPos, Quaternion.identity);
        //pickup.GetComponent<WeaponPickup>().weaponPrefabName = weaponName;

        //var sprite = Resources.Load<Sprite>("Weapons/Sprites/" + weaponName);
        //if (sprite != null)
        //{
        //    pickup.GetComponent<SpriteRenderer>().sprite = sprite;
        //}
    }
}
