using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ePEaCostomFunction;

public class EnemyKnockBase : MonoBehaviour
{
    EnemyController manager;

    Vector3 knockNor = Vector3.zero; //넉백 방향 벡터
    Vector3 originVec;     //넉백 피봇점
    float knockPow = 0.0f; //넉백 세기
    float knockSpeed = 0.0f; //날라가는 속도

    float knockTime = 0.0f; //넉백 경과시간

    // Start is called before the first frame update
    void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
    }

    //현재 타수에 따른 데이터 가져오기
    void SetKnockData(int atkNum)
    {
        knockPow = Data.data.ForceCharging[atkNum].KnockBackForce; //밀려나는 거리 반영
        knockSpeed = Data.data.ForceCharging[atkNum].KnockSpeed; //넉백 경과시간 반영
    }

    //컨트롤러의 업데이트에서 호출하는 코드
    public void KnockUpdate()
    {
        knockTime += Time.deltaTime / knockSpeed; //넉백 경과시간 (0~1)

        float plusKnock = Data.data.forceCurve.Evaluate(knockTime) * knockPow; //밀려난 거리

        manager.transform.position = originVec + plusKnock * knockNor;

        if (knockTime >= 1) //넉백이 다 끝났으면
        {
            manager.OnMoveEvent(); //이동 상태로 변경
            knockPow = 0.0f; //넉백 파워 초기화
            knockNor = Vector3.zero; //넉백 방향 초기화
            knockTime = 0.0f; //넉백 경과시간 초기화
            EndDamage(); //피격 종료 이벤트 실행
        }
    }

    //컨트롤러에서 호출되는 이벤트에서 호출되는 이벤트
    protected virtual void KnockEvent()
    {
        //피봇점 설정 (이 좌표를 기준으로 넉백이 됨)
        originVec = manager.transform.position;

        Debug.Log("[" + manager.name + "] Knockbacked");
    }

    //컨트롤러에서 호출되는 이벤트
    public void PlayKnockEvent(Vector3 nor, int atkNum)
    {
        //넉백 방향 설정
        knockNor = nor;

        //맞은 타수에 따라 넉백 데이터 설정
        SetKnockData(atkNum);

        //넉백 구현부
        KnockEvent();
    }

    //피격 종료 이벤트
    public virtual void EndDamage()
    {
        if (manager.EnemyHp <= 0) //남은 체력이 0 이하면
            manager.OnDeadEvent(); //사망 이벤트 실행
        else
            manager.OnMoveEvent(); //이동 상태로 변경
    }
}
