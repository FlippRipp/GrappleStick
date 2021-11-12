using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField]
    private Transform healthBar;

    public void OnHealthChanged(float currentHealth, float maxHealth)
    {
        healthBar.localScale = new Vector3(currentHealth / maxHealth, currentHealth / maxHealth, 1);
    }
}
