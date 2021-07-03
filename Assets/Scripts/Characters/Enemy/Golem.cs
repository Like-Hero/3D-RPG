using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float repelForce;
    private void Repel()
    {
        if(attackTarget != null && TargetInSkillRange())
        {
            transform.LookAt(attackTarget.transform);
            Vector3 dir = attackTarget.transform.position - transform.position;
            NavMeshAgent playerAgent = attackTarget.GetComponent<NavMeshAgent>();
            playerAgent.velocity = repelForce * dir;
            //攻击玩家
            Hit();
            //产生眩晕
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
        else
        {
            print(name + "Skill攻击失败");
        }
    }
}
