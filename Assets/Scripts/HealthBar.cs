using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public Image foregroundImage;

    Health health;


    private void Awake() {
        health = GetComponentInParent<Health>();
    }

    private void Update() {
        foregroundImage.fillAmount = health.GetPercentHealth();
    }

}
