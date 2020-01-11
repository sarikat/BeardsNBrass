using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextImporter : MonoBehaviour
{
    public TextAsset file;
    public string[] textLines;

    // Start is called before the first frame update
    void Start()
    {
        if (file != null)
        {
            textLines = file.text.Split('\n');
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
