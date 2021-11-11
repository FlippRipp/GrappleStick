using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityDeadZone : MonoBehaviour
{
    private Rigidbody2D playerRB;
    [SerializeField]
    private float deadZoneRange;

    [SerializeField] private Transform gravityVisuals;

    private void OnValidate()
    {
        gravityVisuals.localScale = new Vector3(deadZoneRange, deadZoneRange, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRB = FindObjectOfType<PlayerMovement>().GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!playerRB) return;
        if (Vector2.Distance(playerRB.position, transform.position) < deadZoneRange)
        {
            playerRB.AddForce(Vector2.down * playerRB.gravityScale * Physics2D.gravity);
        }
    }
}
