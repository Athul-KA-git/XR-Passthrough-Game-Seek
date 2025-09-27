using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public float eatDistance = 0.3f;

    public Animator animator;
    public NavMeshAgent agent;
    public float speed = 1;

    //  Event for notifying death
    public event Action<GameObject> OnGhostDeath;

    void Start()
    {

    }

    public GameObject GetClosestOrb()
    {
        GameObject Closest = null;
        float minDistance = Mathf.Infinity;

        List<GameObject> orbs = OrbSpwaner.instance.spawnedOrbs;

        foreach (var item in orbs)
        {
            Vector3 ghostPosition = transform.position;
            ghostPosition.y = 0;
            Vector3 orbPosition = item.transform.position;
            orbPosition.y = 0;

            float distance = Vector3.Distance(ghostPosition, orbPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                Closest = item;
            }
        }

        if (Closest != null && minDistance < eatDistance)
        {
            OrbSpwaner.instance.DestroyOrb(Closest);
        }

        return Closest;
    }

    void Update()
    {
        if (!agent.enabled)
            return;

        GameObject closest = GetClosestOrb();

        if (closest != null)
        {
            Vector3 targetPosition = closest.transform.position;
            agent.SetDestination(targetPosition);
            agent.speed = speed;
        }
    }

    public void Kill()
    {
        agent.enabled = false;
        animator.SetTrigger("Death");

        //  Fire event so spawner knows this ghost died
        OnGhostDeath?.Invoke(gameObject);
    }

    //  Called via Animation Event at end of death animation
    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //  Safety: make sure event fires even if destroyed directly
        OnGhostDeath?.Invoke(gameObject);
    }
}
