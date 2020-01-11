using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerRespawner : MonoBehaviour
{
    public Text[] playerTimerTexts;
    private PlayerGenerator playerGen;
    GameObject ghost;

    // Start is called before the first frame update
    void Start()
    {
        playerGen = GameObject.Find("PlayerGenerator").GetComponent<PlayerGenerator>();
    }

    public void StartTimer(int player_num, float duration)
    {
        StartCoroutine(RunTimer(player_num, duration));
    }
    
    public void AddGhost(GameObject new_ghost) {
        ghost = new_ghost;
    }

    IEnumerator RunTimer(int player_num, float duration)
    {
        float remainder = duration;

        Color from = new Color32(253, 238, 0, 255);
        Color to = Color.green;

        Text timerText = playerTimerTexts[player_num];

        while(remainder > 0)
        {
            string prepend = "P" + (player_num+1) + ": 0:";
            if (remainder < 10f)
            {
                prepend += "0";
            }
            timerText.text = prepend + remainder.ToString("F1");
            timerText.color = Color.Lerp(from, to, (duration - remainder) / duration);
            remainder -= TimeManager.deltaTime;
            yield return null;
        }
        yield return null;
        timerText.text = "";
        if (!PlayerManager.Players.ContainsKey(player_num))
        {
            GameObject player = Instantiate(playerGen.PlayerPrefab,
                PlayerManager.GetCenterOfMass(), Quaternion.identity);
            if (ghost) {
                player.transform.position = ghost.transform.position;
                Destroy(ghost);
                ghost = null;
            }
            player.GetComponent<PlayerInput>().SetPlayerInput(false, player_num);
            player.GetComponent<ColorManager>().SetColor(playerGen.PlayerColors[player_num]);
            PlayerGenerator.activePlayers.Add(player_num);
            PlayerGenerator.activeGamepads.Add(Gamepad.all[player_num]);
            PlayerManager.AddPlayer(player_num, player);
        }
    }
}
