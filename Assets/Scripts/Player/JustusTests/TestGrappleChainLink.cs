using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TestGrappleChainLink : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    [SerializeField]
    private HingeJoint2D grappleDistanceJoint;

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        grappleDistanceJoint = GetComponent<HingeJoint2D>();
    }
}
