using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : MonoBehaviour
{
    Collider2D iconCollider;
    Renderer arrowRenderer;
    Renderer[] iconRenderers;
    GameObject chester;

    private bool isFlickering;
    private float counter;
    private readonly float viewportMin = 0.1f, viewportMax = 0.9f, flickerThresh = 8f;

    // Start is called before the first frame update
    void Start()
    {
        chester = GameObject.FindGameObjectWithTag("Chester");
        arrowRenderer = GetComponent<Renderer>();
        iconRenderers = GetComponentsInChildren<Renderer>();
        iconCollider = GetComponentInChildren<Collider2D>();

        counter = 0f;
        isFlickering = false;
        SetRenderers(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerGenerator.activePlayers.Count > 0)
        {
            Vector3 toPosition = chester.transform.position;
            Vector3 fromPosition = Camera.main.transform.position;
            float angle1 = Vector3.SignedAngle(fromPosition, toPosition, -Vector3.forward);
            float angle = Mathf.Atan2(toPosition.y - fromPosition.y, toPosition.x - fromPosition.x) * Mathf.Rad2Deg;
            transform.localEulerAngles = new Vector3(0, 0, angle);

            float size = (Camera.main.transform.position - transform.position).magnitude;
            transform.localScale = new Vector3(size, size, size) * 0.09f;

            bool chesterVisible = chester.GetComponentInChildren<Renderer>().isVisible;
            if (!chesterVisible)
            {
                counter += TimeManager.deltaTime;

                Vector3 arrowViewPos = Camera.main.WorldToViewportPoint(chester.transform.position);

                arrowViewPos.x = Mathf.Clamp(arrowViewPos.x, viewportMin, viewportMax);
                arrowViewPos.y = Mathf.Clamp(arrowViewPos.y, viewportMin, viewportMax);
                Vector3 arrowWorldPos = Camera.main.ViewportToWorldPoint(arrowViewPos);

                transform.position = new Vector3(arrowWorldPos.x, arrowWorldPos.y, 0);

                if (!arrowRenderer.enabled)
                {
                    SetRenderers(true);
                }

                if (!isFlickering && counter >= flickerThresh)
                {
                    isFlickering = true;
                    //Debug.LogWarning("Calling Flicker: " + counter);
                    StartCoroutine(Flicker());
                }
            }
            else
            {
                if (arrowRenderer.enabled)
                {
                    if (isFlickering)
                    {
                        StopAllCoroutines();
                        SetAlphas(1f);
                        isFlickering = false;
                        counter = 0f;
                        //Debug.Log("Stopped Flicker");
                    }
                    SetRenderers(false);
                }
            }
        }
    }

    IEnumerator Flicker()
    {
        bool fadeOut = true;
        float flickerCounter = 0f;
        const float flickerInterval = 0.1f;
        int numConsecs = 0;
        const int maxConsecs = 7;

        while (true)
        {
            if (numConsecs == maxConsecs)
            {
                numConsecs = 0;
                SetAlphas(1f);
                fadeOut = true;
                yield return new WaitForSeconds(3f);
                continue;
            }

            float a, b;
            if (fadeOut)
            {
                a = 1f;
                b = 0f;
            }
            else
            {
                a = 0f;
                b = 1f;
            }
            float alph = Mathf.Lerp(a, b, flickerCounter / flickerInterval);

            SetAlphas(alph);

            flickerCounter += TimeManager.deltaTime;

            if (flickerCounter >= flickerInterval)
            {
                flickerCounter = 0f;
                numConsecs++;
                fadeOut = !fadeOut;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void SetAlphas(float alph)
    {
        Color col = arrowRenderer.material.color;
        col.a = alph;
        arrowRenderer.material.color = col;

        foreach (Renderer iconRend in iconRenderers)
        {
            Color orig = iconRend.material.color;
            orig.a = col.a;
            iconRend.material.color = orig;
        }
    }

    private void SetRenderers(bool show)
    {
        arrowRenderer.enabled = show;
        foreach (Renderer rend in iconRenderers)
        {
            rend.enabled = show;
        }
    }
}
