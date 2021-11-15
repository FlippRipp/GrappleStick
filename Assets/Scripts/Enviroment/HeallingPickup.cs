using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeallingPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 10;
    //[SerializeField] private bool reespawns;
    
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
