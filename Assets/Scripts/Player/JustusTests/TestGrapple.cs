using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrapple : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    private HingeJoint2D myJoint;

    private GrapplingState grapplingState = GrapplingState.Inactive;

    private List<TestGrappleChainLink> links = new List<TestGrappleChainLink>();

    [SerializeField]
    private LayerMask grappleLayers;

    [SerializeField, Min(0), Tooltip("How long should the grappling hook rope length be?")]
    private float grapplingHookRopeLength = 10f;

    [SerializeField, Min(0), Tooltip("How many joints should exist along the rope of the grappling hook? The more, the less rigid.")]
    private int ropeJointCount = 20;

    [SerializeField, Min(0)]
    private float grappleSpeed = 5f;

    [Space]

    [SerializeField]
    private TestGrappleChainLink chainLinkPrefab;

    public void LaunchGrapple(Rigidbody2D owner)
    {
        links[links.Count - 1].GetComponent<HingeJoint2D>().connectedBody = owner;

        transform.position = owner.transform.position;

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (!chainLinkPrefab)
        {
            Destroy(gameObject);
            Debug.LogError("Grappling Hook: No chain link prefab could be found.");
        }

        rigidBody = GetComponent<Rigidbody2D>();
        myJoint = GetComponent<HingeJoint2D>();

        //float jointDistance = (float)grapplingHookRopeLength / ropeJointCount;

        // Create the chain links
        for (int i = 0; i < ropeJointCount; i++)
        {
            links.Add(Instantiate(chainLinkPrefab, transform.position, transform.rotation));
        }

        for (int i = ropeJointCount - 1; i > 0; i--)
        {
            links[i].GetComponent<HingeJoint2D>().connectedBody = links[i - 1].GetComponent<Rigidbody2D>();
            links[i].GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
            links[i].GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -1);
        }

        links[0].GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
        links[0].GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
        links[0].GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -1);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Vector2 mousePosition = Vector2.zero;

        if (Camera.main) mousePosition = Camera.main.ScreenToWorldPoint(
             new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

        Vector2 moveDirection = (mousePosition - (Vector2)transform.position).normalized;

        rigidBody.bodyType = RigidbodyType2D.Dynamic;

        grapplingState = GrapplingState.Shooting;

        rigidBody.velocity = moveDirection * grappleSpeed;

        Vector3 ownerPos = transform.position;

        //Vector3 lerp;
        for (int i = 0; i < links.Count; i++)
        {
            links[i].gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        
    }

    private void OnDisable()
    {
        grapplingState = GrapplingState.Inactive;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (grappleLayers == (grappleLayers | (1 << collision.gameObject.layer)))
        {
            
            grapplingState = GrapplingState.LatchedOn;

            rigidBody.bodyType = RigidbodyType2D.Static;

            rigidBody.velocity = Vector2.zero;

            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public enum GrapplingState
    {
        Inactive,
        Shooting,
        LatchedOn
    }
}
