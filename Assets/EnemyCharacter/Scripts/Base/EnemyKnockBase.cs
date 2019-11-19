using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ePEaCostomFunction;

public class EnemyKnockBase : MonoBehaviour
{
    EnemyController manager;

    Vector3 knockNor = Vector3.zero; //넉백 방향 벡터
    float knockPow = 0.0f; //넉백 파워

    int knockNum = 0; //누적 타수

    float finalNockSpeed = 0.0f; //실제 날라가는 속도

    [SerializeField, LabelText("넉백 저항 값")] float knockGuard;
    [SerializeField, LabelText("넉백 느려지는 타이밍"), Range(0.01f, 0.99f)] float slowKnock;

    // Start is called before the first frame update
    void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
    }

    //현재 타수에 따른 데이터 가져오기
    //수정할 예정임
    float GetKnockData(int knockNum)
    {
        float ret = 0.0f;

        for (int i =0;i<Data.data.ForceCharging.Length;i++)
        {
            if (Data.data.ForceCharging[i].AtkValue <= knockNum)
                ret = Data.data.ForceCharging[i].KnockBackForce;
        }

        return ret;
    }

    //컨트롤러의 업데이트에서 호출하는 코드
    public void KnockUpdate()
    {
        //실제 넉백당할 세기 계산
        finalNockSpeed = manager.GetKnockSpeed() * Time.deltaTime * CostomFunctions.Min(1.0f, knockPow / GetKnockData(knockNum) * slowKnock);
        //좌표상으로 밀려나가게 하기
        manager.transform.Translate(knockNor.normalized * finalNockSpeed);

        //넉백 세기 감소
        knockPow -= Time.deltaTime * knockGuard;

        if (knockPow<=0) //넉백 파워 다 떨어졌으면
        {
            manager.OnMoveEvent(); //이동 상태로 변경
            knockPow = 0.0f; //넉백 파워 초기화
            knockNor = Vector3.zero; //넉백 방향 쵝화
            knockNum = 0; //타수 초기화
        }
    }

    //컨트롤러에서 호출되는 이벤트에서 호출되는 이벤트
    protected virtual void KnockEvent()
    {
        //몬스터 상태 넉백으로 변경
        manager.ChangeStat(EnemyController.EStat.KNOCKBACK);

        //넉백당하는 애니메이션으로 변경
        manager.GetAnimator().SetBool("Konck", true);

        //맞은 타수에 따라 넉백 세기 설정
        knockPow = GetKnockData(knockNum);
    }

    //컨트롤러에서 호출되는 이벤트
    public void PlayKnockEvent(Vector3 nor)
    {
        //넉백타수 추가
        knockNum++;
        //넉백 각도 추가
        knockNor += nor;

        //넉백 구현부
        KnockEvent();
    }

    public virtual void Crash()
    {
        //넉백 데미지 계산하는거였는데 수정해야됨
        //manager.EnemyHp -= knockDam;
        //knockDam = 0.0f; 

        manager.OnMoveEvent(); //이동 상태로 변경
    }
}
