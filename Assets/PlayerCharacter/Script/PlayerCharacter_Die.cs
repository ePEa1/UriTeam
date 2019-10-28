using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Die : CharacterAction
{
    protected override void OnStartAction()
    {
    }
    protected override CharacterAction OnUpdateAction()
    {
        return this;
    }
    protected override void OnEndAction()
    {
    }
    protected override void OnAniEvent(string aniID, string aniName, string eventName)
    {
    }
}
