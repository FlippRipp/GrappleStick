using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VelocityUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text velocityText;

    public void OnVelocityUpdated(float velocity)
    {
        velocity = Mathf.Round(velocity * 100) / 100;
        velocityText.text = velocity.ToString();
    }
}
