using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ClampDialogue : MonoBehaviour
{
    public GameObject dialogue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dialogue.transform.position = transform.position;
    }
}
