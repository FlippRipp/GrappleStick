using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour
{
    private Rigidbody2D playerRB;
    [SerializeField]
    private float gravityRange;
    [SerializeField]
    private float gravityStrength;

    [SerializeField] private Transform gravityVisuals;

    private void OnValidate()
    {
        if(!gravityVisuals) return;
        gravityVisuals.localScale = new Vector3(gravityRange, gravityRange, 1);
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
        float strength = Mathf.Lerp(0, gravityStrength,
            1 - (Vector2.Distance((Vector2) transform.position, playerRB.position) / gravityRange));
        
        //strength = Mathf.Min(0, strength);

        Vector2 dir = (playerRB.position - (Vector2) transform.position).normalized;

        playerRB.AddForce(dir * strength);
        
        Debug.DrawLine(playerRB.position, playerRB.position + dir * 100);
    }
}
