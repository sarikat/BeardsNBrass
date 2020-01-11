using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPlant : MonoBehaviour
{
    public float Spread;
    public int Leaves = 5;
    public int Stems = 3;
    public Sprite[] LeaveSprites;
    public Sprite[] StemSprites;

    GameObject[] children;

    // Start is called before the first frame update
    void Start()
    {
        for (int child_num = 0; child_num < Leaves; child_num++) {
            GameObject leaf = new GameObject();
            // leaf.AddComponent(SpriteRenderer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
