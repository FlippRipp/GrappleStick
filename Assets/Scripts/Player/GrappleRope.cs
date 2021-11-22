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

    private GameObject GetLastNodeObject() => ropeNodes.Count > 0 ? ropeNodes[ropeNodes.Count - 1].startTrans.gameObject : null;

    private LineRenderer lineRenderer;

    private float minDistance = .25f;

    private Dictionary<Vector2, int> wrapPointsLookup = new Dictionary<Vector2, int>();

    private RaycastHit2D[] hits = new RaycastHit2D[32];

    private Vector2[] vertexCache = new Vector2[128];

    private Vector2 anchorPos;
    public Vector2 AnchorPosition => anchorPos;

    void Start()
    {
        ropeNodes.Add(new RopeNode(grappleHook.transform, player.transform));

        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        HandleGrapple();
        HandleLineRenderer();
    }

    private void HandleGrapple()
    {
        if (!player || !grappleHook)
            return;

        // The node the player is connected to in the beginning of this method.
        // Is always the last index.
        RopeNode playerNode = ropeNodes[ropeNodes.Count - 1];
        
        Vector2 start = playerNode.endTrans.position;
        Vector2 end = playerNode.startTrans.position;

        Vector2 dir = end - start;

        int hitCount = Physics2D.RaycastNonAlloc(start, dir.normalized, hits, dir.magnitude - 0.1f, touchLayer);
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit && hit.collider)
            {
                if (hit.collider.CompareTag("Platform"))
                {
                    if (hit.collider.transform.parent.parent.GetComponent<MovingPlatform>())
                        continue;
                }

                Vector2 closestPoint = GetNearestVertex(hit, out int path, out int vertexIndex);

                if (Vector2.Distance(end, closestPoint) > minDistance)
                {
                    GameObject newObject = new GameObject("Node", typeof(Node));
                    newObject.transform.position = closestPoint;

                    Vector2 normal = GetVertexNormal(hit.collider as CompositeCollider2D, path, vertexIndex);

                    newObject.GetComponent<Node>().normalDirection = normal;

                    RopeNode newClosest = new RopeNode(newObject.transform, playerNode.endTrans);
                    ropeNodes.Add(newClosest);

                    playerNode.endTrans = newClosest.startTrans;

                    anchorPos = newObject.transform.position;

                    player.OnGrappleHit(anchorPos);
                    player.grappleJoint.transform.position = anchorPos;
                }
            }
        }

        // Angle checking
        if (ropeNodes.Count >= 2 && GetLastNodeObject())
        {
            Vector3 prevNodeDelta = 
                ropeNodes[ropeNodes.Count - 2].endTrans.position - ropeNodes[ropeNodes.Count - 2].startTrans.position;

            Vector3 perpPrevNodeDelta = Vector2.Perpendicular(prevNodeDelta);

            Vector3 playerNodeDelta = 
                ropeNodes[ropeNodes.Count - 1].endTrans.position - ropeNodes[ropeNodes.Count - 1].startTrans.position;

            if (Vector3.Dot(perpPrevNodeDelta, GetLastNodeObject().GetComponent<Node>().normalDirection) < 0)
                perpPrevNodeDelta *= -1f;

            // Allow for margin of error since the dot product can end up in
            // the positive despite the rope being bent.
            if (Vector3.Dot(playerNodeDelta.normalized, perpPrevNodeDelta.normalized) < .125f)
            {
                return;
            }
        }
        
        if (ropeNodes.Count < 2)
            return;

        // Unwrapping
        RopeNode prevPrevNode = ropeNodes[ropeNodes.Count - 2];

        dir = (Vector2)prevPrevNode.endTrans.position - start;

        hitCount = Physics2D.RaycastNonAlloc(start, dir.normalized, hits, dir.magnitude - 0.1f, touchLayer);
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit && hit.collider)
            {
                if (hit.collider.CompareTag("Platform"))
                {
                    if (hit.collider.transform.parent.parent.GetComponent<MovingPlatform>())
                        continue;
                }

                return;
            }
        }

        RopeNode rn = playerNode;
        ropeNodes.Remove(rn);

        if (rn.startTrans.GetComponent<Node>())
            Destroy(rn.startTrans.gameObject);

        prevPrevNode.endTrans = player.transform;

        anchorPos = prevPrevNode.startTrans.position;

        player.OnGrappleHit(anchorPos);
        player.grappleJoint.transform.position = anchorPos;
    }

    private void HandleLineRenderer()
    {
        if (lineRenderer.positionCount != ropeNodes.Count + 1)
        {
            lineRenderer.positionCount = ropeNodes.Count + 1;
        }

        for (int i = 0; i < lineRenderer.positionCount && i < ropeNodes.Count; i++)
        {
            lineRenderer.SetPosition(i, ropeNodes[i].startTrans.position);
            lineRenderer.SetPosition(i + 1, ropeNodes[i].endTrans.position);
        }
    }

    private Vector2 GetVertexNormal(CompositeCollider2D cc2d, int path, int vertexIndex)
    {
        Vector2 result = Vector2.zero;
        int vertices = cc2d.GetPath(path, vertexCache);

        Vector2 thisVertex = vertexCache[vertexIndex];

        Vector2 prevVertex = vertexIndex == 0 ? vertexCache[vertices - 1] : vertexCache[vertexIndex - 1];

        Vector2 nextVertex = vertexIndex == vertices - 1 ? vertexCache[0] : vertexCache[vertexIndex + 1];

        Vector2 prevToThis = (thisVertex - prevVertex).normalized;

        Vector2 nextToThis = (thisVertex - nextVertex).normalized;

        result = (prevToThis + nextToThis).normalized;

        return result;
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
            Gizmos.DrawSphere(ropeNodes[i].endTrans.position, 0.1f);

            if (ropeNodes[i].startTrans && ropeNodes[i].endTrans)
                Debug.DrawLine(ropeNodes[i].startTrans.position, ropeNodes[i].endTrans.position, Color.red);
        }

        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawSphere(anchorPos, 0.1f);
    }

    private Vector2 GetNearestVertex(RaycastHit2D hit, out int path, out int vertexIndex)
    {
        CompositeCollider2D cc2d = hit.collider as CompositeCollider2D;

        path = -1;
        vertexIndex = -1;

        if (!cc2d)
            return hit.point;

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
                    vertexIndex = ii;
                    path = i;
                }
            }
        }

        return point;
    }

    private void OnDestroy()
    {
        foreach (RopeNode node in ropeNodes)
        {
            if (node.startTrans.GetComponent<Node>())
                Destroy(node.startTrans.gameObject);
        }
    }

}

[System.Serializable]
public class RopeNode
{
    public RopeNode(Transform startTrans, Transform endTrans)
    {
        this.startTrans = startTrans;
        this.endTrans = endTrans;
    }

    public Transform startTrans;
    public Transform endTrans;
}

