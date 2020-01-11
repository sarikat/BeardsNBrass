using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingCrystals : MonoBehaviour
{
    public GameObject[] Crystals;
    public float Speed = 2f;
    public float Height = 4f;
    public float Width = 7f;
    public float Wobble = 10f;

    float[] times;

    private void Start() {
        times = new float[Crystals.Length];
        for (int i = 0; i < Crystals.Length; i++) {
            times[i] = (float)i / Crystals.Length * 2 * Mathf.PI * (1 + Random.Range(-1f, 1f) * 0.2f);
        }
        foreach (float i in times) {
            Debug.Log(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Crystals.Length; i++) {
            Vector3 target = new Vector3(Mathf.Sin(times[i] + TimeManager.time * Speed) * Width, Mathf.Cos(times[i] + TimeManager.time * Speed) * Height, 0);
            // Vector3 current_loc = Vector3.Lerp(transform.localPosition, target, TimeManager.deltaTime);
            Vector3 current_loc = target;
            if (Crystals[i]) {
                Crystals[i].transform.localPosition = current_loc;
                Crystals[i].transform.GetChild(0).eulerAngles = new Vector3(0, 0, Mathf.Cos(times[i] + TimeManager.time * Speed) * Wobble);
            }
            
        }
    }
}
