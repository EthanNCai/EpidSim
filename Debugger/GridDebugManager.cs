using System.Collections.Generic;
using UnityEngine;

public class GridDebugManager : MonoBehaviour
{
    public GameObject rootOfTheAll;
    List<GameObject> gridDebugRoots = new List<GameObject>();
    GameObject nowSelected = null;
    public int nowSelectedRoot = -1;

    private int uidCounter = 0;


    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     DeactivateAll();
        // }
        // else if (Input.GetKeyDown(KeyCode.K))
        // {
        //     SelectPrevious();
        // }
        // else if (Input.GetKeyDown(KeyCode.L))
        // {
        //     SelectNext();
        // }
    }

    public GameObject GetListedRoot(string debugName)
    {
        GameObject newRoot = new GameObject(debugName + "_DEBUG" + $"_DBGID_{uidCounter.ToString()}");
        newRoot.SetActive(false);
        newRoot.transform.parent = rootOfTheAll.transform;
        gridDebugRoots.Add(newRoot);
        uidCounter ++;
        return newRoot;
    }

    void DeactivateAll()
    {
        foreach (var root in gridDebugRoots){
            root.SetActive(false);
        }
        nowSelected = null;
        nowSelectedRoot = -1;
    }

    void SelectPrevious()
    {
        if (gridDebugRoots.Count == 0) return;
        nowSelectedRoot = (nowSelectedRoot - 1 + gridDebugRoots.Count) % gridDebugRoots.Count;
        ActivateCurrent();
    }

    void SelectNext()
    {
        if (gridDebugRoots.Count == 0) return;
        nowSelectedRoot = (nowSelectedRoot + 1) % gridDebugRoots.Count;
        ActivateCurrent();
    }

    void ActivateCurrent()
    {
        DeactivateAll();
        nowSelected = gridDebugRoots[nowSelectedRoot];
        nowSelected.SetActive(true);
        Debug.Log("Current Active Root: " + nowSelected.name);
    }
}