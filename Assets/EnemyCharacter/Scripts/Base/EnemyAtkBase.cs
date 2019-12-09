using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ePEaCostomFunction;

public abstract class EnemyAtkBase : MonoBehaviour
{
    protected EnemyController manager;

    [Title("기본 속성")]
    [SerializeField, LabelText("타겟의 태그 이름")] string targetTag;
    [SerializeField, LabelText("공격 판정 타이밍")] float at;
    [SerializeField, LabelText("공격 종료 시간")] float endTime;

    protected GameObject target; //타겟 오브젝트
    bool nowAtk = false;
    bool nowEnd = false;
    float nowAT = 0.0f;
    float nowEndTime = 0.0f;

    protected virtual void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
        target = GameObject.FindWithTag(targetTag);
    }

    protected virtual void Update()
    {
        if (manager.GetStat()==EnemyController.EStat.ATK)
        {
            if (nowAT > 0)
                nowAT -= Time.deltaTime;
            else nowAT = 0.0f;

            if (nowEndTime > 0)
                nowEndTime -= Time.deltaTime;
            else nowEndTime = 0.0f;


            if (nowAT == 0.0f && !nowAtk)
            {
                RealAtk();
                nowAtk = true;
            }

            if (nowEndTime == 0.0f && !nowEnd)
            {
                EndAtk();
                nowEnd = true;
            }
        }
    }

    //실제 공격 구현부
    protected virtual void AtkEvent()
    {
        if (manager.curAtkType == EnemyController.EAtkType.LONG)
            manager.hitBox.SetActive(true);
        nowAtk = false;
        nowEnd = false;
        nowAT = at;
        nowEndTime = endTime;
        manager.ChangeStat(EnemyController.EStat.ATK); //공격 상태로 변경
        manager.GetAnimator().Play("Atk");
        manager.transform.rotation = CostomFunctions.PointDirection(manager.transform.position, target.transform.position); //타겟 방향으로 회전
        Debug.Log("[" + manager.name + "] is attacking");
    }

    //컨트롤러에서 호출하는 이벤트
    public void PlayAtkEvent()
    {
        AtkEvent();
    }

    //애니메이션에서 호출하는 공격 이벤트
    public abstract void RealAtk();

    //공격 애니메이션 끝났을때 호출하는 이벤트
    protected virtual void EndAtk()
    {
        manager.OnMoveEvent();
    }
}
