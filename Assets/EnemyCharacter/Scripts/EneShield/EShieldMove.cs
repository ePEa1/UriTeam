using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class EShieldMove : EnemyMoveBase
{
    [SerializeField, LabelText("타겟의 태그 이름")] string targetTag;

    GameObject target; //타겟 오브젝트

    float targetDis; //타겟과의 거리

    NavMeshAgent nav;

    protected override void Awake()
    {
        base.Awake();

        target = GameObject.FindWithTag(targetTag);
        nav = manager.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
            targetDis = Vector3.Distance(manager.transform.position, target.transform.position);

        if (manager.GetStat() == EnemyController.EStat.MOVE) //이동 상태일 경우
        {
            nav.isStopped = false; //네비 작동
        }
        else //다른 상태면
        {
            nav.isStopped = true; //네비 끄기
        }
    }

    public override void Moving(float rushSpeed, float rushRad, float runSpeed, float runRad)
    {
        if (target != null) // 타겟이 있으면
        {
            if (manager.IsDelayOk())
            {
                if (manager.GetAtkRad() >= targetDis) //공격사거리 안에 들어가있으면
                {
                    manager.OnAtkEvent(); //공격 이벤트 발생
                }
                else if (rushRad >= targetDis) //그렇지 않고 접근 범위 안에 위치시
                {
                    nav.speed = rushSpeed; //접근속도로
                    nav.SetDestination(target.transform.position); //타겟에게 이동
                }
            }
            else
            {
                if (runRad <= targetDis && rushRad >= targetDis) //도망 범위 밖이면서 접근 범위 안에 있을 경우
                {
                    nav.speed = rushSpeed; //접근속도로
                    nav.SetDestination(target.transform.position); //타겟위치를 목표지점으로 이동
                }
                else if (rushRad > targetDis)
                {
                    nav.speed = runSpeed; //도망속도로
                    nav.SetDestination(manager.transform.position -
                        (target.transform.position - manager.transform.position).normalized * runSpeed); //타겟위치의 반대방향으로 이동
                }
            }
        }
    }
}
