using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyKnockBase : MonoBehaviour
{
    EnemyController manager;

    Vector3 knockNor = Vector3.zero; //넉백 방향 벡터
    float knockPow = 0.0f; //넉백 파워

    [Title("넉백 저항(높을수록 덜 밀려남)")]
    [SerializeField, LabelText("저항 값"), Range(0.1f, 5.0f)] float knockGuard;

    // Start is called before the first frame update
    void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
    }

    void Update()
    {
        knockPow -= knockPow * knockGuard * Time.deltaTime;
    }

    protected virtual void KnockEvent()
    {
        manager.ChangeStat(EnemyController.EStat.KNOCKBACK);
        manager.GetAnimator().SetBool("Konck", true);
    }

    public void PlayKnockEvent(Vector3 nor, float pow)
    {
        //넉백 이벤트 실행
        KnockEvent();

        //넉백정보 저장
        knockNor = nor;
        knockPow = pow;
    }

    //넉백정보 반환
    public Vector3 KnockVector()
    {
        return knockNor * knockPow;
    }
}
