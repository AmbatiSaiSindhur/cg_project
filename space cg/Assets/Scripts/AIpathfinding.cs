using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform targetPlayer1; // Reference to Player 1
    public Transform targetPlayer2; // Reference to Player 2
    private NavMeshAgent agent;    // Reference to the NavMeshAgent

    void Start()
    {
        // Initialize NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }
    }

    void Update()
    {
        // Ensure both players are active and find the closest one
        if (targetPlayer1 != null && targetPlayer2 != null)
        {
            float distanceToPlayer1 = Vector3.Distance(transform.position, targetPlayer1.position);
            float distanceToPlayer2 = Vector3.Distance(transform.position, targetPlayer2.position);

            // Set the closer player as the target
            Transform closestPlayer = distanceToPlayer1 <= distanceToPlayer2 ? targetPlayer1 : targetPlayer2;
            agent.SetDestination(closestPlayer.position);
        }
        else if (targetPlayer1 != null) // Fallback to Player 1
        {
            agent.SetDestination(targetPlayer1.position);
        }
        else if (targetPlayer2 != null) // Fallback to Player 2
        {
            agent.SetDestination(targetPlayer2.position);
        }
        else
        {
            Debug.LogWarning("Both players are missing! No target to follow.");
        }
    }
}
