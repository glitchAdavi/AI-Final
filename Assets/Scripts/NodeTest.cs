using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NodeTest : MonoBehaviour
{
    public PathfindingManager _pm;

    List<Node> path = new List<Node>();
    List<Vector3> v3Path = new List<Vector3>();

    public Transform a;
    public Transform b;

    public bool updating = false;

    // Update is called once per frame
    void Update()
    {
        if (updating)
        {
            path = _pm.GetPath(a, b.position);
            v3Path = ConvertPath();
        }
    }

    public List<Vector3> ConvertPath()
    {
        List<Vector3> final = new List<Vector3>();

        final.Add(a.transform.position);

        for (int i = 1; i < path.Count - 1; i++)
        {
            final.Add(path[i].transform.position);
        }

        final.Add(b.transform.position);

        return final;
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 node in v3Path)
        {
            Gizmos.DrawWireSphere(node + new Vector3(0, 1, 0), 0.6f);
        }
        for (int i = 0; i < v3Path.Count; i++)
        {
            if (i + 1 < v3Path.Count) Gizmos.DrawLine(v3Path[i], v3Path[i + 1]);
        }
    }
}
