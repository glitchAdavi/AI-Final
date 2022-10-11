using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TilingTool : MonoBehaviour
{
    public GameObject objectToTile;
    public Vector3 objectSize;

    public Vector3 startPos = Vector3.zero;

    public int xNum = 1;
    public int yNum = 1;
    public int zNum = 1;

    public float xSpace = 0;
    public float ySpace = 0;
    public float zSpace = 0;

    [MenuItem("GameObject/Other/TilingToolObject")]
    [MenuItem("Tools/PlacementTools/TilingTool")]
    public static void TilingToolObject()
    {
        GameObject result = new GameObject("TilingTool");
        result.transform.position = Vector3.zero;
        result.AddComponent<TilingTool>();
        Selection.activeObject = result;
    }

    public void PlaceObjects()
    {
        if (CheckForChildren()) RemoveObjects();

        bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(objectToTile);

        Debug.Log("Placing...");
        int totalObjects = 0;

        Renderer renderer;
        if (objectToTile.TryGetComponent<Renderer>(out renderer)) objectSize = renderer.bounds.size;
        else objectSize = Vector3.zero;

        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                for (int z = 0; z < zNum; z++)
                {
                    InstantiateFunction(isPrefab, CalculatePosition(x, y, z));
                    totalObjects++;
                }
            }
        }

        Debug.Log("Placed " + totalObjects + " objects.");
    }

    public Vector3 CalculatePosition(int x, int y, int z)
    {
        Vector3 result = Vector3.zero;

        result.x = startPos.x + ((objectSize.x) * x) + (xSpace * (x - 1));
        result.y = startPos.y + ((objectSize.y) * y) + (ySpace * (y - 1));
        result.z = startPos.z + ((objectSize.z) * z) + (zSpace * (z - 1));

        return result;
    }

    public void InstantiateFunction(bool isPrefab, Vector3 pos)
    {
        if (isPrefab)
        {
            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(objectToTile);
            inst.gameObject.transform.position = pos;
            inst.gameObject.transform.rotation = Quaternion.identity;
            inst.gameObject.transform.parent = transform;
        }
        else Instantiate(objectToTile, pos, Quaternion.identity, transform);
    }

    public void RemoveObjects()
    {
        Debug.Log("Deleting...");
        int totalObjects = 0;

        List<Transform> allChildren = new List<Transform>();
        foreach (Transform child in transform)
        {
            allChildren.Add(child);
            totalObjects++;
        }
        foreach (Transform child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }
        Debug.Log("Deleted " + totalObjects + " objects.");
    }

    public void ConfirmObjects()
    {
        Debug.Log("Moving...");
        int totalObjects = 0;

        GameObject result = new GameObject("TiledObjects");
        List<Transform> allChildren = new List<Transform>();
        foreach (Transform child in transform)
        {
            allChildren.Add(child);
            totalObjects++;
        }
        foreach (Transform child in allChildren)
        {
            child.parent = result.transform;
        }
        Debug.Log("Moved " + totalObjects + " objects.");
    }

    private bool CheckForChildren()
    {
        return transform.TryGetComponent<Transform>(out Transform aux);
    }
}
