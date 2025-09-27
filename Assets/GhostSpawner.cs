using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;

public class GhostSpawner : MonoBehaviour
{
    [Header("Ghost Spawning")]
    public GameObject prefabToSpawn;
    public float spawnTimer = 2f;
    public int maxGhosts = 5;
    public float normalOffset = 0.1f;
    public float minEdgeDistance = 0.3f;
    public MRUKAnchor.SceneLabels sceneLabels;

    [Header("Tries")]
    public int spawnTry = 100;

    private float timer;
    private List<GameObject> activeGhosts = new List<GameObject>();

    void Update()
    {
        if (!MRUK.Instance || !MRUK.Instance.IsInitialized)
            return;

        timer += Time.deltaTime;

        if (timer > spawnTimer && activeGhosts.Count < maxGhosts)
        {
            SpawnGhost();
            timer = 0f;
        }
    }

    void SpawnGhost()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        int currentTry = 0;

        while (currentTry < spawnTry)
        {
            bool found = room.GenerateRandomPositionOnSurface(
                MRUK.SurfaceType.FACING_UP,  // FLOOR only
                minEdgeDistance,
                new LabelFilter(sceneLabels),
                out Vector3 pos,
                out Vector3 norm
            );

            if (found)
            {
                Vector3 spawnPos = pos + norm * normalOffset;

                //  Snap to NavMesh
                if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    spawnPos = hit.position;

                    GameObject ghost = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                    activeGhosts.Add(ghost);

                    Ghost ghostScript = ghost.GetComponent<Ghost>();
                    if (ghostScript != null)
                    {
                        ghostScript.OnGhostDeath += OnGhostDeath;
                    }
                    return;
                }
            }
            currentTry++;
        }
    }

    void OnGhostDeath(GameObject ghost)
    {
        if (activeGhosts.Contains(ghost))
        {
            activeGhosts.Remove(ghost);
        }
    }
}

