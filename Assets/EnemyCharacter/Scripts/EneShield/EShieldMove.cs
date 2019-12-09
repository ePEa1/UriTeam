using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ePEaCostomFunction;

public class EShieldMove : EnemyMoveBase
{
    [SerializeField, LabelText("타겟의 태그 이름")] string targetTag;

    GameObject target; //타겟 오브젝트

    float targetDis; //타겟과의 거리

    protected override void Awake()
    {
        base.Awake();

        target = GameObject.FindWithTag(targetTag);
    }

    void Update()
    {
        if (target != null)
            targetDis = Vector3.Distance(manager.transform.position, target.transform.position);

        if (manager.GetStat() == EnemyController.EStat.MOVE) //이동 상태일 경우
        {
            //캐릭터 방향으로 시선 고정
            manager.transform.rotation = CostomFunctions.PointDirection(manager.transform.position, target.transform.position); //타겟 방향으로 회전
        }
    }

    public override void Moving(float rushSpeed, float rushRad, float runSpeed, float runRad)
    {
        if (target != null) // 타겟이 있으면
        {
            if (manager.IsDelayOk()) //공격 가능 상태면
            {
                if (manager.GetAtkRad() >= targetDis) //공격사거리 안에 들어가있으면
                {
                    manager.OnAtkEvent(); //공격 이벤트 발생

                }
                else if (rushRad >= targetDis) //그렇지 않고 접근 범위 안에 위치시
                {
                    //Debug.Log("[" + manager.name + "] Atk Rush");

                    //접근 속도로 캐릭터에게 이동
                    manager.transform.position += CostomFunctions.PointNormalize(manager.transform.position, target.transform.position) * rushSpeed * Time.deltaTime;
                }
            }
            else //공격 불가 상태면
            {
                if (runRad <= targetDis && rushRad >= targetDis) //도망 범위 밖이면서 접근 범위 안에 있을 경우
                {
                    //Debug.Log("[" + manager.name + "] Rush");

                    //접근 속도로 캐릭터에게 이동
                    manager.transform.position += CostomFunctions.PointNormalize(manager.transform.position, target.transform.position) * rushSpeed * Time.deltaTime;
                }
                else if (rushRad > targetDis)
                {
                    //Debug.Log("[" + manager.name + "] Run");

                    //도망속도로 캐릭터 반대방향으로 이동
                    manager.transform.position += -CostomFunctions.PointNormalize(manager.transform.position, target.transform.position) * runSpeed * Time.deltaTime;
                }
            }
        }
    }
}
