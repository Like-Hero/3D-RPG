using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyStates enemyStates;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        SwitchStates();
    }
    private void SwitchStates()
    {
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:
                break;
            case EnemyStates.DEAD:
                break;
        }
    }
}
