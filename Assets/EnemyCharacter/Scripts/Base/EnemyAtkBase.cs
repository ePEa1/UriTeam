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

    protected GameObject target; //타겟 오브젝트

    protected virtual void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
        target = GameObject.FindWithTag(targetTag);
    }

    protected virtual void Update()
    {

    }

    //실제 공격 구현부
    protected virtual void AtkEvent()
    {
        manager.ChangeStat(EnemyController.EStat.ATK); //공격 상태로 변경
        manager.GetAnimator().SetTrigger("Atk"); //공격 애니메이션 재생
        manager.transform.rotation = CostomFunctions.PointDirection(manager.transform.position, target.transform.position); //타겟 방향으로 회전
        Debug.Log("[" + manager.name + "] is attacking");
    }

    //컨트롤러에서 호출하는 이벤트
    public void PlayAtkEvent()
    {
        AtkEvent();
    }

    //애니메이션에서 호출하는 공격 이벤트
    protected abstract void RealAtk();

    //공격 애니메이션 끝났을때 호출하는 이벤트
    protected virtual void EndAtk()
    {
        manager.OnMoveEvent();
    }
}
