using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Default : PlayerCharacter_ActionBase
{
    #region Event
    protected override void OnStartAction()
    {
    }
    protected override CharacterAction OnUpdateAction()
    {
        //사용할 값들 캐스팅 (에러나면 게임작동하면 안되니까 그냥 예외처리 없음)
        PlayerCharacterControl control = CurrentCharacter.CurrentControl as PlayerCharacterControl;
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;
        CharacterPhysicCharacterController physic = player.ChildPhysic as CharacterPhysicCharacterController;

        //기본 업데이트 (다른 액션으로 이동 처리)
        CharacterAction action = base.OnUpdateAction();
        if (action != this)
            return action;

        //Move
        if (0.1f < control.Move.magnitude)
        {
            CurrentAni.PlayAnimation("Move");
            player.SetLookVector(control.Move, false);
            physic.Move(control.Move);
        }
        //Idle
        else
        {
            CurrentAni.PlayAnimation("Idle");
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
