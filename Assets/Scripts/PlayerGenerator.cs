using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGenerator : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public static HashSet<int> activePlayers;
    public static HashSet<Gamepad> activeGamepads;
    public Color[] PlayerColors;

    private void Start()
    {
        activePlayers = new HashSet<int>();
        activeGamepads = new HashSet<Gamepad>();

        if (Gamepad.all.Count > 0)
        {
            CreatePlayer(0, false);
            activeGamepads.Add(Gamepad.all[0]);

			CreatePlayer(1, false);
			activeGamepads.Add(Gamepad.all[1]);
		}
        // Debug.LogError("There are " + Gamepad.all.Count + " gamepads connected");
    }

    private void Update()
    {
        CheckForInputs();
    }

    void CheckForInputs()
    {
        if (activePlayers.Count < 4 && !activePlayers.Contains(3))
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //TODO Make a better player spawn in position
                CreatePlayer(3, true);
            }
        }
        // if (activePlayers.Count < 4 && !activePlayers.Contains(2)) {
        //     if (Input.GetKeyDown(KeyCode.Return)) {
        //         //TODO Make a better player spawn in position
        //         GameObject player = Instantiate (PlayerPrefab, Vector3.zero, Quaternion.identity);
        //         player.GetComponent<PlayerInput>().SetPlayerInput(true, 2);
        //         activePlayers.Add(2);
        //         PlayerManager.AddPlayer(2, player);
        //     }
        // }

       /*for (int pad_num = 0; pad_num < Gamepad.all.Count; pad_num++)
        {
            if (activePlayers.Count < 4 && !activePlayers.Contains(pad_num))
            {
                if (Gamepad.all[pad_num].aButton.wasPressedThisFrame)
                {
                    // Debug.LogError("pad_num: " + pad_num);
                    //TODO Make a better player spawn in position
                    CreatePlayer(pad_num, false);
                    activeGamepads.Add(Gamepad.all[pad_num]);
                }
            }
        }*/
    }

    void CreatePlayer(int player_num, bool is_keyboard)
    {
        //var spawnLoc = new Vector3(0, 5, 0);
        var spawnLoc = new Vector3(-78.2f, 40.3f, 0);
        GameObject player = Instantiate(PlayerPrefab, spawnLoc, Quaternion.identity);
        player.name = "Player " + (player_num + 1).ToString();
        player.GetComponent<PlayerInput>().SetPlayerInput(is_keyboard, player_num);
        player.GetComponent<ColorManager>().SetColor(PlayerColors[player_num]);
        activePlayers.Add(player_num);
        PlayerManager.AddPlayer(player_num, player);
    }
}
