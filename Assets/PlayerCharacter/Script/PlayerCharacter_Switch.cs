using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;

public class PlayerCharacter_Switch : PlayerCharacter_ActionBase
{
    #region Value
    private float m_Timer;
    private bool m_IsSwitched;      //실제 스위치 처리를 했는지
    #endregion

    #region Event
    protected override void OnStartAction()
    {
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;

        //기본 초기화
        m_Timer = 0;
        m_IsSwitched = false;

        //무기 변경 애니메이션 재생
        if (player.IsWeaponRange.Value)
            CurrentAni.PlayAnimation("Switch_RangeToMelee");
        else
            CurrentAni.PlayAnimation("Switch_MeleeToRange");
    }
    protected override CharacterAction OnUpdateAction()
    {
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;

        //무기 스위칭 캔슬타임
        m_Timer += Time.unscaledDeltaTime * player.PlayerTimeScale;
        if (data.Switching_ActiveTime <= m_Timer)
        {
            //실제 스위치 처리
            if (!m_IsSwitched)
            {
                m_IsSwitched = true;
                player.IsWeaponRange.Value = !player.IsWeaponRange.Value;
            }

            //기본 업데이트 (다른 액션으로 이동 처리)
            CharacterAction action = base.OnUpdateAction();
            if (action != this)
                return action;
        }

        return this;
    }
    protected override void OnEndAction()
    {
    }
    protected override void OnAniEvent(string aniID, string aniName, string eventName)
    {
    }
    #endregion
}
