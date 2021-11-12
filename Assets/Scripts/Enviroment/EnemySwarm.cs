using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarm : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D[] enemyBodies;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private float followForce = 30;
    
    private void FixedUpdate()
    {
        foreach (Rigidbody2D rb in enemyBodies)
        {
            rb.AddForce(((Vector2)target.position - rb.position).normalized * followForce);
        }
    }
}
