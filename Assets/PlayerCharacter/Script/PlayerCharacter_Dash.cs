using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;

public class PlayerCharacter_Dash : PlayerCharacter_ActionBase
{
    #region Value
    private float m_Timer;
    private Vector2 m_DashDirection;                            //대쉬 방향
    private Coroutine m_AvoidCoroutine;                         //회피효과 줬다뺐었다 하는 코루틴
    private AvoidEffect m_AvoidEffect = new AvoidEffect();      //회피효과
    private bool m_IsDashStarted;                               //대쉬 애니메이션이 시작했는지 (약간의 코드문제를 회피하기 위함)
    private bool m_IsDashEnd;                                   //대쉬 애니메이션이 끝났는지 (끝나면 바로 다시 Default로 돌아감)
    #endregion

    #region Event
    protected override void OnStartAction()
    {
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;
        PlayerCharacterControl control = CurrentCharacter.CurrentControl as PlayerCharacterControl;

        //초기값들 설정
        m_Timer = 0;
        m_DashDirection = control.Move;
        m_IsDashEnd = false;
        m_IsDashStarted = false;

        //플레이어를 해당 위치를 보도록 만들기
        player.SetLookVector(m_DashDirection, true);

        //회피효과 코루틴 실행
        m_AvoidCoroutine = StartCoroutine(AvoidCoroutine());
    }
    protected override CharacterAction OnUpdateAction()
    {
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;
        CharacterPhysicCharacterController physic = player.ChildPhysic as CharacterPhysicCharacterController;

        //실제 구르기 처리되는 시간
        m_Timer += Time.unscaledDeltaTime * player.PlayerTimeScale;
        if (m_Timer <= data.Rolling_RollTime)
        {
            float normalizedTime = Mathf.Clamp01(m_Timer / data.Rolling_RollTime);
            float dashSpeedFactor = data.Rolling_MoveSpd / data.Char_MoveSpd;

            physic.Move(m_DashDirection * dashSpeedFactor * data.Rolling_Curve.Evaluate(normalizedTime));
        }
        //캔슬
        if(data.Rolling_ActiveTime <= m_Timer)
        {
            CharacterAction action = base.OnUpdateAction();
            if (action != this)
                return action;
        }

        //대쉬 애니메이션이 끝났으면 끝...
        if (m_IsDashEnd)
            return player.DefaultAction;

        return this;
    }
    protected override void OnEndAction()
    {
        if (m_AvoidCoroutine != null)
            StopCoroutine(m_AvoidCoroutine);
    }
    protected override void OnAniEvent(string aniID, string aniName, string eventName)
    {
        //대쉬 애니메이션이 끝나면 대쉬 끝!
        if (aniName == "Dash")
        {
            if (eventName == "start")
                m_IsDashStarted = true;
            else if(eventName == "end" && m_IsDashStarted)
                m_IsDashEnd = true;
        }
    }
    #endregion
    #region Function
    //Coroutine
    /// <summary>
    /// 회피 효과 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator AvoidCoroutine()
    {
        yield return new WaitForSecondsRealtime(data.Rolling_MsBetweenInvincible);

        CurrentCharacter.AddEffect(m_AvoidEffect, new CharacterEffectList.EffectStateStruct(null));
        yield return new WaitForSecondsRealtime(data.Rolling_InvincibleActiveTime);
        CurrentCharacter.RemoveEffect(m_AvoidEffect);

        m_AvoidCoroutine = null;
    }
    #endregion
}
