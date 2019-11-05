using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyController : MonoBehaviour
{
    #region Values

    [SerializeField]
    float EnemyHp; //적 hp

    float EnemyView_RushRad; //접근가능 사정거리
    float EnemyView_RushSpd; //접근 시 이동속도

    float EnemyView_RunRad; //도망 사정거리
    float EnemyView_RunSpd; //도망 시 이동속도

    float EnemyView_AtkRad; //공격 사정거리
    float Enemy_AtkCoolDown; //공격 쿨타임
    float Enemy_NowAtkCool; //현재 공격 쿨타임

    bool IsKnockback = false; //넉백중인지

    public enum Stat
    {
        CREATE, //생성
        MOVE, //이동
        DAMAGE, //피격중
        KNOCKBACK, //넉백중
        ATK, //공격중
        DEAD, //죽는중
        MAX
    };

    Stat NowStat; //현재 몬스터 스탯

    #endregion

    //------------------

    #region Delegate

    delegate void VoidF4Delegate(float f1, float f2, float f3, float f4);
    VoidF4Delegate Moving; //이동 델리게이트

    delegate void VoidF1Delegate(float i1);
    VoidF1Delegate Damage; //피격 델리게이트

    delegate void VoidNDelegate();
    VoidNDelegate Create;
    VoidNDelegate Attack; //공격 델리게이트
    VoidNDelegate Dead; // 죽음 델리게이트

    //Close타입----------------------------------
    VoidNDelegate OpenMove; //Move로 변경 시 실행
    VoidNDelegate OpenDamage; //Damage로 변경 시 실행
    VoidNDelegate OpenKnockback; //Knockback으로 변경 시 실행
    VoidNDelegate OpenAtk; //Atk로 변경 시 실행
    VoidNDelegate OpenDead; //Dead로 변경 시 실행

    //Open타입----------------------------------
    VoidNDelegate CloseCreate; //Create에서 변경 시 실행
    VoidNDelegate CloseMove; //Move에서 변경 시 실행
    VoidNDelegate CloseDamage; //Damage에서 변경 시 실행
    VoidNDelegate CloseKnockback; //Knockback에서 변경 시 실행
    VoidNDelegate CloseAtk; //Atk에서 변경 시 실행

    #endregion

    //------------------

    #region Method

    void Logic()
    {
        switch(NowStat)
        {
            case Stat.MOVE:
                Moving(EnemyView_RushRad, EnemyView_RushSpd, EnemyView_RunSpd, EnemyView_RunSpd);

                if (Enemy_NowAtkCool<=0)
                {
                    Attack();
                    Enemy_NowAtkCool = Enemy_AtkCoolDown;
                    ChangeStat(Stat.ATK);
                }
                break;

            case Stat.DAMAGE:
            case Stat.KNOCKBACK:
                break;

            case Stat.DEAD:
                Dead();
                break;
        }
    }

    public void ChangeStat(Stat s)
    {
        switch(NowStat)//상태 값 변경 이벤트(Close 타입)
        {
            case Stat.CREATE:
                CloseCreate();
                break;

            case Stat.ATK:
                CloseAtk();
                break;

            case Stat.DAMAGE:
                CloseDamage();
                break;

            case Stat.KNOCKBACK:
                CloseKnockback();
                break;

            case Stat.MOVE:
                CloseMove();
                break;
        }

        NowStat = s; //스탯 값 변경

        switch(s) //상태 값 변경 이벤트(Open 타입)
        {
            case Stat.ATK:
                OpenAtk();
                break;

            case Stat.DAMAGE:
                OpenDamage();
                break;

            case Stat.DEAD:
                OpenDead();
                break;

            case Stat.KNOCKBACK:
                OpenKnockback();
                break;

            case Stat.MOVE:
                OpenMove();
                break;
        }
    }

    #endregion
}
