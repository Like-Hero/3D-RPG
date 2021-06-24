using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    private float attackDistance;
    private float attackCD;
    private float attackLastCD;
    private bool isAttacking;//角色是否正在攻击
    public int cnt;//攻击次数，测试用
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        attackDistance = 1.0f;//测试先用着，按照逻辑应该放在 武器类 里面
        attackCD = 1.0f;//测试先用着，按照逻辑应该放在 武器类 里面
        attackLastCD = attackCD;
    }
    private void Start()
    {
        MouseManager.Ins.OnMouseClicked += MoveToTarget;

        MouseManager.Ins.OnEnemyClicked += EventAttack;
    }
    private void Update()
    {
        SwitchAnimation();
        //如果CD大于0，那么就一直减CD
        if(attackLastCD > 0)
        {
            attackLastCD -= Time.deltaTime;
        }
    }
    private void SwitchAnimation()
    {
        if (Input.GetKeyDown(KeyCode.S))//测试用，按住S停下
        {
            agent.destination = transform.position;
        }
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }
    private void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        isAttacking = false;//再将攻击状态设置为false
        agent.destination = target;
    }
    private void EventAttack(GameObject enemy)
    {
        if (enemy != null)
        {
            //先移动到敌人位置
            StartCoroutine(MoveToEnemy(enemy));
        }
    }
    private IEnumerator MoveToEnemy(GameObject enemy)
    {
        while(Vector3.Distance(transform.position, enemy.transform.position) > attackDistance)
        {
            agent.destination = enemy.transform.position;
            yield return null;
        }
        //角色走到敌人之后需要看向敌人才攻击，不然视觉上可能会攻击空气
        transform.LookAt(enemy.transform);
        //移动到敌人面前之后：攻击
        AttackEnemy();
    }
    
    private void AttackEnemy()
    {
        //CD小于等于0说明可以攻击
        if (attackLastCD <= 0)
        {
            cnt++;
            attackLastCD = attackCD;
            anim.SetTrigger("Attack");
        }
    }
}

//协程版本,如果用该方法则会出BUG，留下了做学习
//private IEnumerator AttackEnemy()
//{
//    //print(isAttacking);
//    //如果没有在攻击，则攻击
//    if (!isAttacking)
//    {
//        cnt++;
//        isAttacking = true;//将攻击状态设为true
//        anim.SetTrigger("Attack");
//        yield return new WaitForSeconds(attackCD);//等待攻击CD时间之后
//        isAttacking = false;//再将攻击状态设置为false
//    }
//    yield return null;
//    }
//}

//有个小Bug，比较近的话可以先点敌人，然后角色会攻击，马上点地板，然后再去点敌人。
//虽然角色动画还是和一直点敌人一样，但是实际执行的方法会再次调用
//这就导致了可以利用自己的手速可以一直攻击敌人（说游戏特色算吗）
//可能的解决方法是用计数器来实现，例如这个计数器会再update里面一直减，不能放到协程是因为协程可能会被删掉（这个Bug也是如此）