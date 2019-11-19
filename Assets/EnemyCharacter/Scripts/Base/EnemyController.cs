using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

interface IDamage
{
    void OnDamEvent(float d);
    void OnKnockEvent(Vector3 nor);

}

public class EnemyController : MonoBehaviour, IDamage
{
    #region Values

    [Title("전투 능력치")]
    [SerializeField, TabGroup("Option"), LabelText("HP")] public float EnemyHp; //적 hp
    [SerializeField, TabGroup("Option"), LabelText("공격력")] float EnemyPower; //적 공격력
    [SerializeField, TabGroup("Option"), LabelText("공격 딜레이")] float Enemy_AtkCoolDown; //공격 쿨타임
    [SerializeField, TabGroup("Option"), LabelText("공격 사거리")] float EnemyView_AtkRad; //공격 사정거리

    [Title("전투 컴포넌트")]
    [SerializeField, TabGroup("Component"), LabelText("공격")] EnemyAtkBase compAtk; //적 공격 구현 컴포넌트
    [SerializeField, TabGroup("Component"), LabelText("피격")] EnemyDamBase compDam; //적 피격 구현 컴포넌트
    [SerializeField, TabGroup("Component"), LabelText("넉백")] EnemyKnockBase compKnock; //적 넉백 구현 컴포넌트
    [SerializeField, TabGroup("Component"), LabelText("사망")] EnemyDeadBase compDead; //적 사망 구현 컴포넌트

    [Title("이동 능력치")]
    [SerializeField, TabGroup("Option"), LabelText("접근 이동속도")] float EnemyView_RushSpd; //접근 시 이동속도
    [SerializeField, TabGroup("Option"), LabelText("접근 가능 사정거리")] float EnemyView_RushRad; //접근가능 사정거리
    [SerializeField, TabGroup("Option"), LabelText("도망 이동속도")] float EnemyView_RunSpd; //도망 시 이동속도
    [SerializeField, TabGroup("Option"), LabelText("도망 가능 사정거리")] float EnemyView_RunRad; //도망 사정거리
    [SerializeField, TabGroup("Option"), LabelText("넉백 날라가는 속도")] float Enemy_KnockSpeed; //넉백당할 시 날라가는 속도

    [Title("이동 컴포넌트")]
    [SerializeField, TabGroup("Component"), LabelText("이동")] EnemyMoveBase compMove; //적 이동 구현 컴포넌트
    [SerializeField, TabGroup("Component"), LabelText("시선")] EnemyViewBase compView; //적 시선 구현 컴포넌트
    [SerializeField, TabGroup("Component"), LabelText("애니메이션")] Animator animator; //적 애니메이션 컨트롤러

    float Enemy_NowAtkCool; //현재 공격 쿨타임

    bool IsKnockback = false; //넉백중인가

    bool IsSuperArmor = false; //슈퍼아머(피격, 경직 애니메이션 무시) 상태인가

    bool notDam = true; //무적 상태인가

    public enum EStat //상태값 enum
    {
        CREATE, //생성
        MOVE, //이동
        DAMAGE, //피격중
        KNOCKBACK, //넉백중
        ATK, //공격중
        DEAD, //죽는중
        MAX
    };

    EStat NowStat = EStat.CREATE; //현재 몬스터 상태

    //타임스케일
    //float ts = GameManager.gameManager.TimeScale;

    #endregion

    //------------------

    #region Event
    
    //공격 이벤트
    public void OnAtkEvent()
    {
        compAtk.PlayAtkEvent();
    }

    //피격 이벤트
    public void OnDamEvent(float d)
    {
        compDam.PlayDamEvent(d);
    }

    //이동 이벤트
    public void OnMoveEvent()
    {
        compMove.PlayMoveEvent();
    }

    //사망 이벤트
    public void OnDeadEvent()
    {
        compDead.PlayDeadEvent();
    }

    //넉백 이벤트
    public void OnKnockEvent(Vector3 nor)
    {
        compKnock.PlayKnockEvent(nor);
    }

    void Update()
    {
        switch(NowStat)
        {
            case EStat.MOVE:
                //이동 구현되있는거 실행
                compMove.Moving(EnemyView_RushSpd, EnemyView_RushRad, EnemyView_RunSpd, EnemyView_RunRad);

                //공격 가능하면 공격이벤트 발생
                if (Enemy_NowAtkCool <= 0 && EnemyView_AtkRad >= 10.0f)
                {
                    OnAtkEvent();
                }
                break;

            case EStat.KNOCKBACK:
                //날라가는 코드 실행
                compKnock.KnockUpdate();
                break;
        }

        //테스트코드
        KnockTest();
    }

    #endregion

    //------------------

    #region Method

    public void SetSuperArmor(bool armor) { IsSuperArmor = armor; } //슈퍼아머상태로 만들기
    public bool GetSuperArmor() { return IsSuperArmor; } //슈퍼아머 상태인지 받기
    public bool IsNotDam() { return notDam; } //무적상태인지
    public float GetKnockSpeed() { return Enemy_KnockSpeed; } //넉백속도 받기

    //애니메이터 반환
    public Animator GetAnimator() { return animator; }

    //상태값 반환
    public EStat GetStat() { return NowStat; }

    //상태값 변경
    public void ChangeStat(EStat s) { NowStat = s; }

    #endregion

    //------------------

    #region TestCode

    public void KnockTest()
    {
        if (Input.GetKeyDown(KeyCode.N))
            OnKnockEvent(new Vector3(10, 0, 0));
    }

    #endregion
}
