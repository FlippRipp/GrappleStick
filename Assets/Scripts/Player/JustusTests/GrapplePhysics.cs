using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePhysics : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    [SerializeField]
    private GameObject chainLinkPrefab;

    [SerializeField]
    private int linkCount = 10;

    private GameObject[] objs = null;

    private HingeJoint2D playerHingeJoint = null;

    Vector3 moveDir;

    private bool inited = false;

    void Init()
    {
        if (!inited)
        {
            inited = true;

            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0;

            SpawnChainLinks();

            transform.DetachChildren();
        }

    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        //rigidBody.AddForce(Vector3.up * 10, ForceMode2D.Impulse);
        rigidBody.velocity = moveDir * 15;
    }

    private void SpawnChainLinks()
    {
        Vector2 localPos = new Vector2(0, -0.85f);

        objs = new GameObject[linkCount + 1];

        objs[0] = gameObject;

        for (int i = 1; i < objs.Length; i++)
        {
            objs[i] = Instantiate(chainLinkPrefab, transform.TransformPoint(localPos), Quaternion.identity);
            HingeJoint2D joint = objs[i].GetComponent<HingeJoint2D>();
            joint.connectedBody = objs[i - 1].GetComponent<Rigidbody2D>();
            

            localPos.y -= 0.3f;
        }


    }

    private void OnEnable()
    {
        Init();

        foreach (GameObject obj in objs)
        {
            if (obj != gameObject)
                obj.SetActive(true);
        }

        PlayerMovement pm = FindObjectOfType<PlayerMovement>();

        foreach (GameObject obj in objs)
        {
            //Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), pm.GetComponent<Collider2D>());

            if (obj.GetComponent<HingeJoint2D>())
            {
                obj.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
            }
            
            obj.transform.position = pm.transform.position + (Vector3)pm.GetComponent<Rigidbody2D>().velocity * Time.deltaTime;
            obj.GetComponent<Rigidbody2D>().MovePosition(pm.transform.position);
            
        }

        for (int i = 0; i < objs.Length; i++)
        {
            Physics2D.IgnoreCollision(objs[i].GetComponent<Collider2D>(), pm.GetComponent<Collider2D>());
            for (int j = 0; j < objs.Length; j++)
            {
                Physics2D.IgnoreCollision(objs[i].GetComponent<Collider2D>(), objs[j].GetComponent<Collider2D>());
            }
        }

        Vector2 mousePosition = Vector2.zero;

        if (Camera.main) mousePosition = Camera.main.ScreenToWorldPoint(
             new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

        moveDir = (mousePosition - (Vector2)FindObjectOfType<PlayerMovement>().transform.position).normalized;

        if (!playerHingeJoint)
        {
            playerHingeJoint = pm.gameObject.AddComponent<HingeJoint2D>();
        }

        if (playerHingeJoint)
        {
            playerHingeJoint.connectedBody = objs[objs.Length - 1].GetComponent<Rigidbody2D>();

            foreach (GameObject obj in objs)
            obj.GetComponent<Rigidbody2D>().velocity = moveDir * 5;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject obj in objs)
        {
            if (obj != gameObject)
            {
                obj.SetActive(false);
            } 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (GameObject obj in objs)
        {
            if (obj != gameObject)
                obj.GetComponent<Rigidbody2D>().gravityScale = 1;
        }
    }
}
