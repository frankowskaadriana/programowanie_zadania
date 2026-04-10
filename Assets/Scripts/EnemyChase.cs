using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("AI")]
    public float updateRate = 0.2f;

    private NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        InvokeRepeating(nameof(UpdatePath), 0f, updateRate);
    }

    // Update is called once per frame
    void UpdatePath()
    {
        if (player != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }
}