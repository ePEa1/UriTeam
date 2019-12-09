using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Data;

public class PlayerCharacter_AttackMelee : PlayerCharacter_ActionBase
{
    #region Value
    private float m_Timer;
    private bool m_IsTriggered;
    private float m_TriggerTimer;
    private bool m_IsMeleeAttackEnd;                        //근접공격 애니메이션이 끝났는지  (끝나면 바로 다시 Default로 돌아감)
    public int m_ComboIndex;

    Vector3 originVec;
    public Vector3 atkNor;
    float moveLength;
    float maxMoveTime = 0.0f;
    float moveTime = 0.0f;
    bool nowDash;
    #endregion

    #region Event
    protected override void OnStartAction()
    {
        //초기값들 설정
        m_IsMeleeAttackEnd = false;
        m_ComboIndex = -1;

        //공격!!!
        Attack();
    }
    protected override CharacterAction OnUpdateAction()
    {
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;
        PlayerCharacterControl control = CurrentCharacter.CurrentControl as PlayerCharacterControl;

        m_Timer += Time.unscaledDeltaTime * player.PlayerTimeScale;

        if (!m_IsTriggered)
        {
            if (data.MeleeAtk[m_ComboIndex].TriggerTime <= m_Timer)
            {
                m_IsTriggered = true;
                m_TriggerTimer = data.MeleeAtk[m_ComboIndex].TriggerDur;
                player.AttackTrigger.Enable(data.MeleeAtk[m_ComboIndex].Dmg);
            }
        }
        else if(0 < m_TriggerTimer)
        {
            m_TriggerTimer -= Time.deltaTime;
            if (m_TriggerTimer <= 0)
                player.AttackTrigger.Disable();
        }

        if (data.MeleeAtk[m_ComboIndex].ActiveTime <= m_Timer)
        {
            //계속 공격하면 콤보!
            if (control.Attack)
            {
                Attack();
                return this;
            }


        }

        //약진 중일 경우 약진 업데이트 내용 실행
        if (nowDash)
            AtkMove();

        //대쉬로 캔슬 가능
        if (0.1f < control.Move.magnitude && control.Dash)
        {
            player.AttackTrigger.Disable();
            return player.DashAction;
        }

        if (control.Parry && player.ParryOk())
        {
            return player.ParryAction;
        }

        //공격 애니메이션이 끝났으면 끝...
        if (m_IsMeleeAttackEnd)
            return player.DefaultAction;

        return this;
    }
    protected override void OnEndAction()
    {
    }
    protected override void OnAniEvent(string aniID, string aniName, string eventName)
    {
        if (aniName == $"Attack_Melee_{m_ComboIndex}" && eventName == "end")
            m_IsMeleeAttackEnd = true;
    }
    #endregion
    #region Function
    //Private
    /// <summary>
    /// 공격하는 함수
    /// </summary>
    private void Attack()
    {
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;
        PlayerCharacterControl control = CurrentCharacter.CurrentControl as PlayerCharacterControl;

        m_ComboIndex = (m_ComboIndex + 1) % data.MeleeAtk.Length;
        player.AttackTrigger.SetAtkIndex(m_ComboIndex);
        player.AttackTrigger.SetAtkNormalize(atkNor);
        CurrentAni.PlayAnimation($"Attack_Melee_{m_ComboIndex}", true);
        m_IsTriggered = false;
        m_Timer = 0;

        AttackDash(m_ComboIndex);

        if(m_ComboIndex == 0)
        {
            atkNor = new Vector3(control.AttackDirection.x, 0.0f, control.AttackDirection.y).normalized;
            player.SetLookVector(control.AttackDirection, true);
        }
            
    }
    #endregion

    void AtkMove()
    {
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;
        if (moveTime-Time.deltaTime>0)
        {
            moveTime -= Time.deltaTime;
            player.GetComponent<CharacterController>().Move(atkNor * moveLength * Time.deltaTime * (1.0f / maxMoveTime));
        }
        else
        {
            nowDash = false;
        }
    }

    void AttackDash(int atkNum)
    {
        maxMoveTime = data.MeleeAtk[atkNum].AtkMoveSpeed;
        moveTime = maxMoveTime;
        moveLength = data.MeleeAtk[atkNum].AtkMove;
        originVec = CurrentCharacter.transform.position;
        nowDash = true;
    }
}
