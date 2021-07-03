using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region Read from Var
    private CharacterBaseStats characterStats;
    private AttackStats attackStats;

    private NavMeshAgent agent;
    private Animator anim;

    private float attackLastCD;
    private bool isAttacking;//角色是否正在攻击
    private bool isDead;

    private Coroutine moveCoroutine;
    private Coroutine attackCoroutine;

    private GameObject attackTarget;

    public float maxSpeed;
    public float normalSpeed;
    public float minSpeed;

    #endregion
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterBaseStats>();
        attackStats = GetComponent<AttackStats>();
        attackLastCD = 0;
    }
    private void Start()
    {
        MouseManager.Ins.OnMouseClicked += MoveToTarget;

        MouseManager.Ins.OnEnemyClicked += EventAttack;

        GameManager.Ins.RigisterPlayer(characterStats);
    }
    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        if (isDead)
        {
            Dead();
            return;
        }
        AlterSpeed();
        SwitchAnimation();
        //如果攻击CD大于0，那么就一直减CD
        if (attackLastCD > 0)
        {
            attackLastCD -= Time.deltaTime;
        }
    }
    private void AlterSpeed()
    {
        float velocity = Input.GetAxisRaw("Alter Speed");
        if(velocity == 1)
        {
            agent.speed = Mathf.Min(Mathf.Lerp(agent.speed, maxSpeed, 0.02f), maxSpeed);
        }else if(velocity == -1)
        {
            agent.speed = Mathf.Max(Mathf.Lerp(agent.speed, minSpeed, 0.02f), minSpeed);
        }
        else
        {
            agent.speed = Mathf.Lerp(agent.speed, normalSpeed, 0.02f);
        }
    }

    private void Dead()
    {
        anim.SetTrigger("Dead");
        agent.enabled = false;
        GameManager.Ins.NotifyObservers();
    }

    private void SwitchAnimation()
    {
        if (Input.GetKeyDown(KeyCode.S))//测试用，按住S停下
        {
            agent.destination = transform.position;
            anim.Play("Blend Tree");
            if (isAttacking)
            {
                BreakAttack();
            }
        }
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }
    private void MoveToTarget(Vector3 target)
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        anim.Play("Blend Tree");
        agent.isStopped = false;
        //如果正在寻找敌人，则打断寻找敌人的协程
        //if (moveCoroutine != null)
        //{
            //StopCoroutine(moveCoroutine);
            StopAllCoroutines();
        //}
        //如果正在攻击，那么需要打断攻击
        if (isAttacking)
        {
            BreakAttack();
        }
        agent.destination = target;
    }
    private void EventAttack(GameObject enemy)
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        if (enemy != null)
        {
            attackTarget = enemy;
            //先移动到敌人位置
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
            moveCoroutine = StartCoroutine(MoveToEnemy());
        }
    }
    private IEnumerator MoveToEnemy()
    {
        agent.isStopped = false;
        while (Vector3.Distance(transform.position, attackTarget.transform.position) > attackStats.AttackRange)
        {
            agent.destination = attackTarget.transform.position;
            //print(Vector3.Distance(transform.position, attackTarget.transform.position));
            yield return null;
        }
        //角色走到敌人之后需要看向敌人才攻击，不然视觉上可能会攻击空气
        transform.LookAt(attackTarget.transform);
        //移动到敌人面前之后：如果没有在攻击，则攻击
        //如果没有在攻击，并且攻击CD好了
        if(!isAttacking && attackLastCD <= 0)
        {
            attackCoroutine = StartCoroutine(AttackEnemy());
        }
        else
        {   
            agent.isStopped = true;
        }
    }
    private IEnumerator AttackEnemy()
    {
        anim.SetTrigger("Attack");
        //判断暴击
        attackStats.IsCritical = Random.value < attackStats.CriticalRate;
        //因为玩家什么时候都有可能攻击，所以不在Update里面更新状态机
        anim.SetBool("Critical", attackStats.IsCritical);
        agent.isStopped = true;
        agent.destination = transform.position;
        attackLastCD = attackStats.CoolDown;//重置攻击CD
        isAttacking = true;
        yield return new WaitForSeconds(attackStats.CoolDown);
        isAttacking = false;
    }
    private void BreakAttack()
    {
        if (isDead) return;
        isAttacking = false;
        agent.isStopped = false;
        attackLastCD = attackStats.CoolDown;//打断攻击需要恢复CD
        if (attackCoroutine != null)
        {
            anim.Play("Blend Tree");
            StopCoroutine(attackCoroutine);//停止攻击协程,因为攻击协程被删除了,所以isAttacking需要被设置为false
        }
    }
    public void Hit()
    {
        CharacterBaseStats targetBaseStats = attackTarget.GetComponent<CharacterBaseStats>();
        targetBaseStats.Hurt(attackStats);
    }
}