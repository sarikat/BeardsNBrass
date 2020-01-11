using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    bool huh;
    public Transform t_in;
    CameraFollow mainGameCam;

    Vector3 mainGameStartPos = new Vector3(1.04f, 8.69f, -10);

    // Start is called before the first frame update
    void Start()
    {
        mainGameCam = GetComponent<CameraFollow>();
        StartCoroutine(TestCam());
    }

    // Update is called once per frame
    void Update()
    {
        if (huh)
        {
            Debug.Log("huh!1");
            Vector3 newplace = new Vector3(t_in.position.x, t_in.position.y, transform.position.z);
            transform.position = newplace;
            mainGameCam.mainGame = true;
            huh = false;
        }
    }

    IEnumerator TestCam()
    {
        yield return new WaitForSeconds(3f);
        huh = true;


    }
}
