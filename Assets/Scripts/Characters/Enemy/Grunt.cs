using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float repelForce;
    public void Repel()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
            Vector3 dir = attackTarget.transform.position - transform.position;
            dir.Normalize();
            attackTarget.GetComponent<NavMeshAgent>().velocity = repelForce * dir;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
