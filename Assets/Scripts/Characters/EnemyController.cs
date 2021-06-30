﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    ATTACK,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterBaseStats))]
[RequireComponent(typeof(AttackStats))]//非攻击敌人可以不要
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    #region Read from var
    private AttackStats attackStats;
    private CharacterBaseStats characterBaseStats;

    private NavMeshAgent agent;
    private Collider collider;
    private EnemyStates enemyState;
    
    [Header("Base Setting")]
    public float sightRange;//视野范围
    public bool isGuard;
    public float resumeStatsTime;
    private float resumeStatsLastTime;

    [Header("Patrol State")]
    public float patrolRange;//巡逻范围


    private Vector3 originPos;
    private Quaternion originRotation;
    private Vector3 patrolTargetPoint;

    private float speed;
    private float attackLastCD;

    private GameObject attackTarget;
    private Transform chasePoint;
    private Animator anim;

    private bool isWalk;
    private bool isFollow;
    private bool isAttack;
    private bool isFoundPlayer;
    private bool isDead;
    private bool isPlayerDead;
    #endregion
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        attackStats = GetComponent<AttackStats>();
        characterBaseStats = GetComponent<CharacterBaseStats>();
        collider = GetComponent<Collider>();
        speed = agent.speed;
        resumeStatsLastTime = resumeStatsTime;
        originPos = transform.position;
        originRotation = transform.rotation;
    }
    private void Start()
    {
        if (isGuard)
        {
            enemyState = EnemyStates.GUARD;
        }
        else
        {
            enemyState = EnemyStates.PATROL;
            patrolTargetPoint = GetNewPatrolTargetPoint();
        }
        GameManager.Ins.AddObserver(this);
    }
    //private void OnEnable()
    //{
    //    GameManager.Ins.AddObserver(this);
    //}
    //后于OnDestrosy
    private void OnDisable()
    {
        if (GameManager.IsInitialized)
        {
            GameManager.Ins.RemoveObserver(this);
        }
    }
    private void Update()
    {
        if (isPlayerDead)
        {
            return;
        }
        if (attackLastCD > 0)
        {
            attackLastCD -= Time.deltaTime;
        }
        if (characterBaseStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        JudgeState();
        SwitchStates();
        SwitchAnimation();
    }
    private void JudgeState()
    {
        if (isDead)
        {
            enemyState = EnemyStates.DEAD;
            return;
        }
        //chasePoint只要找到就会一直更新
        isFoundPlayer = FoundPlayer();
        //如果找到攻击目标，则追踪
        if (isFoundPlayer)
        {
            enemyState = EnemyStates.CHASE;
            //在攻击范围内则攻击
            if (Vector3.Distance(chasePoint.position, transform.position) < attackStats.AttackRange)
            {
                attackTarget = chasePoint.gameObject;
                enemyState = EnemyStates.ATTACK;
            }
            //不在则丢失攻击目标，但是可以继续追踪（因为可能敌人在攻击玩家的时候，玩家溜了，所以可能会找不到目标，实现拉扯）
            else
            {
                attackTarget = null;
            }
        }
        else
        {
            //如果之前的状态不是站桩状态并且不是巡逻状态就恢复之前的状态，不然会一直是GUARD或者PATROL状态
            if (enemyState != EnemyStates.GUARD && enemyState != EnemyStates.PATROL)
            {
                ResumeState();
            }
        }
    }
    private void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("FoundPlayer", isFoundPlayer);
        anim.SetBool("Critical", attackStats.IsCritical);
        anim.SetBool("Dead", isDead);
        if (isAttack)
        {
            anim.SetTrigger("Attack");
        }
    }
    private void SwitchStates()
    {
        switch (enemyState)
        {
            case EnemyStates.GUARD:
                Guard();
                break;
            case EnemyStates.PATROL:
                Patrol();
                break;
            case EnemyStates.CHASE:
                Chase();
                break;
            case EnemyStates.ATTACK:
                Attack();
                break;
            case EnemyStates.DEAD:
                Dead();
                break;
        }
    }
    public void EventDead()
    {
        Destroy(gameObject, 1.0f);
    }
    private void Dead()
    {
        agent.enabled = false;
        collider.enabled = false;
    }
    private void Chase()
    {
        //追Player
        agent.speed = speed;
        isWalk = false;
        isFollow = true;
        agent.destination = chasePoint.position;
    }
    private void Attack()
    {
        //TODO:主角扣血，攻击CD
        isFollow = false;
        agent.velocity = Vector3.zero;
        transform.LookAt(attackTarget.transform);
        if (attackLastCD > 0)
        {
            isAttack = false;
        }
        else
        {
            isAttack = true;
            //先判断暴击，在动画的过程中进行Hurt伤害计算
            attackStats.IsCritical = Random.value < attackStats.CriticalRate;
            attackLastCD = attackStats.CoolDown;
        }
    }
    private void Guard()
    {   
        if (Vector3.SqrMagnitude(originPos - transform.position) <= agent.stoppingDistance)
        {
            isWalk = false;
            transform.rotation = Quaternion.Lerp(transform.rotation, originRotation, 0.02f);
        }
        else
        {
            if (DelayTimer())
            {
                isWalk = true;
                agent.destination = originPos;
            }
            
        }
    }
    private void Patrol()
    {
        agent.speed = speed * 0.5f;
        if (Vector3.Distance(transform.position, patrolTargetPoint) <= agent.stoppingDistance)
        {
            isWalk = false;
            if (DelayTimer())
            {
                patrolTargetPoint = GetNewPatrolTargetPoint();
            }
        }
        else
        {
            isWalk = true;
            agent.destination = patrolTargetPoint;
        }
    }
    public void Hit()
    {
        if (attackTarget != null)
        {
            CharacterBaseStats targetBaseStats = attackTarget.GetComponent<CharacterBaseStats>();
            targetBaseStats.Hurt(attackStats);
        }
        else
        {
            print(name + "攻击Miss");
        }
    }
    private void BreakAttack()
    {
        anim.Play("IdleBattle");
        isAttack = false;
        agent.isStopped = false;
        attackLastCD = attackStats.CoolDown;//打断攻击需要恢复CD
    }
    public void AnimationFinishedCanMove()
    {
        if(agent != null && agent.enabled)
        {
            
            agent.destination = transform.position;
            agent.isStopped = false;
        }
    }
    public void AnimationStartedCanNotMove()
    {
        agent.isStopped = true;
    }
    private void ResumeState()
    {
        patrolTargetPoint = transform.position;
        chasePoint = transform;
        agent.destination = transform.position;
        isFollow = false;
        isAttack = false;
        resumeStatsLastTime = resumeStatsTime;
        if (isGuard)
        {
            enemyState = EnemyStates.GUARD;
        }
        else
        {
            enemyState = EnemyStates.PATROL;
        }
    }
    private bool DelayTimer()
    {
        if (resumeStatsLastTime <= 0)
        {
            resumeStatsLastTime = resumeStatsTime;
            return true;
        }
        else
        {
            resumeStatsLastTime -= Time.deltaTime;
            return false;
        }
    }
    private bool FoundPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                chasePoint = collider.transform;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    private Vector3 GetNewPatrolTargetPoint()
    {
        float x = Random.Range(-patrolRange, patrolRange);
        float z = Random.Range(-patrolRange, patrolRange);
        Vector3 tempPos = new Vector3(originPos.x + x, transform.position.y, originPos.z + z);
        NavMeshHit hit;
        tempPos = NavMesh.SamplePosition(tempPos, out hit, patrolRange, 1) ? hit.position : transform.position;
        return tempPos;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void EndNotify()
    {
        //停止其他行为
        //播放对应动画
        //禁用agent
        anim.SetTrigger("Win");
        attackTarget = null;
        isFollow = false;
        isPlayerDead = true;
    }
}
