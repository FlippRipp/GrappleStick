using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeallingPickup : MonoBehaviour
{
    private float healAmount = 10;
    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerMovement pM = other.gameObject.GetComponent<PlayerMovement>();
        
        if (pM)
        {
            pM.HealPlayer(healAmount);
            Destroy(gameObject);
        }
    }
}
