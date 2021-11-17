using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [HideInInspector]
    public PortalController portalController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb)
            {
                Debug.Log("aaaaaa");
                portalController.Teleport(this, rb);
            }
        }
    }
}
