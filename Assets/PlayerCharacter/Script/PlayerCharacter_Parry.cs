using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;

public class PlayerCharacter_Parry : PlayerCharacter_ActionBase
{
    #region Value
    private float m_Timer;
    private bool m_IsSwitched;      //실제 스위치 처리를 했는지
    #endregion

    #region Event
    protected override void OnStartAction()
    {
    }
    protected override CharacterAction OnUpdateAction()
    {
        /*if(시간지남)
            return base.OnUpdateAction();
        else*/
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
