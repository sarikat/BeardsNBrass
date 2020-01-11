using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static Dictionary<int, GameObject> Players;
    public static PlayerManager main;

    public static bool spawnStarted;
    
    private void Awake() {
        if (main && main != this) {
            Destroy(gameObject);
        }
        if (!main) {
            main = this;
        }
        Players = new Dictionary<int, GameObject>();
        spawnStarted = false;
    }

    public static void AddPlayer(int num, GameObject new_player) {
        if (!Players.ContainsKey(num))
        {
            Players.Add(num, new_player);
        }
        if (!spawnStarted)
        {
            spawnStarted = true;
        }
    }

    public static void RemovePlayer(int num)
    {
        Players.Remove(num);
    }

    public static int GetNumPlayers() {
        return Players.Count;
    }

    public static Vector3 GetCenterOfMass() {
        Vector3 center_of_mass = Vector3.zero;

        foreach (GameObject player in Players.Values) {
            center_of_mass += player.transform.position * (1f / Players.Count);
        }
        return center_of_mass;
    }

    public static float GetDispersion() {
        float dispersion = 0;
        Vector3 com = GetCenterOfMass();

        foreach (GameObject player in Players.Values) {
            dispersion += Vector3.Distance(player.transform.position, com);
        }
        return dispersion / Players.Count;
    }

    public static GameObject GetPlayer(int player_num) {
        if (player_num < Players.Count) {
            return Players[player_num];
        }
        Debug.Log("There is no player number " + player_num.ToString());

        return null;
    }

    public static GameObject[] GetAllPlayers() {
        List<GameObject> cur_players = new List<GameObject>();
        foreach(int player_num in Players.Keys) {
            if (!Players[player_num]) {
                Players.Remove(player_num);
            } else {
                cur_players.Add(Players[player_num]);
            }
        }
        return cur_players.ToArray();
    }

    public static GameObject GetClosestPlayer(Vector3 pos) {
        Vector2 pos2 = VUtils.Vec3ToVec2(pos);
        GameObject closest_player = null;
        float closest = Mathf.Infinity;
        if (Players.Count < 1) {
            return null;
        }

        foreach(GameObject p in Players.Values) {
            if (Vector2.Distance(pos2, VUtils.Vec3ToVec2(p.transform.position)) < closest) {
                closest_player = p;
                closest = Vector2.Distance(pos2, VUtils.Vec3ToVec2(p.transform.position));
            }
        }
        return closest_player;
    }
}
