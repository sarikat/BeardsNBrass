using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : InteractionBehaviour
{
    public string weaponPrefabName;

    GameObject player;
    private Toast toast;

    void Start()
    {
        BaseStart();
    }

    protected override void CheckInteraction()
    {
        bool padPressed = false;

        foreach (PlayerInput playerInput in playerInputsInRange)
        {
            Gamepad gpad = playerInput.GetGamepad();
            if (gpad != null && gpad.xButton.wasPressedThisFrame && !playerInput.armed)
            {
                padPressed = true;
                player = playerInput.gameObject;
            }
            else
            {
                padPressed = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            padPressed = true;
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (padPressed)
        {
            Debug.Log("e");
            DoInteraction();
        }
    }

    protected override void DoInteraction()
    {
        string filepath = "Weapons/Prefabs/" + weaponPrefabName;
        GameObject weaponPrefab = Resources.Load<GameObject>(filepath);
        if (weaponPrefab == null)
        {
            Debug.LogError("Couldn't find prefab at " + filepath);
        }
        else
        {
            DropWeapons(player);

            // Give weapon to player
            player.GetComponent<PlayerInput>().armed = true;
            GameObject weapon = Instantiate(weaponPrefab, Vector2.zero, Quaternion.identity);
            weapon.transform.SetParent(player.transform);
            weapon.transform.localPosition = Vector2.zero;
            weapon.GetComponent<Weapon>().weaponName = weaponPrefabName;

            toast = GameObject.FindGameObjectWithTag("Toast").GetComponent<Toast>();
            toast.StopAllCoroutines();
            toast.QueueToast("Press 'B' to attack.", 4);
        }

        Destroy(gameObject);
    }

    void DropWeapons(GameObject player)
    {
        foreach (Transform child in player.transform)
        {
            var weapon = child.gameObject.GetComponent<Weapon>();
            if (weapon != null)
            {
                ChestInteraction.SpawnWeaponPickup(weapon.weaponName, transform.position);
                Destroy(child.gameObject);
            }
        }
    }
}
