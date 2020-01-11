using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public int StaminaResevoir = 100;
    public float StaminaRegen = 2f;
    private float stamina;

    public float CurrentStamina { get { return stamina; } }

    void Start()
    {
        stamina = StaminaResevoir;
    }

    void Update()
    {
        stamina += StaminaRegen * 10 * TimeManager.deltaTime;
        stamina = Mathf.Min(StaminaResevoir, stamina);
    }

    public float GetPercentStamina() {
        return stamina / StaminaResevoir;
    }

    public bool TryExertStamina(float amount)
    {
        if (stamina > amount) {
            stamina -= amount;
            return true;
        }
        return false;
    }
}
