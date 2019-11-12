using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyKnockBase : MonoBehaviour
{
    EnemyController manager;

    Vector3 knockNor = Vector3.zero; //넉백 방향 벡터
    float knockPow = 0.0f; //넉백 파워
    float knockDam = 0.0f; //부딪혔을 때 받는 데미지

    int knockNum = 0; //누적 타수

    [Title("넉백 저항(높을수록 덜 밀려남)")]
    [SerializeField, LabelText("저항 값"), Range(0.1f, 5.0f)] float knockGuard;
    [Title("물리누적 최대치")]
    [SerializeField, LabelText("최대 누적 타수")] int maxKnock;

    // Start is called before the first frame update
    void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
    }

    void Update()
    {
        knockPow -= knockPow * knockGuard * Time.deltaTime;
    }

    protected virtual void KnockEvent(float dam)
    {
        manager.ChangeStat(EnemyController.EStat.KNOCKBACK);
        manager.GetAnimator().SetBool("Konck", true);
    }

    //컨트롤러에서 호출되는 이벤트
    public void PlayKnockEvent(Vector3 nor)
    {
        //넉백 이벤트 실행
        //KnockEvent(Data.data.ForceCharging[]);

        //넉백정보 저장
        knockNum++;

        //knockNor = nor;
        //knockPow = pow;
        //knockDam = dam;
    }



    //넉백정보 반환
    public Vector3 KnockVector()
    {
        return knockNor * knockPow;
    }

    public virtual void Crash()
    {
        manager.EnemyHp -= knockDam;
        knockDam = 0.0f;
        manager.OnMoveEvent();
    }
}
