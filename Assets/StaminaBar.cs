using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Image foregroundImage;

    Stamina stamina;


    private void Awake() {
        stamina = GetComponentInParent<Stamina>();
    }

    private void Update() {
        foregroundImage.fillAmount = stamina.GetPercentStamina();
    }
}
