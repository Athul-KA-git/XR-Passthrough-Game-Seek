using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Meta.XR.MRUtilityKit;

public class RuntimeNavmeshBuilder : MonoBehaviour
{

    private NavMeshSurface navmeshSurface;

    // Start is called before the first frame update
    void Start()
    {
        navmeshSurface = GetComponent<NavMeshSurface>();
        MRUK.Instance.RegisterSceneLoadedCallback(BuildNavMesh);

    }

   public void BuildNavMesh()
    {
        StartCoroutine(BuildNavMeshCoroutine());

    }
    public IEnumerator BuildNavMeshCoroutine()
    {
        yield return new WaitForEndOfFrame();   
        navmeshSurface.BuildNavMesh();
    }   
}
