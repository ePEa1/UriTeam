using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기본적인 액션 업데이트 내용
/// </summary>
public abstract class PlayerCharacter_ActionBase : CharacterAction
{
    #region Event
    protected override CharacterAction OnUpdateAction()
    {
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;
        PlayerCharacterControl control = player.CurrentControl as PlayerCharacterControl;

        //Attack
        if (control.Attack)
        {
            /*
            if (control.Range)
            {
                if (0 < player.BulletCount.Value)
                    return player.AttackRangeAction;
                else
                    return player.DefaultAction;
            }
            else
            */
            return player.AttackMeleeAction;
        }

        //Switch Weapon
        //if (control.SwitchWeapon)
        //    return player.SwitchAction;

        //Dash, Default
        if (0.1f < control.Move.magnitude)
        {
            if (control.Dash)
                return player.DashAction;
            else
                return player.DefaultAction;
        }

        return this;
    }
    #endregion
}
