using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    public PlayerMovement player;
    public GameObject grappleHook;

    public LayerMask touchLayer;

    public List<RopeNode> ropeNodes = new List<RopeNode>();

    private GameObject lastNode;
    private GameObject grappleNode;

    private bool distanceSet = false;

    private float minDistance = .25f;

    private Dictionary<Vector2, int> wrapPointsLookup = new Dictionary<Vector2, int>();

    private RaycastHit2D[] hits = new RaycastHit2D[32];

    private Vector2[] vertexCache = new Vector2[128];

    private Vector2 colPathCenter;

    private Vector2 anchorPos;
    public Vector2 AnchorPosition => anchorPos;

    void Start()
    {
        ropeNodes.Add(new RopeNode(grappleHook.transform, player.transform));
    }

    void Update()
    {
        HandleGrapple(); 
    }

    private void HandleGrapple()
    {
        if (!player || !grappleHook)
            return;

        // Beginning, as in beginning of this method.
        RopeNode closestAtBeginning = ropeNodes[ropeNodes.Count - 1];
        
        Vector2 start = closestAtBeginning.destinationTrans.position;
        Vector2 end = closestAtBeginning.positionTrans.position;

        Vector2 dir = end - start;

        // Wrapping process
        int hitCount = Physics2D.RaycastNonAlloc(start, dir.normalized, hits, dir.magnitude - 0.1f, touchLayer);
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit && hit.collider)
            {
                print("hitta nemo");
                Vector2 closestPoint = GetNearestVertex(hit, out int path);

                if (Vector2.Distance(end, closestPoint) > minDistance)
                {
                    GameObject newObject = new GameObject("Node", typeof(Node));
                    newObject.transform.position = closestPoint;

                    colPathCenter = GetPathCenter(hit.collider as CompositeCollider2D, path);

                    newObject.GetComponent<Node>().normalDirection = (closestPoint - colPathCenter).normalized;
                    //newObject.GetComponent<Node>().normalDirection = hit.normal;

                    RopeNode newClosest = new RopeNode(newObject.transform, closestAtBeginning.destinationTrans);
                    ropeNodes.Add(newClosest);

                    closestAtBeginning.destinationTrans = newClosest.positionTrans;

                    anchorPos = newObject.transform.position;

                    lastNode = newObject;
                }
            }
        }

        // Angle checking
        if (ropeNodes.Count >= 2 && lastNode)
        {
            Vector3 playerPos = player.transform.position;

            Vector3 currentNodePos = ropeNodes[ropeNodes.Count - 1].positionTrans.position;

            Vector3 prevNodePos = ropeNodes[ropeNodes.Count - 2].positionTrans.position;

            Vector3 nodePlane = currentNodePos - prevNodePos;

            Vector3 rhs = playerPos - currentNodePos;

            Vector3 lhs = new Vector3(-nodePlane.y, nodePlane.x, 0) / Mathf.Sqrt(nodePlane.x * nodePlane.x + nodePlane.y * nodePlane.y);
            
            if (Vector3.Dot(lhs, lastNode.GetComponent<Node>().normalDirection) < 0)
                lhs *= -1f;

            print(Vector3.Dot(lhs, rhs));

            if (Vector3.Dot(lhs, rhs) < 0)
            {
                return;
            }
        }
        
        if (ropeNodes.Count < 2)
            return;

        // Unwrapping
        RopeNode prevPrevNode = ropeNodes[ropeNodes.Count - 2];

        dir = (Vector2)prevPrevNode.destinationTrans.position - start;

        hitCount = Physics2D.RaycastNonAlloc(start, dir.normalized, hits, dir.magnitude - 0.1f, touchLayer);
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit && hit.collider)
            {
                return;
            }
        }

        RopeNode rn = closestAtBeginning;
        ropeNodes.Remove(rn);

        if (rn.positionTrans.GetComponent<Node>())
            Destroy(rn.positionTrans.gameObject);

        prevPrevNode.destinationTrans = player.transform;

        lastNode = prevPrevNode.positionTrans.gameObject;
    }

    private Vector2 GetPathCenter(CompositeCollider2D cc2d, int path)
    {
        Vector2 result = Vector2.zero;
        int vertices = cc2d.GetPath(path, vertexCache);

        for (int i = 0; i < vertices; i++)
        {
            result += vertexCache[i];
        }

        if (vertices > 0)
            result /= vertices;

        return result;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < ropeNodes.Count; i++)
        {
            Gizmos.DrawSphere(ropeNodes[i].destinationTrans.position, 0.1f);

            if (ropeNodes[i].positionTrans && ropeNodes[i].destinationTrans)
                Debug.DrawLine(ropeNodes[i].positionTrans.position, ropeNodes[i].destinationTrans.position, Color.red);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(anchorPos, 0.1f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(colPathCenter, 0.1f);
    }

    private Vector2 GetNearestVertex(RaycastHit2D hit, out int path)
    {
        CompositeCollider2D cc2d = hit.collider as CompositeCollider2D;

        path = -1;

        if (!cc2d)
        {
            
            return hit.point;
        }

        Vector2 point = Vector2.zero;

        float currentLength = Mathf.Infinity;

        for (int i = 0; i < cc2d.pathCount; i++)
        {
            
            for (int ii = 0; ii < cc2d.GetPath(i, vertexCache); ii++)
            {
                float thisLength = Vector2.SqrMagnitude((Vector2)hit.point - vertexCache[ii]);
                if (thisLength < currentLength)
                {
                    currentLength = thisLength;
                    point = vertexCache[ii];
                    path = i;
                }
            }
        }

        return point;
    }

}

[System.Serializable]
public class RopeNode
{
    public RopeNode(Transform positionTrans, Transform destinationTrans)
    {
        this.positionTrans = positionTrans;
        this.destinationTrans = destinationTrans;
    }

    public Transform positionTrans;
    public Transform destinationTrans;
}

