using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChestInteraction : InteractionBehaviour
{
    public GameObject weaponPickupPrefab;
    static GameObject weaponPickupPrefabStatic;
    public GameObject Loot;
    public float InteractionTime;

    bool waveStarted;
    float progress;

    private Toast toast;

    void Start()
    {
        BaseStart();
        if (weaponPickupPrefabStatic == null)
        {
            weaponPickupPrefabStatic = weaponPickupPrefab;
        }
        waveStarted = false;
    }

    protected override void CheckInteraction()
    {

        if (GetNumInteracting() > 0)
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

    void InteractionHelper()
    {
        // Debug.Log("interaction helper, progress: " + progress.ToString() + ", interaction time: " + InteractionTime.ToString());

        progress += TimeManager.deltaTime;
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
        toast = GameObject.FindGameObjectWithTag("Toast").GetComponent<Toast>();
        toast.StopAllCoroutines();
        // toast.ShowToast("Press 'x' to pick up weapon", 4);

        // SpawnWeaponPickup(RandomWeapon(), transform.position);
        // for (int i = 0; i < 5; ++i)
        // {
        //     Vector3 spawn_position = transform.position;
        //     Vector2 random_offset = Random.insideUnitCircle;
        //     spawn_position.x += random_offset.x;
        //     spawn_position.y += random_offset.y;
        //     Instantiate(Loot, spawn_position, Quaternion.identity);
        // }
        if (waveStarted == false)
        {
            waveStarted = true;
            WaveManager.StartWave(0);
        }

        Destroy(gameObject);
    }

    public static void SpawnWeaponPickup(string weaponName, Vector3 spawnPos)
    {
        GameObject pickup = Instantiate(weaponPickupPrefabStatic, spawnPos, Quaternion.identity);
        pickup.GetComponent<WeaponPickup>().weaponPrefabName = weaponName;

        var sprite = Resources.Load<Sprite>("Weapons/Sprites/" + weaponName);
        if (sprite != null)
        {
            pickup.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    // string RandomWeapon()
    // {
    //     var weapons = Weapon.List;
    //     return Weapon.List[Random.Range(0, weapons.Count)];
    // }
}